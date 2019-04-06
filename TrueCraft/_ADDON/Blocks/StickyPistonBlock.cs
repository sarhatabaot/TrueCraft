using System;
using TrueCraft.Extensions;
using TrueCraft.Logic.Items;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public class StickyPistonBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x1D;

		public override byte Id => 0x1D;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Sticky Piston";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(SlimeballItem.ItemID)},
				{new ItemStack(PistonBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(10, 6);
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			world.SetMetadata(descriptor.Coordinates,
				(byte) MathHelper.DirectionByRotation(user.Entity.Position, user.Entity.Yaw,
					descriptor.Coordinates.AsVector3(), true));
		}
	}
}