using System;
using TrueCraft.Extensions;
using TrueCraft.Items;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Blocks
{
	public class PistonBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x21;

		public override byte Id => 0x21;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Piston";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				},
				{
					new ItemStack(CobblestoneBlock.BlockId),
					new ItemStack(IronIngotItem.ItemId),
					new ItemStack(CobblestoneBlock.BlockId)
				},
				{
					new ItemStack(CobblestoneBlock.BlockId),
					new ItemStack(RedstoneItem.ItemId),
					new ItemStack(CobblestoneBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

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