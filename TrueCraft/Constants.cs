using System.Collections.Generic;

namespace TrueCraft
{

	public static class Constants
	{
		public static readonly HashSet<byte> IgnoredPacketIds = new HashSet<byte>(new[]
		{
			PacketIds.PlayerGrounded,
			PacketIds.PlayerPositionAndLook
		});

		public static readonly HashSet<string> IgnoredEvents = new HashSet<string>(new[]
		{
			Events.Grass,
			Events.Fluid,
			Events.FireSpread
		});

		public static class PacketIds
		{
			public const byte PlayerGrounded = 0x0A;
			public const byte PlayerPositionAndLook = 0x0D;

			public const byte EntityTeleport = 0x22;

			public const byte Disconnect = 0xFF;
		}

		public static class Events
		{
			public const string ClientUpdateChunks = "client.update-chunks";

			public const string RemoteChunks = "remote.chunks";
			public const string RemoteKeepAlive = "remote.keepalive";

			public const string WorldSave = "world.save";

			public const string Grass = "grass";
			public const string Fluid = "fluid";
			public const string FireSpread = "fire.spread";
		}
	}
}