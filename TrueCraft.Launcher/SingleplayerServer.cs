using System.Diagnostics;
using System.Net;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft.Launcher
{
	public class SingleplayerServer
	{
		private readonly ServerConfiguration _configuration;

		public SingleplayerServer(World.World world)
		{
			_configuration = new ServerConfiguration
			{
				MOTD = null,
				Singleplayer = true
			};

			World = world;
			Server = new MultiplayerServer(_configuration);

			world.BlockRepository = Server.BlockRepository;
			Server.AddWorld(world);
		}

		public MultiplayerServer Server { get; set; }
		public World.World World { get; set; }

		public void Initialize(ProgressNotification progressNotification = null)
		{
			Server.Trace.TraceEvent(TraceEventType.Information, 0, "Generating world around spawn point...");
			for (var x = -5; x < 5; x++)
			{
				for (var z = -5; z < 5; z++)
					World.GetChunk(new Coordinates2D(x, z));
				var progress = (int) ((x + 5) / 10.0 * 100);
				progressNotification?.Invoke(progress / 100.0, "Generating world...");
				if (progress % 10 == 0)
					Server.Trace.TraceEvent(TraceEventType.Information, 0, "{0}% complete", progress + 10);
			}

			Server.Trace.TraceEvent(TraceEventType.Information, 0, "Simulating the world for a moment...");
			for (var x = -5; x < 5; x++)
			{
				for (var z = -5; z < 5; z++)
				{
					var chunk = World.GetChunk(new Coordinates2D(x, z));
					for (byte _x = 0; _x < Chunk.Width; _x++)
					for (byte _z = 0; _z < Chunk.Depth; _z++)
					for (var _y = 0; _y < chunk.GetHeight(_x, _z); _y++)
					{
						var coords = new Coordinates3D(x + _x, _y, z + _z);
						var data = World.GetBlockData(coords);
						var provider = World.BlockRepository.GetBlockProvider(data.ID);
						provider.BlockUpdate(data, data, Server, World);
					}
				}

				var progress = (int) ((x + 5) / 10.0 * 100);
				progressNotification?.Invoke(progress / 100.0, "Simulating world...");
				if (progress % 10 == 0)
					Server.Trace.TraceEvent(TraceEventType.Information, 0, "{0}% complete", progress + 10);
			}

			World.Save();
		}

		public void Start()
		{
			Server.Start(new IPEndPoint(IPAddress.Loopback, 0));
		}

		public void Stop()
		{
			Server.Stop();
		}

		public delegate void ProgressNotification(double progress, string stage);
	}
}