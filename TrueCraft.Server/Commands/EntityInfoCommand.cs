using System.Linq;
using TrueCraft.Entities;
using TrueCraft.Networking;

namespace TrueCraft.Server.Commands
{
	public class EntityInfoCommand : Command
	{
		public EntityInfoCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "entity";

		public override string Description => "Provides information about an entity Id.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 1)
			{
				Help(client, alias, arguments);
				return;
			}

			int Id;
			if (!int.TryParse(arguments[0], out Id))
			{
				Help(client, alias, arguments);
				return;
			}

			var manager = client.Server.GetEntityManagerForWorld(client.World);
			var entity = manager.GetEntityById(Id);
			if (entity == null)
			{
				client.SendMessage(ChatColor.Red + "An entity with that Id does not exist in this world.");
				return;
			}

			client.SendMessage(string.Format(
				"{0} {1}", entity.GetType().Name, entity.Position));
			if (entity is MobEntity)
			{
				var mob = entity as MobEntity;
				client.SendMessage(string.Format(
					"{0}/{1} HP, {2} State, moving to to {3}",
					mob.Health, mob.MaxHealth,
					mob.CurrentState?.GetType().Name ?? "null",
					mob.CurrentPath?.Waypoints.Last().ToString() ?? "null"));
			}
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/entity [Id]: Shows information about this entity.");
		}
	}
}