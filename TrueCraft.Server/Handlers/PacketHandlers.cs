using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Server.Exceptions;

namespace TrueCraft.Server.Handlers
{
	public static class PacketHandlers
	{
		public static void RegisterHandlers(IMultiPlayerServer server)
		{
			server.RegisterPacketHandler(Constants.PacketIds.KeepAlive, HandleKeepAlive);
			server.RegisterPacketHandler(Constants.PacketIds.ChatMessage, HandleChatMessage);
			server.RegisterPacketHandler(Constants.PacketIds.Disconnect, HandleDisconnect);
			server.RegisterPacketHandler(Constants.PacketIds.Handshake, LoginHandlers.HandleHandshakePacket);
			server.RegisterPacketHandler(Constants.PacketIds.LoginRequest, LoginHandlers.HandleLoginRequestPacket);
			server.RegisterPacketHandler(Constants.PacketIds.PlayerGrounded, (a, b, c) => { /* no-op */ });
			server.RegisterPacketHandler(Constants.PacketIds.PlayerPosition, EntityHandlers.HandlePlayerPositionPacket);
			server.RegisterPacketHandler(Constants.PacketIds.PlayerLook, EntityHandlers.HandlePlayerLookPacket);
			server.RegisterPacketHandler(Constants.PacketIds.PlayerPositionAndLook, EntityHandlers.HandlePlayerPositionAndLookPacket);
			server.RegisterPacketHandler(Constants.PacketIds.PlayerDigging, InteractionHandlers.HandlePlayerDiggingPacket);
			server.RegisterPacketHandler(Constants.PacketIds.PlayerBlockPlacement, InteractionHandlers.HandlePlayerBlockPlacementPacket);
			server.RegisterPacketHandler(Constants.PacketIds.ChangeHeldItem, InteractionHandlers.HandleChangeHeldItem);
			server.RegisterPacketHandler(Constants.PacketIds.PlayerAction, InteractionHandlers.HandlePlayerAction);
			server.RegisterPacketHandler(Constants.PacketIds.Animation, InteractionHandlers.HandleAnimation);
			server.RegisterPacketHandler(Constants.PacketIds.ClickWindow, InteractionHandlers.HandleClickWindowPacket);
			server.RegisterPacketHandler(Constants.PacketIds.CloseWindow, InteractionHandlers.HandleCloseWindowPacket);
			server.RegisterPacketHandler(Constants.PacketIds.UpdateSign, InteractionHandlers.HandleUpdateSignPacket);
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