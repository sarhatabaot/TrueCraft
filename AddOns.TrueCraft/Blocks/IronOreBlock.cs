using System;
using TrueCraft.Items;
using TrueCraft.Logic;

namespace TrueCraft.Blocks
{
	public class IronOreBlock : BlockProvider, ISmeltableItem
	{
		public static readonly byte BlockId = 0x0F;

		public override byte Id => 0x0F;

		public override double BlastResistance => 15;

		public override double Hardness => 3;

		public override byte Luminance => 0;

		public override string DisplayName => "Iron Ore";

		public override ToolMaterial EffectiveToolMaterials =>
			ToolMaterial.Stone | ToolMaterial.Iron | ToolMaterial.Diamond;

		public override ToolType EffectiveTools => ToolType.Pickaxe;

		public ItemStack SmeltingOutput => new ItemStack(IronIngotItem.ItemId);

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(1, 2);
		}
	}
}