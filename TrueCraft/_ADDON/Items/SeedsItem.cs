using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Items
{
	public class SeedsItem : ItemProvider
	{
		public static readonly short ItemId = 0x127;

		public override short Id => 0x127;

		public override string DisplayName => "Seeds";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(9, 0);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			if (world.GetBlockId(coordinates) == FarmlandBlock.BlockId)
			{
				world.SetBlockId(coordinates + MathHelper.BlockFaceToCoordinates(face), CropsBlock.BlockId);
				world.BlockRepository.GetBlockProvider(CropsBlock.BlockId).BlockPlaced(
					new BlockDescriptor {Coordinates = coordinates}, face, world, user);
			}
		}
	}
}