using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Windows;

namespace TrueCraft.Client.Handlers
{
	internal static class InventoryHandlers
	{
		public static void HandleWindowItems(IPacket _packet, MultiPlayerClient client)
		{
			var packet = (WindowItemsPacket) _packet;
			if (packet.WindowID == 0)
				client.Inventory.SetSlots(packet.Items);
			else
				client.CurrentWindow.SetSlots(packet.Items);
		}

		public static void HandleSetSlot(IPacket _packet, MultiPlayerClient client)
		{
			var packet = (SetSlotPacket) _packet;
			IWindow window = null;
			if (packet.WindowID == 0)
				window = client.Inventory;
			else
				window = client.CurrentWindow;
			if (window != null)
				if (packet.SlotIndex >= 0 && packet.SlotIndex < window.Length)
					window[packet.SlotIndex] = new ItemStack(packet.ItemID, packet.Count, packet.Metadata);
		}

		public static void HandleOpenWindowPacket(IPacket _packet, MultiPlayerClient client)
		{
			var packet = (OpenWindowPacket) _packet;
			IWindow window = null;
			switch (packet.Type)
			{
				case 1: // Crafting bench window
					window = new CraftingBenchWindow(client.CraftingRepository, client.Inventory);
					break;
			}

			window.Id = packet.WindowID;
			client.CurrentWindow = window;
		}

		public static void HandleCloseWindowPacket(IPacket _packet, MultiPlayerClient client)
		{
			client.CurrentWindow = null;
		}
	}
}