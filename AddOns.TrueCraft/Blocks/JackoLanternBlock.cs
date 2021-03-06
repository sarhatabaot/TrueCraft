using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Blocks
{
	public class JackoLanternBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x5B;

		public override byte Id => 0x5B;

		public override double BlastResistance => 5;

		public override double Hardness => 1;

		public override byte Luminance => 15;

		public override bool Opaque => false;

		public override byte LightOpacity => 255;

		public override string DisplayName => "Jack 'o' Lantern";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(PumpkinBlock.BlockId)},
				{new ItemStack(TorchBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(6, 6);
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			world.SetMetadata(descriptor.Coordinates, (byte) MathHelper.DirectionByRotationFlat(user.Entity.Yaw, true));
		}
	}
}