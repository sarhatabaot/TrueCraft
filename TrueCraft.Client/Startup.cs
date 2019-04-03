﻿using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace TrueCraft.Client
{
	public static class Bootstrap
	{
		public static void Start(params string[] args)
		{
			var username = args.Length == 0 ? "" : args[0];
			var endpoint = args.Length < 2 ? "127.0.0.1" : args[1];

			UserSettings.Local = new UserSettings();
			UserSettings.Local.Load();

			var user = new TrueCraftUser { Username = username };
			var client = new MultiplayerClient(user);
			var game = new TrueCraftGame(client, ParseEndPoint(endpoint));
			game.Run();
			client.Disconnect();
		}

		private static IPEndPoint ParseEndPoint(string arg)
		{
			IPAddress address;
			int port;
			if (arg.Contains(':'))
			{
				// Both IP and port are specified
				var parts = arg.Split(':');
				if (!IPAddress.TryParse(parts[0], out address))
					address = Resolve(parts[0]);
				return new IPEndPoint(address, int.Parse(parts[1]));
			}

			if (IPAddress.TryParse(arg, out address))
				return new IPEndPoint(address, 25565);
			if (int.TryParse(arg, out port))
				return new IPEndPoint(IPAddress.Loopback, port);
			return new IPEndPoint(Resolve(arg), 25565);
		}

		private static IPAddress Resolve(string arg)
		{
			return Dns.GetHostEntry(arg).AddressList
				.FirstOrDefault(item => item.AddressFamily == AddressFamily.InterNetwork);
		}
	}
}
