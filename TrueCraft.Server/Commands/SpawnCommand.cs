using System;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.API;
using TrueCraft.API.Entities;
using TrueCraft.API.Networking;
using TrueCraft.Core;

namespace TrueCraft.Commands
{
	public class SpawnCommand : Command
	{
		public SpawnCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "spawn";

		public override string Description => "Spawns a mob.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 1)
			{
				Help(client, alias, arguments);
				return;
			}

			var entityTypes = new List<Type>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			foreach (var t in assembly.GetTypes())
				if (typeof(IEntity).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
					entityTypes.Add(t);

			arguments[0] = arguments[0].ToUpper();
			var type = entityTypes.SingleOrDefault(t => t.Name.ToUpper() == arguments[0] + "ENTITY");
			if (type == null)
			{
				client.SendMessage(ChatColor.Red + "Unknown entity type.");
				return;
			}

			var entity = (IEntity) Activator.CreateInstance(type);
			var em = client.Server.GetEntityManagerForWorld(client.World);
			entity.Position = client.Entity.Position + MathHelper.FowardVector(client.Entity.Yaw) * 3;
			em.SpawnEntity(entity);
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/spawn [type]: Spawns a mob of that type.");
		}
	}
}