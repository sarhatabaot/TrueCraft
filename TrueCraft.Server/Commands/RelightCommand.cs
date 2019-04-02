using System.Linq;
using TrueCraft.API;
using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
	public class RelightCommand : Command
	{
		public RelightCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "relight";

		public override string Description => "Relights the chunk you're standing in.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			if (arguments.Length != 0)
			{
				Help(client, alias, arguments);
				return;
			}

			var server = client.Server as MultiplayerServer;
			var chunk = client.World.FindChunk((Coordinates3D) client.Entity.Position);
			var lighter = server.WorldLighters.SingleOrDefault(l => l.World == client.World);
			if (lighter != null)
			{
				lighter.InitialLighting(chunk, true);
				(client as RemoteClient).UnloadChunk(chunk.Coordinates);
				(client as RemoteClient).LoadChunk(chunk);
			}
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/reinv: Resends your inventory.");
		}
	}
}