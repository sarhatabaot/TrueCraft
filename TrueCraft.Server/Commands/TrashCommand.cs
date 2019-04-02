using TrueCraft.API;
using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
	public class TrashCommand : Command
	{
		public TrashCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "trash";

		public override string Description => "Discards selected item, hotbar, or entire inventory.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 0)
			{
				if (arguments[0] == "hotbar")
					for (short i = 36; i <= 44; i++)
						client.Inventory[i] = ItemStack.EmptyStack;
				else if (arguments[0] == "all")
					for (short i = 0; i <= 44; i++)
						client.Inventory[i] = ItemStack.EmptyStack;
				else
				{
					Help(client, alias, arguments);
				}
			}
			else
				client.Inventory[client.SelectedSlot] = ItemStack.EmptyStack;
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("Correct usage is /trash <hotbar/all> or leave blank to clear\nselected slot.");
		}
	}
}