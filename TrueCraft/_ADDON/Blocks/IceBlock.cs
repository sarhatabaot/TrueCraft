using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft._ADDON.Blocks
{
	public class IceBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x4F;

		public override byte Id => 0x4F;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override byte LightOpacity => 2;

		public override string DisplayName => "Ice";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Glass;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 4);
		}

		public override void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			world.SetBlockId(descriptor.Coordinates, WaterBlock.BlockId);
			BlockRepository.GetBlockProvider(WaterBlock.BlockId).BlockPlaced(descriptor, face, world, user);
		}
	}
}