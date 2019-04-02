﻿using System;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.API.Networking;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
	public abstract class DoorItem : ItemProvider, ICraftingRecipe
	{
		[Flags]
		public enum DoorFlags
		{
			Northeast = 0x0,
			Southeast = 0x1,
			Southwest = 0x2,
			Northwest = 0x3,
			Lower = 0x0,
			Upper = 0x8,
			Closed = 0x0,
			Open = 0x4
		}

		protected abstract byte BlockID { get; }

		public override sbyte MaximumStack => 1;

		public ItemStack[,] Pattern
		{
			get
			{
				var baseMaterial = this is IronDoorItem ? IronIngotItem.ItemID : WoodenPlanksBlock.BlockID;
				return new[,]
				{
					{new ItemStack(baseMaterial), new ItemStack(baseMaterial)},
					{new ItemStack(baseMaterial), new ItemStack(baseMaterial)},
					{new ItemStack(baseMaterial), new ItemStack(baseMaterial)}
				};
			}
		}

		public ItemStack Output => new ItemStack(ID);

		public bool SignificantMetadata => false;

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			var bottom = coordinates + MathHelper.BlockFaceToCoordinates(face);
			var top = bottom + Coordinates3D.Up;
			if (world.GetBlockID(top) != 0 || world.GetBlockID(bottom) != 0)
				return;
			DoorFlags direction;
			switch (MathHelper.DirectionByRotationFlat(user.Entity.Yaw))
			{
				case Direction.North:
					direction = DoorFlags.Northwest;
					break;
				case Direction.South:
					direction = DoorFlags.Southeast;
					break;
				case Direction.East:
					direction = DoorFlags.Northeast;
					break;
				default: // Direction.West:
					direction = DoorFlags.Southwest;
					break;
			}

			user.Server.BlockUpdatesEnabled = false;
			world.SetBlockID(bottom, BlockID);
			world.SetMetadata(bottom, (byte) direction);
			world.SetBlockID(top, BlockID);
			world.SetMetadata(top, (byte) (direction | DoorFlags.Upper));
			user.Server.BlockUpdatesEnabled = true;
			item.Count--;
			user.Inventory[user.SelectedSlot] = item;
		}
	}
}