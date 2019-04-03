using TrueCraft.Entities;
using TrueCraft.Networking;

namespace TrueCraft.Server.Commands
{
	public class DestroyCommand : Command
	{
		public DestroyCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "destroy";

		public override string Description => "Destroys a mob. Violently.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 1)
			{
				Help(client, alias, arguments);
				return;
			}

			int id;
			if (!int.TryParse(arguments[0], out id))
			{
				Help(client, alias, arguments);
				return;
			}

			var manager = client.Server.GetEntityManagerForWorld(client.World);
			var entity = manager.GetEntityByID(id) as MobEntity;
			if (entity == null)
			{
				client.SendMessage(ChatColor.Red + "An entity with that ID does not exist in this world.");
				return;
			}

			manager.DespawnEntity(entity);
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/destroy [id]: " + Description);
		}
	}
}