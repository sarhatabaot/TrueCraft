using System;
using TrueCraft.Logic.Blocks;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic.Items
{
	public class BucketItem : ToolItem
	{
		public static readonly short ItemID = 0x145;

		public override short ID => 0x145;

		public override string DisplayName => "Bucket";

		protected virtual byte? RelevantBlockType => null;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(10, 4);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			coordinates += MathHelper.BlockFaceToCoordinates(face);
			if (item.ID == ItemID) // Empty bucket
			{
				var block = world.GetBlockId(coordinates);
				if (block == WaterBlock.BlockID || block == StationaryWaterBlock.BlockID)
				{
					var meta = world.GetMetadata(coordinates);
					if (meta == 0) // Is source block?
					{
						user.Inventory[user.SelectedSlot] = new ItemStack(WaterBucketItem.ItemID);
						world.SetBlockId(coordinates, 0);
					}
				}
				else if (block == LavaBlock.BlockID || block == StationaryLavaBlock.BlockID)
				{
					var meta = world.GetMetadata(coordinates);
					if (meta == 0) // Is source block?
					{
						user.Inventory[user.SelectedSlot] = new ItemStack(LavaBucketItem.ItemID);
						world.SetBlockId(coordinates, 0);
					}
				}
			}
			else
			{
				var provider = user.Server.BlockRepository.GetBlockProvider(world.GetBlockId(coordinates));
				if (!provider.Opaque)
				{
					if (RelevantBlockType != null)
					{
						var blockType = RelevantBlockType.Value;
						user.Server.BlockUpdatesEnabled = false;
						world.SetBlockId(coordinates, blockType);
						world.SetMetadata(coordinates, 0); // Source block
						user.Server.BlockUpdatesEnabled = true;
						var liquidProvider = world.BlockRepository.GetBlockProvider(blockType);
						liquidProvider.BlockPlaced(new BlockDescriptor {Coordinates = coordinates}, face, world, user);
					}

					user.Inventory[user.SelectedSlot] = new ItemStack(ItemID);
				}
			}
		}
	}
}