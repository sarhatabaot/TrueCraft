using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
	public class PositionCommand : Command
	{
		public PositionCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "pos";

		public override string Description => "Shows your position.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 0)
			{
				Help(client, alias, arguments);
				return;
			}

			client.SendMessage(client.Entity.Position.ToString());
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/pos: Shows your position.");
		}
	}
}