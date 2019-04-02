using System.IO;
using System.Xml.Serialization;

namespace TrueCraft.Client
{
	public class FontLoader
	{
		public static FontFile Load(Stream stream)
		{
			var deserializer = new XmlSerializer(typeof(FontFile));
			var file = (FontFile) deserializer.Deserialize(stream);
			return file;
		}
	}
}