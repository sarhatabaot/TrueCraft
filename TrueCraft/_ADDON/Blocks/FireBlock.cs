using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft._ADDON.Blocks
{
	public class FireBlock : BlockProvider
	{
		public static readonly int MinSpreadTime = 1;
		public static readonly int MaxSpreadTime = 5;

		public static readonly byte BlockId = 0x33;

		private static readonly Coordinates3D[] SpreadableBlocks =
		{
			Coordinates3D.Down,
			Coordinates3D.Left,
			Coordinates3D.Right,
			Coordinates3D.Forwards,
			Coordinates3D.Backwards,
			Coordinates3D.Up * 1,
			Coordinates3D.Up * 2,
			Coordinates3D.Up * 3,
			Coordinates3D.Up * 4
		};

		private static readonly Coordinates3D[] AdjacentBlocks =
		{
			Coordinates3D.Up,
			Coordinates3D.Down,
			Coordinates3D.Left,
			Coordinates3D.Right,
			Coordinates3D.Forwards,
			Coordinates3D.Backwards
		};

		public override byte Id => 0x33;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 15;

		public override bool Opaque => false;

		public override string DisplayName => "Fire";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(15, 1);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new ItemStack[0];
		}

		public void DoUpdate(IMultiPlayerServer server, IWorld world, BlockDescriptor descriptor)
		{
			var down = descriptor.Coordinates + Coordinates3D.Down;

			var current = world.GetBlockId(descriptor.Coordinates);
			if (current != BlockId && current != LavaBlock.BlockId && current != StationaryLavaBlock.BlockId)
				return;

			// Decay
			var meta = world.GetMetadata(descriptor.Coordinates);
			meta++;
			if (meta == 0xE)
				if (!world.IsValidPosition(down) || world.GetBlockId(down) != NetherrackBlock.BlockId)
				{
					world.SetBlockId(descriptor.Coordinates, AirBlock.BlockId);
					return;
				}

			world.SetMetadata(descriptor.Coordinates, meta);

			if (meta > 9)
			{
				var pick = AdjacentBlocks[meta % AdjacentBlocks.Length];
				var provider = BlockRepository
					.GetBlockProvider(world.GetBlockId(pick + descriptor.Coordinates));
				if (provider.Flammable)
					world.SetBlockId(pick + descriptor.Coordinates, AirBlock.BlockId);
			}

			// Spread
			DoSpread(server, world, descriptor);

			// Schedule next event
			ScheduleUpdate(server, world, descriptor);
		}

		public void DoSpread(IMultiPlayerServer server, IWorld world, BlockDescriptor descriptor)
		{
			foreach (var coord in SpreadableBlocks)
			{
				var check = descriptor.Coordinates + coord;
				if (world.GetBlockId(check) == AirBlock.BlockId)
					foreach (var adj in AdjacentBlocks)
					{
						var provider = BlockRepository.GetBlockProvider(
							world.GetBlockId(check + adj));
						if (provider.Flammable)
						{
							if (provider.Hardness == 0)
								check = check + adj;

							// Spread to this block
							world.SetBlockId(check, BlockId);
							ScheduleUpdate(server, world, world.GetBlockData(check));
							break;
						}
					}
			}
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			ScheduleUpdate(user.Server, world, descriptor);
		}

		public void ScheduleUpdate(IMultiPlayerServer server, IWorld world, BlockDescriptor descriptor)
		{
			var chunk = world.FindChunk(descriptor.Coordinates);
			server.Scheduler.ScheduleEvent("fire.spread", chunk,
				TimeSpan.FromSeconds(MathHelper.Random.Next(MinSpreadTime, MaxSpreadTime)),
				s => DoUpdate(s, world, descriptor));
		}
	}
}