namespace TrueCraft.Networking
{
	public interface IPacket
	{
		byte Id { get; }
		void ReadPacket(IMcStream stream);
		void WritePacket(IMcStream stream);
	}
}