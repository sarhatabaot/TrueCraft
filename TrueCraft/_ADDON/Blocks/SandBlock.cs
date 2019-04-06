using System;
using TrueCraft.Extensions;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;
using TrueCraft._ADDON.Entities;

namespace TrueCraft._ADDON.Blocks
{
	public class SandBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x0C;

		public override byte Id => 0x0C;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override string DisplayName => "Sand";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Sand;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(2, 1);
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			BlockUpdate(descriptor, descriptor, user.Server, world);
		}

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server,
			IWorld world)
		{
			if (world.GetBlockId(descriptor.Coordinates + Coordinates3D.Down) == AirBlock.BlockId)
			{
				world.SetBlockId(descriptor.Coordinates, AirBlock.BlockId);
				server.GetEntityManagerForWorld(world).SpawnEntity(new FallingSandEntity(descriptor.Coordinates.AsVector3()));
			}
		}
	}
}