using System;
using TrueCraft.Entities;
using TrueCraft.Extensions;
using TrueCraft.Networking;
using TrueCraft.Windows;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public class CraftingTableBlock : BlockProvider, ICraftingRecipe, IBurnableItem
	{
		public static readonly byte BlockId = 0x3A;

		public override byte Id => 0x3A;

		public override double BlastResistance => 12.5;

		public override double Hardness => 2.5;

		public override byte Luminance => 0;

		public override string DisplayName => "Crafting Table";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WoodenPlanksBlock.BlockId), new ItemStack(WoodenPlanksBlock.BlockId)},
				{new ItemStack(WoodenPlanksBlock.BlockId), new ItemStack(WoodenPlanksBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			var window = new CraftingBenchWindow(user.Server.CraftingRepository, (InventoryWindow) user.Inventory);
			user.OpenWindow(window);
			window.Disposed += (sender, e) =>
			{
				var entityManager = user.Server.GetEntityManagerForWorld(world);
				for (var i = 0; i < window.CraftingGrid.StartIndex + window.CraftingGrid.Length; i++)
				{
					var item = window[i];
					if (!item.Empty)
					{
						var entity = new ItemEntity(descriptor.Coordinates.AsVector3() + Coordinates3D.Up.AsVector3(), item);
						entityManager.SpawnEntity(entity);
					}
				}
			};
			return false;
		}

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(11, 3);
		}
	}
}