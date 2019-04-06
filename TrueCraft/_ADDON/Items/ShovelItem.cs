﻿using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
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
						baseMaterial = DiamondItem.ItemID;
						break;
					case ToolMaterial.Gold:
						baseMaterial = GoldIngotItem.ItemID;
						break;
					case ToolMaterial.Iron:
						baseMaterial = IronIngotItem.ItemID;
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
					{new ItemStack(StickItem.ItemID)},
					{new ItemStack(StickItem.ItemID)}
				};
			}
		}

		public ItemStack Output => new ItemStack(Id);

		public bool SignificantMetadata => false;
	}
}