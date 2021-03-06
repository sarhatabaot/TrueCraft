using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Items
{
	public class SugarCanesItem : ItemProvider
	{
		public static readonly short ItemId = 0x152;

		public override short Id => 0x152;

		public override string DisplayName => "Sugar Canes";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(11, 1);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			coordinates += MathHelper.BlockFaceToCoordinates(face);
			if (SugarcaneBlock.ValidPlacement(new BlockDescriptor {Coordinates = coordinates}, world))
			{
				world.SetBlockId(coordinates, SugarcaneBlock.BlockId);
				item.Count--;
				user.Inventory[user.SelectedSlot] = item;
				user.Server.BlockRepository.GetBlockProvider(SugarcaneBlock.BlockId).BlockPlaced(
					new BlockDescriptor {Coordinates = coordinates}, face, world, user);
			}
		}
	}
}