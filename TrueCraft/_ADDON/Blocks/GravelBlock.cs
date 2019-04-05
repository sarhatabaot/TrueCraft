using System;
using TrueCraft.Entities;
using TrueCraft.Extensions;
using TrueCraft.Logic.Items;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public class GravelBlock : BlockProvider
	{
		public static readonly byte BlockID = 0x0D;

		public override byte ID => 0x0D;

		public override double BlastResistance => 3;

		public override double Hardness => 0.6;

		public override byte Luminance => 0;

		public override string DisplayName => "Gravel";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Gravel;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 1);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			//Gravel has a 10% chance of dropping flint.
			if (MathHelper.Random.Next(10) == 0)
				return new[] {new ItemStack(FlintItem.ItemID, 1, descriptor.Metadata)};
			return new ItemStack[0];
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			BlockUpdate(descriptor, descriptor, user.Server, world);
		}

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server,
			IWorld world)
		{
			if (world.GetBlockId(descriptor.Coordinates + Coordinates3D.Down) == AirBlock.BlockID)
			{
				world.SetBlockId(descriptor.Coordinates, AirBlock.BlockID);
				server.GetEntityManagerForWorld(world).SpawnEntity(new FallingGravelEntity(descriptor.Coordinates.AsVector3()));
			}
		}
	}
}