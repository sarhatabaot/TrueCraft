﻿using System;
using System.Linq;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;
using TrueCraft.Core.Entities;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Handlers
{
	internal static class LoginHandlers
	{
		public static void HandleHandshakePacket(IPacket packet, IRemoteClient client, IMultiplayerServer server)
		{
			var handshakePacket = (HandshakePacket) packet;
			var remoteClient = (RemoteClient) client;
			remoteClient.Username = handshakePacket.Username;
			remoteClient.QueuePacket(new HandshakeResponsePacket("-")); // TODO: Implement some form of authentication
		}

		public static void HandleLoginRequestPacket(IPacket packet, IRemoteClient client, IMultiplayerServer server)
		{
			var loginRequestPacket = (LoginRequestPacket) packet;
			var remoteClient = (RemoteClient) client;
			if (loginRequestPacket.ProtocolVersion < server.PacketReader.ProtocolVersion)
				remoteClient.QueuePacket(new DisconnectPacket("Client outdated! Use beta 1.7.3."));
			else if (loginRequestPacket.ProtocolVersion > server.PacketReader.ProtocolVersion)
				remoteClient.QueuePacket(new DisconnectPacket("Server outdated! Use beta 1.7.3."));
			else if (server.Worlds.Count == 0)
				remoteClient.QueuePacket(new DisconnectPacket("Server has no worlds configured."));
			else if (!server.PlayerIsWhitelisted(remoteClient.Username) &&
			         server.PlayerIsBlacklisted(remoteClient.Username))
				remoteClient.QueuePacket(new DisconnectPacket("You're banned from this server"));
			else if (server.Clients.Count(c => c.Username == client.Username) > 1)
				remoteClient.QueuePacket(new DisconnectPacket("The player with this username is already logged in"));
			else
			{
				remoteClient.LoggedIn = true;
				remoteClient.Entity = new PlayerEntity(remoteClient.Username);
				remoteClient.World = server.Worlds[0];
				remoteClient.ChunkRadius = 2;

				if (!remoteClient.Load())
					remoteClient.Entity.Position = remoteClient.World.SpawnPoint.AsVector3();
				// Make sure they don't spawn in the ground
				var collision = new Func<bool>(() =>
				{
					var feet = client.World.GetBlockID((Coordinates3D) client.Entity.Position);
					var head = client.World.GetBlockID((Coordinates3D) (client.Entity.Position + Directions.Up));
					var feetBox = server.BlockRepository.GetBlockProvider(feet).BoundingBox;
					var headBox = server.BlockRepository.GetBlockProvider(head).BoundingBox;
					return feetBox != null || headBox != null;
				});
				while (collision())
					client.Entity.Position += Directions.Up;

				var entityManager = server.GetEntityManagerForWorld(remoteClient.World);
				entityManager.SpawnEntity(remoteClient.Entity);

				// Send setup packets
				remoteClient.QueuePacket(new LoginResponsePacket(client.Entity.EntityID, 0, Dimension.Overworld));
				remoteClient.UpdateChunks(true);
				remoteClient.QueuePacket(new WindowItemsPacket(0, remoteClient.Inventory.GetSlots()));
				remoteClient.QueuePacket(new UpdateHealthPacket((remoteClient.Entity as PlayerEntity).Health));
				remoteClient.QueuePacket(new SpawnPositionPacket((int) remoteClient.Entity.Position.X,
					(int) remoteClient.Entity.Position.Y, (int) remoteClient.Entity.Position.Z));
				remoteClient.QueuePacket(new SetPlayerPositionPacket(remoteClient.Entity.Position.X,
					remoteClient.Entity.Position.Y + 1,
					remoteClient.Entity.Position.Y + remoteClient.Entity.Size.Height + 1,
					remoteClient.Entity.Position.Z, remoteClient.Entity.Yaw, remoteClient.Entity.Pitch, true));
				remoteClient.QueuePacket(new TimeUpdatePacket(remoteClient.World.Time));

				// Start housekeeping for this client
				entityManager.SendEntitiesToClient(remoteClient);
				server.Scheduler.ScheduleEvent("remote.keepalive", remoteClient, TimeSpan.FromSeconds(10),
					remoteClient.SendKeepAlive);
				server.Scheduler.ScheduleEvent("remote.chunks", remoteClient, TimeSpan.FromSeconds(1),
					remoteClient.ExpandChunkRadius);

				if (!string.IsNullOrEmpty(server.ServerConfiguration.MOTD))
					remoteClient.SendMessage(server.ServerConfiguration.MOTD);
				if (!server.ServerConfiguration.Singleplayer)
					server.SendMessage(ChatColor.Yellow + "{0} joined the server.", remoteClient.Username);
			}
		}
	}
}