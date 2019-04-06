using System;

namespace TrueCraft.Networking
{
	[Flags]
	public enum MessageTarget : byte
	{
		None,
		Server = 1 << 0,
		Client = 1 << 1,
		Duplex = Server | Client,
		All = byte.MaxValue
	}
}