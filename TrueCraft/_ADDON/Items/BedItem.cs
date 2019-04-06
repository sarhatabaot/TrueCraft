using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Items
{
	public class BedItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x163;

		public override short Id => 0x163;

		public override sbyte MaximumStack => 1;

		public override string DisplayName => "Bed";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoolBlock.BlockId),
					new ItemStack(WoolBlock.BlockId),
					new ItemStack(WoolBlock.BlockId)
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(13, 2);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			coordinates += MathHelper.BlockFaceToCoordinates(face);
			var head = coordinates;
			var foot = coordinates;
			var direction = BedBlock.BedDirection.North;
			switch (MathHelper.DirectionByRotationFlat(user.Entity.Yaw))
			{
				case Direction.North:
					head += Coordinates3D.North;
					direction = BedBlock.BedDirection.North;
					break;
				case Direction.South:
					head += Coordinates3D.South;
					direction = BedBlock.BedDirection.South;
					break;
				case Direction.East:
					head += Coordinates3D.East;
					direction = BedBlock.BedDirection.East;
					break;
				case Direction.West:
					head += Coordinates3D.West;
					direction = BedBlock.BedDirection.West;
					break;
			}

			var bedProvider = (BedBlock) user.Server.BlockRepository.GetBlockProvider(BedBlock.BlockId);
			if (!bedProvider.ValidBedPosition(new BlockDescriptor {Coordinates = head},
				    user.Server.BlockRepository, user.World, false, true) ||
			    !bedProvider.ValidBedPosition(new BlockDescriptor {Coordinates = foot},
				    user.Server.BlockRepository, user.World, false, true))
				return;
			user.Server.BlockUpdatesEnabled = false;
			world.SetBlockData(head, new BlockDescriptor
				{Id = BedBlock.BlockId, Metadata = (byte) ((byte) direction | (byte) BedBlock.BedType.Head)});
			world.SetBlockData(foot, new BlockDescriptor
				{Id = BedBlock.BlockId, Metadata = (byte) ((byte) direction | (byte) BedBlock.BedType.Foot)});
			user.Server.BlockUpdatesEnabled = true;
			item.Count--;
			user.Inventory[user.SelectedSlot] = item;
		}
	}
}