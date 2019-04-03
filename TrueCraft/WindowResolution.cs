namespace TrueCraft
{
	public class WindowResolution
	{
		public static readonly WindowResolution[] Defaults =
		{
			// (from Wikipedia/other)
			FromString("800 x 600"), // SVGA
			FromString("960 x 640"), // DVGA
			FromString("1024 x 600"), // WSVGA
			FromString("1024 x 768"), // XGA
			FromString("1280 x 1024"), // SXGA
			FromString("1600 x 1200"), // UXGA
			FromString("1920 x 1080"), // big
			FromString("1920 x 1200"), // really big
			FromString("4096 x 2160") // huge
		};

		public int Width { get; set; }
		public int Height { get; set; }

		public static WindowResolution FromString(string str)
		{
			var tmp = str.Split('x');
			return new WindowResolution
			{
				Width = int.Parse(tmp[0].Trim()),
				Height = int.Parse(tmp[1].Trim())
			};
		}

		public override string ToString()
		{
			return string.Format("{0} x {1}", Width, Height);
		}
	}
}