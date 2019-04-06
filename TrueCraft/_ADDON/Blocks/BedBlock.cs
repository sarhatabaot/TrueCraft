using System;
using TrueCraft.Logic.Items;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public class BedBlock : BlockProvider
	{
		[Flags]
		public enum BedDirection : byte
		{
			South = 0x0,
			West = 0x1,
			North = 0x2,
			East = 0x3
		}

		[Flags]
		public enum BedType : byte
		{
			Foot = 0x0,
			Head = 0x8
		}

		public static readonly byte BlockId = 0x1A;

		public override byte Id => 0x1A;

		public override double BlastResistance => 1;

		public override double Hardness => 0.2;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Bed";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(6, 8);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(BedItem.ItemID)};
		}

		public bool ValidBedPosition(BlockDescriptor descriptor, IBlockRepository repository, IWorld world,
			bool checkNeighbor = true, bool checkSupport = false)
		{
			if (checkNeighbor)
			{
				var other = Coordinates3D.Zero;
				switch ((BedDirection) (descriptor.Metadata & 0x3))
				{
					case BedDirection.East:
						other = Coordinates3D.East;
						break;
					case BedDirection.West:
						other = Coordinates3D.West;
						break;
					case BedDirection.North:
						other = Coordinates3D.North;
						break;
					case BedDirection.South:
						other = Coordinates3D.South;
						break;
				}

				if ((descriptor.Metadata & (byte) BedType.Head) == (byte) BedType.Head)
					other = -other;
				if (world.GetBlockId(descriptor.Coordinates + other) != BlockId)
					return false;
			}

			if (checkSupport)
			{
				var supportingBlock =
					repository.GetBlockProvider(world.GetBlockId(descriptor.Coordinates + Coordinates3D.Down));
				if (!supportingBlock.Opaque)
					return false;
			}

			return true;
		}

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server,
			IWorld world)
		{
			if (!ValidBedPosition(descriptor, server.BlockRepository, world))
				world.SetBlockId(descriptor.Coordinates, 0);
			base.BlockUpdate(descriptor, source, server, world);
		}
	}
}