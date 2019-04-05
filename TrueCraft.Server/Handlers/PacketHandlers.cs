using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Server.Exceptions;

namespace TrueCraft.Server.Handlers
{
	public static class PacketHandlers
	{
		public static void RegisterHandlers(IMultiPlayerServer server)
		{
			server.RegisterPacketHandler(new KeepAlivePacket().ID, HandleKeepAlive);
			server.RegisterPacketHandler(new ChatMessagePacket().ID, HandleChatMessage);
			server.RegisterPacketHandler(new DisconnectPacket().ID, HandleDisconnect);
			server.RegisterPacketHandler(new HandshakePacket().ID, LoginHandlers.HandleHandshakePacket);
			server.RegisterPacketHandler(new LoginRequestPacket().ID, LoginHandlers.HandleLoginRequestPacket);
			server.RegisterPacketHandler(new PlayerGroundedPacket().ID, (a, b, c) => { /* no-op */ });
			server.RegisterPacketHandler(new PlayerPositionPacket().ID, EntityHandlers.HandlePlayerPositionPacket);
			server.RegisterPacketHandler(new PlayerLookPacket().ID, EntityHandlers.HandlePlayerLookPacket);
			server.RegisterPacketHandler(new PlayerPositionAndLookPacket().ID, EntityHandlers.HandlePlayerPositionAndLookPacket);
			server.RegisterPacketHandler(new PlayerDiggingPacket().ID, InteractionHandlers.HandlePlayerDiggingPacket);
			server.RegisterPacketHandler(new PlayerBlockPlacementPacket().ID, InteractionHandlers.HandlePlayerBlockPlacementPacket);
			server.RegisterPacketHandler(new ChangeHeldItemPacket().ID, InteractionHandlers.HandleChangeHeldItem);
			server.RegisterPacketHandler(new PlayerActionPacket().ID, InteractionHandlers.HandlePlayerAction);
			server.RegisterPacketHandler(new AnimationPacket().ID, InteractionHandlers.HandleAnimation);
			server.RegisterPacketHandler(new ClickWindowPacket().ID, InteractionHandlers.HandleClickWindowPacket);
			server.RegisterPacketHandler(new CloseWindowPacket().ID, InteractionHandlers.HandleCloseWindowPacket);
			server.RegisterPacketHandler(new UpdateSignPacket().ID, InteractionHandlers.HandleUpdateSignPacket);
		}

		internal static void HandleKeepAlive(IPacket packet, IRemoteClient client, IMultiPlayerServer server)
		{
			// TODO
		}

		internal static void HandleChatMessage(IPacket packet, IRemoteClient client, IMultiPlayerServer server)
		{
			// TODO: Abstract this to support things like commands
			// TODO: Sanitize messages

			server.OnChatMessageReceived(new ChatMessageEventArgs(client, ((ChatMessagePacket) packet).Message));
		}

		internal static void HandleDisconnect(IPacket packet, IRemoteClient client, IMultiPlayerServer server)
		{
			throw new PlayerDisconnectException(true);
		}
	}
}