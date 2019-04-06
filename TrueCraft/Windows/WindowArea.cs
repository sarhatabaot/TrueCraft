using System;

namespace TrueCraft.Windows
{
	public class WindowArea : IWindowArea
	{
		public WindowArea(int startIndex, int length, int width, int height)
		{
			StartIndex = startIndex;
			Length = length;
			Items = new ItemStack[Length];
			Width = width;
			Height = height;
			for (var i = 0; i < Items.Length; i++)
				Items[i] = ItemStack.EmptyStack;
		}

		public int StartIndex { get; set; }
		public int Length { get; set; }
		public virtual int Width { get; set; }
		public virtual int Height { get; set; }
		public ItemStack[] Items { get; set; }
		public event EventHandler<WindowChangeEventArgs> WindowChange;

		public virtual ItemStack this[int index]
		{
			get => Items[index];
			set
			{
				if (IsValid(value, index))
					Items[index] = value;
				OnWindowChange(new WindowChangeEventArgs(index, value));
			}
		}

		public virtual int MoveOrMergeItem(int index, ItemStack item, IWindowArea from)
		{
			var emptyIndex = -1;
			//var maximumStackSize = Item.GetMaximumStackSize(new ItemDescriptor(item.Id, item.Metadata));
			// TODO
			var maximumStackSize = 64;
			for (var i = 0; i < Length; i++)
				if (this[i].Empty && emptyIndex == -1)
					emptyIndex = i;
				else if (this[i].Id == item.Id &&
				         this[i].Metadata == item.Metadata &&
				         this[i].Count < maximumStackSize)
				{
					// Merging takes precedence over empty slots
					emptyIndex = -1;
					if (from != null)
						from[index] = ItemStack.EmptyStack;
					if (this[i].Count + item.Count > maximumStackSize)
					{
						item = new ItemStack(item.Id, (sbyte) (item.Count - (maximumStackSize - this[i].Count)),
							item.Metadata, item.Nbt);
						this[i] = new ItemStack(item.Id, (sbyte) maximumStackSize, item.Metadata, item.Nbt);
						continue;
					}

					this[i] = new ItemStack(item.Id, (sbyte) (this[i].Count + item.Count), item.Metadata);
					return i;
				}

			if (emptyIndex != -1)
			{
				if (from != null)
					from[index] = ItemStack.EmptyStack;
				this[emptyIndex] = item;
			}

			return emptyIndex;
		}

		public void CopyTo(IWindowArea area)
		{
			for (var i = 0; i < area.Length && i < Length; i++)
				area[i] = this[i];
		}

		public virtual void Dispose()
		{
			WindowChange = null;
		}

		/// <summary>
		///  Returns true if the specified slot is valid to
		///  be placed in this index.
		/// </summary>
		protected virtual bool IsValid(ItemStack slot, int index)
		{
			return true;
		}

		protected internal virtual void OnWindowChange(WindowChangeEventArgs e)
		{
			if (WindowChange != null)
				WindowChange(this, e);
		}
	}
}