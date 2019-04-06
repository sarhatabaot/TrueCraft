using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class DiamondOreBlock : BlockProvider, ISmeltableItem
	{
		public static readonly byte BlockId = 0x38;

		public override byte Id => 0x38;

		public override double BlastResistance => 15;

		public override double Hardness => 3;

		public override byte Luminance => 0;

		public override string DisplayName => "Diamond Ore";

		public override ToolMaterial EffectiveToolMaterials => ToolMaterial.Iron | ToolMaterial.Diamond;

		public override ToolType EffectiveTools => ToolType.Pickaxe;

		public ItemStack SmeltingOutput => new ItemStack(DiamondItem.ItemID);

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(2, 3);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(DiamondItem.ItemID, 1, descriptor.Metadata)};
		}
	}
}