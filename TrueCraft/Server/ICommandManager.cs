using System.Collections.Generic;
using TrueCraft.Networking;

namespace TrueCraft.Server
{
	public interface ICommandManager
	{
		IList<ICommand> Commands { get; }
		void HandleCommand(IRemoteClient Client, string Alias, string[] Arguments);
	}
}