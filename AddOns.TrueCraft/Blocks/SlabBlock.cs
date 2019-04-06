using System;
using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Blocks
{
	public class SlabBlock : BlockProvider
	{
		public enum SlabMaterial
		{
			Stone = 0x0,
			Standstone = 0x1,
			Wooden = 0x2,
			Cobblestone = 0x3
		}

		public static readonly byte BlockId = 0x2C;

		public override byte Id => 0x2C;

		public override double BlastResistance => 30;

		public override double Hardness => 2;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override byte LightOpacity => 255;

		public override string DisplayName => "Stone Slab";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(6, 0);
		}

		public class StoneSlabRecipe : ICraftingRecipe
		{
			public ItemStack[,] Pattern => new[,]
			{
				{
					new ItemStack(StoneBlock.BlockId), new ItemStack(StoneBlock.BlockId),
					new ItemStack(StoneBlock.BlockId)
				}
			};

			public ItemStack Output => new ItemStack(BlockId, 3, (short) SlabMaterial.Stone);

			public bool SignificantMetadata => true;
		}

		public class StandstoneSlabRecipe : ICraftingRecipe
		{
			public ItemStack[,] Pattern => new[,]
			{
				{
					new ItemStack(SandstoneBlock.BlockId), new ItemStack(SandstoneBlock.BlockId),
					new ItemStack(SandstoneBlock.BlockId)
				}
			};

			public ItemStack Output => new ItemStack(BlockId, 3, (short) SlabMaterial.Standstone);

			public bool SignificantMetadata => true;
		}

		public class WoodenSlabRecipe : ICraftingRecipe
		{
			public ItemStack[,] Pattern => new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockId), new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				}
			};

			public ItemStack Output => new ItemStack(BlockId, 3, (short) SlabMaterial.Wooden);

			public bool SignificantMetadata => true;
		}

		public class CobblestoneSlabRecipe : ICraftingRecipe
		{
			public ItemStack[,] Pattern => new[,]
			{
				{
					new ItemStack(CobblestoneBlock.BlockId), new ItemStack(CobblestoneBlock.BlockId),
					new ItemStack(CobblestoneBlock.BlockId)
				}
			};

			public ItemStack Output => new ItemStack(BlockId, 3, (short) SlabMaterial.Cobblestone);

			public bool SignificantMetadata => true;
		}
	}
}