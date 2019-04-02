﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using TrueCraft.API;
using TrueCraft.API.Logging;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.Commands;
using TrueCraft.Core.Logging;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.World;
using TrueCraft.Profiling;

namespace TrueCraft
{
	public class Program
	{
		public static ServerConfiguration ServerConfiguration;

		public static CommandManager CommandManager;

		public static MultiplayerServer Server;

		public static void Main(string[] args)
		{
			ServerConfiguration = Configuration.LoadConfiguration<ServerConfiguration>("config.yaml");

			Server = new MultiplayerServer(ServerConfiguration);

			Server.AddLogProvider(
				new ConsoleLogProvider(LogCategory.Notice | LogCategory.Warning | LogCategory.Error |
				                       LogCategory.Debug));
#if DEBUG
			Server.AddLogProvider(new FileLogProvider(new StreamWriter("packets.log", false), LogCategory.Packets));
#endif

			var buckets = ServerConfiguration.Debug?.Profiler?.Buckets?.Split(',');
			if (buckets != null)
				foreach (var bucket in buckets)
					Profiler.EnableBucket(bucket.Trim());

			if (ServerConfiguration.Debug.DeleteWorldOnStartup)
				if (Directory.Exists("world"))
					Directory.Delete("world", true);
			if (ServerConfiguration.Debug.DeletePlayersOnStartup)
				if (Directory.Exists("players"))
					Directory.Delete("players", true);
			IWorld world;
			try
			{
				world = World.LoadWorld("world");
				Server.AddWorld(world);
			}
			catch
			{
				world = new World("default", new StandardGenerator());
				world.BlockRepository = Server.BlockRepository;
				world.Save("world");
				Server.AddWorld(world);
				Server.Log(LogCategory.Notice, "Generating world around spawn point...");
				for (var x = -5; x < 5; x++)
				{
					for (var z = -5; z < 5; z++)
						world.GetChunk(new Coordinates2D(x, z));
					var progress = (int) ((x + 5) / 10.0 * 100);
					if (progress % 10 == 0)
						Server.Log(LogCategory.Notice, "{0}% complete", progress + 10);
				}

				Server.Log(LogCategory.Notice, "Simulating the world for a moment...");
				for (var x = -5; x < 5; x++)
				{
					for (var z = -5; z < 5; z++)
					{
						var chunk = world.GetChunk(new Coordinates2D(x, z));
						for (byte _x = 0; _x < Chunk.Width; _x++)
						for (byte _z = 0; _z < Chunk.Depth; _z++)
						for (var _y = 0; _y < chunk.GetHeight(_x, _z); _y++)
						{
							var coords = new Coordinates3D(x + _x, _y, z + _z);
							var data = world.GetBlockData(coords);
							var provider = world.BlockRepository.GetBlockProvider(data.ID);
							provider.BlockUpdate(data, data, Server, world);
						}
					}

					var progress = (int) ((x + 5) / 10.0 * 100);
					if (progress % 10 == 0)
						Server.Log(LogCategory.Notice, "{0}% complete", progress + 10);
				}

				Server.Log(LogCategory.Notice, "Lighting the world (this will take a moment)...");
				foreach (var lighter in Server.WorldLighters)
					while (lighter.TryLightNext())
						;
			}

			world.Save();
			CommandManager = new CommandManager();
			Server.ChatMessageReceived += HandleChatMessageReceived;
			Server.Start(new IPEndPoint(IPAddress.Parse(ServerConfiguration.ServerAddress),
				ServerConfiguration.ServerPort));
			Console.CancelKeyPress += HandleCancelKeyPress;
			Server.Scheduler.ScheduleEvent("world.save", null,
				TimeSpan.FromSeconds(ServerConfiguration.WorldSaveInterval), SaveWorlds);
			while (true) Thread.Yield();
		}

		private static void SaveWorlds(IMultiplayerServer server)
		{
			Server.Log(LogCategory.Notice, "Saving world...");
			foreach (var w in Server.Worlds)
				w.Save();
			Server.Log(LogCategory.Notice, "Done.");
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
				.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

			if (messageArray.Length <= 0) return false; // command not found

			var alias = messageArray[0];
			var trimmedMessageArray = new string[messageArray.Length - 1];
			if (trimmedMessageArray.Length != 0)
				Array.Copy(messageArray, 1, trimmedMessageArray, 0, messageArray.Length - 1);

			CommandManager.HandleCommand(e.Client, alias, trimmedMessageArray);

			return true;
		}
	}
}