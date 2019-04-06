using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft._ADDON.Blocks
{
	public class GrassBlock : BlockProvider
	{
		public static readonly int MinGrowthTime = 60 * 5;
		public static readonly int MaxGrowthTime = 60 * 10;

		private static readonly Coordinates3D[] GrowthCandidates;

		public static readonly int MaxDecayTime = 60 * 10;
		public static readonly int MinDecayTime = 60 * 2;

		public static readonly byte BlockId = 0x02;

		static GrassBlock()
		{
			GrowthCandidates = new Coordinates3D[3 * 3 * 5];
			var i = 0;
			for (var x = -1; x <= 1; x++)
			for (var z = -1; z <= 1; z++)
			for (var y = -3; y <= 1; y++)
				GrowthCandidates[i++] = new Coordinates3D(x, y, z);
		}

		public override byte Id => 0x02;

		public override double BlastResistance => 3;

		public override double Hardness => 0.6;

		public override byte Luminance => 0;

		public override string DisplayName => "Grass";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 0);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(DirtBlock.BlockId, 1)};
		}

		private void ScheduledUpdate(IWorld world, Coordinates3D coords)
		{
			if (world.IsValidPosition(coords + Coordinates3D.Up))
			{
				var Id = world.GetBlockId(coords + Coordinates3D.Up);
				var provider = world.BlockRepository.GetBlockProvider(Id);
				if (provider.Opaque)
					world.SetBlockId(coords, DirtBlock.BlockId);
			}
		}

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server,
			IWorld world)
		{
			if (source.Coordinates == descriptor.Coordinates + Coordinates3D.Up)
			{
				var provider = world.BlockRepository.GetBlockProvider(source.Id);
				if (provider.Opaque)
				{
					var chunk = world.FindChunk(descriptor.Coordinates, false);
					server.Scheduler.ScheduleEvent("grass", chunk,
						TimeSpan.FromSeconds(MathHelper.Random.Next(MinDecayTime, MaxDecayTime)),
						s => { ScheduledUpdate(world, descriptor.Coordinates); });
				}
			}
		}

		public void TrySpread(Coordinates3D coords, IWorld world, IMultiPlayerServer server)
		{
			if (!world.IsValidPosition(coords + Coordinates3D.Up))
				return;
			var sky = world.GetSkyLight(coords + Coordinates3D.Up);
			var block = world.GetBlockLight(coords + Coordinates3D.Up);
			if (sky < 9 && block < 9)
				return;
			for (int i = 0, j = MathHelper.Random.Next(GrowthCandidates.Length); i < GrowthCandidates.Length; i++, j++)
			{
				var candidate = GrowthCandidates[j % GrowthCandidates.Length] + coords;
				if (!world.IsValidPosition(candidate) || !world.IsValidPosition(candidate + Coordinates3D.Up))
					continue;
				var Id = world.GetBlockId(candidate);
				if (Id == DirtBlock.BlockId)
				{
					var _sky = world.GetSkyLight(candidate + Coordinates3D.Up);
					var _block = world.GetBlockLight(candidate + Coordinates3D.Up);
					if (_sky < 4 && _block < 4)
						continue;
					IChunk chunk;
					var _candidate = world.FindBlockPosition(candidate, out chunk);
					var grow = true;
					for (var y = candidate.Y; y < chunk.GetHeight((byte) _candidate.X, (byte) _candidate.Z); y++)
					{
						var b = world.GetBlockId(new Coordinates3D(candidate.X, y, candidate.Z));
						var p = world.BlockRepository.GetBlockProvider(b);
						if (p.LightOpacity >= 2)
						{
							grow = false;
							break;
						}
					}

					if (grow)
					{
						world.SetBlockId(candidate, BlockId);
						server.Scheduler.ScheduleEvent("grass", chunk,
							TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthTime, MaxGrowthTime)),
							s => TrySpread(candidate, world, server));
					}

					break;
				}
			}
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			var chunk = world.FindChunk(descriptor.Coordinates);
			user.Server.Scheduler.ScheduleEvent("grass", chunk,
				TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthTime, MaxGrowthTime)),
				s => TrySpread(descriptor.Coordinates, world, user.Server));
		}

		public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiPlayerServer server, IWorld world)
		{
			var chunk = world.FindChunk(coords);
			server.Scheduler.ScheduleEvent("grass", chunk,
				TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthTime, MaxGrowthTime)),
				s => TrySpread(coords, world, server));
		}
	}
}