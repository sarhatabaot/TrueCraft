using System.Linq;
using System.Threading.Tasks;
using TrueCraft.AI;
using TrueCraft.Entities;
using TrueCraft.Networking;

namespace TrueCraft.Server.Commands
{
	public class ToMeCommand : Command
	{
		public ToMeCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "tome";

		public override string Description => "Moves a mob towards your position.";

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
			var entity = manager.GetEntityById(Id) as MobEntity;
			if (entity == null)
			{
				client.SendMessage(ChatColor.Red + "An entity with that Id does not exist in this world.");
				return;
			}

			Task.Factory.StartNew(() =>
			{
				var astar = new AStarPathFinder();
				var path = astar.FindPath(client.World, entity.BoundingBox, (Coordinates3D) entity.Position,
					(Coordinates3D) client.Entity.Position);
				if (path == null)
					client.SendMessage(ChatColor.Red + "It is impossible for this entity to reach you.");
				else
				{
					client.SendMessage(string.Format(ChatColor.Blue
					                                 + "Executing path with {0} waypoints", path.Waypoints.Count()));
					entity.CurrentPath = path;
				}
			});
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/tome [Id]: Moves a mob to your position.");
		}
	}
}