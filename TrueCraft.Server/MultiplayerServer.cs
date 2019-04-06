using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using TrueCraft.Extensions;
using TrueCraft.Lighting;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Profiling;
using TrueCraft.World;

namespace TrueCraft.Server
{
	public class MultiPlayerServer : IMultiPlayerServer, IDisposable
	{
		private static readonly int MillisecondsPerTick = 1000 / 20;
		private readonly PacketHandler[] PacketHandlers;

		private bool _BlockUpdatesEnabled = true;
		private readonly ConcurrentBag<Tuple<IWorld, IChunk>> ChunksToSchedule;

		private readonly Timer EnvironmentWorker;
		private TcpListener Listener;

		private readonly QueryProtocol QueryProtocol;
		private readonly Stopwatch Time;

		public TraceSource Trace { get; }

		public MultiPlayerServer(ServerConfiguration configuration)
		{
			Trace = new TraceSource(configuration.ServerAddress);
			Trace.Switch.Level = SourceLevels.All;
			Trace.Listeners.Add(new ServerTraceWriter());

			ServerConfiguration = configuration;

			PacketReader = new PacketReader(Trace);
			Clients = new List<IRemoteClient>();
			EnvironmentWorker = new Timer(DoEnvironment);
			PacketHandlers = new PacketHandler[0x100];
			Worlds = new List<IWorld>();
			EntityManagers = new List<IEntityManager>();
			Scheduler = new EventScheduler(this);
			var blockRepository = new BlockRepository();
			blockRepository.DiscoverBlockProviders();
			BlockRepository = blockRepository;
			var itemRepository = new ItemRepository();
			itemRepository.DiscoverItemProviders();
			ItemRepository = itemRepository;
			BlockProvider.ItemRepository = ItemRepository;
			BlockProvider.BlockRepository = BlockRepository;
			var craftingRepository = new CraftingRepository();
			craftingRepository.DiscoverRecipes();
			CraftingRepository = craftingRepository;
			PendingBlockUpdates = new Queue<BlockUpdate>();
			EnableClientLogging = configuration.EnableClientLogging;
			QueryProtocol = new QueryProtocol(this, configuration);
			WorldLighters = new List<WorldLighting>();
			ChunksToSchedule = new ConcurrentBag<Tuple<IWorld, IChunk>>();
			Time = new Stopwatch();

			AccessConfiguration = Configuration.LoadConfiguration<AccessConfiguration>(Bootstrap.ResolvePath("access.yaml"));

			((PacketReader)PacketReader).RegisterCorePackets(typeof(KeepAlivePacket).Assembly);
			Handlers.PacketHandlers.RegisterHandlers(this);
		}

		public IList<IEntityManager> EntityManagers { get; }
		public IList<WorldLighting> WorldLighters { get; set; }
		private Queue<BlockUpdate> PendingBlockUpdates { get; }

		internal bool ShuttingDown { get; private set; }

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		public event EventHandler<ChatMessageEventArgs> ChatMessageReceived;
		public event EventHandler<PlayerJoinedQuitEventArgs> PlayerJoined;
		public event EventHandler<PlayerJoinedQuitEventArgs> PlayerQuit;

		public ServerConfiguration ServerConfiguration { get; internal set; }
		public IAccessConfiguration AccessConfiguration { get; internal set; }

		public IPacketReader PacketReader { get; }
		public IList<IRemoteClient> Clients { get; }
		public IList<IWorld> Worlds { get; }
		public EventScheduler Scheduler { get; }
		public IBlockRepository BlockRepository { get; }
		public IItemRepository ItemRepository { get; }
		public ICraftingRepository CraftingRepository { get; }
		public bool EnableClientLogging { get; set; }
		public IPEndPoint EndPoint { get; private set; }

		public bool BlockUpdatesEnabled
		{
			get => _BlockUpdatesEnabled;
			set
			{
				_BlockUpdatesEnabled = value;
				if (_BlockUpdatesEnabled) ProcessBlockUpdates();
			}
		}

		public object ClientLock { get; } = new object();

		public void RegisterPacketHandler(byte packetId, PacketHandler handler)
		{
			PacketHandlers[packetId] = handler;
		}

		public void Start(IPEndPoint endPoint)
		{
			Scheduler.DisabledEvents.Clear();
			if (ServerConfiguration.DisabledEvents != null)
				ServerConfiguration.DisabledEvents.ToList().ForEach(
					ev => Scheduler.DisabledEvents.Add(ev));
			ShuttingDown = false;
			Time.Reset();
			Time.Start();
			Listener = new TcpListener(endPoint);
			Listener.Start();
			EndPoint = (IPEndPoint) Listener.LocalEndpoint;

			var args = new SocketAsyncEventArgs();
			args.Completed += AcceptClient;

			if (!Listener.Server.AcceptAsync(args))
				AcceptClient(this, args);

			Trace.TraceEvent(TraceEventType.Information, 0, $"Running TrueCraft server on {EndPoint}");
			EnvironmentWorker.Change(MillisecondsPerTick, Timeout.Infinite);
			if (ServerConfiguration.Query)
				QueryProtocol.Start();
		}

		public void Stop()
		{
			ShuttingDown = true;
			Listener.Stop();
			if (ServerConfiguration.Query)
				QueryProtocol.Stop();
			foreach (var w in Worlds)
				w.Save();
			foreach (var c in Clients)
				DisconnectClient(c);
		}

		public void AddWorld(IWorld world)
		{
			Worlds.Add(world);
			world.BlockRepository = BlockRepository;
			world.ChunkGenerated += HandleChunkGenerated;
			world.ChunkLoaded += HandleChunkLoaded;
			world.BlockChanged += HandleBlockChanged;
			var manager = new EntityManager(this, world);
			EntityManagers.Add(manager);
			var lighter = new WorldLighting(world, BlockRepository);
			WorldLighters.Add(lighter);
			foreach (var chunk in world)
				HandleChunkLoaded(world, new ChunkLoadedEventArgs(chunk));
		}

		public IEntityManager GetEntityManagerForWorld(IWorld world)
		{
			for (var i = 0; i < EntityManagers.Count; i++)
			{
				var manager = EntityManagers[i] as EntityManager;
				if (manager.World == world)
					return manager;
			}

			return null;
		}

		public void SendMessage(string message, params object[] args)
		{
			var compiled = string.Format(message, args);
			var parts = compiled.Split('\n');
			foreach (var client in Clients)
			foreach (var part in parts)
				client.SendMessage(part);

			Trace.TraceEvent(TraceEventType.Information, 0, ChatColor.RemoveColors(compiled));
		}

		public void DisconnectClient(IRemoteClient client)
		{
			lock (ClientLock)
				Clients.Remove(client);

			if (client.Disconnected)
				return;

			client.Disconnect();

			if (client.LoggedIn)
			{
				SendMessage($"{ChatColor.Yellow}{client.Username} has left the server.");
				GetEntityManagerForWorld(client.World).DespawnEntity(client.Entity);
				GetEntityManagerForWorld(client.World).FlushDespawns();
			}

			client.Save();
			client.Disconnect();
			OnPlayerQuit(new PlayerJoinedQuitEventArgs(client));

			client.Dispose();
		}

		public bool PlayerIsWhitelisted(string client)
		{
			return AccessConfiguration.Whitelist.Contains(client, StringComparer.CurrentCultureIgnoreCase);
		}

		public bool PlayerIsBlacklisted(string client)
		{
			return AccessConfiguration.Blacklist.Contains(client, StringComparer.CurrentCultureIgnoreCase);
		}

		public bool PlayerIsOp(string client)
		{
			return AccessConfiguration.Oplist.Contains(client, StringComparer.CurrentCultureIgnoreCase);
		}

		private void HandleChunkLoaded(object sender, ChunkLoadedEventArgs e)
		{
			if (ServerConfiguration.EnableEventLoading)
				ChunksToSchedule.Add(new Tuple<IWorld, IChunk>(sender as IWorld, e.Chunk));

			if (ServerConfiguration.EnableLighting)
			{
				var lighter = WorldLighters.SingleOrDefault(l => l.World == sender);
				lighter?.InitialLighting(e.Chunk, false);
			}
		}

		private void HandleBlockChanged(object sender, BlockChangeEventArgs e)
		{
			// TODO: Propagate lighting changes to client (not possible with beta 1.7.3 protocol)
			if (e.NewBlock.ID != e.OldBlock.ID || e.NewBlock.Metadata != e.OldBlock.Metadata)
			{
				for (int i = 0, ClientsCount = Clients.Count; i < ClientsCount; i++)
				{
					var client = (RemoteClient) Clients[i];

					// TODO: Confirm that the client knows of this block
					if (client.LoggedIn && client.World == sender)
						client.QueuePacket(new BlockChangePacket(e.Position.X, (sbyte) e.Position.Y, e.Position.Z,
							(sbyte) e.NewBlock.ID, (sbyte) e.NewBlock.Metadata));
				}

				PendingBlockUpdates.Enqueue(new BlockUpdate {Coordinates = e.Position, World = sender as IWorld});
				ProcessBlockUpdates();
				if (ServerConfiguration.EnableLighting)
				{
					var lighter = WorldLighters.SingleOrDefault(l => l.World == sender);
					if (lighter != null)
					{
						var posA = e.Position;
						posA.Y = 0;
						var posB = e.Position;
						posB.Y = World.World.Height;
						posB.X++;
						posB.Z++;
						lighter.EnqueueOperation(new BoundingBox(posA.AsVector3(), posB.AsVector3()), true);
						lighter.EnqueueOperation(new BoundingBox(posA.AsVector3(), posB.AsVector3()), false);
					}
				}
			}
		}

		private void HandleChunkGenerated(object sender, ChunkLoadedEventArgs e)
		{
			if (ServerConfiguration.EnableLighting)
			{
				var lighter = new WorldLighting(sender as IWorld, BlockRepository);
				lighter.InitialLighting(e.Chunk, false);
			}
			else
				for (var i = 0; i < e.Chunk.SkyLight.Length * 2; i++)
					e.Chunk.SkyLight[i] = 0xF;

			HandleChunkLoaded(sender, e);
		}

		private void ScheduleUpdatesForChunk(IWorld world, IChunk chunk)
		{
			chunk.UpdateHeightMap();
			var w = chunk.Coordinates.X * Chunk.Width;
			var d = chunk.Coordinates.Z * Chunk.Depth;

			for (byte x = 0; x < Chunk.Width; x++)
			for (byte z = 0; z < Chunk.Depth; z++)
			for (var y = 0; y < chunk.GetHeight(x, z); y++)
			{
				Coordinates3D _coords;
				_coords.X = x;
				_coords.Y = y;
				_coords.Z = z;
				var id = chunk.GetBlockID(_coords);
				if (id == 0)
					continue;
				Coordinates3D coords;
				coords.X = w + x;
				coords.Y = y;
				coords.Z = d + z;
				var provider = BlockRepository.GetBlockProvider(id);
				provider.BlockLoadedFromChunk(coords, this, world);
			}
		}

		private void ProcessBlockUpdates()
		{
			if (!BlockUpdatesEnabled)
				return;
			var adjacent = new[]
			{
				Coordinates3D.Up, Coordinates3D.Down,
				Coordinates3D.Left, Coordinates3D.Right,
				Coordinates3D.Forwards, Coordinates3D.Backwards
			};
			while (PendingBlockUpdates.Count != 0)
			{
				var update = PendingBlockUpdates.Dequeue();
				var source = update.World.GetBlockData(update.Coordinates);
				foreach (var offset in adjacent)
				{
					var descriptor = update.World.GetBlockData(update.Coordinates + offset);
					var provider = BlockRepository.GetBlockProvider(descriptor.ID);
					provider?.BlockUpdate(descriptor, source, this, update.World);
				}
			}
		}

		public void OnChatMessageReceived(ChatMessageEventArgs e)
		{
			ChatMessageReceived?.Invoke(this, e);
		}

		public void OnPlayerJoined(PlayerJoinedQuitEventArgs e)
		{
			PlayerJoined?.Invoke(this, e);
		}

		public void OnPlayerQuit(PlayerJoinedQuitEventArgs e)
		{
			PlayerQuit?.Invoke(this, e);
		}

		private void AcceptClient(object sender, SocketAsyncEventArgs args)
		{
			try
			{
				var client = new RemoteClient(this, ServerConfiguration, PacketReader, PacketHandlers,
					args.AcceptSocket);

				lock (ClientLock)
					Clients.Add(client);
			}
			catch(Exception ex)
			{
				Trace.TraceData(TraceEventType.Critical, 0, "error accepting client", ex);
			}
			finally
			{
				args.AcceptSocket = null;

				if (!ShuttingDown && !Listener.Server.AcceptAsync(args))
					AcceptClient(this, args);
			}
		}

		private void DoEnvironment(object discarded)
		{
			if (ShuttingDown)
				return;

			var start = Time.ElapsedMilliseconds;
			var limit = Time.ElapsedMilliseconds + MillisecondsPerTick;
			Profiler.Start("environment");

			Scheduler.Update();

			Profiler.Start("environment.entities");
			foreach (var manager in EntityManagers) manager.Update();
			Profiler.Done();

			if (ServerConfiguration.EnableLighting)
			{
				Profiler.Start("environment.lighting");

				foreach (var lighter in WorldLighters)
				{
					while (Time.ElapsedMilliseconds < limit && lighter.TryLightNext())
					{
						// This space intentionally left blank
					}

					if (Time.ElapsedMilliseconds >= limit)
					{
						Trace.TraceEvent(TraceEventType.Warning, 0, ChatColor.RemoveColors("lighting queue is backed up"));
					}
				}
				
				Profiler.Done();
			}

			if (ServerConfiguration.EnableEventLoading)
			{
				Profiler.Start("environment.chunks");
				if (ChunksToSchedule.TryTake(out var t))
					ScheduleUpdatesForChunk(t.Item1, t.Item2);
				Profiler.Done();
			}

			Profiler.Done(MillisecondsPerTick);
			var end = Time.ElapsedMilliseconds;
			var next = MillisecondsPerTick - (end - start);
			if (next < 0)
				next = 0;

			EnvironmentWorker.Change(next, Timeout.Infinite);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) Stop();
		}

		~MultiPlayerServer() => Dispose(false);

		private struct BlockUpdate
		{
			public Coordinates3D Coordinates;
			public IWorld World;
		}
	}
}