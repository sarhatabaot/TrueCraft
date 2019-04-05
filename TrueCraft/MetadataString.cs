using System;
using TrueCraft.Networking;

namespace TrueCraft
{
	public class MetadataString : MetadataEntry
	{
		public string Value;

		public MetadataString()
		{
		}

		public MetadataString(string value)
		{
			if (value.Length > 16)
				throw new ArgumentOutOfRangeException("value", "Maximum string length is 16 characters");
			while (value.Length < 16)
				value = value + "\0";
			Value = value;
		}

		public override byte Identifier => 4;
		public override string FriendlyName => "string";

		public static implicit operator MetadataString(string value)
		{
			return new MetadataString(value);
		}

		public override void FromStream(IMcStream stream)
		{
			Value = stream.ReadString();
		}

		public override void WriteTo(IMcStream stream, byte index)
		{
			stream.WriteUInt8(GetKey(index));
			stream.WriteString(Value);
		}
	}
}