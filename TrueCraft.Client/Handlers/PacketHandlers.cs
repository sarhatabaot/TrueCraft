using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using TrueCraft.Client.Events;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;

namespace TrueCraft.Client.Handlers
{
	internal static class PacketHandlers
	{
		public static void RegisterHandlers(MultiPlayerClient client)
		{
			client.RegisterPacketHandler(Constants.PacketIds.KeepAlive, HandleKeepAlive);
			client.RegisterPacketHandler(Constants.PacketIds.HandshakeResponse, HandleHandshake);
			client.RegisterPacketHandler(Constants.PacketIds.ChatMessage, HandleChatMessage);
			client.RegisterPacketHandler(Constants.PacketIds.SetPlayerPosition, HandlePositionAndLook);
			client.RegisterPacketHandler(Constants.PacketIds.LoginResponse, HandleLoginResponse);
			client.RegisterPacketHandler(Constants.PacketIds.UpdateHealth, HandleUpdateHealth);
			client.RegisterPacketHandler(Constants.PacketIds.TimeUpdate, HandleTimeUpdate);
			client.RegisterPacketHandler(Constants.PacketIds.EntityTeleport, HandleEntityTeleport);

			client.RegisterPacketHandler(Constants.PacketIds.ChunkPreamble, ChunkHandlers.HandleChunkPreamble);
			client.RegisterPacketHandler(Constants.PacketIds.ChunkData, ChunkHandlers.HandleChunkData);
			client.RegisterPacketHandler(Constants.PacketIds.BlockChange, ChunkHandlers.HandleBlockChange);
			client.RegisterPacketHandler(Constants.PacketIds.WindowItems, InventoryHandlers.HandleWindowItems);
			client.RegisterPacketHandler(Constants.PacketIds.SetSlot, InventoryHandlers.HandleSetSlot);
			client.RegisterPacketHandler(Constants.PacketIds.CloseWindow, InventoryHandlers.HandleCloseWindowPacket);
			client.RegisterPacketHandler(Constants.PacketIds.OpenWindow, InventoryHandlers.HandleOpenWindowPacket);
		}

		private static void HandleKeepAlive(IPacket packet, MultiPlayerClient client)
		{
			// TODO
		}

		public static void HandleChatMessage(IPacket packet, MultiPlayerClient client)
		{
			client.OnChatMessage(new ChatMessageEventArgs(((ChatMessagePacket) packet).Message));
		}

		public static void HandleHandshake(IPacket packet, MultiPlayerClient client)
		{
			if (((HandshakeResponsePacket) packet).ConnectionHash != "-")
			{
				Console.WriteLine("Online mode is not supported");
				Process.GetCurrentProcess().Kill();
			}

			// TODO: Authentication
			client.QueuePacket(new LoginRequestPacket(PacketReader.Version, client.User.Username));
		}

		public static void HandleLoginResponse(IPacket packet, MultiPlayerClient client)
		{
			client.EntityId = ((LoginResponsePacket) packet).EntityID;
			client.QueuePacket(new PlayerGroundedPacket());
		}

		public static void HandlePositionAndLook(IPacket packet, MultiPlayerClient client)
		{
			var position = (SetPlayerPositionPacket) packet;
			client._Position = new Vector3((float) position.X, (float) position.Y, (float) position.Z);
			client.QueuePacket(position);
			client.LoggedIn = true;
			// TODO: Pitch and yaw
		}

		public static void HandleUpdateHealth(IPacket packet, MultiPlayerClient client)
		{
			client.Health = ((UpdateHealthPacket) packet).Health;
		}

		public static void HandleTimeUpdate(IPacket packet, MultiPlayerClient client)
		{
			var time = ((TimeUpdatePacket) packet).Time / 20.0;
			client.World.World.BaseTime = DateTime.UtcNow - TimeSpan.FromSeconds(time);
		}

		public static void HandleEntityTeleport(IPacket packet, MultiPlayerClient client)
		{
			var teleport = (EntityTeleportPacket) packet;

			client.Trace.TraceData(TraceEventType.Warning, 0,
				$"server wants client to teleport entity {teleport.EntityID}, but we don't have an entity manager");
		}
	}
}