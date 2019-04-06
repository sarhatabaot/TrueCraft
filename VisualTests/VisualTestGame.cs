using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualTests
{
	public class VisualTestGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		public IVisualTest CurrentTest;

		public VisualTestGame(string[] args)
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1280;
			_graphics.PreferredBackBufferHeight = 800;

			Content.RootDirectory = "Content";

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		public SpriteBatch SpriteBatch;
		public SpriteFont Font;

		protected override void LoadContent()
		{
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			Font = Content.Load<SpriteFont>("DebugFontSmall");
		}

		protected override void UnloadContent()
		{
			SpriteBatch?.Dispose();
			SpriteBatch = null;

			if (CurrentTest is IDisposable disposable)
				disposable.Dispose();
			CurrentTest = null;

			Content?.Unload();
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);

			SpriteBatch.Begin();
			if (CurrentTest != null)
				SpriteBatch.DrawString(Font, CurrentTest.Name, Vector2.Zero, Color.Black);
			SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
