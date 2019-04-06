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
			public const byte SpawnPosition = 0x06;
			public const byte PlayerGrounded = 0x0A;
			public const byte PlayerPositionAndLook = 0x0D;
			public const byte PlayerAction = 0x13;
			public const byte SpawnPlayer = 0x14;
			public const byte SpawnItem = 0x15;
			public const byte SpawnMob = 0x18;
			public const byte SpawnPainting = 0x19;
			public const byte EntityTeleport = 0x22;
			public const byte Disconnect = 0xFF;
			public const byte TimeUpdate = 0x04;
			public const byte WindowItems = 0x68;
			public const byte ChatMessage = 0x03;
			public const byte PlayerBlockPlacement = 0x0F;
			public const byte Explosion = 0x3C;
			public const byte Handshake = 0x02;
			public const byte EnvironmentState = 0x46;
			public const byte Lightning = 0x47;
			public const byte LoginRequest = 0x01;
			public const byte LoginResponse = 0x01;
			public const byte MapData = 0x83;
			public const byte OpenWindow = 0x64;
			public const byte Animation = 0x12;
			public const byte AttachEntity = 0x27;
			public const byte UpdateStatistic = 0xC8;
			public const byte UseBed = 0x11;
			public const byte UseEntity = 0x07;
			public const byte UselessEntity = 0x1E;
			public const byte UpdateSign = 0x82;
			public const byte UpdateProgress = 0x69;
			public const byte UpdateHealth = 0x08;
			public const byte TransactionStatus = 0x6A;
			public const byte SpawnGenericEntity = 0x17;
			public const byte SoundEffect = 0x3D;
			public const byte SetSlot = 0x67;
			public const byte SetPlayerPosition = 0x0D;
			public const byte Respawn = 0x09;
			public const byte PlayerPosition = 0x0B;
			public const byte PlayerLook = 0x0C;
			public const byte KeepAlive = 0x00;
			public const byte HandshakeResponse = 0x02;
			public const byte EntityVelocity = 0x1C;
			public const byte EntityStatus = 0x26;
			public const byte EntityRelativeMove = 0x1F;
			public const byte EntityMetadata = 0x28;
			public const byte EntityLook = 0x20;
			public const byte EntityLookAndRelativeMove = 0x21;
			public const byte EntityEquipment = 0x05;
			public const byte DestroyEntity = 0x1D;
			public const byte CollectItem = 0x16;
			public const byte CloseWindow = 0x65;
			public const byte ClickWindow = 0x66;
			public const byte ChunkPreamble = 0x32;
			public const byte ChunkData = 0x33;
			public const byte ChangeHeldItem = 0x10;
			public const byte BulkBlockChange = 0x34;
			public const byte BlockChange = 0x35;
			public const byte BlockAction = 0x36;
			public const byte PlayerDigging = 0x0E;
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