using System.Reflection;
using TrueCraft.Launcher.Views;
using Xwt;
using Xwt.Drawing;

namespace TrueCraft.Launcher
{
	public class LauncherWindow : Window
	{
		public LauncherWindow()
		{
			Title = "TrueCraft Launcher";
			Width = 300;
			Height = 100;
			User = new TrueCraftUser();

			MainContainer = new HBox();
			OptionView = new OptionView(this);
			MultiPlayerView = new MultiPlayerView(this);
			SinglePlayerView = new SinglePlayerView(this);
			InteractionBox = new VBox();
			MainMenuView = new MainMenuView(this);

			using (var stream = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("TrueCraft.Launcher.Content.truecraft_logo.png"))
				TrueCraftLogoImage = new ImageView(Image.FromStream(stream).WithBoxSize(300, 75));

			InteractionBox.PackStart(TrueCraftLogoImage);
			InteractionBox.PackEnd(MainMenuView);
			MainContainer.PackEnd(InteractionBox);

			Content = MainContainer;
		}

		public TrueCraftUser User { get; set; }
		public HBox MainContainer { get; set; }
		public MainMenuView MainMenuView { get; set; }
		public OptionView OptionView { get; set; }
		public MultiPlayerView MultiPlayerView { get; set; }
		public SinglePlayerView SinglePlayerView { get; set; }
		public VBox InteractionBox { get; set; }
		public ImageView TrueCraftLogoImage { get; set; }

		private void ClientExited()
		{
			Show();
			ShowInTaskbar = true;
		}
	}
}