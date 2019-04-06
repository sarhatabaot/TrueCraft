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

			DisconnectPacket error = default;

			if (loginRequestPacket.ProtocolVersion < server.PacketReader.ProtocolVersion)
				error = new DisconnectPacket("Client outdated! Use beta 1.7.3.");
			else if (loginRequestPacket.ProtocolVersion > server.PacketReader.ProtocolVersion)
				error = new DisconnectPacket("Server outdated! Use beta 1.7.3.");
			else if (server.Worlds.Count == 0)
				error = new DisconnectPacket("Server has no worlds configured.");
			else if (!server.PlayerIsWhitelisted(client.Username) && server.PlayerIsBlacklisted(client.Username))
				error = new DisconnectPacket("You are banned from this server.");
			else if (server.Clients.Count(c => c.Username == client.Username) > 1)
				error = new DisconnectPacket("The player with this username is already logged in.");

			if (error.Reason != null)
			{
				server.Trace.TraceData(TraceEventType.Start, 0, $"sending disconnect for reason: " + error.Reason);
			}
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

			server.Trace.TraceData(TraceEventType.Start, 0, $"{username} is logging in with reported position {client.Entity?.Position.ToString()}");

			if (!client.Load())
				client.Entity.Position = client.World.SpawnPoint.AsVector3();

			// Make sure player doesn't spawn in the ground
			while (ClientCollidesWithGround(client, server, client))
			{
				server.Trace.TraceData(TraceEventType.Warning, 0, "spawn point is in the ground");
				client.Entity.Position += Directions.Up;
			}

			var entityManager = server.GetEntityManagerForWorld(client.World);
			entityManager.SpawnEntity(client.Entity);

			var position = client.Entity.Position;

			server.Trace.TraceData(TraceEventType.Start, 0, "updating initial chunks");
			client.UpdateChunks(true);
			server.Trace.TraceData(TraceEventType.Start, 0, "done");

			// Send setup packets
			server.Trace.TraceData(TraceEventType.Start, 0, "sending setup packets");
			client.QueuePacket(new LoginResponsePacket(client.Entity.EntityId, 0, Dimension.Overworld));
			client.QueuePacket(new WindowItemsPacket(0, client.Inventory.GetSlots()));
			client.QueuePacket(new UpdateHealthPacket(((PlayerEntity) client.Entity).Health));
			client.QueuePacket(new SpawnPositionPacket((int) position.X, (int) position.Y, (int) position.Z));
			client.QueuePacket(new SetPlayerPositionPacket(position.X,
				position.Y + 1,
				position.Y + client.Entity.Size.Height + 1,
				position.Z, client.Entity.Yaw, client.Entity.Pitch, true));
			client.QueuePacket(new TimeUpdatePacket(client.World.Time));

			// Start housekeeping for this client
			server.Trace.TraceData(TraceEventType.Start, 0, "starting housekeeping");
			entityManager.SendEntitiesToClient(client);
			server.Scheduler.ScheduleEvent(Constants.Events.RemoteKeepAlive, client, TimeSpan.FromSeconds(10), client.SendKeepAlive);
			server.Scheduler.ScheduleEvent(Constants.Events.RemoteChunks, client, TimeSpan.FromSeconds(1), client.ExpandChunkRadius);

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

			var head = client.World.GetBlockId((Coordinates3D) (position + Directions.Up));
			var feetBox = server.BlockRepository.GetBlockProvider(blockId).BoundingBox;
			var headBox = server.BlockRepository.GetBlockProvider(head).BoundingBox;
			return feetBox != null || headBox != null; // colliding
		}
	}
}