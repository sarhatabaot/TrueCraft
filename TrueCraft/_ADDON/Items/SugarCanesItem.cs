using System;
using TrueCraft.Logic.Blocks;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Items
{
	public class SugarCanesItem : ItemProvider
	{
		public static readonly short ItemID = 0x152;

		public override short ID => 0x152;

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
				world.SetBlockId(coordinates, SugarcaneBlock.BlockID);
				item.Count--;
				user.Inventory[user.SelectedSlot] = item;
				user.Server.BlockRepository.GetBlockProvider(SugarcaneBlock.BlockID).BlockPlaced(
					new BlockDescriptor {Coordinates = coordinates}, face, world, user);
			}
		}
	}
}