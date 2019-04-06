using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TrueCraft.Entities;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Server
{
	public interface IEntityManager
	{
		IWorld World { get; }
		TimeSpan TimeSinceLastUpdate { get; }

		/// <summary>
		///  Adds an entity to the world and assigns it an entity Id.
		/// </summary>
		void SpawnEntity(IEntity entity);

		void DespawnEntity(IEntity entity);
		void FlushDespawns();
		IEntity GetEntityById(int Id);
		void Update();
		void SendEntitiesToClient(IRemoteClient client);
		IList<IEntity> EntitiesInRange(Vector3 center, float radius);
		IList<IRemoteClient> ClientsForEntity(IEntity entity);
	}
}