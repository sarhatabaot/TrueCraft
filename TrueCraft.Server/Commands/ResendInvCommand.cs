using TrueCraft.Networking;
using TrueCraft.Networking.Packets;

namespace TrueCraft.Server.Commands
{
	public class ResendInvCommand : Command
	{
		public ResendInvCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "reinv";

		public override string Description => "Resends your inventory to the selected client.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 0)
			{
				Help(client, alias, arguments);
				return;
			}

			client.QueuePacket(new WindowItemsPacket(0, client.Inventory.GetSlots()));
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/reinv: Resends your inventory.");
		}
	}
}