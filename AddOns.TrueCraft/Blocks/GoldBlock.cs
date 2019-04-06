using System;
using TrueCraft.Items;
using TrueCraft.Logic;

namespace TrueCraft.Blocks
{
	public class GoldBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x29;

		public override byte Id => 0x29;

		public override double BlastResistance => 30;

		public override double Hardness => 3;

		public override byte Luminance => 0;

		public override string DisplayName => "Block of Gold";

		public override ToolMaterial EffectiveToolMaterials => ToolMaterial.Iron | ToolMaterial.Diamond;

		public override ToolType EffectiveTools => ToolType.Pickaxe;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(GoldIngotItem.ItemId), new ItemStack(GoldIngotItem.ItemId),
					new ItemStack(GoldIngotItem.ItemId)
				},
				{
					new ItemStack(GoldIngotItem.ItemId), new ItemStack(GoldIngotItem.ItemId),
					new ItemStack(GoldIngotItem.ItemId)
				},
				{
					new ItemStack(GoldIngotItem.ItemId), new ItemStack(GoldIngotItem.ItemId),
					new ItemStack(GoldIngotItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(7, 1);
		}
	}
}