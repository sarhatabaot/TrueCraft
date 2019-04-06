using System;

namespace TrueCraft.Logic.Items
{
	public class SugarItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x161;

		public override short Id => 0x161;

		public override string DisplayName => "Sugar";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(SugarCanesItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(13, 0);
		}
	}
}