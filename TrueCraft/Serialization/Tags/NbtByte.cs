﻿using System.Text;

namespace TrueCraft.Serialization.Tags
{
	/// <summary> A tag containing a single byte. </summary>
	public sealed class NbtByte : NbtTag
	{
		/// <summary> Creates an unnamed NbtByte tag with the default value of 0. </summary>
		public NbtByte()
		{
		}


		/// <summary> Creates an unnamed NbtByte tag with the given value. </summary>
		/// <param name="value"> Value to assign to this tag. </param>
		public NbtByte(byte value)
			: this(null, value)
		{
		}


		/// <summary> Creates an NbtByte tag with the given name and the default value of 0. </summary>
		/// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
		public NbtByte([CanBeNull] string tagName)
			: this(tagName, 0)
		{
		}


		/// <summary> Creates an NbtByte tag with the given name and value. </summary>
		/// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
		/// <param name="value"> Value to assign to this tag. </param>
		public NbtByte([CanBeNull] string tagName, byte value)
		{
			Name = tagName;
			Value = value;
		}

		/// <summary> Type of this tag (Byte). </summary>
		public override NbtTagType TagType => NbtTagType.Byte;

		/// <summary> Value/payload of this tag (a single byte). </summary>
		public byte Value { get; set; }


		internal override bool ReadTag(NbtBinaryReader readStream)
		{
			if (readStream.Selector != null && !readStream.Selector(this))
			{
				readStream.ReadByte();
				return false;
			}

			Value = readStream.ReadByte();
			return true;
		}


		internal override void SkipTag(NbtBinaryReader readStream)
		{
			readStream.ReadByte();
		}


		internal override void WriteTag(NbtBinaryWriter writeStream, bool writeName)
		{
			writeStream.Write(NbtTagType.Byte);
			if (writeName)
			{
				if (Name == null)
					throw new NbtFormatException("Name is null");
				writeStream.Write(Name);
			}

			writeStream.Write(Value);
		}


		internal override void WriteData(NbtBinaryWriter writeStream)
		{
			writeStream.Write(Value);
		}


		/// <summary>
		///  Returns a String that represents the current NbtByte object.
		///  Format: TAG_Byte("Name"): Value
		/// </summary>
		/// <returns> A String that represents the current NbtByte object. </returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			PrettyPrint(sb, null, 0);
			return sb.ToString();
		}


		internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
		{
			for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
			sb.Append("TAG_Byte");
			if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
			sb.Append(": ");
			sb.Append(Value);
		}
	}
}