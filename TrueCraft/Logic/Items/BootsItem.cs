using TrueCraft.API;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
	public abstract class BootsItem : ArmorItem, ICraftingRecipe
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
						baseMaterial = DiamondItem.ItemID;
						break;
					case ArmorMaterial.Gold:
						baseMaterial = GoldIngotItem.ItemID;
						break;
					case ArmorMaterial.Iron:
						baseMaterial = IronIngotItem.ItemID;
						break;
					case ArmorMaterial.Leather:
						baseMaterial = LeatherItem.ItemID;
						break;
				}

				return new[,]
				{
					{new ItemStack(baseMaterial), ItemStack.EmptyStack, new ItemStack(baseMaterial)},
					{new ItemStack(baseMaterial), ItemStack.EmptyStack, new ItemStack(baseMaterial)}
				};
			}
		}

		public ItemStack Output => new ItemStack(ID);

		public bool SignificantMetadata => false;
	}
}