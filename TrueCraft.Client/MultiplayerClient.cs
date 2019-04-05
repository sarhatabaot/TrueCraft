﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using TrueCraft.Client.Events;
using TrueCraft.Extensions;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Physics;
using TrueCraft.TerrainGen;
using TrueCraft.Windows;
using TrueCraft.World;

namespace TrueCraft.Client
{
	public class MultiplayerClient : IAABBEntity, INotifyPropertyChanged, IDisposable 
	{
		private readonly CancellationTokenSource cancel;

		private readonly PacketHandler[] PacketHandlers;

		private long connected;
		private int hotbarSelection;

		private SemaphoreSlim sem = new SemaphoreSlim(1, 1);

		public TraceSource Trace { get; }
		public ClientTraceWriter TraceListener { get; }

		public MultiplayerClient(TrueCraftUser user)
		{
			TraceListener = new ClientTraceWriter();
			Trace = new TraceSource(nameof(MultiplayerClient));
			Trace.Switch.Level = SourceLevels.All;
			Trace.Listeners.Add(TraceListener);

			User = user;
			Client = new TcpClient();
			PacketReader = new PacketReader(Trace);
			PacketReader.RegisterCorePackets();
			PacketHandlers = new PacketHandler[0x100];
			Handlers.PacketHandlers.RegisterHandlers(this);
			World = new ReadOnlyWorld();
			Inventory = new InventoryWindow(null);
			var repo = new BlockRepository();
			repo.DiscoverBlockProviders();
			World.World.BlockRepository = repo;
			World.World.ChunkProvider = new EmptyGenerator();
			Physics = new PhysicsEngine(World.World, repo);
			SocketPool = new SocketAsyncEventArgsPool(100, 200, 65536);
			connected = 0;
			cancel = new CancellationTokenSource();
			Health = 20;
			var crafting = new CraftingRepository();
			CraftingRepository = crafting;
			crafting.DiscoverRecipes();
		}

		public TrueCraftUser User { get; set; }
		public ReadOnlyWorld World { get; }
		public PhysicsEngine Physics { get; set; }
		public bool LoggedIn { get; internal set; }
		public int EntityID { get; internal set; }
		public InventoryWindow Inventory { get; set; }
		public int Health { get; set; }
		public IWindow CurrentWindow { get; set; }
		public ICraftingRepository CraftingRepository { get; set; }

		public bool Connected => Interlocked.Read(ref connected) == 1;

		public int HotbarSelection
		{
			get => hotbarSelection;
			set
			{
				hotbarSelection = value;
				QueuePacket(new ChangeHeldItemPacket
					{Slot = (short) value});
			}
		}

		private TcpClient Client { get; }
		private IMcStream Stream { get; set; }
		private PacketReader PacketReader { get; }

		private SocketAsyncEventArgsPool SocketPool { get; }

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<ChatMessageEventArgs> ChatMessage;
		public event EventHandler<ChunkEventArgs> ChunkModified;
		public event EventHandler<ChunkEventArgs> ChunkLoaded;
		public event EventHandler<ChunkEventArgs> ChunkUnloaded;
		public event EventHandler<BlockChangeEventArgs> BlockChanged;

		public void RegisterPacketHandler(byte packetId, PacketHandler handler)
		{
			PacketHandlers[packetId] = handler;
		}

		public void Connect(IPEndPoint endPoint)
		{
			var args = new SocketAsyncEventArgs();
			args.Completed += Connection_Completed;
			args.RemoteEndPoint = endPoint;

			if (!Client.Client.ConnectAsync(args))
				Connection_Completed(this, args);
		}

		private void Connection_Completed(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				Interlocked.CompareExchange(ref connected, 1, 0);

				Physics.AddEntity(this);
				StartReceive();
				QueuePacket(new HandshakePacket(User.Username));
			}
			else
				throw new Exception("Could not connect to server!");
		}

		public void Disconnect()
		{
			if (!Connected)
				return;

			QueuePacket(new DisconnectPacket("Disconnecting"));

			Interlocked.CompareExchange(ref connected, 0, 1);
		}

		public void SendMessage(string message)
		{
			var parts = message.Split('\n');
			foreach (var part in parts)
				QueuePacket(new ChatMessagePacket(part));
		}

		public void QueuePacket(IPacket packet)
		{
			if (!Connected || Client != null && !Client.Connected)
				return;

			if (packet.ID != Ids.PlayerGrounded) // prevents noise, as this is emitted constantly
				Trace.TraceData(TraceEventType.Verbose, 0, $"queuing packet #{packet.ID:X2} ({packet.GetType().Name})");

			using (var writeStream = new MemoryStream())
			{
				using (var ms = new McStream(writeStream))
				{
					ms.WriteUInt8(packet.ID);
					packet.WritePacket(ms);
				}

				var buffer = writeStream.ToArray();

				var args = new SocketAsyncEventArgs();
				args.UserToken = packet;
				args.Completed += OperationCompleted;
				args.SetBuffer(buffer, 0, buffer.Length);

				if (Client != null && !Client.Client.SendAsync(args))
					OperationCompleted(this, args);
			}
		}

		private void StartReceive()
		{
			var args = SocketPool.Get();
			args.Completed += OperationCompleted;

			if (!Client.Client.ReceiveAsync(args))
				OperationCompleted(this, args);
		}

		private void OperationCompleted(object sender, SocketAsyncEventArgs e)
		{
			e.Completed -= OperationCompleted;

			switch (e.LastOperation)
			{
				case SocketAsyncOperation.Receive:
					ProcessNetwork(e);
					SocketPool.Add(e);
					break;
				case SocketAsyncOperation.Send:
					var packet = e.UserToken as IPacket;

					if (packet is DisconnectPacket)
					{
						Client.Client.Shutdown(SocketShutdown.Send);
						Client.Close();
						cancel.Cancel();
					}
					e.SetBuffer(null, 0, 0);
					break;
			}
		}

		private void ProcessNetwork(SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
			{
				var newArgs = SocketPool.Get();
				newArgs.Completed += OperationCompleted;

				if (Client != null && !Client.Client.ReceiveAsync(newArgs))
					OperationCompleted(this, newArgs);

				try
				{
					sem.Wait(cancel.Token);
				}
				catch (OperationCanceledException ex)
				{
					Trace.TraceData(TraceEventType.Error, 0, ex);
					return;
				}

				var packets = PacketReader.ReadPackets(this, e.Buffer, e.Offset, e.BytesTransferred, false);

				foreach (var packet in packets)
				{
					Trace.TraceEvent(TraceEventType.Verbose, 0, $"received {packet.GetType().Name}");

					var handler = PacketHandlers[packet.ID];
					if (handler == null)
					{
						Trace.TraceEvent(TraceEventType.Warning, 0, $"no handler found for packet {packet.ID:X2} ({packet.GetType().Name})");
						continue;
					}

					handler.Invoke(packet, this);
				}

				if (sem.CurrentCount == 0)
					sem?.Release();
			}
			else
				Disconnect();
		}

		protected internal void OnChatMessage(ChatMessageEventArgs e)
		{
			ChatMessage?.Invoke(this, e);
		}

		protected internal void OnChunkLoaded(ChunkEventArgs e)
		{
			ChunkLoaded?.Invoke(this, e);
		}

		protected internal void OnChunkUnloaded(ChunkEventArgs e)
		{
			ChunkUnloaded?.Invoke(this, e);
		}

		protected internal void OnChunkModified(ChunkEventArgs e)
		{
			ChunkModified?.Invoke(this, e);
		}

		protected internal void OnBlockChanged(BlockChangeEventArgs e)
		{
			BlockChanged?.Invoke(this, e);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Disconnect();

				sem?.Dispose();
			}

			sem = null;
		}

		~MultiplayerClient() => Dispose(false);

		#region IAABBEntity implementation

		public const double Width = 0.6;
		public const double Height = 1.62;
		public const double Depth = 0.6;

		public void TerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection)
		{
			// This space intentionally left blank
		}

		public BoundingBox BoundingBox
		{
			get
			{
				var pos = Position - new Vector3((float) (Width / 2), 0f, (float) (Depth / 2));
				return new BoundingBox(pos, pos + Size.AsVector3());
			}
		}

		public Size Size => new Size(Width, Height, Depth);

		#endregion

		#region IPhysicsEntity implementation

		public bool BeginUpdate()
		{
			return true;
		}

		public void EndUpdate(Vector3 newPosition)
		{
			Position = newPosition;
		}

		public float Yaw { get; set; }
		public float Pitch { get; set; }

		internal Vector3 _Position;

		public Vector3 Position
		{
			get => _Position;
			set
			{
				if (_Position != value)
				{
					QueuePacket(new PlayerPositionAndLookPacket(value.X, value.Y, value.Y + Height,
						value.Z, Yaw, Pitch, false));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
				}

				_Position = value;
			}
		}

		public Vector3 Velocity { get; set; }

		public float AccelerationDueToGravity => 1.6f;

		public float Drag => 0.40f;

		public float TerminalVelocity => 78.4f;

		#endregion
	}
}