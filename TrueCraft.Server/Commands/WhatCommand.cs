using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
	public class WhatCommand : Command
	{
		public WhatCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "what";

		public override string Description => "Tells you what you're holding.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 0)
			{
				Help(client, alias, arguments);
				return;
			}

			client.SendMessage(client.SelectedItem.ToString());
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/what: Tells you what you're holding.");
		}
	}
}