using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Items
{
	public class RedstoneItem : ItemProvider
	{
		public static readonly short ItemId = 0x14B;

		public override short Id => 0x14B;

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
				world.SetBlockId(coordinates, RedstoneDustBlock.BlockId);
				item.Count--;
				user.Inventory[user.SelectedSlot] = item;
			}
		}
	}
}