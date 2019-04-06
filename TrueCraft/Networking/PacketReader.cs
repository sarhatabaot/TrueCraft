using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using TrueCraft.Networking.Packets;

namespace TrueCraft.Networking
{
	public class PacketReader : IPacketReader
	{
		public const int Version = 14;

		private readonly TraceSource _trace;
		private static readonly byte[] EmptyBuffer = new byte[0];

		internal Func<IPacket>[] ClientBoundPackets = new Func<IPacket>[0x100];
		internal Func<IPacket>[] ServerBoundPackets = new Func<IPacket>[0x100];

		public PacketReader(TraceSource trace)
		{
			_trace = trace;
			Processors = new ConcurrentDictionary<object, IPacketSegmentProcessor>();
		}

		public int ProtocolVersion => Version;

		public ConcurrentDictionary<object, IPacketSegmentProcessor> Processors { get; }

		public void RegisterPacketType<T>(bool clientBound = true, bool serverBound = true) where T : IPacket
		{
			var func = Expression.Lambda<Func<IPacket>>(Expression.Convert(Expression.New(typeof(T)), typeof(IPacket))).Compile();
			var packet = func();

			if (clientBound)
				ClientBoundPackets[packet.Id] = func;
			if (serverBound)
				ServerBoundPackets[packet.Id] = func;
		}

		public IEnumerable<IPacket> ReadPackets(object key, byte[] buffer, int offset, int length, bool serverBound = true)
		{
			if (!Processors.ContainsKey(key))
				Processors[key] = new PacketSegmentProcessor(this, serverBound, _trace);

			var processor = Processors[key];
			if (processor == null)
			{
				yield break;
			}

			processor.ProcessNextSegment(buffer, offset, length, out var packet);
			if (packet == null)
				yield break;

			while (true)
			{
				yield return packet;

				if (processor.ProcessNextSegment(EmptyBuffer, 0, 0, out packet))
					continue;

				if (packet != null)
					yield return packet;

				yield break;
			}
		}

		public void WritePacket(IMcStream stream, IPacket packet)
		{
			stream.WriteUInt8(packet.Id);
			packet.WritePacket(stream);
			stream.BaseStream.Flush();
		}

		/// <summary>
		///  Registers TrueCraft.Core implementations of all packets used by vanilla MC.
		/// </summary>
		public void RegisterCorePackets()
		{
			RegisterPacketType<KeepAlivePacket>(); // 0x00
			RegisterPacketType<LoginRequestPacket>(serverBound: true, clientBound: false); // 0x01
			RegisterPacketType<LoginResponsePacket>(serverBound: false, clientBound: true); // 0x01
			RegisterPacketType<HandshakePacket>(serverBound: true, clientBound: false); // 0x02
			RegisterPacketType<HandshakeResponsePacket>(serverBound: false, clientBound: true); // 0x02
			RegisterPacketType<ChatMessagePacket>(); // 0x03
			RegisterPacketType<TimeUpdatePacket>(serverBound: false, clientBound: true); // 0x04
			RegisterPacketType<EntityEquipmentPacket>(serverBound: false, clientBound: true); // 0x05 // NOTE: serverBound not confirmed
			RegisterPacketType<SpawnPositionPacket>(serverBound: false, clientBound: true); // 0x06
			RegisterPacketType<UseEntityPacket>(serverBound: true, clientBound: false); // 0x07
			RegisterPacketType<UpdateHealthPacket>(serverBound: false, clientBound: true); // 0x08
			RegisterPacketType<RespawnPacket>(); // 0x09
			RegisterPacketType<PlayerGroundedPacket>(serverBound: true, clientBound: false); // 0x0A
			RegisterPacketType<PlayerPositionPacket>(serverBound: true, clientBound: false); // 0x0B
			RegisterPacketType<PlayerLookPacket>(serverBound: true, clientBound: false); // 0x0C
			RegisterPacketType<PlayerPositionAndLookPacket>(serverBound: true, clientBound: false); // 0x0D
			RegisterPacketType<SetPlayerPositionPacket>(serverBound: false, clientBound: true); // 0x0D
			RegisterPacketType<PlayerDiggingPacket>(serverBound: true, clientBound: false); // 0x0E
			RegisterPacketType<PlayerBlockPlacementPacket>(serverBound: true, clientBound: false); // 0x0F
			RegisterPacketType<ChangeHeldItemPacket>(serverBound: true, clientBound: false); // 0x10
			RegisterPacketType<UseBedPacket>(serverBound: false, clientBound: true); // 0x11
			RegisterPacketType<AnimationPacket>(); // 0x12
			RegisterPacketType<PlayerActionPacket>(serverBound: true, clientBound: false); // 0x13
			RegisterPacketType<SpawnPlayerPacket>(serverBound: false, clientBound: true); // 0x14
			RegisterPacketType<SpawnItemPacket>(serverBound: true, clientBound: true); // 0x15
			RegisterPacketType<CollectItemPacket>(serverBound: false, clientBound: true); // 0x16
			RegisterPacketType<SpawnGenericEntityPacket>(serverBound: false, clientBound: true); // 0x17
			RegisterPacketType<SpawnMobPacket>(serverBound: false, clientBound: true); // 0x18
			RegisterPacketType<SpawnPaintingPacket>(serverBound: false, clientBound: true); // 0x19
			RegisterPacketType<EntityVelocityPacket>(serverBound: false, clientBound: true); // 0x1C
			RegisterPacketType<DestroyEntityPacket>(serverBound: false, clientBound: true); // 0x1D
			RegisterPacketType<UselessEntityPacket>(serverBound: false, clientBound: true); // 0x1E
			RegisterPacketType<EntityRelativeMovePacket>(serverBound: false, clientBound: true); // 0x1F
			RegisterPacketType<EntityLookPacket>(serverBound: false, clientBound: true); // 0x20
			RegisterPacketType<EntityLookAndRelativeMovePacket>(serverBound: false, clientBound: true); // 0x21
			RegisterPacketType<EntityTeleportPacket>(serverBound: false, clientBound: true); // 0x22
			RegisterPacketType<EntityStatusPacket>(serverBound: false, clientBound: true); // 0x26
			RegisterPacketType<AttachEntityPacket>(serverBound: false, clientBound: true); // 0x27
			RegisterPacketType<EntityMetadataPacket>(serverBound: false, clientBound: true); // 0x28
			RegisterPacketType<ChunkPreamblePacket>(serverBound: false, clientBound: true); // 0x32
			RegisterPacketType<ChunkDataPacket>(serverBound: false, clientBound: true); // 0x33
			RegisterPacketType<BulkBlockChangePacket>(serverBound: false, clientBound: true); // 0x34
			RegisterPacketType<BlockChangePacket>(serverBound: false, clientBound: true); // 0x35
			RegisterPacketType<BlockActionPacket>(serverBound: false, clientBound: true); // 0x36
			RegisterPacketType<ExplosionPacket>(serverBound: false, clientBound: true); // 0x3C
			RegisterPacketType<SoundEffectPacket>(serverBound: false, clientBound: true); // 0x3D
			RegisterPacketType<EnvironmentStatePacket>(serverBound: false, clientBound: true); // 0x46
			RegisterPacketType<LightningPacket>(serverBound: false, clientBound: true); // 0x47
			RegisterPacketType<OpenWindowPacket>(serverBound: false, clientBound: true); // 0x64
			RegisterPacketType<CloseWindowPacket>(); // 0x65
			RegisterPacketType<ClickWindowPacket>(serverBound: true, clientBound: false); // 0x66
			RegisterPacketType<SetSlotPacket>(serverBound: false, clientBound: true); // 0x67
			RegisterPacketType<WindowItemsPacket>(serverBound: false, clientBound: true); // 0x68
			RegisterPacketType<UpdateProgressPacket>(serverBound: false, clientBound: true); // 0x69
			RegisterPacketType<TransactionStatusPacket>(serverBound: false, clientBound: true); // 0x6A
			RegisterPacketType<UpdateSignPacket>(); // 0x82
			RegisterPacketType<MapDataPacket>(serverBound: false, clientBound: true); // 0x83
			RegisterPacketType<UpdateStatisticPacket>(serverBound: false, clientBound: true); // 0xC8
			RegisterPacketType<DisconnectPacket>(); // 0xFF
		}
	}
}