using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using TrueCraft.Entities;
using TrueCraft.Extensions;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Physics;
using TrueCraft.World;

namespace TrueCraft.Server
{
	public class EntityManager : IEntityManager
	{
		private readonly object _lock = new object();

		public EntityManager(IMultiPlayerServer server, IWorld world)
		{
			Server = server;
			World = world;
			PhysicsEngine = new PhysicsEngine(world, (BlockRepository) server.BlockRepository);
			PendingDespawns = new ConcurrentBag<IEntity>();
			Entities = new List<IEntity>();

			// TODO: Handle loading worlds that already have entities
			// Note: probably not the concern of EntityManager. The server could manually set this?
			NextEntityID = 1;
			LastUpdate = DateTime.UtcNow;
			TimeSinceLastUpdate = TimeSpan.Zero;
		}

		public IMultiPlayerServer Server { get; set; }
		public PhysicsEngine PhysicsEngine { get; set; }

		private int NextEntityID { get; set; }
		private List<IEntity> Entities { get; } // TODO: Persist to disk
		private ConcurrentBag<IEntity> PendingDespawns { get; }
		private DateTime LastUpdate { get; set; }
		public TimeSpan TimeSinceLastUpdate { get; private set; }
		public IWorld World { get; set; }

		public IList<IEntity> EntitiesInRange(Vector3 center, float radius)
		{
			return Entities.Where(e => !e.Despawned && e.Position.DistanceTo(center) < radius).ToList();
		}

		public IList<IRemoteClient> ClientsForEntity(IEntity entity)
		{
			return Server.Clients.Where(c => (c as RemoteClient).KnownEntities.Contains(entity)).ToList();
		}

		public void SpawnEntity(IEntity entity)
		{
			if (entity.Despawned)
				return;
			entity.SpawnTime = DateTime.UtcNow;
			entity.EntityManager = this;
			entity.World = World;
			entity.EntityId = NextEntityID++;
			entity.PropertyChanged -= HandlePropertyChanged;
			entity.PropertyChanged += HandlePropertyChanged;
			lock (_lock) Entities.Add(entity);
			foreach (var clientEntity in GetEntitiesInRange(entity, 8)) // Note: 8 is pretty arbitrary here
				if (clientEntity != entity && clientEntity is PlayerEntity)
				{
					var client = (RemoteClient) GetClientForEntity((PlayerEntity) clientEntity);
					SendEntityToClient(client, entity);
				}

			if (entity is IPhysicsEntity)
				PhysicsEngine.AddEntity(entity as IPhysicsEntity);
		}

		public void DespawnEntity(IEntity entity)
		{
			entity.Despawned = true;
			PendingDespawns.Add(entity);
		}

		public void FlushDespawns()
		{
			IEntity entity;
			while (PendingDespawns.Count != 0)
			{
				while (!PendingDespawns.TryTake(out entity))
					;
				if (entity is IPhysicsEntity)
					PhysicsEngine.RemoveEntity((IPhysicsEntity) entity);
				lock ((Server as MultiPlayerServer).ClientLock
				) // TODO: Thread safe way to iterate over client collection
					for (int i = 0, ServerClientsCount = Server.Clients.Count; i < ServerClientsCount; i++)
					{
						var client = (RemoteClient) Server.Clients[i];
						if (client.KnownEntities.Contains(entity) && !client.Disconnected)
						{
							client.QueuePacket(new DestroyEntityPacket(entity.EntityId));
							client.KnownEntities.Remove(entity);
							client.MaybeEchoToClient("Destroying entity {0} ({1})", entity.EntityId, entity.GetType().Name);
						}
					}

				lock (_lock)
					Entities.Remove(entity);
			}
		}

		public IEntity GetEntityById(int Id)
		{
			return Entities.SingleOrDefault(e => e.EntityId == Id);
		}

		public void Update()
		{
			TimeSinceLastUpdate = DateTime.UtcNow - LastUpdate;
			LastUpdate = DateTime.UtcNow;
			PhysicsEngine.Update(TimeSinceLastUpdate);
			try
			{
				lock (Entities)
					foreach (var e in Entities)
						if (!e.Despawned)
							e.Update(this);
			}
			catch
			{
				// Do nothing
			}

			FlushDespawns();
		}

		/// <summary>
		///  Performs the initial population of client entities.
		/// </summary>
		public void SendEntitiesToClient(IRemoteClient _client)
		{
			if (_client is RemoteClient client)
				foreach (var entity in GetEntitiesInRange(client.Entity, client.ChunkRadius))
					if (entity != client.Entity)
						SendEntityToClient(client, entity);
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is IEntity entity))
				throw new InvalidCastException("attempted to handle property changes for a non-entity");

			if (entity is PlayerEntity player)
				HandlePlayerPropertyChanged(e.PropertyName, player);

			switch (e.PropertyName)
			{
				case nameof(Entity.Position):
				case nameof(Entity.Yaw):
				case nameof(Entity.Pitch):
					PropagateEntityPositionUpdates(entity);
					break;
				case nameof(Entity.Metadata):
					PropagateEntityMetadataUpdates(entity);
					break;
			}
		}

		private void HandlePlayerPropertyChanged(string property, PlayerEntity entity)
		{
			if (!(GetClientForEntity(entity) is RemoteClient client))
				return; // Note: would an exception be appropriate here?

			switch (property)
			{
				case nameof(Entity.Position):

					if ((int) entity.Position.X >> 4 != (int) entity.OldPosition.X >> 4 || (int) entity.Position.Z >> 4 != (int) entity.OldPosition.Z >> 4)
					{
						client.MaybeEchoToClient("Passed chunk boundary at {0}, {1}", (int) entity.Position.X >> 4, (int) entity.Position.Z >> 4);
						Server.Scheduler.ScheduleEvent(Constants.Events.ClientUpdateChunks, client, TimeSpan.Zero, s => client.UpdateChunks());
						UpdateClientEntities(client);
					}
					break;
			}
		}

		internal void UpdateClientEntities(RemoteClient client)
		{
			var entity = client.Entity;

			// Calculate entities you shouldn't know about anymore
			for (var i = 0; i < client.KnownEntities.Count; i++)
			{
				var knownEntity = client.KnownEntities[i];
				if (!(knownEntity.Position.DistanceTo(entity.Position) > client.ChunkRadius * Chunk.Depth))
					continue;

				client.QueuePacket(new DestroyEntityPacket(knownEntity.EntityId));
				client.KnownEntities.Remove(knownEntity);
				i--;

				// Make sure you're de-spawned on other clients if you move away from stationary players
				if (knownEntity is PlayerEntity player)
				{
					var c = (RemoteClient) GetClientForEntity(player);
					if (c.KnownEntities.Contains(entity))
					{
						c.KnownEntities.Remove(entity);
						c.QueuePacket(new DestroyEntityPacket(entity.EntityId));
						c.MaybeEchoToClient("Destroying entity {0} ({1})", player.EntityId, player.GetType().Name);
					}
				}

				client.MaybeEchoToClient("Destroying entity {0} ({1})", knownEntity.EntityId, knownEntity.GetType().Name);
			}

			// Calculate entities you should now know about
			var toSpawn = GetEntitiesInRange(entity, client.ChunkRadius);
			foreach (var e in toSpawn)
				if (e != entity && !client.KnownEntities.Contains(e))
				{
					SendEntityToClient(client, e);

					// Make sure other players know about you since you've moved
					if (e is PlayerEntity player)
					{
						var c = (RemoteClient) GetClientForEntity(player);
						if (!c.KnownEntities.Contains(entity))
							SendEntityToClient(c, entity);
					}
				}
		}

		private void PropagateEntityPositionUpdates(IEntity entity)
		{
			for (int i = 0, serverClientsCount = Server.Clients.Count; i < serverClientsCount; i++)
			{
				if (!(Server.Clients[i] is RemoteClient client))
					continue; // Note: would an exception be appropriate here?

				if (client.Entity == entity)
					continue; // Do not send movement updates back to the client that triggered them

				if (client.KnownEntities.Contains(entity))
					client.QueuePacket(new EntityTeleportPacket(entity.EntityId,
						MathHelper.CreateAbsoluteInt(entity.Position.X),
						MathHelper.CreateAbsoluteInt(entity.Position.Y),
						MathHelper.CreateAbsoluteInt(entity.Position.Z),
						MathHelper.CreateRotationByte(entity.Yaw),
						MathHelper.CreateRotationByte(entity.Pitch)));
			}
		}

		private void PropagateEntityMetadataUpdates(IEntity entity)
		{
			if (!entity.SendMetadataToClients)
				return;
			for (int i = 0, ServerClientsCount = Server.Clients.Count; i < ServerClientsCount; i++)
			{
				var client = Server.Clients[i] as RemoteClient;
				if (client.Entity == entity)
					continue; // Do not send movement updates back to the client that triggered them
				if (client.KnownEntities.Contains(entity))
					client.QueuePacket(new EntityMetadataPacket(entity.EntityId, entity.Metadata));
			}
		}

		private bool IsInRange(Vector3 a, Vector3 b, int range)
		{
			return Math.Abs(a.X - b.X) < range * Chunk.Width &&
			       Math.Abs(a.Z - b.Z) < range * Chunk.Depth;
		}

		private IEntity[] GetEntitiesInRange(IEntity entity, int maxChunks)
		{
			return Entities.Where(e =>
					e.EntityId != entity.EntityId && !e.Despawned && IsInRange(e.Position, entity.Position, maxChunks))
				.ToArray();
		}

		private void SendEntityToClient(RemoteClient client, IEntity entity)
		{
			if (entity.EntityId == -1)
				return; // We haven't finished setting this entity up yet
			client.MaybeEchoToClient("Spawning entity {0} ({1}) at {2}", entity.EntityId, entity.GetType().Name,
				(Coordinates3D) entity.Position);
			RemoteClient spawnedClient = null;
			if (entity is PlayerEntity)
				spawnedClient = (RemoteClient) GetClientForEntity(entity as PlayerEntity);
			client.KnownEntities.Add(entity);
			client.QueuePacket(entity.SpawnPacket);
			if (entity is IPhysicsEntity)
			{
				var pentity = entity as IPhysicsEntity;
				client.QueuePacket(new EntityVelocityPacket
				{
					EntityId = entity.EntityId,
					XVelocity = (short) (pentity.Velocity.X * 320),
					YVelocity = (short) (pentity.Velocity.Y * 320),
					ZVelocity = (short) (pentity.Velocity.Z * 320)
				});
			}

			if (entity.SendMetadataToClients)
				client.QueuePacket(new EntityMetadataPacket(entity.EntityId, entity.Metadata));
			if (spawnedClient != null)
			{
				// Send equipment when spawning player entities
				client.QueuePacket(new EntityEquipmentPacket(entity.EntityId,
					0, spawnedClient.SelectedItem.Id, spawnedClient.SelectedItem.Metadata));
				client.QueuePacket(new EntityEquipmentPacket(entity.EntityId,
					4, spawnedClient.InventoryWindow.Armor[0].Id, spawnedClient.InventoryWindow.Armor[0].Metadata));
				client.QueuePacket(new EntityEquipmentPacket(entity.EntityId,
					3, spawnedClient.InventoryWindow.Armor[1].Id, spawnedClient.InventoryWindow.Armor[1].Metadata));
				client.QueuePacket(new EntityEquipmentPacket(entity.EntityId,
					2, spawnedClient.InventoryWindow.Armor[2].Id, spawnedClient.InventoryWindow.Armor[2].Metadata));
				client.QueuePacket(new EntityEquipmentPacket(entity.EntityId,
					1, spawnedClient.InventoryWindow.Armor[3].Id, spawnedClient.InventoryWindow.Armor[3].Metadata));
			}
		}

		private IRemoteClient GetClientForEntity(PlayerEntity entity)
		{
			return Server.Clients.SingleOrDefault(c => c.Entity != null && c.Entity.EntityId == entity.EntityId);
		}
	}
}