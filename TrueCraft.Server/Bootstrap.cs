using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using TrueCraft.Profiling;
using TrueCraft.Server.Commands;
using TrueCraft.TerrainGen;
using TrueCraft.World;

namespace TrueCraft.Server
{
	public class Bootstrap
	{
		public static ServerConfiguration ServerConfiguration;
		public static CommandManager CommandManager;
		public static MultiPlayerServer Server;

		public static void Start(params string[] args)
		{
			string baseDirectory = "";
			if (args.Length == 1)
			{
				baseDirectory = args[0];
				Directory.CreateDirectory(baseDirectory);
			}

			ServerConfiguration =
				Configuration.LoadConfiguration<ServerConfiguration>(Path.Combine(baseDirectory, "config.yaml"))
				?? new ServerConfiguration();

			Server = new MultiPlayerServer(ServerConfiguration);

			var buckets = ServerConfiguration.Debug?.Profiler?.Buckets?.Split(',');
			if (buckets != null)
				foreach (var bucket in buckets)
					Profiler.EnableBucket(bucket.Trim());

			if (ServerConfiguration.Debug != null && ServerConfiguration.Debug.DeleteWorldOnStartup)
				if (Directory.Exists(Path.Combine(baseDirectory, "world")))
					Directory.Delete(Path.Combine(baseDirectory, "world"), true);

			if (ServerConfiguration.Debug != null && ServerConfiguration.Debug.DeletePlayersOnStartup)
				if (Directory.Exists(Path.Combine(baseDirectory, "players")))
					Directory.Delete(Path.Combine(baseDirectory, "players"), true);
			IWorld world;
			try
			{
				world = World.World.LoadWorld(Path.Combine(baseDirectory, "world"));
				Server.AddWorld(world);
			}
			catch
			{
				world = new World.World(Path.Combine(baseDirectory, "default"), new StandardGenerator())
				{
					BlockRepository = Server.BlockRepository
				};
				world.Save(Path.Combine(baseDirectory, "world"));
				Server.AddWorld(world);

				Server.Trace.TraceEvent(TraceEventType.Information, 0, "Generating world around spawn point...");
				for (var x = -5; x < 5; x++)
				{
					for (var z = -5; z < 5; z++)
						world.GetChunk(new Coordinates2D(x, z));
					var progress = (int) ((x + 5) / 10.0 * 100);
					if (progress % 10 == 0)
					{
						Server.Trace.TraceEvent(TraceEventType.Information, 0, "Generating world around spawn point...");
						Server.Trace.TraceEvent(TraceEventType.Information, 0, "{0}% complete", progress + 10);
					}
				}

				Server.Trace.TraceEvent(TraceEventType.Information, 0, "Simulating the world for a moment...");
				for (var x = -5; x < 5; x++)
				{
					for (var z = -5; z < 5; z++)
					{
						var chunk = world.GetChunk(new Coordinates2D(x, z));
						for (byte w = 0; w < Chunk.Width; w++)
							for (byte d = 0; d < Chunk.Depth; d++)
								for (int y = 0; y < chunk.GetHeight(w, d); y++)
								{
									var coords = new Coordinates3D(x + w, y, z + d);
									var data = world.GetBlockData(coords);
									var provider = world.BlockRepository.GetBlockProvider(data.ID);
									provider.BlockUpdate(data, data, Server, world);
								}
					}

					var progress = (int) ((x + 5) / 10.0 * 100);
					if (progress % 10 == 0)
					{
						Server.Trace.TraceEvent(TraceEventType.Information, 0, "{0}% complete", progress + 10);
					}
				}

				Server.Trace.TraceEvent(TraceEventType.Information, 0, "Lighting the world (this will take a moment)...");
				foreach (var lighter in Server.WorldLighters)
					while (lighter.TryLightNext())
					{ }
			}

			world.Save();
			CommandManager = new CommandManager();
			Server.ChatMessageReceived += HandleChatMessageReceived;
			Server.Start(new IPEndPoint(IPAddress.Parse(ServerConfiguration.ServerAddress),
				ServerConfiguration.ServerPort));
			Console.CancelKeyPress += HandleCancelKeyPress;
			Server.Scheduler.ScheduleEvent(Path.Combine(baseDirectory, "world.save"), null,
				TimeSpan.FromSeconds(ServerConfiguration.WorldSaveInterval), SaveWorlds);
			while (true)
				Thread.Yield();
		}

		private static void SaveWorlds(IMultiPlayerServer server)
		{
			Server.Trace.TraceEvent(TraceEventType.Information, 0, "Saving world...");
			foreach (var w in Server.Worlds)
				w.Save();
			Server.Trace.TraceEvent(TraceEventType.Information, 0, "Done.");
			server.Scheduler.ScheduleEvent("world.save", null,
				TimeSpan.FromSeconds(ServerConfiguration.WorldSaveInterval), SaveWorlds);
		}

		private static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Server.Stop();
		}

		private static void HandleChatMessageReceived(object sender, ChatMessageEventArgs e)
		{
			var message = e.Message;

			if (!message.StartsWith("/") || message.StartsWith("//"))
				SendChatMessage(e.Client.Username, message);
			else
				e.PreventDefault = ProcessChatCommand(e);
		}

		private static void SendChatMessage(string username, string message)
		{
			if (message.StartsWith("//"))
				message = message.Substring(1);

			Server.SendMessage("<{0}> {1}", username, message);
		}

		/// <summary>
		///  Parse sent message as chat command
		/// </summary>
		/// <param name="e"></param>
		/// <returns>true if the command was successfully executed</returns>
		private static bool ProcessChatCommand(ChatMessageEventArgs e)
		{
			var commandWithoutSlash = e.Message.TrimStart('/');
			var messageArray = commandWithoutSlash
				.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (messageArray.Length <= 0)
				return false; // command not found

			var alias = messageArray[0];
			var trimmedMessageArray = new string[messageArray.Length - 1];
			if (trimmedMessageArray.Length != 0)
				Array.Copy(messageArray, 1, trimmedMessageArray, 0, messageArray.Length - 1);

			CommandManager.HandleCommand(e.Client, alias, trimmedMessageArray);

			return true;
		}
	}
}
