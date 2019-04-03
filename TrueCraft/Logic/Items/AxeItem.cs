﻿using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public abstract class AxeItem : ToolItem, ICraftingRecipe
	{
		public override ToolType ToolType => ToolType.Axe;

		public ItemStack[,] Pattern
		{
			get
			{
				short baseMaterial = 0;
				switch (Material)
				{
					case ToolMaterial.Diamond:
						baseMaterial = DiamondItem.ItemID;
						break;
					case ToolMaterial.Gold:
						baseMaterial = GoldIngotItem.ItemID;
						break;
					case ToolMaterial.Iron:
						baseMaterial = IronIngotItem.ItemID;
						break;
					case ToolMaterial.Stone:
						baseMaterial = CobblestoneBlock.BlockID;
						break;
					case ToolMaterial.Wood:
						baseMaterial = WoodenPlanksBlock.BlockID;
						break;
				}

				return new[,]
				{
					{new ItemStack(baseMaterial), new ItemStack(baseMaterial)},
					{new ItemStack(baseMaterial), new ItemStack(StickItem.ItemID)},
					{ItemStack.EmptyStack, new ItemStack(StickItem.ItemID)}
				};
			}
		}

		public ItemStack Output => new ItemStack(ID);

		public bool SignificantMetadata => false;
	}
}