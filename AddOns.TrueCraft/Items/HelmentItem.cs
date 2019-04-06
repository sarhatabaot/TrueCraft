using TrueCraft.Logic;
using TrueCraft.Logic.Items;

namespace TrueCraft.Items
{
	public abstract class HelmentItem : ArmorItem, ICraftingRecipe
	{
		public override sbyte MaximumStack => 1;

		public ItemStack[,] Pattern
		{
			get
			{
				short baseMaterial = 0;
				switch (Material)
				{
					case ArmorMaterial.Diamond:
						baseMaterial = DiamondItem.ItemId;
						break;
					case ArmorMaterial.Gold:
						baseMaterial = GoldIngotItem.ItemId;
						break;
					case ArmorMaterial.Iron:
						baseMaterial = IronIngotItem.ItemId;
						break;
					case ArmorMaterial.Leather:
						baseMaterial = LeatherItem.ItemId;
						break;
				}

				return new[,]
				{
					{new ItemStack(baseMaterial), new ItemStack(baseMaterial), new ItemStack(baseMaterial)},
					{new ItemStack(baseMaterial), ItemStack.EmptyStack, new ItemStack(baseMaterial)}
				};
			}
		}

		public ItemStack Output => new ItemStack(Id);

		public bool SignificantMetadata => false;
	}
}