using TrueCraft.Logic.Blocks;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Items
{
	public abstract class HoeItem : ToolItem, ICraftingRecipe
	{
		public override ToolType ToolType => ToolType.Hoe;

		public ItemStack[,] Pattern
		{
			get
			{
				short baseMaterial = 0;
				switch (Material)
				{
					case ToolMaterial.Diamond:
						baseMaterial = DiamondItem.ItemId;
						break;
					case ToolMaterial.Gold:
						baseMaterial = GoldIngotItem.ItemId;
						break;
					case ToolMaterial.Iron:
						baseMaterial = IronIngotItem.ItemId;
						break;
					case ToolMaterial.Stone:
						baseMaterial = CobblestoneBlock.BlockId;
						break;
					case ToolMaterial.Wood:
						baseMaterial = WoodenPlanksBlock.BlockId;
						break;
				}

				return new[,]
				{
					{new ItemStack(baseMaterial), new ItemStack(baseMaterial)},
					{ItemStack.EmptyStack, new ItemStack(StickItem.ItemId)},
					{ItemStack.EmptyStack, new ItemStack(StickItem.ItemId)}
				};
			}
		}

		public ItemStack Output => new ItemStack(Id);

		public bool SignificantMetadata => false;

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			var Id = world.GetBlockId(coordinates);
			if (Id == DirtBlock.BlockId || Id == GrassBlock.BlockId)
			{
				world.SetBlockId(coordinates, FarmlandBlock.BlockId);
				user.Server.BlockRepository.GetBlockProvider(FarmlandBlock.BlockId).BlockPlaced(
					new BlockDescriptor {Coordinates = coordinates}, face, world, user);
			}
		}
	}
}