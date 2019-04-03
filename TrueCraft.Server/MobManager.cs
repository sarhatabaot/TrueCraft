﻿using System.Collections.Generic;
using TrueCraft.AI;
using TrueCraft.World;

namespace TrueCraft.Server
{
	public class MobManager
	{
		public MobManager(EntityManager manager)
		{
			EntityManager = manager;
			SpawnRules = new Dictionary<Dimension, List<ISpawnRule>>();
		}

		public EntityManager EntityManager { get; set; }

		private Dictionary<Dimension, List<ISpawnRule>> SpawnRules { get; }

		public void AddRules(Dimension dimension, ISpawnRule rules)
		{
			if (!SpawnRules.ContainsKey(dimension))
				SpawnRules[dimension] = new List<ISpawnRule>();
			SpawnRules[dimension].Add(rules);
		}

		public void SpawnInitialMobs(IChunk chunk, Dimension dimension)
		{
			if (!SpawnRules.ContainsKey(dimension))
				return;
			var rules = SpawnRules[dimension];
			foreach (var rule in rules)
				if (MathHelper.Random.Next(rule.ChunkSpawnChance) == 0)
					rule.GenerateMobs(chunk, EntityManager);
		}

		/// <summary>
		///  Call at dusk and it'll spawn baddies.
		/// </summary>
		public void DayCycleSpawn(IChunk chunk, Dimension dimension)
		{
			// TODO
		}
	}
}