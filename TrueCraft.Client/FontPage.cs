using System;
using System.Xml.Serialization;

namespace TrueCraft.Client
{
	[Serializable]
	public class FontPage
	{
		[XmlAttribute("id")] public int ID { get; set; }

		[XmlAttribute("file")] public string File { get; set; }
	}
}