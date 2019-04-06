using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Blocks
{
	public class ObsidianBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x31;

		public override byte Id => 0x31;

		public override double BlastResistance => 6000;

		public override double Hardness => 10;

		public override byte Luminance => 0;

		public override string DisplayName => "Obsidian";

		public override ToolMaterial EffectiveToolMaterials => ToolMaterial.Diamond;

		public override ToolType EffectiveTools => ToolType.Pickaxe;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(5, 2);
		}
	}
}