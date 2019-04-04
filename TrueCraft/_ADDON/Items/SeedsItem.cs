using System;
using TrueCraft.Logic.Blocks;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Items
{
	public class SeedsItem : ItemProvider
	{
		public static readonly short ItemID = 0x127;

		public override short ID => 0x127;

		public override string DisplayName => "Seeds";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(9, 0);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			if (world.GetBlockId(coordinates) == FarmlandBlock.BlockID)
			{
				world.SetBlockId(coordinates + MathHelper.BlockFaceToCoordinates(face), CropsBlock.BlockID);
				world.BlockRepository.GetBlockProvider(CropsBlock.BlockID).BlockPlaced(
					new BlockDescriptor {Coordinates = coordinates}, face, world, user);
			}
		}
	}
}