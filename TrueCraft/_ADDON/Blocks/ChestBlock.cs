using System;
using Microsoft.Xna.Framework;
using TrueCraft.Entities;
using TrueCraft.Extensions;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Serialization;
using TrueCraft.Serialization.Tags;
using TrueCraft.Windows;
using TrueCraft.World;

namespace TrueCraft._ADDON.Blocks
{
	public class ChestBlock : BlockProvider, ICraftingRecipe, IBurnableItem
	{
		public static readonly byte BlockId = 0x36;

		private static readonly Coordinates3D[] AdjacentBlocks =
		{
			Coordinates3D.North,
			Coordinates3D.South,
			Coordinates3D.West,
			Coordinates3D.East
		};

		public override byte Id => 0x36;

		public override double BlastResistance => 12.5;

		public override double Hardness => 2.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Chest";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					ItemStack.EmptyStack,
					new ItemStack(WoodenPlanksBlock.BlockId)
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(10, 1);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			var adjacent = 0;
			var coords = coordinates + MathHelper.BlockFaceToCoordinates(face);
			var _ = Coordinates3D.Down;
			// Check for adjacent chests. We can only allow one adjacent check block.
			for (var i = 0; i < AdjacentBlocks.Length; i++)
				if (world.GetBlockId(coords + AdjacentBlocks[i]) == BlockId)
				{
					_ = coords + AdjacentBlocks[i];
					adjacent++;
				}

			if (adjacent <= 1)
			{
				if (_ != Coordinates3D.Down)
					for (var i = 0; i < AdjacentBlocks.Length; i++)
						if (world.GetBlockId(_ + AdjacentBlocks[i]) == BlockId)
							adjacent++;
				if (adjacent <= 1)
					base.ItemUsedOnBlock(coordinates, item, face, world, user);
			}
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			world.SetMetadata(descriptor.Coordinates, (byte) MathHelper.DirectionByRotationFlat(user.Entity.Yaw, true));
		}

		public override bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			var adjacent = -Coordinates3D.One; // -1, no adjacent chest
			var self = descriptor.Coordinates;
			for (var i = 0; i < AdjacentBlocks.Length; i++)
			{
				var test = self + AdjacentBlocks[i];
				if (world.GetBlockId(test) == BlockId)
				{
					adjacent = test;
					var up = world.BlockRepository.GetBlockProvider(world.GetBlockId(test + Coordinates3D.Up));
					if (up.Opaque && !(up is WallSignBlock)) // Wall sign blocks are an exception
						return false; // Obstructed
					break;
				}
			}

			var upSelf = world.BlockRepository.GetBlockProvider(world.GetBlockId(self + Coordinates3D.Up));
			if (upSelf.Opaque && !(upSelf is WallSignBlock))
				return false; // Obstructed

			if (adjacent != -Coordinates3D.One)
				if (adjacent.X < self.X ||
				    adjacent.Z < self.Z)
				{
					var _ = adjacent;
					adjacent = self;
					self = _; // Swap
				}

			var window = new ChestWindow((InventoryWindow) user.Inventory, adjacent != -Coordinates3D.One);
			// Add items
			var entity = world.GetTileEntity(self);
			if (entity != null)
				foreach (var item in (NbtList) entity["Items"])
				{
					var slot = ItemStack.FromNbt((NbtCompound) item);
					window.ChestInventory[slot.Index] = slot;
				}

			// Add adjacent items
			if (adjacent != -Coordinates3D.One)
			{
				entity = world.GetTileEntity(adjacent);
				if (entity != null)
					foreach (var item in (NbtList) entity["Items"])
					{
						var slot = ItemStack.FromNbt((NbtCompound) item);
						window.ChestInventory[slot.Index + ChestWindow.DoubleChestSecondaryIndex] = slot;
					}
			}

			window.WindowChange += (sender, e) =>
			{
				var entitySelf = new NbtList("Items", NbtTagType.Compound);
				var entityAdjacent = new NbtList("Items", NbtTagType.Compound);
				for (var i = 0; i < window.ChestInventory.Items.Length; i++)
				{
					var item = window.ChestInventory.Items[i];
					if (!item.Empty)
					{
						if (i < ChestWindow.DoubleChestSecondaryIndex)
						{
							item.Index = i;
							entitySelf.Add(item.ToNbt());
						}
						else
						{
							item.Index = i - ChestWindow.DoubleChestSecondaryIndex;
							entityAdjacent.Add(item.ToNbt());
						}
					}
				}

				var newEntity = world.GetTileEntity(self);
				if (newEntity == null)
					newEntity = new NbtCompound(new[] {entitySelf});
				else
					newEntity["Items"] = entitySelf;
				world.SetTileEntity(self, newEntity);
				if (adjacent != -Coordinates3D.One)
				{
					newEntity = world.GetTileEntity(adjacent);
					if (newEntity == null)
						newEntity = new NbtCompound(new[] {entityAdjacent});
					else
						newEntity["Items"] = entityAdjacent;
					world.SetTileEntity(adjacent, newEntity);
				}
			};
			user.OpenWindow(window);
			return false;
		}

		public override void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			var self = descriptor.Coordinates;
			var entity = world.GetTileEntity(self);
			var manager = user.Server.GetEntityManagerForWorld(world);
			if (entity != null)
				foreach (var item in (NbtList) entity["Items"])
				{
					var slot = ItemStack.FromNbt((NbtCompound) item);
					manager.SpawnEntity(new ItemEntity(descriptor.Coordinates.AsVector3() + new Vector3(0.5f), slot));
				}

			world.SetTileEntity(self, null);
			base.BlockMined(descriptor, face, world, user);
		}
	}
}