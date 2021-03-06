using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class DiamondBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x39;

		public override byte Id => 0x39;

		public override double BlastResistance => 30;

		public override double Hardness => 5;

		public override byte Luminance => 0;

		public override string DisplayName => "Block of Diamond";

		public override ToolMaterial EffectiveToolMaterials => ToolMaterial.Iron | ToolMaterial.Diamond;

		public override ToolType EffectiveTools => ToolType.Pickaxe;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(DiamondItem.ItemId), new ItemStack(DiamondItem.ItemId),
					new ItemStack(DiamondItem.ItemId)
				},
				{
					new ItemStack(DiamondItem.ItemId), new ItemStack(DiamondItem.ItemId),
					new ItemStack(DiamondItem.ItemId)
				},
				{
					new ItemStack(DiamondItem.ItemId), new ItemStack(DiamondItem.ItemId),
					new ItemStack(DiamondItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(8, 1);
		}
	}
}