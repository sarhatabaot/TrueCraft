using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.API.Networking;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
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
						baseMaterial = DiamondItem.ItemID;
						break;
					case ToolMaterial.Gold:
						baseMaterial = GoldIngotItem.ItemID;
						break;
					case ToolMaterial.Iron:
						baseMaterial = IronIngotItem.ItemID;
						break;
					case ToolMaterial.Stone:
						baseMaterial = CobblestoneBlock.BlockID;
						break;
					case ToolMaterial.Wood:
						baseMaterial = WoodenPlanksBlock.BlockID;
						break;
				}

				return new[,]
				{
					{new ItemStack(baseMaterial), new ItemStack(baseMaterial)},
					{ItemStack.EmptyStack, new ItemStack(StickItem.ItemID)},
					{ItemStack.EmptyStack, new ItemStack(StickItem.ItemID)}
				};
			}
		}

		public ItemStack Output => new ItemStack(ID);

		public bool SignificantMetadata => false;

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			var id = world.GetBlockID(coordinates);
			if (id == DirtBlock.BlockID || id == GrassBlock.BlockID)
			{
				world.SetBlockID(coordinates, FarmlandBlock.BlockID);
				user.Server.BlockRepository.GetBlockProvider(FarmlandBlock.BlockID).BlockPlaced(
					new BlockDescriptor {Coordinates = coordinates}, face, world, user);
			}
		}
	}
}