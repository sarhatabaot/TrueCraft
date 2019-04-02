using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
	public class LogCommand : Command
	{
		public LogCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "log";

		public override string Description => "Toggles client logging.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 0)
			{
				Help(client, alias, arguments);
				return;
			}

			client.EnableLogging = !client.EnableLogging;
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/pos: Toggles client logging.");
		}
	}
}