using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class SnowfallBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x4E;

		public override byte Id => 0x4E;

		public override double BlastResistance => 0.5;

		public override double Hardness => 0.6;

		public override byte Luminance => 0;

		public override bool RenderOpaque => true;

		public override bool Opaque => false;

		public override string DisplayName => "Snow";

		public override BoundingBox? BoundingBox => null;

		public override SoundEffectClass SoundEffect => SoundEffectClass.Snow;

		public override BoundingBox? InteractiveBoundingBox =>
			new BoundingBox(Vector3.Zero, new Vector3(1, 1 / 16.0f, 1));

		public override ToolType EffectiveTools => ToolType.Shovel;

		public override Coordinates3D GetSupportDirection(BlockDescriptor descriptor)
		{
			return Coordinates3D.Down;
		}

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(2, 4);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(SnowballItem.ItemId)};
		}
	}
}