using System;
using System.Linq;
using Microsoft.Xna.Framework;
using TrueCraft.Entities;
using TrueCraft.Extensions;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Serialization.Tags;
using TrueCraft.Windows;
using TrueCraft._ADDON.Blocks;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Server.Handlers
{
	public static class InteractionHandlers
	{
		public static void HandlePlayerDiggingPacket(IPacket _packet, IRemoteClient _client, IMultiPlayerServer server)
		{
			var packet = (PlayerDiggingPacket) _packet;
			var client = (RemoteClient) _client;
			var world = _client.World;
			var position = new Coordinates3D(packet.X, packet.Y, packet.Z);
			var descriptor = world.GetBlockData(position);
			var provider = server.BlockRepository.GetBlockProvider(descriptor.Id);
			short damage;
			int time;
			switch (packet.PlayerAction)
			{
				case PlayerDiggingPacket.Action.DropItem:
					// Throwing item
					if (client.SelectedItem.Empty)
						break;
					var spawned = client.SelectedItem;
					spawned.Count = 1;
					var inventory = client.SelectedItem;
					inventory.Count--;
					var item = new ItemEntity(client.Entity.Position + new Vector3(0, (float) PlayerEntity.Height, 0), spawned);
					item.Velocity = MathHelper.RotateY(Directions.Forwards, MathHelper.DegreesToRadians(client.Entity.Yaw)) * (float) 0.5;
					client.Inventory[client.SelectedSlot] = inventory;
					server.GetEntityManagerForWorld(client.World).SpawnEntity(item);
					break;
				case PlayerDiggingPacket.Action.StartDigging:
					foreach (var nearbyClient in server.Clients
					) // TODO: Send this repeatedly during the course of the digging
					{
						var c = (RemoteClient) nearbyClient;
						if (c.KnownEntities.Contains(client.Entity))
							c.QueuePacket(new AnimationPacket(client.Entity.EntityId,
								AnimationPacket.PlayerAnimation.SwingArm));
					}

					if (provider == null)
						server.SendMessage(
							ChatColor.Red + "WARNING: block provider for Id {0} is null (player digging)",
							descriptor.Id);
					else
						provider.BlockLeftClicked(descriptor, packet.Face, world, client);

					// "But why on Earth does this behavior change if you use shears on leaves?"
					// "This is poor separation of concerns"
					// "Let me do a git blame and flame whoever wrote the next line"
					// To answer all of those questions, here:
					// Minecraft sends a player digging packet when the player starts and stops digging a block (two packets)
					// However, it only sends ONE packet if the block would be mined immediately - which usually is only the case
					// for blocks that have a hardness equal to zero.
					// The exception to this rule is shears on leaves. Leaves normally have a hardness of 0.2, but when you mine them
					// using shears the client only sends the start digging packet and expects them to be mined immediately.
					// So if you want to blame anyone, send flames to Notch for the stupid idea of not sending "stop digging" packets
					// for hardness == 0 blocks.

					time = BlockProvider.GetHarvestTime(descriptor.Id, client.SelectedItem.Id, out damage);
					if (time <= 20)
					{
						provider?.BlockMined(descriptor, packet.Face, world, client);
						break;
					}

					client.ExpectedDigComplete = DateTime.UtcNow.AddMilliseconds(time);
					break;
				case PlayerDiggingPacket.Action.StopDigging:
					foreach (var nearbyClient in server.Clients)
					{
						var c = (RemoteClient) nearbyClient;
						if (c.KnownEntities.Contains(client.Entity))
							c.QueuePacket(new AnimationPacket(client.Entity.EntityId,
								AnimationPacket.PlayerAnimation.None));
					}

					if (provider != null && descriptor.Id != 0)
					{
						time = BlockProvider.GetHarvestTime(descriptor.Id, client.SelectedItem.Id, out damage);
						if (time <= 20)
							break; // Already handled earlier
						var diff = (DateTime.UtcNow - client.ExpectedDigComplete).TotalMilliseconds;
						if (diff > -100) // Allow a small tolerance
						{
							provider.BlockMined(descriptor, packet.Face, world, client);
							// Damage the item
							if (damage != 0)
							{
								if (server.ItemRepository.GetItemProvider(client.SelectedItem.Id) is ToolItem tool && tool.Uses != -1)
								{
									var slot = client.SelectedItem;
									slot.Metadata += damage;
									if (slot.Metadata >= tool.Uses)
										slot.Count = 0; // Destroy item
									client.Inventory[client.SelectedSlot] = slot;
								}
							}
						}
					}

					break;
			}
		}

		public static void HandlePlayerBlockPlacementPacket(IPacket _packet, IRemoteClient _client,
			IMultiPlayerServer server)
		{
			var packet = (PlayerBlockPlacementPacket) _packet;
			var client = (RemoteClient) _client;

			var slot = client.SelectedItem;
			var position = new Coordinates3D(packet.X, packet.Y, packet.Z);
			BlockDescriptor? block;
			if (position != -Coordinates3D.One)
			{
				if (position.DistanceTo((Coordinates3D) client.Entity.Position) > 10 /* TODO: Reach */)
					return;
				block = client.World.GetBlockData(position);
			}
			else
				return;

			var provider = server.BlockRepository.GetBlockProvider(block.Value.Id);
			if (provider == null)
			{
				server.SendMessage($"{ChatColor.Red}WARNING: block provider for Id {{0}} is null (player placing)", block.Value.Id);
				server.SendMessage($"{ChatColor.Red}Error occured from client {{0}} at coordinates {{1}}", client.Username, block.Value.Coordinates);
				server.SendMessage($"{ChatColor.Red}Packet logged at {{0}}, please report upstream", DateTime.UtcNow);
				return;
			}

			if (!provider.BlockRightClicked(block.Value, packet.Face, client.World, client))
			{
				position += MathHelper.BlockFaceToCoordinates(packet.Face);
				var oldId = client.World.GetBlockId(position);
				var oldMeta = client.World.GetMetadata(position);
				client.QueuePacket(new BlockChangePacket(position.X, (sbyte) position.Y, position.Z, (sbyte) oldId, (sbyte) oldMeta));
				client.QueuePacket(new SetSlotPacket(0, client.SelectedSlot, client.SelectedItem.Id, client.SelectedItem.Count, client.SelectedItem.Metadata));
				return;
			}

			if (!slot.Empty)
			{
				var itemProvider = server.ItemRepository.GetItemProvider(slot.Id);
				if (itemProvider == null)
				{
					server.SendMessage(ChatColor.Red + "WARNING: item provider for Id {0} is null (player placing)",
						block.Value.Id);
					server.SendMessage(ChatColor.Red + "Error occured from client {0} at coordinates {1}",
						client.Username, block.Value.Coordinates);
					server.SendMessage(ChatColor.Red + "Packet logged at {0}, please report upstream",
						DateTime.UtcNow);
				}

				itemProvider?.ItemUsedOnBlock(position, slot, packet.Face, client.World, client);
			}
		}

		public static void HandleClickWindowPacket(IPacket _packet, IRemoteClient _client, IMultiPlayerServer server)
		{
			var packet = (ClickWindowPacket) _packet;
			var client = (RemoteClient) _client;
			var window = client.CurrentWindow;
			if (packet.SlotIndex == -999)
			{
				// Throwing item
				ItemEntity item;
				if (packet.RightClick)
				{
					var spawned = client.ItemStaging;
					spawned.Count = 1;
					var inventory = client.ItemStaging;
					inventory.Count--;
					item = new ItemEntity(client.Entity.Position + new Vector3(0, (float) PlayerEntity.Height, 0), spawned);
					client.ItemStaging = inventory;
				}
				else
				{
					item = new ItemEntity(client.Entity.Position + new Vector3(0, (float) PlayerEntity.Height, 0),
						client.ItemStaging);
					client.ItemStaging = ItemStack.EmptyStack;
				}

				item.Velocity = MathHelper.FowardVector(client.Entity.Yaw) * 0.3f;
				server.GetEntityManagerForWorld(client.World).SpawnEntity(item);
				return;
			}

			var staging = (ItemStack) client.ItemStaging.Clone();
			Window.HandleClickPacket(packet, window, ref staging);
			client.ItemStaging = staging;
			if (packet.SlotIndex >= window.Length || packet.SlotIndex < 0)
				return;
			client.QueuePacket(new WindowItemsPacket(packet.WindowID, window.GetSlots()));
		}

		public static void HandleCloseWindowPacket(IPacket _packet, IRemoteClient _client, IMultiPlayerServer server)
		{
			var packet = (CloseWindowPacket) _packet;
			if (packet.WindowID != 0)
				(_client as RemoteClient).CloseWindow(true);
		}

		public static void HandleChangeHeldItem(IPacket _packet, IRemoteClient _client, IMultiPlayerServer server)
		{
			var packet = (ChangeHeldItemPacket) _packet;
			var client = (RemoteClient) _client;
			client.SelectedSlot = (short) (packet.Slot + InventoryWindow.HotbarIndex);
			var notified = server.GetEntityManagerForWorld(client.World).ClientsForEntity(client.Entity);
			foreach (var c in notified)
				c.QueuePacket(new EntityEquipmentPacket(client.Entity.EntityId, 0, client.SelectedItem.Id,
					client.SelectedItem.Metadata));
		}

		public static void HandlePlayerAction(IPacket _packet, IRemoteClient _client, IMultiPlayerServer server)
		{
			var packet = (PlayerActionPacket) _packet;
			var client = (RemoteClient) _client;
			var entity = (PlayerEntity) client.Entity;
			switch (packet.Action)
			{
				case PlayerActionPacket.PlayerAction.Crouch:
					entity.EntityFlags |= EntityFlags.Crouched;
					break;
				case PlayerActionPacket.PlayerAction.Uncrouch:
					entity.EntityFlags &= ~EntityFlags.Crouched;
					break;
			}
		}

		public static void HandleAnimation(IPacket _packet, IRemoteClient _client, IMultiPlayerServer server)
		{
			var packet = (AnimationPacket) _packet;
			var client = (RemoteClient) _client;
			if (packet.EntityId == client.Entity.EntityId)
			{
				var nearby = server.GetEntityManagerForWorld(client.World)
					.ClientsForEntity(client.Entity);
				foreach (var player in nearby)
					player.QueuePacket(packet);
			}
		}

		public static void HandleUpdateSignPacket(IPacket _packet, IRemoteClient _client, IMultiPlayerServer server)
		{
			var packet = (UpdateSignPacket) _packet;
			var client = (RemoteClient) _client;
			var coords = new Coordinates3D(packet.X, packet.Y, packet.Z);
			if (client.Entity.Position.DistanceTo(coords.AsVector3()) < 10) // TODO: Reach
			{
				var block = client.World.GetBlockId(coords);
				if (block == UprightSignBlock.BlockId || block == WallSignBlock.BlockId)
				{
					client.World.SetTileEntity(coords, new NbtCompound(new[]
					{
						new NbtString("Text1", packet.Text[0]),
						new NbtString("Text2", packet.Text[1]),
						new NbtString("Text3", packet.Text[2]),
						new NbtString("Text4", packet.Text[3])
					}));
					// TODO: Some utility methods for things like "clients with given chunk loaded"
					server.Clients.Where(c => ((RemoteClient) c).LoggedIn && c.World == _client.World).ToList()
						.ForEach(c => c.QueuePacket(packet));
				}
			}
		}
	}
}