using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;

namespace TrueCraft.Commands
{
	public class CommandManager : ICommandManager
	{
		public CommandManager()
		{
			Commands = new List<ICommand>();
			LoadCommands();
		}

		public IList<ICommand> Commands { get; set; }

		/// <summary>
		///  Tries to find the specified command by first performing a
		///  case-insensitive search on the command names, then a
		///  case-sensitive search on the aliases.
		/// </summary>
		/// <param name="client">Client which called the command</param>
		/// <param name="alias">Case-insensitive name or case-sensitive alias of the command</param>
		/// <param name="arguments"></param>
		public void HandleCommand(IRemoteClient client, string alias, string[] arguments)
		{
			var foundCommand = FindByName(alias) ?? FindByAlias(alias);
			if (foundCommand == null)
			{
				client.SendMessage("Invalid command \"" + alias + "\".");
				return;
			}

			foundCommand.Handle(client, alias, arguments);
		}

		private void LoadCommands()
		{
			var truecraftAssembly = Assembly.GetExecutingAssembly();

			var types = truecraftAssembly.GetTypes()
				.Where(t => typeof(ICommand).IsAssignableFrom(t))
				.Where(t => !t.IsDefined(typeof(DoNotAutoLoadAttribute), true))
				.Where(t => !t.IsAbstract);

			foreach (var command in types.Select(type => (ICommand) Activator.CreateInstance(type, this)))
				Commands.Add(command);
		}

		public ICommand FindByName(string name)
		{
			return Commands.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public ICommand FindByAlias(string alias)
		{
			// uncomment below if alias searching should be case-insensitive
			return Commands.FirstOrDefault(c => c.Aliases.Contains(alias /*, StringComparer.OrdinalIgnoreCase*/));
		}
	}
}