using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Client.Rendering;
using TrueCraft.Logic;

namespace TrueCraft.Client.Modules
{
	public class HUDModule : IGraphicalModule
	{
		private static readonly Color CrosshairColor = new Color(255, 255, 255, 70);

		public HUDModule(TrueCraftGame game, FontRenderer font)
		{
			Game = game;
			Font = font;
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);
			GUI = game.TextureMapper.GetTexture("gui/gui.png");
			Icons = game.TextureMapper.GetTexture("gui/icons.png");
			Items = game.TextureMapper.GetTexture("gui/items.png");
		}

		private TrueCraftGame Game { get; }
		private SpriteBatch SpriteBatch { get; }
		private Texture2D GUI { get; }
		private Texture2D Icons { get; }
		private Texture2D Items { get; }
		private FontRenderer Font { get; }

		public void Update(GameTime gameTime)
		{
		}

		public void Draw(GameTime gameTime)
		{
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp,
				DepthStencilState.None, RasterizerState.CullCounterClockwise);

			SpriteBatch.Draw(Icons, new Vector2(
					Game.GraphicsDevice.Viewport.Width / 2 - 8 * Game.ScaleFactor * 2,
					Game.GraphicsDevice.Viewport.Height / 2 - 8 * Game.ScaleFactor * 2),
				new Rectangle(0, 0, 16, 16), CrosshairColor,
				0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);

			DrawHotbar(gameTime);
			DrawHotbarItemSprites(gameTime);
			DrawLife(gameTime);

			SpriteBatch.End();

			DrawHotbarBlockSprites(gameTime);

			// Once more, with feeling
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp,
				DepthStencilState.None, RasterizerState.CullCounterClockwise);
			DrawHotbarSlotCounts(gameTime);
			SpriteBatch.End();
		}

		/// <summary>
		///  Scales a float depending on the game's scale factor.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private float Scale(float value)
		{
			return value * Game.ScaleFactor * 2;
		}

		#region "Hotbar"

		/// <summary>
		///  The dimensions of the hotbar background.
		/// </summary>
		private static readonly Rectangle HotbarBackgroundRect =
			new Rectangle(0, 0, 182, 22);

		/// <summary>
		///  The dimensions of the hotbar selection.
		/// </summary>
		private static readonly Rectangle HotbarSelectionRect =
			new Rectangle(0, 22, 24, 24);

		private static readonly Rectangle FullHeartRect =
			new Rectangle(52, 0, 9, 9);

		private static readonly Rectangle HalfHeartRect =
			new Rectangle(61, 0, 9, 9);

		private static readonly Rectangle EmptyHeartRect =
			new Rectangle(16, 0, 9, 9);

		private static readonly Rectangle EmptyHeartHighlightedRect =
			new Rectangle(25, 0, 9, 9);

		/// <summary>
		///  Draws the inventory hotbar.
		/// </summary>
		/// <param name="gameTime"></param>
		private void DrawHotbar(GameTime gameTime)
		{
			// Background
			SpriteBatch.Draw(GUI, new Vector2(
					Game.GraphicsDevice.Viewport.Width / 2 - Scale(HotbarBackgroundRect.Width / 2),
					Game.GraphicsDevice.Viewport.Height - Scale(HotbarBackgroundRect.Height + 5)),
				HotbarBackgroundRect, Color.White, 0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);

			// Selection
			SpriteBatch.Draw(GUI, new Vector2(
					Game.GraphicsDevice.Viewport.Width / 2 - Scale(HotbarBackgroundRect.Width / 2) +
					Scale(Game.Client.HotBarSelection * 20 - 1),
					Game.GraphicsDevice.Viewport.Height - Scale(HotbarBackgroundRect.Height + 6)),
				HotbarSelectionRect, Color.White, 0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);
		}

		private void DrawLife(GameTime gameTime)
		{
			var x = (int) (Game.GraphicsDevice.Viewport.Width / 2 - Scale(HotbarBackgroundRect.Width / 2));
			var y = (int) (Game.GraphicsDevice.Viewport.Height - Scale(HotbarBackgroundRect.Height + 5));
			y -= (int) (Scale(EmptyHeartRect.Height) * 1.25);

			for (var i = 0; i < 10; i++)
			{
				SpriteBatch.Draw(Icons, new Vector2(x + i * Scale(EmptyHeartRect.Width), y), EmptyHeartRect,
					Color.White,
					0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);
				if (Game.Client.Health >= i * 2)
					SpriteBatch.Draw(Icons, new Vector2(x + i * Scale(FullHeartRect.Width), y), FullHeartRect,
						Color.White,
						0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);
				else if (Game.Client.Health >= i * 2 - 1)
					SpriteBatch.Draw(Icons, new Vector2(x + i * Scale(HalfHeartRect.Width), y), HalfHeartRect,
						Color.White,
						0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);
			}
		}

		private void DrawHotbarItemSprites(GameTime gameTime)
		{
			var scale = new Point((int)(16 * Game.ScaleFactor * 2), (int)(16 * Game.ScaleFactor * 2));
			var origin = new Point(
				(int) (Game.GraphicsDevice.Viewport.Width / 2 - Scale(HotbarBackgroundRect.Width / 2)),
				(int) (Game.GraphicsDevice.Viewport.Height - Scale(HotbarBackgroundRect.Height + 5)));
			origin.X += (int) Scale(3);
			origin.Y += (int) Scale(3);
			for (var i = 0; i < Game.Client.Inventory.Hotbar.Length; i++)
			{
				var item = Game.Client.Inventory.Hotbar[i];
				if (item.Empty)
					continue;
				var provider = Game.ItemRepository.GetItemProvider(item.Id);
				if (provider.GetIconTexture((byte) item.Metadata) == null)
					continue;
				var position = origin + new Point((int) Scale(i * 20), 0);
				var rect = new Rectangle(position.X, position.Y, scale.X, scale.Y);
				IconRenderer.RenderItemIcon(SpriteBatch, Items, provider,
					(byte) item.Metadata, rect, Color.White); // TODO: metadata was supposed to be a short
			}
		}

		private void DrawHotbarBlockSprites(GameTime gameTime)
		{
			var scale = new Point((int)(16 * Game.ScaleFactor * 2), (int)(16 * Game.ScaleFactor * 2));
			var origin = new Point(
				(int) (Game.GraphicsDevice.Viewport.Width / 2 - Scale(HotbarBackgroundRect.Width / 2)),
				(int) (Game.GraphicsDevice.Viewport.Height - Scale(HotbarBackgroundRect.Height + 5)));
			origin.X += (int) Scale(3);
			origin.Y += (int) Scale(3);
			for (var i = 0; i < Game.Client.Inventory.Hotbar.Length; i++)
			{
				var item = Game.Client.Inventory.Hotbar[i];
				if (item.Empty)
					continue;
				var provider = Game.ItemRepository.GetItemProvider(item.Id) as IBlockProvider;
				if (provider == null || provider.GetIconTexture((byte) item.Metadata) != null)
					continue;
				var position = origin + new Point((int) Scale(i * 20), 0);
				var rect = new Rectangle(position.X, position.Y, scale.X, scale.Y);
				IconRenderer.RenderBlockIcon(Game, provider, (byte) item.Metadata, rect);
			}
		}

		private void DrawHotbarSlotCounts(GameTime gameTime)
		{
			var origin = new Point(
				(int) (Game.GraphicsDevice.Viewport.Width / 2 - Scale(HotbarBackgroundRect.Width / 2)),
				(int) (Game.GraphicsDevice.Viewport.Height - Scale(HotbarBackgroundRect.Height + 5)));
			origin.X += (int) Scale(3);
			origin.Y += (int) Scale(3);
			for (var i = 0; i < Game.Client.Inventory.Hotbar.Length; i++)
			{
				var item = Game.Client.Inventory.Hotbar[i];
				if (item.Empty || item.Count == 1)
					continue;
				var offset = 10;
				if (item.Count >= 10)
					offset -= 6;
				var position = origin + new Point((int) Scale(i * 20 + offset), (int) Scale(5));
				Font.DrawText(SpriteBatch, position.X, position.Y, item.Count.ToString(), Game.ScaleFactor);
			}
		}

		#endregion
	}
}