using System;

namespace TrueCraft.Networking
{
	public interface IPacket
	{
		byte Id { get; }
		void ReadPacket(IMcStream stream);
		void WritePacket(IMcStream stream);
	}

	[Flags]
	public enum MessageTarget : byte
	{
		None,
		Server = 1 << 0,
		Client = 1 << 1,
		Duplex = Server | Client,
		All = byte.MaxValue
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class MessageTargetAttribute : Attribute
	{
		public MessageTargetAttribute(MessageTarget target)
		{
			Target = target;
		}

		public MessageTarget Target { get; }
	}
}