using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
		public void RegisterPacketType(Type type, bool clientBound = true, bool serverBound = true)
		{
			var func = Expression.Lambda<Func<IPacket>>(Expression.Convert(Expression.New(type), typeof(IPacket))).Compile();
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

		public void RegisterCorePackets(params Assembly[] assemblies)
		{
			var types = assemblies.SelectMany(a => a.GetTypes());

			foreach(var type in types)
			{
				if (type.IsAbstract || type.IsInterface)
					continue;

				if (!type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IPacket)))
					continue;

				if (Attribute.IsDefined(type, typeof(MessageTargetAttribute)))
				{
					MessageTargetAttribute message = (MessageTargetAttribute) Attribute.GetCustomAttribute(type, typeof(MessageTargetAttribute));

					RegisterPacketType(type, message.Target.HasFlag(MessageTarget.Client), message.Target.HasFlag(MessageTarget.Server));
				}
				else
				{
					RegisterPacketType(type);
				}
			}
		}
	}
}