using System;
using System.IO;
using System.Text;
using TrueCraft.Networking;
using TrueCraft.Serialization;
using TrueCraft.Serialization.Serialization;
using TrueCraft.Serialization.Tags;

namespace TrueCraft
{
	/// <summary>
	///  Represents a stack of items.
	/// </summary>
	public struct ItemStack : ICloneable, IEquatable<ItemStack>
	{
		/// <summary>
		///  Returns the hash code for this item stack.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _Id.GetHashCode();
				hashCode = (hashCode * 397) ^ _Count.GetHashCode();
				hashCode = (hashCode * 397) ^ Metadata.GetHashCode();
				hashCode = (hashCode * 397) ^ Index;
				hashCode = (hashCode * 397) ^ (Nbt != null ? Nbt.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(ItemStack left, ItemStack right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ItemStack left, ItemStack right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///  Creates a new item stack with the specified values.
		/// </summary>
		/// <param name="Id">The item Id for the item stack.</param>
		public ItemStack(short Id) : this()
		{
			_Id = Id;
			_Count = 1;
			Metadata = 0;
			Nbt = null;
			Index = 0;
		}

		/// <summary>
		///  Creates a new item stack with the specified values.
		/// </summary>
		/// <param name="Id">The item Id for the item stack.</param>
		/// <param name="count">The item count for the item stack.</param>
		public ItemStack(short Id, sbyte count) : this(Id) => Count = count;

		/// <summary>
		///  Creates a new item stack with the specified values.
		/// </summary>
		/// <param name="Id">The item Id for the item stack.</param>
		/// <param name="count">The item count for the item stack.</param>
		/// <param name="metadata">The metadata for the item stack.</param>
		public ItemStack(short Id, sbyte count, short metadata) : this(Id, count) => Metadata = metadata;

		/// <summary>
		///  Creates a new item stack with the specified values.
		/// </summary>
		/// <param name="Id">The item Id for the item stack.</param>
		/// <param name="count">The item count for the item stack.</param>
		/// <param name="metadata">The metadata for the item stack.</param>
		/// <param name="nbt">The NBT compound tag for the item stack.</param>
		public ItemStack(short Id, sbyte count, short metadata, NbtCompound nbt) : this(Id, count, metadata)
		{
			Nbt = nbt;
			if (Count == 0)
			{
				Id = -1;
				Metadata = 0;
				Nbt = null;
			}
		}

		/// <summary>
		///  Creates and returns a new item stack read from a Minecraft stream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <returns></returns>
		public static ItemStack FromStream(IMcStream stream)
		{
			var slot = EmptyStack;
			slot.Id = stream.ReadInt16();
			if (slot.Empty)
				return slot;
			slot.Count = stream.ReadInt8();
			slot.Metadata = stream.ReadInt16();
			var length = stream.ReadInt16();
			if (length == -1)
				return slot;
			slot.Nbt = new NbtCompound();
			var buffer = stream.ReadUInt8Array(length);
			var nbt = new NbtFile();
			nbt.LoadFromBuffer(buffer, 0, length, NbtCompression.GZip, null);
			slot.Nbt = nbt.RootTag;
			return slot;
		}

		/// <summary>
		///  Writes this item stack to a Minecraft stream.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		public void WriteTo(IMcStream stream)
		{
			stream.WriteInt16(Id);
			if (Empty)
				return;
			stream.WriteInt8(Count);
			stream.WriteInt16(Metadata);
			if (Nbt == null)
			{
				stream.WriteInt16(-1);
				return;
			}

			var mStream = new MemoryStream();
			var file = new NbtFile(Nbt);
			file.SaveToStream(mStream, NbtCompression.GZip);
			stream.WriteInt16((short) mStream.Position);
			stream.WriteUInt8Array(mStream.GetBuffer());
		}

		/// <summary>
		///  Creates and returns a new item stack created from an NBT compound tag.
		/// </summary>
		/// <param name="compound">The compound tag to create the item stack from.</param>
		/// <returns></returns>
		public static ItemStack FromNbt(NbtCompound compound)
		{
			var s = EmptyStack;
			s.Id = compound.Get<NbtShort>("Id").Value;
			s.Metadata = compound.Get<NbtShort>("Damage").Value;
			s.Count = (sbyte) compound.Get<NbtByte>("Count").Value;
			s.Index = compound.Get<NbtByte>("Slot").Value;
			if (compound.Get<NbtCompound>("tag") != null)
				s.Nbt = compound.Get<NbtCompound>("tag");
			return s;
		}

		/// <summary>
		///  Creates and returns a new NBT compound tag containing this item stack.
		/// </summary>
		/// <returns></returns>
		public NbtCompound ToNbt()
		{
			var c = new NbtCompound();
			c.Add(new NbtShort("Id", Id));
			c.Add(new NbtShort("Damage", Metadata));
			c.Add(new NbtByte("Count", (byte) Count));
			c.Add(new NbtByte("Slot", (byte) Index));
			if (Nbt != null)
				c.Add(new NbtCompound("tag"));
			return c;
		}

		/// <summary>
		///  Gets whether this item stack is empty.
		/// </summary>
		[NbtIgnore]
		public bool Empty => Id == -1;

		/// <summary>
		///  Gets or sets the item Id for this item stack.
		/// </summary>
		public short Id
		{
			get => _Id;
			set
			{
				_Id = value;
				if (_Id == -1)
				{
					_Count = 0;
					Metadata = 0;
					Nbt = null;
				}
			}
		}

		/// <summary>
		///  Gets or sets the item count for this item stack.
		/// </summary>
		public sbyte Count
		{
			get => _Count;
			set
			{
				_Count = value;
				if (_Count == 0)
				{
					_Id = -1;
					Metadata = 0;
					Nbt = null;
				}
			}
		}

		/// <summary>
		///  Gets or sets the metadata for this item stack.
		/// </summary>
		public short Metadata { get; set; }

		private short _Id;
		private sbyte _Count;

		/// <summary>
		///  The NBT compound tag for this item stack, if any.
		/// </summary>
		[IgnoreOnNull]
		public NbtCompound Nbt { get; set; }

		/// <summary>
		///  The index (slot) of this item stack in an inventory.
		/// </summary>
		[NbtIgnore] public int Index;

		/// <summary>
		///  Returns the string representation of this item stack.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (Empty)
				return "(Empty)";

			var resultBuilder = new StringBuilder("Id: " + Id);

			if (Count != 1) resultBuilder.Append("; Count: " + Count);
			if (Metadata != 0) resultBuilder.Append("; Metadata: " + Metadata);
			if (Nbt != null) resultBuilder.Append(Environment.NewLine + Nbt);

			return "(" + resultBuilder + ")";
		}

		/// <summary>
		///  Returns a clone of this item stack.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new ItemStack(Id, Count, Metadata, Nbt);
		}

		/// <summary>
		///  Gets an empty item stack.
		/// </summary>
		[NbtIgnore]
		public static ItemStack EmptyStack => new ItemStack(-1);

		/// <summary>
		///  Determines whether this item stack can merge with another.
		/// </summary>
		/// <param name="other">The other item stack.</param>
		/// <returns></returns>
		public bool CanMerge(ItemStack other)
		{
			if (Empty || other.Empty)
				return true;
			return _Id == other._Id && Metadata == other.Metadata && Equals(Nbt, other.Nbt);
		}

		/// <summary>
		///  Determines whether this item stack and another object are equal.
		/// </summary>
		/// <param name="obj">The other object.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is ItemStack && Equals((ItemStack) obj);
		}

		/// <summary>
		///  Determines whether this item stack and another are equal.
		/// </summary>
		/// <param name="other">The other item stack.</param>
		/// <returns></returns>
		public bool Equals(ItemStack other)
		{
			return _Id == other._Id && _Count == other._Count && Metadata == other.Metadata && Index == other.Index &&
			       Equals(Nbt, other.Nbt);
		}
	}
}