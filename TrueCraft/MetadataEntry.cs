using TrueCraft.API.Networking;

namespace TrueCraft.API
{
	public abstract class MetadataEntry
	{
		public abstract byte Identifier { get; }
		public abstract string FriendlyName { get; }

		internal byte Index { get; set; }

		public abstract void FromStream(IMinecraftStream stream);
		public abstract void WriteTo(IMinecraftStream stream, byte index);

		public static implicit operator MetadataEntry(byte value)
		{
			return new MetadataByte(value);
		}

		public static implicit operator MetadataEntry(short value)
		{
			return new MetadataShort(value);
		}

		public static implicit operator MetadataEntry(int value)
		{
			return new MetadataInt(value);
		}

		public static implicit operator MetadataEntry(float value)
		{
			return new MetadataFloat(value);
		}

		public static implicit operator MetadataEntry(string value)
		{
			return new MetadataString(value);
		}

		public static implicit operator MetadataEntry(ItemStack value)
		{
			return new MetadataSlot(value);
		}

		protected byte GetKey(byte index)
		{
			Index = index; // Cheat to get this for ToString
			return (byte) ((Identifier << 5) | (index & 0x1F));
		}

		public override string ToString()
		{
			var type = GetType();
			var fields = type.GetFields();
			var result = FriendlyName + "[" + Index + "]: ";
			if (fields.Length != 0)
				result += fields[0].GetValue(this).ToString();
			return result;
		}
	}
}