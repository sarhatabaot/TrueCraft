using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Items
{
	public class SignItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x143;

		public override short Id => 0x143;

		public override sbyte MaximumStack => 1;

		public override string DisplayName => "Sign";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockId), new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockId), new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				},
				{ItemStack.EmptyStack, new ItemStack(StickItem.ItemId), ItemStack.EmptyStack}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(10, 2);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			if (face == BlockFace.PositiveY)
			{
				var provider = user.Server.BlockRepository.GetBlockProvider(UprightSignBlock.BlockId);
				provider.ItemUsedOnBlock(coordinates, item, face, world, user);
			}
			else
			{
				var provider = user.Server.BlockRepository.GetBlockProvider(WallSignBlock.BlockId);
				provider.ItemUsedOnBlock(coordinates, item, face, world, user);
			}
		}
	}
}