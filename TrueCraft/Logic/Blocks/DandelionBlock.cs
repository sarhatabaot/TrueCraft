using System;
using Microsoft.Xna.Framework;
using TrueCraft.API.Logic;
using BoundingBox = TrueCraft.API.BoundingBox;

namespace TrueCraft.Core.Logic.Blocks
{
	public class DandelionBlock : BlockProvider
	{
		public static readonly byte BlockID = 0x25;

		public override byte ID => 0x25;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Flower";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

		public override BoundingBox? BoundingBox => null;

		public override BoundingBox? InteractiveBoundingBox => new BoundingBox(new Vector3(4 / 16.0f, 0, 4 / 16.0f),
			new Vector3(12 / 16.0f, 8 / 16.0f, 12 / 16.0f));

		public override bool Flammable => true;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(13, 0);
		}
	}
}