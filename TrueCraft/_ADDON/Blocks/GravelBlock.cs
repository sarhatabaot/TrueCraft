using System;
using TrueCraft.Extensions;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;
using TrueCraft._ADDON.Entities;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class GravelBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x0D;

		public override byte Id => 0x0D;

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
				return new[] {new ItemStack(FlintItem.ItemId, 1, descriptor.Metadata)};
			return new ItemStack[0];
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
				server.GetEntityManagerForWorld(world).SpawnEntity(new FallingGravelEntity(descriptor.Coordinates.AsVector3()));
			}
		}
	}
}