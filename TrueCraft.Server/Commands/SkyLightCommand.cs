using Microsoft.Xna.Framework;
using TrueCraft.Networking;

namespace TrueCraft.Server.Commands
{
	public class SkyLightCommand : Command
	{
		public SkyLightCommand(CommandManager commands) : base(commands)
		{
		}

		public override string Name => "sl";

		public override string Description => "Shows sky light at your current position.";

		public override string[] Aliases => new string[0];

		public override void Handle(IRemoteClient client, string alias, string[] arguments)
		{
			var mod = 0;
			if (arguments.Length == 1)
				int.TryParse(arguments[0], out mod);
			client.SendMessage(client.World.GetSkyLight(
				(Coordinates3D) (client.Entity.Position + new Vector3(0, -mod, 0))).ToString());
		}

		public override void Help(IRemoteClient client, string alias, string[] arguments)
		{
			client.SendMessage("/sl");
		}
	}
}