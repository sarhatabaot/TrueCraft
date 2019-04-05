namespace TrueCraft.Networking
{
	public interface IPacket
	{
		byte ID { get; }
		void ReadPacket(IMcStream stream);
		void WritePacket(IMcStream stream);
	}
}