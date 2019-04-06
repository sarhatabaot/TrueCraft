using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class LapisLazuliBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x16;

		public override byte Id => 0x16;

		public override double BlastResistance => 15;

		public override double Hardness => 3;

		public override byte Luminance => 0;

		public override string DisplayName => "Lapis Lazuli Block";

		public override ToolMaterial EffectiveToolMaterials =>
			ToolMaterial.Stone | ToolMaterial.Iron | ToolMaterial.Diamond;

		public override ToolType EffectiveTools => ToolType.Pickaxe;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli),
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli),
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli)
				},
				{
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli),
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli),
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli)
				},
				{
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli),
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli),
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.LapisLazuli)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 9);
		}
	}
}