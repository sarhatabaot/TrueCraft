using TrueCraft.Networking;

namespace TrueCraft.Server.Commands
{
	public abstract class Command : ICommand
	{
		protected readonly CommandManager Commands;

		protected Command(CommandManager commands) => Commands = commands;

		public abstract string Name { get; }

		public abstract string Description { get; }

		public virtual string[] Aliases => new string[0];

		public virtual void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			Help(client, alias, arguments);
		}

		public virtual void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("Command \"" + alias + "\" is not functional!");
		}
	}
}