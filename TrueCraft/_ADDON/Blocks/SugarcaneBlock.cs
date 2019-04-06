using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class SugarcaneBlock : BlockProvider
	{
		public static readonly int MinGrowthSeconds = 30;
		public static readonly int MaxGrowthSeconds = 120;
		public static readonly int MaxGrowHeight = 3;

		public static readonly byte BlockId = 0x53;

		public override byte Id => 0x53;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Sugar cane";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

		public override BoundingBox? BoundingBox => null;

		public override BoundingBox? InteractiveBoundingBox => new BoundingBox(new Vector3(2 / 16.0f, 0, 2 / 16.0f),
			new Vector3(14 / 16.0f, 1.0f, 14 / 16.0f));

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(9, 4);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(SugarCanesItem.ItemId)};
		}

		public static bool ValidPlacement(BlockDescriptor descriptor, IWorld world)
		{
			var below = world.GetBlockId(descriptor.Coordinates + Coordinates3D.Down);
			if (below != BlockId && below != GrassBlock.BlockId && below != DirtBlock.BlockId)
				return false;
			var toCheck = new[]
			{
				Coordinates3D.Down + Coordinates3D.Left,
				Coordinates3D.Down + Coordinates3D.Right,
				Coordinates3D.Down + Coordinates3D.Backwards,
				Coordinates3D.Down + Coordinates3D.Forwards
			};
			if (below != BlockId)
			{
				var foundWater = false;
				for (var i = 0; i < toCheck.Length; i++)
				{
					var Id = world.GetBlockId(descriptor.Coordinates + toCheck[i]);
					if (Id == WaterBlock.BlockId || Id == StationaryWaterBlock.BlockId)
					{
						foundWater = true;
						break;
					}
				}

				return foundWater;
			}

			return true;
		}

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server,
			IWorld world)
		{
			if (!ValidPlacement(descriptor, world))
			{
				// Destroy self
				world.SetBlockId(descriptor.Coordinates, 0);
				GenerateDropEntity(descriptor, world, server, ItemStack.EmptyStack);
			}
		}

		private void TryGrowth(IMultiPlayerServer server, Coordinates3D coords, IWorld world)
		{
			if (world.GetBlockId(coords) != BlockId)
				return;
			// Find current height of stalk
			var height = 0;
			for (var y = -MaxGrowHeight; y <= MaxGrowHeight; y++)
				if (world.GetBlockId(coords + Coordinates3D.Down * y) == BlockId)
					height++;
			if (height < MaxGrowHeight)
			{
				var meta = world.GetMetadata(coords);
				meta++;
				world.SetMetadata(coords, meta);
				var chunk = world.FindChunk(coords);
				if (meta == 15)
				{
					if (world.GetBlockId(coords + Coordinates3D.Up) == 0)
					{
						world.SetBlockId(coords + Coordinates3D.Up, BlockId);
						server.Scheduler.ScheduleEvent("sugarcane", chunk,
							TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
							_server => TryGrowth(_server, coords + Coordinates3D.Up, world));
					}
				}
				else
					server.Scheduler.ScheduleEvent("sugarcane", chunk,
						TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
						_server => TryGrowth(_server, coords, world));
			}
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			var chunk = world.FindChunk(descriptor.Coordinates);
			user.Server.Scheduler.ScheduleEvent("sugarcane", chunk,
				TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
				server => TryGrowth(server, descriptor.Coordinates, world));
		}

		public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiPlayerServer server, IWorld world)
		{
			var chunk = world.FindChunk(coords);
			server.Scheduler.ScheduleEvent("sugarcane", chunk,
				TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
				s => TryGrowth(s, coords, world));
		}
	}
}