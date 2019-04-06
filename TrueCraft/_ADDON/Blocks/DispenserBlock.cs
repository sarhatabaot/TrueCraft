using System;
using TrueCraft.Logic.Items;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public class DispenserBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x17;

		public override byte Id => 0x17;

		public override double BlastResistance => 17.5;

		public override double Hardness => 3.5;

		public override byte Luminance => 0;

		public override string DisplayName => "Dispenser";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(CobblestoneBlock.BlockId),
					new ItemStack(CobblestoneBlock.BlockId),
					new ItemStack(CobblestoneBlock.BlockId)
				},
				{
					new ItemStack(CobblestoneBlock.BlockId),
					new ItemStack(BowItem.ItemID),
					new ItemStack(CobblestoneBlock.BlockId)
				},
				{
					new ItemStack(CobblestoneBlock.BlockId),
					new ItemStack(RedstoneItem.ItemID),
					new ItemStack(CobblestoneBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(13, 2);
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			world.SetMetadata(descriptor.Coordinates, (byte) MathHelper.DirectionByRotationFlat(user.Entity.Yaw, true));
		}
	}
}