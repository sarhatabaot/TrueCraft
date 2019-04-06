using System;
using TrueCraft.Logic.Blocks;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Items
{
	public class CakeItem : FoodItem, ICraftingRecipe // TODO: This isn't really a FoodItem
	{
		public static readonly short ItemID = 0x162;

		public override short Id => 0x162;

		//This is per "slice"
		public override float Restores => 1.5f;

		public override string DisplayName => "Cake";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(MilkItem.ItemID), new ItemStack(MilkItem.ItemID), new ItemStack(MilkItem.ItemID)},
				{new ItemStack(SugarItem.ItemID), new ItemStack(EggItem.ItemID), new ItemStack(SugarItem.ItemID)},
				{new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID)}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(13, 1);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			coordinates += MathHelper.BlockFaceToCoordinates(face);
			var old = world.BlockRepository.GetBlockProvider(world.GetBlockId(coordinates));
			if (old.Hardness == 0)
			{
				world.SetBlockId(coordinates, CakeBlock.BlockId);
				item.Count--;
				user.Inventory[user.SelectedSlot] = item;
			}
		}
	}
}