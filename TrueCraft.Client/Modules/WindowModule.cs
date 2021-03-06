﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrueCraft.Client.Input;
using TrueCraft.Client.Rendering;
using TrueCraft.Logic;
using TrueCraft.Networking.Packets;
using TrueCraft.Windows;

namespace TrueCraft.Client.Modules
{
	public class WindowModule : InputModule, IGraphicalModule
	{
		private static readonly Rectangle InventoryWindowRect = new Rectangle(0, 0, 176, 166);
		private static readonly Rectangle CraftingWindowRect = new Rectangle(0, 0, 176, 166);

		public WindowModule(TrueCraftGame game, FontRenderer font)
		{
			Game = game;
			Font = font;
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);
			Inventory = game.TextureMapper.GetTexture("gui/inventory.png");
			Crafting = game.TextureMapper.GetTexture("gui/crafting.png");
			Items = game.TextureMapper.GetTexture("gui/items.png");
			SelectedSlot = -1;
			HeldItem = ItemStack.EmptyStack;
		}

		private TrueCraftGame Game { get; }
		private SpriteBatch SpriteBatch { get; }
		private Texture2D Inventory { get; }
		private Texture2D Crafting { get; }
		private Texture2D Items { get; }
		private FontRenderer Font { get; }
		private short SelectedSlot { get; set; }
		private ItemStack HeldItem { get; set; }

		public void Draw(GameTime gameTime)
		{
			if (Game.Client.CurrentWindow != null)
			{
				// TODO: slot == -999 when outside of the window and -1 when inside the window, but not on an item
				SelectedSlot = -999;

				IItemProvider provider = null;
				var scale = new Point((int)(16 * Game.ScaleFactor * 2), (int)(16 * Game.ScaleFactor * 2));
				var state = Mouse.GetState();
				var mouse = new Point((int) (state.X - 8 * Game.ScaleFactor * 2), (int) (state.Y - 8 * Game.ScaleFactor * 2));
				var rect = new Rectangle(mouse.X, mouse.Y, scale.X, scale.Y);
				if (!HeldItem.Empty)
					provider = Game.ItemRepository.GetItemProvider(HeldItem.Id);

				SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp,
					DepthStencilState.None, RasterizerState.CullCounterClockwise);
				SpriteBatch.Draw(Game.WhitePixel, new Rectangle(0, 0,
						Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
					new Color(Color.Black, 180));
				switch (Game.Client.CurrentWindow.Type)
				{
					case -1:
						SpriteBatch.Draw(Inventory, new Vector2(
								Game.GraphicsDevice.Viewport.Width / 2 - Scale(InventoryWindowRect.Width / 2),
								Game.GraphicsDevice.Viewport.Height / 2 - Scale(InventoryWindowRect.Height / 2)),
							InventoryWindowRect, Color.White, 0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None,
							1);
						DrawInventoryWindow(RenderStage.Sprites);
						break;
					case 1: // Crafting bench
						SpriteBatch.Draw(Crafting, new Vector2(
								Game.GraphicsDevice.Viewport.Width / 2 - Scale(CraftingWindowRect.Width / 2),
								Game.GraphicsDevice.Viewport.Height / 2 - Scale(CraftingWindowRect.Height / 2)),
							CraftingWindowRect, Color.White, 0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None,
							1);
						DrawCraftingWindow(RenderStage.Sprites);
						break;
				}

				if (provider != null)
					if (provider.GetIconTexture((byte) HeldItem.Metadata) != null)
						IconRenderer.RenderItemIcon(SpriteBatch, Items, provider,
							(byte) HeldItem.Metadata, rect, Color.White);
				SpriteBatch.End();
				switch (Game.Client.CurrentWindow.Type)
				{
					case -1:
						DrawInventoryWindow(RenderStage.Models);
						break;
					case 1: // Crafting bench
						DrawCraftingWindow(RenderStage.Models);
						break;
				}

				if (provider != null)
					if (provider.GetIconTexture((byte) HeldItem.Metadata) == null && provider is IBlockProvider)
						IconRenderer.RenderBlockIcon(Game, provider as IBlockProvider, (byte) HeldItem.Metadata, rect);
				SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp,
					DepthStencilState.None, RasterizerState.CullCounterClockwise);
				switch (Game.Client.CurrentWindow.Type)
				{
					case -1:
						DrawInventoryWindow(RenderStage.Text);
						break;
					case 1: // Crafting bench
						DrawCraftingWindow(RenderStage.Text);
						break;
				}

				if (provider != null)
					if (HeldItem.Count > 1)
					{
						var offset = 10;
						if (HeldItem.Count >= 10)
							offset -= 6;
						mouse += new Point((int) Scale(offset), (int) Scale(5));
						Font.DrawText(SpriteBatch, mouse.X, mouse.Y, HeldItem.Count.ToString(), Game.ScaleFactor);
					}

				if (SelectedSlot >= 0)
				{
					var item = Game.Client.CurrentWindow[SelectedSlot];
					if (!item.Empty)
					{
						var p = Game.ItemRepository.GetItemProvider(item.Id);
						var size = Font.MeasureText(p.DisplayName);
						mouse.X = state.X + 10;
						mouse.Y = state.Y + 10;
						SpriteBatch.Draw(Game.WhitePixel, new Rectangle(mouse.X, mouse.Y,
								size.X + 10, size.Y + 15),
							new Color(Color.Black, 200));
						Font.DrawText(SpriteBatch, mouse.X + 5, mouse.Y, p.DisplayName);
					}
				}

				SpriteBatch.End();
			}
		}

		public override bool MouseMove(GameTime gameTime, MouseMoveEventArgs e)
		{
			if (Game.Client.CurrentWindow != null)
				return true;
			return base.MouseMove(gameTime, e);
		}

		public override bool MouseButtonDown(GameTime gameTime, MouseButtonEventArgs e)
		{
			if (Game.Client.CurrentWindow == null)
				return false;
			var Id = Game.Client.CurrentWindow.Id;
			if (Id == -1)
				Id = 0; // Minecraft is stupid
			var item = ItemStack.EmptyStack;
			if (SelectedSlot > -1)
				item = Game.Client.CurrentWindow[SelectedSlot];
			var packet = new ClickWindowPacket(Id, SelectedSlot, e.Button == MouseButton.Right,
				0, Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift),
				item.Id, item.Count, item.Metadata);
			if (packet.SlotIndex == -999)
			{
				// Special case (throwing item) TODO
			}
			else
			{
				var backup = Game.Client.CurrentWindow.GetSlots();
				var staging = (ItemStack) HeldItem.Clone();
				Window.HandleClickPacket(packet, Game.Client.CurrentWindow, ref staging);
				HeldItem = staging;
				Game.Client.CurrentWindow.SetSlots(backup);
			}

			Game.Client.QueuePacket(packet);
			return true;
		}

		public override bool MouseButtonUp(GameTime gameTime, MouseButtonEventArgs e)
		{
			return Game.Client.CurrentWindow != null;
		}

		public override bool KeyDown(GameTime gameTime, KeyboardKeyEventArgs e)
		{
			if (Game.Client.CurrentWindow != null)
			{
				if (e.Key == Keys.Escape)
				{
					if (Game.Client.CurrentWindow.Type != -1)
						Game.Client.QueuePacket(new CloseWindowPacket(Game.Client.CurrentWindow.Id));
					Game.Client.CurrentWindow = null;
					Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
					Game.ControlModule.IgnoreNextUpdate = true;
				}

				return true;
			}

			return base.KeyDown(gameTime, e);
		}

		private void DrawInventoryWindow(RenderStage stage)
		{
			DrawWindowArea(Game.Client.Inventory.MainInventory, 8, 84, InventoryWindowRect, stage);
			DrawWindowArea(Game.Client.Inventory.Hotbar, 8, 142, InventoryWindowRect, stage);
			DrawWindowArea(Game.Client.Inventory.CraftingGrid, 88, 26, InventoryWindowRect, stage);
			DrawWindowArea(Game.Client.Inventory.Armor, 8, 8, InventoryWindowRect, stage);
		}

		private void DrawCraftingWindow(RenderStage stage)
		{
			var window = (CraftingBenchWindow) Game.Client.CurrentWindow;
			DrawWindowArea(window.CraftingGrid, 29, 16, CraftingWindowRect, stage);
			DrawWindowArea(window.MainInventory, 8, 84, CraftingWindowRect, stage);
			DrawWindowArea(window.Hotbar, 8, 142, CraftingWindowRect, stage);
		}

		private void DrawWindowArea(IWindowArea area, int _x, int _y, Rectangle frame, RenderStage stage)
		{
			var state = Mouse.GetState();
			var mouse = new Point(state.X, state.Y);
			var scale = new Point((int)(16 * Game.ScaleFactor * 2), (int) (16 * Game.ScaleFactor * 2));
			var origin = new Point((int) (
					Game.GraphicsDevice.Viewport.Width / 2 - Scale(frame.Width / 2) + Scale(_x)),
				(int) (Game.GraphicsDevice.Viewport.Height / 2 - Scale(frame.Height / 2) + Scale(_y)));
			for (var i = 0; i < area.Length; i++)
			{
				var item = area[i];
				var x = (int) (i % area.Width * Scale(18));
				var y = (int) (i / area.Width * Scale(18));
				if (area is CraftingWindowArea)
				{
					// hack
					if (i == 0)
					{
						if (area.Width == 2)
						{
							x = (int) Scale(144 - _x);
							y = (int) Scale(36 - _y);
						}
						else
						{
							x = (int) Scale(124 - _x);
							y = (int) Scale(35 - _y);
						}
					}
					else
					{
						i--;
						x = (int) (i % area.Width * Scale(18));
						y = (int) (i / area.Width * Scale(18));
						i++;
					}
				}

				var position = origin + new Point(x, y);
				var rect = new Rectangle(position.X, position.Y, scale.X, scale.Y);
				if (stage == RenderStage.Sprites && rect.Contains(mouse))
				{
					SelectedSlot = (short) (area.StartIndex + i);
					SpriteBatch.Draw(Game.WhitePixel, rect, new Color(Color.White, 150));
				}

				if (item.Empty)
					continue;
				var provider = Game.ItemRepository.GetItemProvider(item.Id);
				var texture = provider.GetIconTexture((byte) item.Metadata);
				if (texture != null && stage == RenderStage.Sprites)
					IconRenderer.RenderItemIcon(SpriteBatch, Items, provider,
						(byte) item.Metadata, rect, Color.White);
				if (texture == null && stage == RenderStage.Models && provider is IBlockProvider)
					IconRenderer.RenderBlockIcon(Game, provider as IBlockProvider, (byte) item.Metadata, rect);
				if (stage == RenderStage.Text && item.Count > 1)
				{
					var offset = 10;
					if (item.Count >= 10)
						offset -= 6;
					position += new Point((int) Scale(offset), (int) Scale(5));
					Font.DrawText(SpriteBatch, position.X, position.Y, item.Count.ToString(), Game.ScaleFactor);
				}
			}
		}

		public override void Update(GameTime gameTime)
		{
			Game.IsMouseVisible = Game.Client.CurrentWindow != null || !Game.IsActive;
			base.Update(gameTime);
		}

		private float Scale(float value)
		{
			return value * Game.ScaleFactor * 2;
		}

		private enum RenderStage
		{
			Sprites,
			Models,
			Text
		}
	}
}