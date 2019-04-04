using TrueCraft.World;

namespace TrueCraft.Client
{
	public class ReadOnlyWorld
	{
		internal ReadOnlyWorld()
		{
			World = new World.World("default");
			UnloadChunks = true;
		}

		public long Time => World.Time;

		public byte GetBlockId(Coordinates3D coordinates)
		{
			return World.GetBlockId(coordinates);
		}
		
		public byte GetMetadata(Coordinates3D coordinates)
		{
			return World.GetMetadata(coordinates);
		}

		public byte GetSkyLight(Coordinates3D coordinates)
		{
			return World.GetSkyLight(coordinates);
		}

		internal IChunk FindChunk(Coordinates2D coordinates)
		{
			try
			{
				return World.FindChunk(new Coordinates3D(coordinates.X, 0, coordinates.Z));
			}
			catch
			{
				return null;
			}
		}

		public ReadOnlyChunk GetChunk(Coordinates2D coordinates)
		{
			return new ReadOnlyChunk(World.GetChunk(coordinates));
		}

		public bool IsValidPosition(Coordinates3D coords)
		{
			return World.IsValidPosition(coords);
		}


		internal void SetBlockId(Coordinates3D coordinates, byte value)
		{
			World.SetBlockId(coordinates, value);
		}

		internal void SetMetadata(Coordinates3D coordinates, byte value)
		{
			World.SetMetadata(coordinates, value);
		}

		internal void SetChunk(Coordinates2D coordinates, Chunk chunk)
		{
			World.SetChunk(coordinates, chunk);
		}

		internal void RemoveChunk(Coordinates2D coordinates)
		{
			if (UnloadChunks)
				World.UnloadChunk(coordinates);
		}

		private bool UnloadChunks { get; }

		internal World.World World { get; set; }
	}
}