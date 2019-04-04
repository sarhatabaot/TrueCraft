using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Server
{
	public interface IMultiplayerServer
	{
		object ClientLock { get; }
		ServerConfiguration ServerConfiguration { get; }

		TraceSource Trace { get; }
		IAccessConfiguration AccessConfiguration { get; }
		IPacketReader PacketReader { get; }
		IList<IRemoteClient> Clients { get; }
		IList<IWorld> Worlds { get; }
		IEventScheduler Scheduler { get; }
		IBlockRepository BlockRepository { get; }
		ICraftingRepository CraftingRepository { get; }
		IItemRepository ItemRepository { get; }
		IPEndPoint EndPoint { get; }
		bool BlockUpdatesEnabled { get; set; }
		bool EnableClientLogging { get; set; }
		
		event EventHandler<ChatMessageEventArgs> ChatMessageReceived;
		event EventHandler<PlayerJoinedQuitEventArgs> PlayerJoined;
		event EventHandler<PlayerJoinedQuitEventArgs> PlayerQuit;

		void Start(IPEndPoint endPoint);
		void Stop();
		void RegisterPacketHandler(byte packetId, PacketHandler handler);
		void AddWorld(IWorld world);
		IEntityManager GetEntityManagerForWorld(IWorld world);
		void SendMessage(string message, params object[] parameters);

		void DisconnectClient(IRemoteClient client);

		bool PlayerIsWhitelisted(string client);
		bool PlayerIsBlacklisted(string client);
		bool PlayerIsOp(string client);
	}
}