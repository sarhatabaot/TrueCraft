﻿using System;
using TrueCraft.Networking;

namespace TrueCraft.Windows
{
	public interface IWindow : IDisposable, IEventSubject
	{
		IRemoteClient Client { get; set; }
		IWindowArea[] WindowAreas { get; }
		sbyte Id { get; set; }
		string Name { get; }
		sbyte Type { get; }
		int Length { get; }
		ItemStack this[int index] { get; set; }
		bool Empty { get; }
		short[] ReadOnlySlots { get; }
		event EventHandler<WindowChangeEventArgs> WindowChange;

		/// <summary>
		///  Call this to "shift+click" an item from one area to another.
		/// </summary>
		void MoveToAlternateArea(int index);

		/// <summary>
		///  Gets an array of all slots in this window. Suitable for sending to clients over the network.
		/// </summary>
		ItemStack[] GetSlots();

		void SetSlots(ItemStack[] slots);

		/// <summary>
		///  Adds the specified item stack to this window, merging with established slots as neccessary.
		/// </summary>
		bool PickUpStack(ItemStack slot);

		/// <summary>
		///  Copy the contents of this window back into an inventory window after changes have been made.
		/// </summary>
		void CopyToInventory(IWindow inventoryWindow);
	}
}