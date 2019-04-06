using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class IronBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x2A;

		public override byte Id => 0x2A;

		public override double BlastResistance => 30;

		public override double Hardness => 5;

		public override byte Luminance => 0;

		public override string DisplayName => "Block of Iron";

		public override ToolMaterial EffectiveToolMaterials =>
			ToolMaterial.Stone | ToolMaterial.Iron | ToolMaterial.Diamond;

		public override ToolType EffectiveTools => ToolType.Pickaxe;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(IronIngotItem.ItemId), new ItemStack(IronIngotItem.ItemId),
					new ItemStack(IronIngotItem.ItemId)
				},
				{
					new ItemStack(IronIngotItem.ItemId), new ItemStack(IronIngotItem.ItemId),
					new ItemStack(IronIngotItem.ItemId)
				},
				{
					new ItemStack(IronIngotItem.ItemId), new ItemStack(IronIngotItem.ItemId),
					new ItemStack(IronIngotItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(6, 1);
		}
	}
}