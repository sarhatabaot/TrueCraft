using System;
using System.Diagnostics;
using System.Linq;
using TrueCraft.Entities;
using TrueCraft.Extensions;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;

namespace TrueCraft.Server.Handlers
{
	internal static class LoginHandlers
	{
		public static void HandleHandshakePacket(IPacket packet, IRemoteClient client, IMultiPlayerServer server)
		{
			var handshakePacket = (HandshakePacket) packet;
			var remoteClient = (RemoteClient) client;
			remoteClient.Username = handshakePacket.Username;
			remoteClient.QueuePacket(new HandshakeResponsePacket("-")); // TODO: Implement some form of authentication
		}

		public static void HandleLoginRequestPacket(IPacket packet, IRemoteClient client, IMultiPlayerServer server)
		{
			var loginRequestPacket = (LoginRequestPacket) packet;
			
			if (loginRequestPacket.ProtocolVersion < server.PacketReader.ProtocolVersion)
				client.QueuePacket(new DisconnectPacket("Client outdated! Use beta 1.7.3."));
			else if (loginRequestPacket.ProtocolVersion > server.PacketReader.ProtocolVersion)
				client.QueuePacket(new DisconnectPacket("Server outdated! Use beta 1.7.3."));
			else if (server.Worlds.Count == 0)
				client.QueuePacket(new DisconnectPacket("Server has no worlds configured."));
			else if (!server.PlayerIsWhitelisted(client.Username) &&
			         server.PlayerIsBlacklisted(client.Username))
				client.QueuePacket(new DisconnectPacket("You're banned from this server."));
			else if (server.Clients.Count(c => c.Username == client.Username) > 1)
				client.QueuePacket(new DisconnectPacket("The player with this username is already logged in."));
			else
			{
				Login(server, (RemoteClient) client);
			}
		}

		private static void Login(IMultiPlayerServer server, RemoteClient client)
		{
			var username = client.Username ?? "<NO_USERNAME>";
			if (string.IsNullOrWhiteSpace(username))
				username = "<BLANK>";

			client.LoggedIn = true;
			client.Entity = new PlayerEntity(username);
			client.World = server.Worlds[0];
			client.ChunkRadius = 2;

			server.Trace.TraceData(TraceEventType.Start, 0,
				$"{username} is logging in with reported position {client.Entity?.Position.ToString() ?? "<NO_ENTITY>"}");

			if (!client.Load())
				client.Entity.Position = client.World.SpawnPoint.AsVector3();

			// Make sure they don't spawn in the ground
			var collision = new Func<bool>(() => ClientCollidesWithGround(client, server, client));

			while (collision())
			{
				server.Trace.TraceData(TraceEventType.Warning, 0, "spawn point is in the ground");
				client.Entity.Position += Directions.Up;
			}

			var entityManager = server.GetEntityManagerForWorld(client.World);
			entityManager.SpawnEntity(client.Entity);

			// Send setup packets
			client.QueuePacket(new LoginResponsePacket(client.Entity.EntityID, 0, Dimension.Overworld));

			server.Trace.TraceData(TraceEventType.Start, 0, "updating chunks");
			client.UpdateChunks(true);

			server.Trace.TraceData(TraceEventType.Start, 0, "sending setup packets");
			client.QueuePacket(new WindowItemsPacket(0, client.Inventory.GetSlots()));
			client.QueuePacket(new UpdateHealthPacket(((PlayerEntity) client.Entity).Health));
			client.QueuePacket(new SpawnPositionPacket((int) client.Entity.Position.X, (int) client.Entity.Position.Y, (int) client.Entity.Position.Z));
			client.QueuePacket(new SetPlayerPositionPacket(client.Entity.Position.X,
				client.Entity.Position.Y + 1,
				client.Entity.Position.Y + client.Entity.Size.Height + 1,
				client.Entity.Position.Z, client.Entity.Yaw, client.Entity.Pitch, true));
			client.QueuePacket(new TimeUpdatePacket(client.World.Time));

			// Start housekeeping for this client
			entityManager.SendEntitiesToClient(client);
			server.Scheduler.ScheduleEvent("remote.keepalive", client, TimeSpan.FromSeconds(10),
				client.SendKeepAlive);
			server.Scheduler.ScheduleEvent("remote.chunks", client, TimeSpan.FromSeconds(1),
				client.ExpandChunkRadius);

			if (!string.IsNullOrEmpty(server.ServerConfiguration.MOTD))
				client.SendMessage(server.ServerConfiguration.MOTD);
			if (!server.ServerConfiguration.Singleplayer)
				server.SendMessage(ChatColor.Yellow + "{0} joined the server.", client.Username);
		}

		private static bool ClientCollidesWithGround(IRemoteClient client, IMultiPlayerServer server, RemoteClient remoteClient)
		{
			var position = client.Entity?.Position;

			if (client.Entity == null)
			{
				server.Trace.TraceEvent(TraceEventType.Error, 0, "client has no entity data");
				return false; // ???
			}

			Debug.Assert(position != null, $"{nameof(position)} != null");
			var coordinates = (Coordinates3D) position;

			if (!client.World.TryGetBlockId(coordinates, out var blockId))
			{
				client.Entity.Position = remoteClient.World.SpawnPoint.AsVector3(); // fall back to world spawn point
				server.Trace.TraceData(TraceEventType.Error, 0,
					$"client gave a bad starting position: {position}, falling back to spawn point at {client.Entity.Position}");
				return true; // colliding
			}

			if (position != null)
			{
				var head = client.World.GetBlockId((Coordinates3D) (position + Directions.Up));
				var feetBox = server.BlockRepository.GetBlockProvider(blockId).BoundingBox;
				var headBox = server.BlockRepository.GetBlockProvider(head).BoundingBox;
				return feetBox != null || headBox != null; // colliding
			}

			return false;
		}
	}
}