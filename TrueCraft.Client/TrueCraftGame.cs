﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrueCraft.Client.Input;
using TrueCraft.Client.Modules;
using TrueCraft.Client.Rendering;
using TrueCraft.Client.UI;
using TrueCraft.Logic;
using TrueCraft.Networking.Packets;

namespace TrueCraft.Client
{
	public class TrueCraftGame : Game
	{
		public static readonly int Reach = 3;

		public TrueCraftGame(MultiplayerClient client, IPEndPoint endPoint)
		{
			Window.Title = "TrueCraft";
			Content.RootDirectory = "Content";
			Graphics = new GraphicsDeviceManager(this);
			Graphics.SynchronizeWithVerticalRetrace = false;
			Graphics.IsFullScreen = UserSettings.Local.IsFullscreen;
			Graphics.PreferredBackBufferWidth = UserSettings.Local.WindowResolution.Width;
			Graphics.PreferredBackBufferHeight = UserSettings.Local.WindowResolution.Height;
			Graphics.GraphicsProfile = GraphicsProfile.HiDef;
			Graphics.PreparingDeviceSettings += PrepareDeviceSettings;
			Graphics.ApplyChanges();
			Window.ClientSizeChanged += Window_ClientSizeChanged;
			Client = client;
			EndPoint = endPoint;
			LastPhysicsUpdate = DateTime.MinValue;
			NextPhysicsUpdate = DateTime.MinValue;
			PendingMainThreadActions = new ConcurrentBag<Action>();
			MouseCaptured = true;
			Bobbing = 0;

			KeyboardComponent = new KeyboardHandler(this);
			Components.Add(KeyboardComponent);

			MouseComponent = new MouseHandler(this);
			Components.Add(MouseComponent);

			GamePadComponent = new GamePadHandler(this);
			Components.Add(GamePadComponent);
		}

		public MultiplayerClient Client { get; }
		public GraphicsDeviceManager Graphics { get; }
		public TextureMapper TextureMapper { get; private set; }
		public Camera Camera { get; private set; }
		public ConcurrentBag<Action> PendingMainThreadActions { get; set; }
		public double Bobbing { get; set; }
		public ChunkModule ChunkModule { get; set; }
		public ChatModule ChatModule { get; set; }
		public float ScaleFactor { get; set; }
		public Coordinates3D HighlightedBlock { get; set; }
		public BlockFace HighlightedBlockFace { get; set; }
		public DateTime StartDigging { get; set; }
		public DateTime EndDigging { get; set; }
		public Coordinates3D TargetBlock { get; set; }
		public AudioManager Audio { get; set; }
		public Texture2D White1x1 { get; set; }
		public PlayerControlModule ControlModule { get; set; }
		public SkyModule SkyModule { get; set; }

		private List<IGameplayModule> InputModules { get; set; }
		private List<IGameplayModule> GraphicalModules { get; set; }
		private SpriteBatch SpriteBatch { get; set; }
		private KeyboardHandler KeyboardComponent { get; }
		private MouseHandler MouseComponent { get; }
		private GamePadHandler GamePadComponent { get; }
		private RenderTarget2D RenderTarget { get; set; }
		private int ThreadID { get; set; }

		private FontRenderer Pixel { get; set; }
		private IPEndPoint EndPoint { get; }
		private DateTime LastPhysicsUpdate { get; }
		private DateTime NextPhysicsUpdate { get; set; }
		private bool MouseCaptured { get; }
		private GameTime GameTime { get; set; }
		private DebugInfoModule DebugInfoModule { get; set; }

		public IBlockRepository BlockRepository => Client.World.World.BlockRepository;

		public IItemRepository ItemRepository { get; set; }

		private static void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
		}

		private void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			if (GraphicsDevice.Viewport.Width < 640 || GraphicsDevice.Viewport.Height < 480)
				ScaleFactor = 0.5f;
			else if (GraphicsDevice.Viewport.Width < 978 || GraphicsDevice.Viewport.Height < 720)
				ScaleFactor = 1.0f;
			else
				ScaleFactor = 1.5f;
			IconRenderer.PrepareEffects(this);
			UpdateCamera();
			CreateRenderTarget();
		}

		private ImGuiRenderer _imgui;

		protected override void Initialize()
		{
			base.Initialize(); // (calls LoadContent)

			_imgui = new ImGuiRenderer(this);
			_imgui.RebuildFontAtlas();

			InputModules = new List<IGameplayModule>();
			GraphicalModules = new List<IGameplayModule>();
			
			if (GraphicsDevice.Viewport.Width < 640 || GraphicsDevice.Viewport.Height < 480)
				ScaleFactor = 0.5f;
			else if (GraphicsDevice.Viewport.Width < 978 || GraphicsDevice.Viewport.Height < 720)
				ScaleFactor = 1.0f;
			else
				ScaleFactor = 1.5f;

			Camera = new Camera(GraphicsDevice.Viewport.AspectRatio, 70.0f, 0.1f, 1000.0f);
			UpdateCamera();

			White1x1 = new Texture2D(GraphicsDevice, 1, 1);
			White1x1.SetData(new[] {Color.White});

			Audio = new AudioManager();
			Audio.LoadDefaultPacks(Content);

			SkyModule = new SkyModule(this);
			ChunkModule = new ChunkModule(this);
			DebugInfoModule = new DebugInfoModule(this, Pixel);
			ChatModule = new ChatModule(this, Pixel);
			var hud = new HUDModule(this, Pixel);
			var windowModule = new WindowModule(this, Pixel);

			GraphicalModules.Add(SkyModule);
			GraphicalModules.Add(ChunkModule);
			GraphicalModules.Add(new HighlightModule(this));
			GraphicalModules.Add(hud);
			GraphicalModules.Add(ChatModule);
			GraphicalModules.Add(windowModule);
			GraphicalModules.Add(DebugInfoModule);

			InputModules.Add(windowModule);
			InputModules.Add(DebugInfoModule);
			InputModules.Add(ChatModule);
			InputModules.Add(new HUDModule(this, Pixel));
			InputModules.Add(ControlModule = new PlayerControlModule(this));

			Client.PropertyChanged += HandleClientPropertyChanged;
			Client.Connect(EndPoint);

			BlockProvider.BlockRepository = BlockRepository;
			var itemRepository = new ItemRepository();
			itemRepository.DiscoverItemProviders();
			ItemRepository = itemRepository;
			BlockProvider.ItemRepository = ItemRepository;

			IconRenderer.CreateBlocks(this, BlockRepository);

			var centerX = GraphicsDevice.Viewport.Width / 2;
			var centerY = GraphicsDevice.Viewport.Height / 2;
			Mouse.SetPosition(centerX, centerY);

			MouseComponent.Scroll += OnMouseComponentScroll;
			MouseComponent.Move += OnMouseComponentMove;
			MouseComponent.ButtonDown += OnMouseComponentButtonDown;
			MouseComponent.ButtonUp += OnMouseComponentButtonUp;
			KeyboardComponent.KeyDown += OnKeyboardKeyDown;
			KeyboardComponent.KeyUp += OnKeyboardKeyUp;
			GamePadComponent.ButtonDown += OnGamePadButtonDown;
			GamePadComponent.ButtonUp += OnGamePadButtonUp;

			CreateRenderTarget();
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			ThreadID = Thread.CurrentThread.ManagedThreadId;
		}

		public void Invoke(Action action)
		{
			if (ThreadID == Thread.CurrentThread.ManagedThreadId)
				action();
			else
				PendingMainThreadActions.Add(action);
		}

		private void CreateRenderTarget()
		{
			RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
				GraphicsDevice.Viewport.Height,
				false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24,
				0, RenderTargetUsage.PreserveContents);
		}

		private void HandleClientPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Position":
					UpdateCamera();
					break;
			}
		}

		protected override void LoadContent()
		{
			// Ensure we have default textures loaded.
			TextureMapper.LoadDefaults(GraphicsDevice);

			// Load any custom textures if needed.
			TextureMapper = new TextureMapper(GraphicsDevice);
			if (UserSettings.Local.SelectedTexturePack != TexturePack.Default.Name)
				TextureMapper.AddTexturePack(TexturePack.FromArchive(Path.Combine(Paths.TexturePacks,
					UserSettings.Local.SelectedTexturePack)));

			Pixel = new FontRenderer(
				new Font(Content, "Fonts/Pixel"),
				new Font(Content, "Fonts/Pixel", FontStyle.Bold), null, null,
				new Font(Content, "Fonts/Pixel", FontStyle.Italic));

			base.LoadContent();
		}

		private void OnKeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
		{
			foreach (var module in InputModules)
			{
				if (module is IInputModule input)
					if (input.KeyDown(GameTime, e))
						break;
			}
		}

		private void OnKeyboardKeyUp(object sender, KeyboardKeyEventArgs e)
		{
			foreach (var module in InputModules)
			{
				if (module is IInputModule input)
					if (input.KeyUp(GameTime, e))
						break;
			}
		}

		private void OnGamePadButtonUp(object sender, GamePadButtonEventArgs e)
		{
			foreach (var module in InputModules)
			{
				if (module is IInputModule input)
					if (input.GamePadButtonUp(GameTime, e))
						break;
			}
		}

		private void OnGamePadButtonDown(object sender, GamePadButtonEventArgs e)
		{
			foreach (var module in InputModules)
			{
				if (module is IInputModule input)
					if (input.GamePadButtonDown(GameTime, e))
						break;
			}
		}

		private void OnMouseComponentScroll(object sender, MouseScrollEventArgs e)
		{
			foreach (var module in InputModules)
			{
				if (module is IInputModule input)
					if (input.MouseScroll(GameTime, e))
						break;
			}
		}

		private void OnMouseComponentButtonDown(object sender, MouseButtonEventArgs e)
		{
			foreach (var module in InputModules)
			{
				if (module is IInputModule input)
					if (input.MouseButtonDown(GameTime, e))
						break;
			}
		}

		private void OnMouseComponentButtonUp(object sender, MouseButtonEventArgs e)
		{
			foreach (var module in InputModules)
			{
				if (module is IInputModule input)
					if (input.MouseButtonUp(GameTime, e))
						break;
			}
		}

		private void OnMouseComponentMove(object sender, MouseMoveEventArgs e)
		{
			foreach (var module in InputModules)
			{
				if (module is IInputModule input)
					if (input.MouseMove(GameTime, e))
						break;
			}
		}

		public void TakeScreenShot()
		{
			var path = Path.Combine(Paths.Screenshots, DateTime.Now.ToString("yyyy-MM-dd_H.mm.ss") + ".png");
			var directoryName = Path.GetDirectoryName(path);
			Directory.CreateDirectory(directoryName ?? throw new InvalidOperationException());

			using (var stream = File.OpenWrite(path))
				RenderTarget.SaveAsPng(stream, RenderTarget.Width, RenderTarget.Height);

			ChatModule.AddMessage($"Screen shot saved to {Path.GetFileName(path)}");
		}

		public void FlushMainThreadActions()
		{
			while (PendingMainThreadActions.TryTake(out var action))
				action();
		}

		protected override void Update(GameTime gameTime)
		{
			GameTime = gameTime;

			if (PendingMainThreadActions.TryTake(out var action))
				action();

			var adjusted = Client.World.World.FindBlockPosition(
				new Coordinates3D((int) Client.Position.X, 0, (int) Client.Position.Z), out var chunk);
			if (chunk != null && Client.LoggedIn)
				if (chunk.GetHeight((byte) adjusted.X, (byte) adjusted.Z) != 0)
					Client.Physics.Update(gameTime.ElapsedGameTime);
			if (NextPhysicsUpdate < DateTime.UtcNow && Client.LoggedIn)
			{
				// NOTE: This is to make the vanilla server send us chunk packets
				// We should eventually make some means of detecting that we're on a vanilla server to enable this
				// It's a waste of bandwidth to do it on a TrueCraft server
				Client.QueuePacket(new PlayerGroundedPacket {OnGround = true});
				NextPhysicsUpdate = DateTime.UtcNow.AddMilliseconds(50);
			}

			if (IsActive)
			{
				foreach (var module in InputModules)
					module.Update(gameTime);
			}

			foreach (var module in GraphicalModules)
				module.Update(gameTime);

			UpdateCamera();

			base.Update(gameTime);
		}

		private void UpdateCamera()
		{
			const double bobbingMultiplier = 0.05;

			var bobbing = Bobbing * 1.5;
			var xbob = Math.Cos(bobbing + Math.PI / 2) * bobbingMultiplier;
			var ybob = Math.Sin(Math.PI / 2 - 2 * bobbing) * bobbingMultiplier;

			Camera.Position = new Vector3(
				(float) (Client.Position.X + xbob), (float) (Client.Position.Y + Client.Size.Height + ybob), Client.Position.Z);

			Camera.Pitch = Client.Pitch;
			Camera.Yaw = Client.Yaw;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.SetRenderTarget(RenderTarget);
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

			Mesh.ResetStats();

			foreach (var module in GraphicalModules)
			{
				if (module is IGraphicalModule drawable)
					drawable.Draw(gameTime);
			}

			_imgui.BeforeLayout(gameTime);
			ImGuiLayout();
			_imgui.AfterLayout();

			GraphicsDevice.SetRenderTarget(null);
			SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
			SpriteBatch.Draw(RenderTarget, Vector2.Zero, Color.White);
			SpriteBatch.End();
			
			base.Draw(gameTime);
		}

		private bool show_test_window = false;

		
		protected virtual void ImGuiLayout()
		{
			ImGui.SetNextWindowSize(new System.Numerics.Vector2(500, 400), ImGuiCond.Always);
			ImGui.Begin("Trace");
			{
				if (ImGui.Button("Clear"))
					Client.TraceListener.Clear();
				ImGui.Separator();
				ImGui.BeginChild("scrolling");
				ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(0, 1));
				if (Client.TraceListener.Length > 0)
					ImGui.TextUnformatted(Client.TraceListener.GetPendingText());
				if (Client.TraceListener.Tail)
					ImGui.SetScrollHereY(1.0f);
				Client.TraceListener.Tail = false;
				ImGui.PopStyleVar();
				ImGui.EndChild();
			}
			ImGui.End();
			
			if (show_test_window)
			{
				ImGui.SetNextWindowPos(new System.Numerics.Vector2(650, 20), ImGuiCond.FirstUseEver);
				ImGui.ShowDemoWindow(ref show_test_window);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				KeyboardComponent.Dispose();
				MouseComponent.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}