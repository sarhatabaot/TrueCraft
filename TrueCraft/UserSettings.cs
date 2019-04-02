using System.IO;
using Newtonsoft.Json;

namespace TrueCraft.Core
{
	public class UserSettings
	{
		public UserSettings()
		{
			AutoLogin = false;
			Username = "";
			Password = "";
			LastIP = "";
			SelectedTexturePack = TexturePack.Default.Name;
			FavoriteServers = new FavoriteServer[0];
			IsFullscreen = false;
			InvertedMouse = false;
			WindowResolution = new WindowResolution
			{
				Width = 1280,
				Height = 720
			};
		}

		public static UserSettings Local { get; set; }

		public bool AutoLogin { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string LastIP { get; set; }
		public string SelectedTexturePack { get; set; }
		public FavoriteServer[] FavoriteServers { get; set; }
		public bool IsFullscreen { get; set; }
		public bool InvertedMouse { get; set; }
		public WindowResolution WindowResolution { get; set; }

		public void Load()
		{
			if (File.Exists(Paths.Settings))
				JsonConvert.PopulateObject(File.ReadAllText(Paths.Settings), this);
		}

		public void Save()
		{
			Directory.CreateDirectory(Path.GetDirectoryName(Paths.Settings));
			File.WriteAllText(Paths.Settings, JsonConvert.SerializeObject(this, Formatting.Indented));
		}
	}
}