using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TrueCraft.Networking
{
	public interface IPacketReader
	{
		int ProtocolVersion { get; }

		ConcurrentDictionary<object, IPacketSegmentProcessor> Processors { get; }
		void RegisterPacketType<T>(bool clientBound = true, bool serverBound = true) where T : IPacket;
		IEnumerable<IPacket> ReadPackets(object key, byte[] buffer, int offset, int length, bool serverBound = true);
		void WritePacket(IMcStream stream, IPacket packet);
	}
}