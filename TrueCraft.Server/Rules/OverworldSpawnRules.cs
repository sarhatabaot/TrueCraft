using TrueCraft.AI;
using TrueCraft.World;

namespace TrueCraft.Server.Rules
{
	/// <summary>
	///  Default rules for spawning mobs in the overworld.
	/// </summary>
	public class OverworldSpawnRules : ISpawnRule
	{
		public void GenerateMobs(IChunk chunk, IEntityManager entityManager)
		{
			// TODO
		}

		public void SpawnMobs(IChunk chunk, IEntityManager entityManager)
		{
			// TODO
		}

		public int ChunkSpawnChance => 10;
	}
}