using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TrueCraft.Collections;

namespace TrueCraft.Networking
{
	public class PacketSegmentProcessor : IPacketSegmentProcessor
	{
		private readonly TraceSource _trace;

		public PacketSegmentProcessor(PacketReader packetReader, bool serverBound, TraceSource trace)
		{
			_trace = trace;
			PacketBuffer = new List<byte>();
			PacketReader = packetReader;
			ServerBound = serverBound;
		}

		public List<byte> PacketBuffer { get; }

		public PacketReader PacketReader { get; protected set; }

		public bool ServerBound { get; }

		public IPacket CurrentPacket { get; protected set; }

		public IPacket PreviousPacket { get; protected set; }

		public bool ProcessNextSegment(byte[] nextSegment, int offset, int len, out IPacket packet)
		{
			packet = null;
			CurrentPacket = null;

			lock (PacketBuffer)
			{
				if (nextSegment.Length > 0)
					PacketBuffer.AddRange(new ByteArraySegment(nextSegment, offset, len));

				if (PacketBuffer.Count == 0)
					return false;

				if (CurrentPacket == null)
				{
					var packetId = PacketBuffer[0];

					var createPacket = ServerBound ? PacketReader.ServerBoundPackets[packetId] : PacketReader.ClientBoundPackets[packetId];
					if (createPacket != null)
						CurrentPacket = createPacket();
					else
					{
						_trace.TraceData(TraceEventType.Error, 0, $"Unable to read packet type with Id #{packetId:X2}");
						PacketBuffer.Clear();
						return false;
					}
				}

				if (CurrentPacket != null)
				{
					using (var listStream = new ByteListMemoryStream(PacketBuffer, 1))
					{
						using (var ms = new McStream(listStream))
						{
							try
							{
								CurrentPacket.ReadPacket(ms);
							}
							catch (EndOfStreamException)
							{
								_trace.TraceData(TraceEventType.Error, 0, $"unexpected end of packet stream");
								return false;
							}
						}

						PacketBuffer.RemoveRange(0, (int) listStream.Position);
					}

					packet = CurrentPacket;
					CurrentPacket = null;
					PreviousPacket = packet;
				}

				return PacketBuffer.Count > 0;
			}
		}
	}
}