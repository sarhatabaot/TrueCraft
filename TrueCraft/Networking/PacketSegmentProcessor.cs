﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TrueCraft.Collections;

namespace TrueCraft.Networking
{
	public class PacketSegmentProcessor : IPacketSegmentProcessor
	{
		public PacketSegmentProcessor(PacketReader packetReader, bool serverBound)
		{
			PacketBuffer = new List<byte>();
			PacketReader = packetReader;
			ServerBound = serverBound;
		}

		public List<byte> PacketBuffer { get; }

		public PacketReader PacketReader { get; protected set; }

		public bool ServerBound { get; }

		public IPacket CurrentPacket { get; protected set; }

		public bool ProcessNextSegment(byte[] nextSegment, int offset, int len, out IPacket packet)
		{
			packet = null;
			CurrentPacket = null;

			if (nextSegment.Length > 0) PacketBuffer.AddRange(new ByteArraySegment(nextSegment, offset, len));

			if (PacketBuffer.Count == 0)
				return false;

			if (CurrentPacket == null)
			{
				var packetId = PacketBuffer[0];

				Func<IPacket> createPacket;
				if (ServerBound)
					createPacket = PacketReader.ServerboundPackets[packetId];
				else
					createPacket = PacketReader.ClientboundPackets[packetId];

				
				if (createPacket != null)
					CurrentPacket = createPacket();
				else
				{
					Trace.TraceError("Unable to read packet type 0x" + packetId.ToString("X2"));
				}
			}

			if (CurrentPacket != null)
			{
				using (var listStream = new ByteListMemoryStream(PacketBuffer, 1))
				{
					using (var ms = new MinecraftStream(listStream))
						try
						{
							CurrentPacket.ReadPacket(ms);
						}
						catch (EndOfStreamException)
						{
							return false;
						}

					PacketBuffer.RemoveRange(0, (int) listStream.Position);
				}

				packet = CurrentPacket;
				CurrentPacket = null;
			}

			return PacketBuffer.Count > 0;
		}
	}
}