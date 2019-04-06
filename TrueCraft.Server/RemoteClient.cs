using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zlib;
using Microsoft.Xna.Framework;
using TrueCraft.Entities;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Profiling;
using TrueCraft.Serialization;
using TrueCraft.Serialization.Tags;
using TrueCraft.Server.Exceptions;
using TrueCraft.Windows;
using TrueCraft.World;

namespace TrueCraft.Server
{
	public class RemoteClient : IRemoteClient, IEventSubject
	{
		private readonly ServerConfiguration _configuration;
		private readonly CancellationTokenSource _cancel;
		private IEntity _entity;
		private long _disconnected;

		public RemoteClient(IMultiPlayerServer server, ServerConfiguration configuration, IPacketReader packetReader,
			PacketHandler[] packetHandlers, Socket connection)
		{
			_configuration = configuration;
			_cancel = new CancellationTokenSource();

			LoadedChunks = new HashSet<Coordinates2D>();
			Server = server;
			Inventory = new InventoryWindow(server.CraftingRepository);
			InventoryWindow.WindowChange += HandleWindowChange;
			SelectedSlot = InventoryWindow.HotbarIndex;
			CurrentWindow = InventoryWindow;
			ItemStaging = ItemStack.EmptyStack;
			KnownEntities = new List<IEntity>();
			Disconnected = false;
			EnableLogging = server.EnableClientLogging;
			NextWindowID = 1;
			Connection = connection;
			SocketPool = new SocketAsyncEventArgsPool(100, 200, 65536);
			PacketReader = packetReader;
			PacketHandlers = packetHandlers;

			StartReceive();
		}

		/// <summary>
		///  A list of entities that this client is aware of.
		/// </summary>
		internal List<IEntity> KnownEntities { get; set; }

		internal sbyte NextWindowID { get; set; }
		public bool LoggedIn { get; internal set; }
		public ItemStack ItemStaging { get; set; }
		public IWindow CurrentWindow { get; internal set; }

		public Socket Connection { get; }

		private SocketAsyncEventArgsPool SocketPool { get; }

		public IPacketReader PacketReader { get; }

		private PacketHandler[] PacketHandlers { get; }

		public InventoryWindow InventoryWindow => Inventory as InventoryWindow;

		public int ChunkRadius { get; set; }
		internal HashSet<Coordinates2D> LoadedChunks { get; set; }

		public event EventHandler Disposed;

		public void Dispose()
		{
			Dispose(true);
		}

		public IMcStream McStream { get; internal set; }
		public string Username { get; internal set; }
		public IMultiPlayerServer Server { get; set; }
		public IWorld World { get; internal set; }
		public IWindow Inventory { get; }
		public short SelectedSlot { get; internal set; }
		public bool EnableLogging { get; set; }
		public DateTime ExpectedDigComplete { get; set; }

		public bool Disconnected
		{
			get => Interlocked.Read(ref _disconnected) == 1;
			set => Interlocked.CompareExchange(ref _disconnected, value ? 1 : 0, value ? 0 : 1);
		}

		public IEntity Entity
		{
			get => _entity;
			internal set
			{
				if (_entity is PlayerEntity player)
					player.PickUpItem -= HandlePickUpItem;
				_entity = value;
				player = _entity as PlayerEntity;
				if (player != null)
					player.PickUpItem += HandlePickUpItem;
			}
		}

		public ItemStack SelectedItem => Inventory[SelectedSlot];

		public bool DataAvailable => true;

		public bool Load()
		{
			try
			{
				var path = Bootstrap.ResolvePath(Path.Combine("players", $"{Username}.nbt"));
				if (_configuration.Singleplayer)
					path = Bootstrap.ResolvePath(Path.Combine(((World.World) World).BaseDirectory, "player.nbt"));

				if (!File.Exists(path))
					return false;

				Server.Trace.TraceInformation($"Loading {path}...");

				var nbt = new NbtFile(path);
				Entity.Position = new Vector3(
					(float) nbt.RootTag["position"][0].DoubleValue,
					(float) nbt.RootTag["position"][1].DoubleValue,
					(float) nbt.RootTag["position"][2].DoubleValue);

				Inventory.SetSlots(((NbtList) nbt.RootTag["inventory"]).Select(t => ItemStack.FromNbt(t as NbtCompound)).ToArray());
				((PlayerEntity) Entity).Health = nbt.RootTag["health"].ShortValue;
				Entity.Yaw = nbt.RootTag["yaw"].FloatValue;
				Entity.Pitch = nbt.RootTag["pitch"].FloatValue;
			}
			catch(Exception ex)
			{
				Server.Trace.TraceData(TraceEventType.Critical, 0, "Error loading player", ex);
			}

			return true;
		}

		public void Save()
		{
			string path = Bootstrap.ResolvePath(_configuration.Singleplayer
				? Bootstrap.ResolvePath(Path.Combine(((World.World) World).BaseDirectory, "player.nbt"))
				: Bootstrap.ResolvePath(Path.Combine("players", Username + ".nbt")));

			Directory.CreateDirectory(Path.GetDirectoryName(path));

			if (Entity == null) // I didn't think this could happen but null reference exceptions have been reported here
				return;

			var nbt = new NbtFile(new NbtCompound("player", new NbtTag[]
				{
					new NbtString("username", Username),
					new NbtList("position", new[]
					{
						new NbtDouble(Entity.Position.X),
						new NbtDouble(Entity.Position.Y),
						new NbtDouble(Entity.Position.Z)
					}),
					new NbtList("inventory", Inventory.GetSlots().Select(s => s.ToNbt())),
					new NbtShort("health", (Entity as PlayerEntity).Health),
					new NbtFloat("yaw", Entity.Yaw),
					new NbtFloat("pitch", Entity.Pitch)
				}
			));

			nbt.SaveToFile(path, NbtCompression.ZLib);
		}

		public void OpenWindow(IWindow window)
		{
			CurrentWindow = window;
			window.Client = this;
			window.Id = NextWindowID++;
			if (NextWindowID < 0) NextWindowID = 1;
			QueuePacket(new OpenWindowPacket(window.Id, window.Type, window.Name, 0));
			QueuePacket(new WindowItemsPacket(window.Id, window.GetSlots()));
			window.WindowChange += HandleWindowChange;
		}

		public void MaybeEchoToClient(string message, params object[] parameters)
		{
			if (EnableLogging)
			{
				SendMessage(ChatColor.Gray + string.Format(message, parameters));
			}
		}

		public void QueuePacket(IPacket packet)
		{
			if (Disconnected || Connection != null && !Connection.Connected)
				return;

			if(!Constants.IgnoredPacketIds.Contains(packet.Id))
				Server.Trace.TraceData(TraceEventType.Verbose, 0, $"queuing packet #{packet.Id:X2} ({packet.GetType().Name})");

			using (var writeStream = new MemoryStream())
			{
				using (var ms = new McStream(writeStream))
				{
					writeStream.WriteByte(packet.Id);
					packet.WritePacket(ms);
				}

				var buffer = writeStream.ToArray();

				var args = new SocketAsyncEventArgs();
				args.UserToken = packet;
				args.Completed += OperationCompleted;
				args.SetBuffer(buffer, 0, buffer.Length);

				if (Connection == null)
					return;

				if (!Connection.SendAsync(args))
					OperationCompleted(this, args);
			}
		}

		public void Disconnect()
		{
			if (Disconnected)
				return;

			Disconnected = true;

			_cancel.Cancel();

			Connection.Shutdown(SocketShutdown.Send);

			var args = new SocketAsyncEventArgs();
			args.Completed += OperationCompleted;
			Connection.DisconnectAsync(args);
		}

		public void SendMessage(string message)
		{
			var parts = message.Split('\n');
			foreach (var part in parts)
				QueuePacket(new ChatMessagePacket(part));
		}

		private void HandlePickUpItem(object sender, EntityEventArgs e)
		{
			var packet = new CollectItemPacket(e.Entity.EntityId, Entity.EntityId);
			QueuePacket(packet);
			var manager = Server.GetEntityManagerForWorld(World);
			foreach (var client in manager.ClientsForEntity(Entity))
				client.QueuePacket(packet);
			Inventory.PickUpStack(((ItemEntity) e.Entity).Item);
		}

		public void CloseWindow(bool clientInitiated = false)
		{
			if (!clientInitiated)
				QueuePacket(new CloseWindowPacket(CurrentWindow.Id));
			CurrentWindow.CopyToInventory(Inventory);
			CurrentWindow.Dispose();
			CurrentWindow = InventoryWindow;
		}

		private void StartReceive()
		{
			var args = SocketPool.Get();
			args.Completed += OperationCompleted;

			if (!Connection.ReceiveAsync(args))
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
						Server.DisconnectClient(this);
					e.SetBuffer(null, 0, 0);
					break;
				case SocketAsyncOperation.Disconnect:
					Connection.Close();
					break;
			}

			if (Connection != null)
				if (!Connection.Connected && !Disconnected)
					Server.DisconnectClient(this);
		}

		private void ProcessNetwork(SocketAsyncEventArgs e)
		{
			if (Connection == null || !Connection.Connected)
				return;

			lock (this)
			{
				// Server.Trace.TraceData(TraceEventType.Verbose, 0, "entered socket monitor");

				if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
				{
					var newArgs = SocketPool.Get();
					newArgs.Completed += OperationCompleted;

					if (!Connection.ReceiveAsync(newArgs))
					{
						OperationCompleted(this, newArgs);
					}

					var packets = PacketReader.ReadPackets(this, e.Buffer, e.Offset, e.BytesTransferred);
					try
					{
						foreach (var packet in packets)
						{
							if(!Constants.IgnoredPacketIds.Contains(packet.Id))
								Server.Trace.TraceData(TraceEventType.Verbose, 0, $"received packet {packet.Id:X2} ({packet.GetType().Name})");

							if (PacketHandlers[packet.Id] != null)
							{
								try
								{
									PacketHandlers[packet.Id](packet, this, Server);
								}
								catch (PlayerDisconnectException)
								{
									Server.DisconnectClient(this);
								}
								catch (Exception ex)
								{
									Server.Trace.TraceData(TraceEventType.Error, 0, "disconnecting client due to exception in network worker", ex);
									Server.DisconnectClient(this);
								}
							}
							else
							{
								var message = $"unhandled packet {packet.Id:X2} ({packet.GetType().Name})";
								MaybeEchoToClient(message);
								Server.Trace.TraceData(TraceEventType.Error, 0, message);
							}
						}
					}
					catch (NotSupportedException)
					{
						Server.Trace.TraceEvent(TraceEventType.Error, 0, "disconnecting client due to unsupported packet received.");
					}
				}
				else
				{
					Server.DisconnectClient(this);
				}
			}

			// Server.Trace.TraceData(TraceEventType.Verbose, 0, "exited socket monitor");
		}

		internal void ExpandChunkRadius(IMultiPlayerServer server)
		{
			if (Disconnected)
				return;
			Task.Factory.StartNew(() =>
			{
				if (ChunkRadius < 8) // TODO: Allow customization of this number
				{
					ChunkRadius++;
					server.Scheduler.ScheduleEvent(Constants.Events.ClientUpdateChunks, this, TimeSpan.Zero, s => UpdateChunks());
					server.Scheduler.ScheduleEvent(Constants.Events.RemoteChunks, this, TimeSpan.FromSeconds(1), ExpandChunkRadius);
				}
			});
		}

		internal void SendKeepAlive(IMultiPlayerServer server)
		{
			QueuePacket(new KeepAlivePacket());
			server.Scheduler.ScheduleEvent(Constants.Events.RemoteKeepAlive, this, TimeSpan.FromSeconds(10), SendKeepAlive);
		}

		public void UpdateChunks(bool blockingCall = false)
		{
			lock (this)
			{
				var toLoad = new List<Tuple<Coordinates2D, IChunk>>();

				var count = ChunkRadius * 2 * ChunkRadius * 2;
				Server.Trace.TraceEvent(TraceEventType.Verbose, 0, $"... counted {count} total chunks");

				Profiler.Start("client.new-chunks");
				var newChunks = new HashSet<Coordinates2D>();
				for (var x = -ChunkRadius; x < ChunkRadius; x++)
				{
					for (var z = -ChunkRadius; z < ChunkRadius; z++)
					{
						if (Entity == null)
						{
							Server.Trace.TraceEvent(TraceEventType.Error, 0, "aborting chunk update, entity not set");
							return;
						}

						var coords = new Coordinates2D(((int) Entity.Position.X >> 4) + x, ((int) Entity.Position.Z >> 4) + z);
						newChunks.Add(coords);

						if (!LoadedChunks.Contains(coords))
						{
							Server.Trace.TraceEvent(TraceEventType.Verbose, 0, $"... loading new chunk");

							var chunk = World.GetChunk(coords, blockingCall);
							if (chunk == null)
							{
								Server.Trace.TraceEvent(TraceEventType.Warning, 0, $"no chunk found @({coords.X}, {coords.Z})");
							}
							else
							{
								Server.Trace.TraceEvent(TraceEventType.Verbose, 0, $"\t@({chunk.X}, {chunk.Z})");
								toLoad.Add(new Tuple<Coordinates2D, IChunk>(coords, chunk));
							}
						}
						else
						{
							Server.Trace.TraceEvent(TraceEventType.Verbose, 0, $"... skipping already loaded chunk @({coords.X}, {coords.Z})");
						}

						count--;
						//if (count > 0 && count % 100 == 0)
							Server.Trace.TraceEvent(TraceEventType.Start, 0, $"... {count} remaining");
					}
				}

				Profiler.Done();

				Server.Trace.TraceEvent(TraceEventType.Start, 0, "... encoding chunks");
				if (blockingCall)
					EncodeChunks(toLoad);
				else
					Task.Factory.StartNew(() => EncodeChunks(toLoad));

				Server.Trace.TraceEvent(TraceEventType.Start, 0, "... interleaving old chunks with new");
				Profiler.Start("client.old-chunks");
				LoadedChunks.IntersectWith(newChunks);
				Profiler.Done();

				Server.Trace.TraceEvent(TraceEventType.Start, 0, "... updating client entities");
				Profiler.Start("client.update-entities");
				((EntityManager) Server.GetEntityManagerForWorld(World)).UpdateClientEntities(this);
				Profiler.Done();

				Server.Trace.TraceEvent(TraceEventType.Start, 0, "done.");
			}
		}

		private void EncodeChunks(IEnumerable<Tuple<Coordinates2D, IChunk>> map)
		{
			Profiler.Start("client.encode-chunks");
			foreach (var (coords, maybeChunk) in map)
			{
				var chunk = maybeChunk ?? World.GetChunk(coords);
				chunk.LastAccessed = DateTime.UtcNow;
				LoadChunk(chunk);
			}
			Profiler.Done();
		}

		internal void UnloadAllChunks()
		{
			while (LoadedChunks.Any()) UnloadChunk(LoadedChunks.First());
		}

		internal void LoadChunk(IChunk chunk)
		{
			QueuePacket(new ChunkPreamblePacket(chunk.Coordinates.X, chunk.Coordinates.Z));
			QueuePacket(CreatePacket(chunk));
			Server.Scheduler.ScheduleEvent("client.finalize-chunks", this, TimeSpan.Zero, server =>
			{
				LoadedChunks.Add(chunk.Coordinates);
				foreach (var kvp in chunk.TileEntities)
				{
					var coords = kvp.Key;
					var descriptor = new BlockDescriptor
					{
						Coordinates = coords + new Coordinates3D(chunk.X, 0, chunk.Z),
						Metadata = chunk.GetMetadata(coords),
						Id = chunk.GetBlockID(coords),
						BlockLight = chunk.GetBlockLight(coords),
						SkyLight = chunk.GetSkyLight(coords)
					};
					var provider = Server.BlockRepository.GetBlockProvider(descriptor.Id);
					provider.TileEntityLoadedForClient(descriptor, World, kvp.Value, this);
				}});
		}

		internal void UnloadChunk(Coordinates2D position)
		{
			QueuePacket(new ChunkPreamblePacket(position.X, position.Z, false));
			LoadedChunks.Remove(position);
		}

		private void HandleWindowChange(object sender, WindowChangeEventArgs e)
		{
			if (!(sender is InventoryWindow))
			{
				QueuePacket(new SetSlotPacket(((IWindow) sender).Id, (short) e.SlotIndex, e.Value.Id, e.Value.Count,
					e.Value.Metadata));
				return;
			}

			QueuePacket(new SetSlotPacket(0, (short) e.SlotIndex, e.Value.Id, e.Value.Count, e.Value.Metadata));

			if (e.SlotIndex == SelectedSlot)
			{
				var notified = Server.GetEntityManagerForWorld(World).ClientsForEntity(Entity);
				foreach (var c in notified)
					c.QueuePacket(new EntityEquipmentPacket(Entity.EntityId, 0, SelectedItem.Id,
						SelectedItem.Metadata));
			}

			if (e.SlotIndex >= InventoryWindow.ArmorIndex && e.SlotIndex < InventoryWindow.ArmorIndex + InventoryWindow.Armor.Length)
			{
				var slot = (short) (4 - (e.SlotIndex - InventoryWindow.ArmorIndex));
				var notified = Server.GetEntityManagerForWorld(World).ClientsForEntity(Entity);
				foreach (var c in notified)
					c.QueuePacket(new EntityEquipmentPacket(Entity.EntityId, slot, e.Value.Id, e.Value.Metadata));
			}
		}

		private static ChunkDataPacket CreatePacket(IChunk chunk)
		{
			var x = chunk.Coordinates.X;
			var z = chunk.Coordinates.Z;

			Profiler.Start("client.encode-chunks.compress");
			byte[] result;
			using (var ms = new MemoryStream())
			{
				using (var deflate = new ZlibStream(new MemoryStream(chunk.Data),
					CompressionMode.Compress,
					CompressionLevel.BestSpeed))
					deflate.CopyTo(ms);
				result = ms.ToArray();
			}
			Profiler.Done();

			return new ChunkDataPacket(x * Chunk.Width, 0, z * Chunk.Depth,
				Chunk.Width, Chunk.Height, Chunk.Depth, result);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				while (!PacketReader.Processors.TryRemove(this, out var processor))
					Thread.Sleep(1);

				Disconnect();
				Disposed?.Invoke(this, null);
			}
		}
	}
}