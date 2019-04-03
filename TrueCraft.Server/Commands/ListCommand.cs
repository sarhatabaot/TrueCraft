﻿using System.Text;
using TrueCraft.Networking;

namespace TrueCraft.Server.Commands
{
	public class ListCommand : Command
	{
		public ListCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "list";

		public override string Description => "Lists online players";

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			var listMessage = new StringBuilder("Currently connected players: ");
			foreach (var c in client.Server.Clients)
			{
				if (listMessage.Length + c.Username.Length + 2 >= 120)
				{
					client.SendMessage(listMessage.ToString());
					listMessage.Clear();
				}

				listMessage.AppendFormat("{0}, ", c.Username);
			}

			listMessage.Remove(listMessage.Length - 2, 2);
			client.SendMessage(listMessage.ToString());
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("Correct usage is /" + alias);
		}
	}
}