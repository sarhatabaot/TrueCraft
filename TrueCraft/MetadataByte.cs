using TrueCraft.Networking;

namespace TrueCraft
{
	public class MetadataByte : MetadataEntry
	{
		public byte Value;

		public MetadataByte()
		{
		}

		public MetadataByte(byte value) => Value = value;

		public override byte Identifier => 0;
		public override string FriendlyName => "byte";

		public static implicit operator MetadataByte(byte value)
		{
			return new MetadataByte(value);
		}

		public override void FromStream(IMinecraftStream stream)
		{
			Value = stream.ReadUInt8();
		}

		public override void WriteTo(IMinecraftStream stream, byte index)
		{
			stream.WriteUInt8(GetKey(index));
			stream.WriteUInt8(Value);
		}
	}
}