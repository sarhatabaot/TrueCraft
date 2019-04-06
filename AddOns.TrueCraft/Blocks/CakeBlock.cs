using System;
using TrueCraft.Items;
using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;
using TrueCraft.Logic.Items;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Blocks
{
	public class CakeBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x5C;

		public override byte Id => 0x5C;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Cake";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Cloth;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(MilkItem.ItemId),
					new ItemStack(MilkItem.ItemId),
					new ItemStack(MilkItem.ItemId)
				},
				{
					new ItemStack(SugarItem.ItemId),
					new ItemStack(EggItem.ItemId),
					new ItemStack(SugarItem.ItemId)
				},
				{
					new ItemStack(WheatItem.ItemId),
					new ItemStack(WheatItem.ItemId),
					new ItemStack(WheatItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(9, 7);
		}

		public override bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			if (descriptor.Metadata == 5)
				world.SetBlockId(descriptor.Coordinates, AirBlock.BlockId);
			else
				world.SetMetadata(descriptor.Coordinates, (byte) (descriptor.Metadata + 1));
			return false;
		}
	}
}