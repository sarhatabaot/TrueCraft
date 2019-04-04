using System;
using TrueCraft.Logic.Blocks;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Items
{
	public class RedstoneItem : ItemProvider
	{
		public static readonly short ItemID = 0x14B;

		public override short ID => 0x14B;

		public override string DisplayName => "Redstone";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(8, 3);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			coordinates += MathHelper.BlockFaceToCoordinates(face);
			var supportingBlock =
				world.BlockRepository.GetBlockProvider(world.GetBlockId(coordinates + Coordinates3D.Down));

			if (supportingBlock.Opaque)
			{
				world.SetBlockId(coordinates, RedstoneDustBlock.BlockID);
				item.Count--;
				user.Inventory[user.SelectedSlot] = item;
			}
		}
	}
}