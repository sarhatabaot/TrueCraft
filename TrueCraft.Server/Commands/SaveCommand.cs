using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
	public class SaveCommand : Command
	{
		public SaveCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "save";

		public override string Description => "Saves the world!";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 0)
			{
				Help(client, alias, arguments);
				return;
			}

			client.World.Save();
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/save: Saves the world!");
		}
	}
}