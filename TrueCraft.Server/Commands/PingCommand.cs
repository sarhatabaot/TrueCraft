using TrueCraft.Networking;

namespace TrueCraft.Server.Commands
{
	public class PingCommand : Command
	{
		public PingCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "ping";

		public override string Description => "Ping pong";

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("Pong!");
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("Correct usage is /" + alias);
		}
	}
}