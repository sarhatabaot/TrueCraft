using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;
using TrueCraft.Logic.Items;

namespace TrueCraft.Items
{
	public abstract class ShovelItem : ToolItem, ICraftingRecipe
	{
		public override ToolType ToolType => ToolType.Shovel;

		public ItemStack[,] Pattern
		{
			get
			{
				short baseMaterial = 0;
				switch (Material)
				{
					case ToolMaterial.Diamond:
						baseMaterial = DiamondItem.ItemId;
						break;
					case ToolMaterial.Gold:
						baseMaterial = GoldIngotItem.ItemId;
						break;
					case ToolMaterial.Iron:
						baseMaterial = IronIngotItem.ItemId;
						break;
					case ToolMaterial.Stone:
						baseMaterial = CobblestoneBlock.BlockId;
						break;
					case ToolMaterial.Wood:
						baseMaterial = WoodenPlanksBlock.BlockId;
						break;
				}

				return new[,]
				{
					{new ItemStack(baseMaterial)},
					{new ItemStack(StickItem.ItemId)},
					{new ItemStack(StickItem.ItemId)}
				};
			}
		}

		public ItemStack Output => new ItemStack(Id);

		public bool SignificantMetadata => false;
	}
}