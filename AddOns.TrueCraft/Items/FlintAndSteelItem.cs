using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Items
{
	public class FlintAndSteelItem : ToolItem, ICraftingRecipe
	{
		public static readonly short ItemId = 0x103;
		public override short Id => 0x103;
		public override sbyte MaximumStack => 1;
		public override short BaseDurability => 65;
		public override string DisplayName => "Flint and Steel";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(IronIngotItem.ItemId), new ItemStack(FlintItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(5, 0);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			coordinates += MathHelper.BlockFaceToCoordinates(face);
			if (world.GetBlockId(coordinates) == AirBlock.BlockId)
			{
				world.SetBlockId(coordinates, FireBlock.BlockId);
				world.BlockRepository.GetBlockProvider(FireBlock.BlockId)
					.BlockPlaced(world.GetBlockData(coordinates), face, world, user);

				var slot = user.SelectedItem;
				slot.Metadata += 1;
				if (slot.Metadata >= Uses)
					slot.Count = 0; // Destroy item
				user.Inventory[user.SelectedSlot] = slot;
			}
		}
	}
}