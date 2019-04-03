using System;
using TrueCraft.Extensions;
using TrueCraft.Logic.Items;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public class PistonBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockID = 0x21;

		public override byte ID => 0x21;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Piston";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockID),
					new ItemStack(WoodenPlanksBlock.BlockID),
					new ItemStack(WoodenPlanksBlock.BlockID)
				},
				{
					new ItemStack(CobblestoneBlock.BlockID),
					new ItemStack(IronIngotItem.ItemID),
					new ItemStack(CobblestoneBlock.BlockID)
				},
				{
					new ItemStack(CobblestoneBlock.BlockID),
					new ItemStack(RedstoneItem.ItemID),
					new ItemStack(CobblestoneBlock.BlockID)
				}
			};

		public ItemStack Output => new ItemStack(BlockID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(11, 6);
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			world.SetMetadata(descriptor.Coordinates,
				(byte) MathHelper.DirectionByRotation(user.Entity.Position, user.Entity.Yaw,
					descriptor.Coordinates.AsVector3(), true));
		}
	}
}