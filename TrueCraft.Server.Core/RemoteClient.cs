﻿using System;
using TrueCraft.API.Networking;
using System.Net.Sockets;
using TrueCraft.Core.Networking;
using System.Collections.Concurrent;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API.Entities;
using TrueCraft.API;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core.World;
using Ionic.Zlib;
using TrueCraft.API.Windows;
using TrueCraft.Core.Windows;
using System.Threading.Tasks;
using System.Threading;
using TrueCraft.Core.Entities;
using System.IO;
using fNbt;
using TrueCraft.API.Logging;
using TrueCraft.API.Logic;
using TrueCraft.Exceptions;
using TrueCraft.Profiling;

namespace TrueCraft
{
    public class RemoteClient : IRemoteClient, IEventSubject, IDisposable
    {
        private readonly ServerConfiguration _configuration;

        public RemoteClient(IMultiplayerServer server, ServerConfiguration configuration, IPacketReader packetReader, PacketHandler[] packetHandlers, Socket connection)
        {
            _configuration = configuration;
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

            cancel = new CancellationTokenSource();

            StartReceive();
        }

        public event EventHandler Disposed;

        /// <summary>
        /// A list of entities that this client is aware of.
        /// </summary>
        internal List<IEntity> KnownEntities { get; set; }
        internal sbyte NextWindowID { get; set; }

        //public NetworkStream NetworkStream { get; set; }
        public IMinecraftStream MinecraftStream { get; internal set; }
        public string Username { get; internal set; }
        public bool LoggedIn { get; internal set; }
        public IMultiplayerServer Server { get; set; }
        public IWorld World { get; internal set; }
        public IWindow Inventory { get; private set; }
        public short SelectedSlot { get; internal set; }
        public ItemStack ItemStaging { get; set; }
        public IWindow CurrentWindow { get; internal set; }
        public bool EnableLogging { get; set; }
        public DateTime ExpectedDigComplete { get; set; }

        public Socket Connection { get; private set; }

        private SemaphoreSlim sem = new SemaphoreSlim(1, 1);

        private SocketAsyncEventArgsPool SocketPool { get; set; }

        public IPacketReader PacketReader { get; private set; }

        private PacketHandler[] PacketHandlers { get; set; }

        private IEntity _Entity;

        private long disconnected;

        private readonly CancellationTokenSource cancel;

        public bool Disconnected
        {
            get
            {
                return Interlocked.Read(ref disconnected) == 1;
            }
            set
            {
                Interlocked.CompareExchange(ref disconnected, value ? 1 : 0, value ? 0 : 1);
            }
        }

        public IEntity Entity
        {
            get
            {
                return _Entity;
            }
            internal set
            {
                var player = _Entity as PlayerEntity;
                if (player != null)
                    player.PickUpItem -= HandlePickUpItem;
                _Entity = value;
                player = _Entity as PlayerEntity;
                if (player != null)
                    player.PickUpItem += HandlePickUpItem;
            }
        }

        void HandlePickUpItem(object sender, EntityEventArgs e)
        {
            var packet = new CollectItemPacket(e.Entity.EntityID, Entity.EntityID);
            QueuePacket(packet);
            var manager = Server.GetEntityManagerForWorld(World);
            foreach (var client in manager.ClientsForEntity(Entity))
                client.QueuePacket(packet);
            Inventory.PickUpStack((e.Entity as ItemEntity).Item);
        }

        public ItemStack SelectedItem
        {
            get
            {
                return Inventory[SelectedSlot];
            }
        }

        public InventoryWindow InventoryWindow
        {
            get
            {
                return Inventory as InventoryWindow;
            }
        }

        internal int ChunkRadius { get; set; }
        internal HashSet<Coordinates2D> LoadedChunks { get; set; }

        public bool DataAvailable
        {
            get
            {
                return true;
            }
        }

        public bool Load()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "players", Username + ".nbt");
            if (_configuration.Singleplayer)
                path = Path.Combine(((World)World).BaseDirectory, "player.nbt");
            if (!File.Exists(path))
                return false;
            try
            {
                var nbt = new NbtFile(path);
                Entity.Position = new Vector3(
                    nbt.RootTag["position"][0].DoubleValue,
                    nbt.RootTag["position"][1].DoubleValue,
                    nbt.RootTag["position"][2].DoubleValue);
                Inventory.SetSlots(((NbtList)nbt.RootTag["inventory"]).Select(t => ItemStack.FromNbt(t as NbtCompound)).ToArray());
                (Entity as PlayerEntity).Health = nbt.RootTag["health"].ShortValue;
                Entity.Yaw = nbt.RootTag["yaw"].FloatValue;
                Entity.Pitch = nbt.RootTag["pitch"].FloatValue;
            }
            catch { /* Who cares */ }
            return true;
        }

        public void Save()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "players", Username + ".nbt");
            if (_configuration.Singleplayer)
                path = Path.Combine(((World)World).BaseDirectory, "player.nbt");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (Entity == null) // I didn't think this could happen but null reference exceptions have been repoted here
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
                    new NbtFloat("pitch", Entity.Pitch),
                }
            ));
            nbt.SaveToFile(path, NbtCompression.ZLib);
        }

        public void OpenWindow(IWindow window)
        {
            CurrentWindow = window;
            window.Client = this;
            window.ID = NextWindowID++;
            if (NextWindowID < 0) NextWindowID = 1;
            QueuePacket(new OpenWindowPacket(window.ID, window.Type, window.Name, (sbyte)window.MinecraftWasWrittenByFuckingIdiotsLength));
            QueuePacket(new WindowItemsPacket(window.ID, window.GetSlots()));
            window.WindowChange += HandleWindowChange;
        }

        public void CloseWindow(bool clientInitiated = false)
        {
            if (!clientInitiated)
                QueuePacket(new CloseWindowPacket(CurrentWindow.ID));
            CurrentWindow.CopyToInventory(Inventory);
            CurrentWindow.Dispose();
            CurrentWindow = InventoryWindow;
        }

        public void Log(string message, params object[] parameters)
        {
            if (EnableLogging)
                SendMessage(ChatColor.Gray + string.Format("[" + DateTime.UtcNow.ToShortTimeString() + "] " + message, parameters));
        }

        public void QueuePacket(IPacket packet)
        {
            if (Disconnected || (Connection != null && !Connection.Connected))
                return;

            using (MemoryStream writeStream = new MemoryStream())
            {
                using (MinecraftStream ms = new MinecraftStream(writeStream))
                {
                    writeStream.WriteByte(packet.ID);
                    packet.WritePacket(ms);
                }

                byte[] buffer = writeStream.ToArray();

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.UserToken = packet;
                args.Completed += OperationCompleted;
                args.SetBuffer(buffer, 0, buffer.Length);

                if (Connection != null)
                {
                    if (!Connection.SendAsync(args))
                        OperationCompleted(this, args);
                }
            }
        }

        private void StartReceive()
        {
            SocketAsyncEventArgs args = SocketPool.Get();
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
                    IPacket packet = e.UserToken as IPacket;

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

            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                SocketAsyncEventArgs newArgs = SocketPool.Get();
                newArgs.Completed += OperationCompleted;

                if (!Connection.ReceiveAsync(newArgs))
                    OperationCompleted(this, newArgs);

                try
                {
                    sem.Wait(500, cancel.Token);
                }
                catch (OperationCanceledException)
                {
                }
                catch (NullReferenceException)
                {
                }
                catch (TimeoutException)
                {
                    Server.DisconnectClient(this);
                    return;
                }

                var packets = PacketReader.ReadPackets(this, e.Buffer, e.Offset, e.BytesTransferred);
                try
                {
                    foreach (IPacket packet in packets)
                    {
                        if (PacketHandlers[packet.ID] != null)
                        {
                            try
                            {
                                PacketHandlers[packet.ID](packet, this, Server);
                            }
                            catch (PlayerDisconnectException)
                            {
                                Server.DisconnectClient(this);
                            }
                            catch (Exception ex)
                            {
                                Server.Log(LogCategory.Debug, "Disconnecting client due to exception in network worker");
                                Server.Log(LogCategory.Debug, ex.ToString());

                                Server.DisconnectClient(this);
                            }
                        }
                        else
                        {
                            Log("Unhandled packet {0}", packet.GetType().Name);
                        }
                    }
                }
                catch (NotSupportedException)
                {
                    Server.Log(LogCategory.Debug, "Disconnecting client due to unsupported packet received.");
                    return;
                }

                if (sem.CurrentCount == 0 && sem != null)
                    sem.Release();
            }
            else
            {
                Server.DisconnectClient(this);
            }
        }

        public void Disconnect()
        {
            if (Disconnected)
                return;

            Disconnected = true;

            cancel.Cancel();

            Connection.Shutdown(SocketShutdown.Send);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OperationCompleted;
            Connection.DisconnectAsync(args);
        }

        public void SendMessage(string message)
        {
            var parts = message.Split('\n');
            foreach (var part in parts)
                QueuePacket(new ChatMessagePacket(part));
        }

        internal void ExpandChunkRadius(IMultiplayerServer server)
        {
            if (this.Disconnected)
                return;
            Task.Factory.StartNew(() =>
            {
                if (ChunkRadius < 8) // TODO: Allow customization of this number
                {
                    ChunkRadius++;
                    server.Scheduler.ScheduleEvent("client.update-chunks", this,
                        TimeSpan.Zero, s => UpdateChunks());
                    server.Scheduler.ScheduleEvent("remote.chunks", this,
                        TimeSpan.FromSeconds(1), ExpandChunkRadius);
                }
            });
        }

        internal void SendKeepAlive(IMultiplayerServer server)
        {
            QueuePacket(new KeepAlivePacket());
            server.Scheduler.ScheduleEvent("remote.keepalive", this, TimeSpan.FromSeconds(10), SendKeepAlive);
        }

        internal void UpdateChunks(bool block = false)
        {
            var newChunks = new HashSet<Coordinates2D>();
            var toLoad = new List<Tuple<Coordinates2D, IChunk>>();
            Profiler.Start("client.new-chunks");
            for (int x = -ChunkRadius; x < ChunkRadius; x++)
            {
                for (int z = -ChunkRadius; z < ChunkRadius; z++)
                {
                    var coords = new Coordinates2D(
                        ((int)Entity.Position.X >> 4) + x,
                        ((int)Entity.Position.Z >> 4) + z);
                    newChunks.Add(coords);
                    if (!LoadedChunks.Contains(coords))
                        toLoad.Add(new Tuple<Coordinates2D, IChunk>(
                            coords, World.GetChunk(coords, generate: block)));
                }
            }
            Profiler.Done();
            var encode = new Action(() =>
            {
                Profiler.Start("client.encode-chunks");
                foreach (var tup in toLoad)
                {
                    var coords = tup.Item1;
                    var chunk = tup.Item2;
                    if (chunk == null)
                        chunk = World.GetChunk(coords);
                    chunk.LastAccessed = DateTime.UtcNow;
                    LoadChunk(chunk);
                }
                Profiler.Done();
            });
            if (block)
                encode();
            else
                Task.Factory.StartNew(encode);
            Profiler.Start("client.old-chunks");
            LoadedChunks.IntersectWith(newChunks);
            Profiler.Done();
            Profiler.Start("client.update-entities");
            ((EntityManager)Server.GetEntityManagerForWorld(World)).UpdateClientEntities(this);
            Profiler.Done();
        }

        internal void UnloadAllChunks()
        {
            while (LoadedChunks.Any())
            {
                UnloadChunk(LoadedChunks.First());
            }
        }

        internal void LoadChunk(IChunk chunk)
        {
            QueuePacket(new ChunkPreamblePacket(chunk.Coordinates.X, chunk.Coordinates.Z));
            QueuePacket(CreatePacket(chunk));
            Server.Scheduler.ScheduleEvent("client.finalize-chunks", this,
                TimeSpan.Zero, server =>
                {
                    return;
                    LoadedChunks.Add(chunk.Coordinates);
                    foreach (var kvp in chunk.TileEntities)
                    {
                        var coords = kvp.Key;
                        var descriptor = new BlockDescriptor
                        {
                            Coordinates = coords + new Coordinates3D(chunk.X, 0, chunk.Z),
                            Metadata = chunk.GetMetadata(coords),
                            ID = chunk.GetBlockID(coords),
                            BlockLight = chunk.GetBlockLight(coords),
                            SkyLight = chunk.GetSkyLight(coords)
                        };
                        var provider = Server.BlockRepository.GetBlockProvider(descriptor.ID);
                        provider.TileEntityLoadedForClient(descriptor, World, kvp.Value, this);
                    }
                });
        }

        internal void UnloadChunk(Coordinates2D position)
        {
            QueuePacket(new ChunkPreamblePacket(position.X, position.Z, false));
            LoadedChunks.Remove(position);
        }

        void HandleWindowChange(object sender, WindowChangeEventArgs e)
        {
            if (!(sender is InventoryWindow))
            {
                QueuePacket(new SetSlotPacket((sender as IWindow).ID, (short)e.SlotIndex, e.Value.ID, e.Value.Count, e.Value.Metadata));
                return;
            }

            QueuePacket(new SetSlotPacket(0, (short)e.SlotIndex, e.Value.ID, e.Value.Count, e.Value.Metadata));

            if (e.SlotIndex == SelectedSlot)
            {
                var notified = Server.GetEntityManagerForWorld(World).ClientsForEntity(Entity);
                foreach (var c in notified)
                    c.QueuePacket(new EntityEquipmentPacket(Entity.EntityID, 0, SelectedItem.ID, SelectedItem.Metadata));
            }
            if (e.SlotIndex >= InventoryWindow.ArmorIndex && e.SlotIndex < InventoryWindow.ArmorIndex + InventoryWindow.Armor.Length)
            {
                short slot = (short)(4 - (e.SlotIndex - InventoryWindow.ArmorIndex));
                var notified = Server.GetEntityManagerForWorld(World).ClientsForEntity(Entity);
                foreach (var c in notified)
                    c.QueuePacket(new EntityEquipmentPacket(Entity.EntityID, slot, e.Value.ID, e.Value.Metadata));
            }
        }

        private static ChunkDataPacket CreatePacket(IChunk chunk)
        {
            var X = chunk.Coordinates.X;
            var Z = chunk.Coordinates.Z;

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

            return new ChunkDataPacket(X * Chunk.Width, 0, Z * Chunk.Depth,
                Chunk.Width, Chunk.Height, Chunk.Depth, result);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IPacketSegmentProcessor processor;
                while (!PacketReader.Processors.TryRemove(this, out processor))
                    Thread.Sleep(1);

                Disconnect();

                if(sem != null)
                    sem.Dispose();

                if (Disposed != null)
                    Disposed(this, null);
                sem = null;
            }
        }
    }
}
