using TrueCraft.Serialization.Tags;

namespace TrueCraft.Serialization.Serialization
{
	public interface INbtSerializable
	{
		NbtTag Serialize(string tagName);
		void Deserialize(NbtTag value);
	}
}