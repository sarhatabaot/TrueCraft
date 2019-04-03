namespace TrueCraft.Networking
{
	public interface IPacketSegmentProcessor
	{
		bool ProcessNextSegment(byte[] nextSegment, int offset, int len, out IPacket packet);
	}
}