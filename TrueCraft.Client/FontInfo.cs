using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client
{
	[Serializable]
	public class FontInfo
	{
		private Rectangle _Padding;

		private Point _Spacing;

		[XmlAttribute("face")] public string Face { get; set; }

		[XmlAttribute("size")] public int Size { get; set; }

		[XmlAttribute("bold")] public int Bold { get; set; }

		[XmlAttribute("italic")] public int Italic { get; set; }

		[XmlAttribute("charset")] public string CharSet { get; set; }

		[XmlAttribute("unicode")] public int Unicode { get; set; }

		[XmlAttribute("stretchH")] public int StretchHeight { get; set; }

		[XmlAttribute("smooth")] public int Smooth { get; set; }

		[XmlAttribute("aa")] public int SuperSampling { get; set; }

		[XmlAttribute("padding")]
		public string Padding
		{
			get => _Padding.X + "," + _Padding.Y + "," + _Padding.Width + "," + _Padding.Height;
			set
			{
				var padding = value.Split(',');
				_Padding = new Rectangle(Convert.ToInt32(padding[0]), Convert.ToInt32(padding[1]),
					Convert.ToInt32(padding[2]), Convert.ToInt32(padding[3]));
			}
		}

		[XmlAttribute("spacing")]
		public string Spacing
		{
			get => _Spacing.X + "," + _Spacing.Y;
			set
			{
				var spacing = value.Split(',');
				_Spacing = new Point(Convert.ToInt32(spacing[0]), Convert.ToInt32(spacing[1]));
			}
		}

		[XmlAttribute("outline")] public int OutLine { get; set; }
	}
}