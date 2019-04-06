﻿using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Items
{
	public abstract class SwordItem : ToolItem, ICraftingRecipe
	{
		public abstract float Damage { get; }

		public override ToolType ToolType => ToolType.Sword;

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
					{new ItemStack(baseMaterial)},
					{new ItemStack(StickItem.ItemId)}
				};
			}
		}

		public ItemStack Output => new ItemStack(Id);

		public bool SignificantMetadata => false;
	}
}