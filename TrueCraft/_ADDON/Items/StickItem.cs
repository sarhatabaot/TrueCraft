using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class StickItem : ItemProvider, ICraftingRecipe, IBurnableItem
	{
		public static readonly short ItemId = 0x118;

		public override short Id => 0x118;

		public override string DisplayName => "Stick";

		public TimeSpan BurnTime => TimeSpan.FromSeconds(5);

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WoodenPlanksBlock.BlockId)},
				{new ItemStack(WoodenPlanksBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(ItemId, 4);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(5, 3);
		}
	}
}