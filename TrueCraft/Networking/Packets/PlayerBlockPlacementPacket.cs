namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent when the player interacts with a block (generally via right clicking).
	///  This is also used for items that don't interact with blocks (i.e. food) with the coordinates set to -1.
	/// </summary>
	[MessageTarget(MessageTarget.Server)]
	public struct PlayerBlockPlacementPacket : IPacket
	{
		public byte Id => Constants.PacketIds.PlayerBlockPlacement;

		public PlayerBlockPlacementPacket(int x, sbyte y, int z, BlockFace face, short itemID,
			sbyte? amount, short? metadata)
		{
			X = x;
			Y = y;
			Z = z;
			Face = face;
			ItemId = itemID;
			Amount = amount;
			Metadata = metadata;
		}

		public int X;
		public sbyte Y;
		public int Z;
		public BlockFace Face;

		/// <summary>
		///  The block or item Id. You should probably ignore this and use a server-side inventory.
		/// </summary>
		public short ItemId;

		/// <summary>
		///  The amount in the player's hand. Who cares?
		/// </summary>
		public sbyte? Amount;

		/// <summary>
		///  The block metadata. You should probably ignore this and use a server-side inventory.
		/// </summary>
		public short? Metadata;

		public void ReadPacket(IMcStream stream)
		{
			X = stream.ReadInt32();
			Y = stream.ReadInt8();
			Z = stream.ReadInt32();
			Face = (BlockFace) stream.ReadInt8();
			ItemId = stream.ReadInt16();
			if (ItemId != -1)
			{
				Amount = stream.ReadInt8();
				Metadata = stream.ReadInt16();
			}
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(X);
			stream.WriteInt8(Y);
			stream.WriteInt32(Z);
			stream.WriteInt8((sbyte) Face);
			stream.WriteInt16(ItemId);
			if (ItemId != -1)
			{
				stream.WriteInt8(Amount.Value);
				stream.WriteInt16(Metadata.Value);
			}
		}
	}
}