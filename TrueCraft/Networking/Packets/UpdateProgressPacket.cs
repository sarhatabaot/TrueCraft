﻿namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Updates the progress on the furnace UI.
	/// </summary>
	public struct UpdateProgressPacket : IPacket
	{
		public enum ProgressTarget
		{
			ItemCompletion = 0,
			AvailableHeat = 1
		}

		public byte Id => Constants.PacketIds.UpdateProgress;

		public UpdateProgressPacket(sbyte windowID, ProgressTarget target, short value)
		{
			WindowID = windowID;
			Target = target;
			Value = value;
		}

		public sbyte WindowID;
		public ProgressTarget Target;

		/// <summary>
		///  For the item completion, about 180 is full. For the available heat, about 250 is full.
		/// </summary>
		public short Value;

		public void ReadPacket(IMcStream stream)
		{
			WindowID = stream.ReadInt8();
			Target = (ProgressTarget) stream.ReadInt16();
			Value = stream.ReadInt16();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt8(WindowID);
			stream.WriteInt16((short) Target);
			stream.WriteInt16(Value);
		}
	}
}