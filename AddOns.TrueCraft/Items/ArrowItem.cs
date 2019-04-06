using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Items
{
	public class ArrowItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x106;

		public override short Id => 0x106;

		public override string DisplayName => "Arrow";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(FlintItem.ItemId)},
				{new ItemStack(StickItem.ItemId)},
				{new ItemStack(FeatherItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(5, 2);
		}
	}
}