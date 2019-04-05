using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TrueCraft.Logic;
using TrueCraft.Serialization;
using TrueCraft.Serialization.Tags;

namespace TrueCraft.World
{
	public class World : IDisposable, IWorld
	{
		public static readonly int Height = 128;
		private int _seed;
		private Coordinates3D? _spawnPoint;

		private readonly Dictionary<Thread, IChunk> _chunkCache = new Dictionary<Thread, IChunk>();
		private readonly object _chunkCacheLock = new object();

		public World()
		{
			Regions = new Dictionary<Coordinates2D, IRegion>();
			BaseTime = DateTime.UtcNow;
		}

		public World(string name) : this()
		{
			Name = name;
			Seed = MathHelper.Random.Next();
		}

		public World(string name, IChunkProvider chunkProvider) : this(name)
		{
			ChunkProvider = chunkProvider;
			ChunkProvider.Initialize(this);
		}

		public World(string name, int seed, IChunkProvider chunkProvider) : this(name, chunkProvider) => Seed = seed;

		public string BaseDirectory { get; internal set; }
		public IDictionary<Coordinates2D, IRegion> Regions { get; set; }
		public DateTime BaseTime { get; set; }

		public void Dispose()
		{
			foreach (var region in Regions)
				region.Value.Dispose();
			BlockChanged = null;
			ChunkGenerated = null;
		}

		public string Name { get; set; }

		public int Seed
		{
			get => _seed;
			set
			{
				_seed = value;
				BiomeDiagram = new BiomeMap(_seed);
			}
		}

		public Coordinates3D SpawnPoint
		{
			get
			{
				if (_spawnPoint == null)
					_spawnPoint = ChunkProvider.GetSpawn(this);
				return _spawnPoint.Value;
			}
			set => _spawnPoint = value;
		}

		public IBiomeMap BiomeDiagram { get; set; }
		public IChunkProvider ChunkProvider { get; set; }
		public IBlockRepository BlockRepository { get; set; }

		public long Time
		{
			get => (long) ((DateTime.UtcNow - BaseTime).TotalSeconds * 20) % 24000;
			set => BaseTime = DateTime.UtcNow.AddSeconds(-value / 20);
		}

		public event EventHandler<BlockChangeEventArgs> BlockChanged;
		public event EventHandler<ChunkLoadedEventArgs> ChunkGenerated;
		public event EventHandler<ChunkLoadedEventArgs> ChunkLoaded;

		/// <summary>
		///  Finds a chunk that contains the specified block coordinates.
		/// </summary>
		public IChunk FindChunk(Coordinates3D coordinates, bool generate = true)
		{
			IChunk chunk;
			FindBlockPosition(coordinates, out chunk, generate);
			return chunk;
		}

		public IChunk GetChunk(Coordinates2D coordinates, bool generate = true)
		{
			var regionX = coordinates.X / Region.Width - (coordinates.X < 0 ? 1 : 0);
			var regionZ = coordinates.Z / Region.Depth - (coordinates.Z < 0 ? 1 : 0);

			var region = LoadOrGenerateRegion(new Coordinates2D(regionX, regionZ), generate);
			return region?.GetChunk(new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32), generate);
		}

		public byte GetBlockId(Coordinates3D coordinates)
		{
			IChunk chunk;
			coordinates = FindBlockPosition(coordinates, out chunk);
			return chunk.GetBlockID(coordinates);
		}

		public bool TryGetBlockId(Coordinates3D coordinates, out byte id)
		{
			if (!FindBlockPosition(coordinates, out var chunk, out var position))
			{
				id = default;
				return false;
			}
			id = chunk.GetBlockID(position);
			return true;
		}

		public byte GetMetadata(Coordinates3D coordinates)
		{
			coordinates = FindBlockPosition(coordinates, out var chunk);
			return chunk.GetMetadata(coordinates);
		}

		public byte GetSkyLight(Coordinates3D coordinates)
		{
			coordinates = FindBlockPosition(coordinates, out var chunk);
			return chunk.GetSkyLight(coordinates);
		}

		public byte GetBlockLight(Coordinates3D coordinates)
		{
			coordinates = FindBlockPosition(coordinates, out var chunk);
			return chunk.GetBlockLight(coordinates);
		}

		public NbtCompound GetTileEntity(Coordinates3D coordinates)
		{
			coordinates = FindBlockPosition(coordinates, out var chunk);
			return chunk.GetTileEntity(coordinates);
		}

		public BlockDescriptor GetBlockData(Coordinates3D coordinates)
		{
			var adjustedCoordinates = FindBlockPosition(coordinates, out var chunk);
			return GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
		}

		public void SetBlockData(Coordinates3D coordinates, BlockDescriptor descriptor)
		{
			var adjustedCoordinates = FindBlockPosition(coordinates, out var chunk);
			var old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
			chunk.SetBlockID(adjustedCoordinates, descriptor.ID);
			chunk.SetMetadata(adjustedCoordinates, descriptor.Metadata);

			BlockChanged?.Invoke(this,
				new BlockChangeEventArgs(coordinates, old,
					GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)));
		}

		public void SetBlockId(Coordinates3D coordinates, byte value)
		{
			var adjustedCoordinates = FindBlockPosition(coordinates, out var chunk);
			var old = new BlockDescriptor();
			if (BlockChanged != null)
				old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
			chunk.SetBlockID(adjustedCoordinates, value);

			BlockChanged?.Invoke(this,
				new BlockChangeEventArgs(coordinates, old,
					GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)));
		}

		public void SetMetadata(Coordinates3D coordinates, byte value)
		{
			var adjustedCoordinates = FindBlockPosition(coordinates, out var chunk);
			var old = new BlockDescriptor();
			if (BlockChanged != null)
				old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
			chunk.SetMetadata(adjustedCoordinates, value);

			BlockChanged?.Invoke(this,
				new BlockChangeEventArgs(coordinates, old,
					GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)));
		}

		public void SetSkyLight(Coordinates3D coordinates, byte value)
		{
			var adjustedCoordinates = FindBlockPosition(coordinates, out var chunk);
			var old = new BlockDescriptor();
			if (BlockChanged != null)
				old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
			chunk.SetSkyLight(adjustedCoordinates, value);

			BlockChanged?.Invoke(this,
				new BlockChangeEventArgs(coordinates, old,
					GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)));
		}

		public void SetBlockLight(Coordinates3D coordinates, byte value)
		{
			var adjustedCoordinates = FindBlockPosition(coordinates, out var chunk);
			var old = new BlockDescriptor();
			if (BlockChanged != null)
				old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
			chunk.SetBlockLight(adjustedCoordinates, value);

			BlockChanged?.Invoke(this,
				new BlockChangeEventArgs(coordinates, old,
					GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)));
		}

		public void SetTileEntity(Coordinates3D coordinates, NbtCompound value)
		{
			coordinates = FindBlockPosition(coordinates, out var chunk);
			chunk.SetTileEntity(coordinates, value);
		}

		public void Save()
		{
			lock (Regions)
				foreach (var region in Regions)
					region.Value.Save(Path.Combine(BaseDirectory, Region.GetRegionFileName(region.Key)));
			var file = new NbtFile();
			file.RootTag.Add(new NbtCompound("SpawnPoint", new[]
			{
				new NbtInt("X", SpawnPoint.X),
				new NbtInt("Y", SpawnPoint.Y),
				new NbtInt("Z", SpawnPoint.Z)
			}));
			file.RootTag.Add(new NbtInt("Seed", Seed));
			file.RootTag.Add(new NbtString("ChunkProvider", ChunkProvider.GetType().FullName));
			file.RootTag.Add(new NbtString("Name", Name));
			file.SaveToFile(Path.Combine(BaseDirectory, "manifest.nbt"), NbtCompression.ZLib);
		}

		public void Save(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			BaseDirectory = path;
			Save();
		}

		public Coordinates3D FindBlockPosition(Coordinates3D coordinates, out IChunk chunk, bool generate = true)
		{
			if (coordinates.Y < 0 || coordinates.Y >= Chunk.Height)
				throw new ArgumentOutOfRangeException(nameof(coordinates), "Coordinates are out of range");

			FindBlockPosition(coordinates, out chunk, out var position, generate);
			return position;
		}

		public bool FindBlockPosition(Coordinates3D coordinates, out IChunk chunk, out Coordinates3D position, bool generate = true)
		{
			if (coordinates.Y < 0 || coordinates.Y >= Chunk.Height)
			{
				chunk = default;
				position = default;
				return false;
			}

			var chunkX = coordinates.X / Chunk.Width;
			var chunkZ = coordinates.Z / Chunk.Depth;

			if (coordinates.X < 0)
				chunkX = (coordinates.X + 1) / Chunk.Width - 1;
			if (coordinates.Z < 0)
				chunkZ = (coordinates.Z + 1) / Chunk.Depth - 1;

			lock (_chunkCacheLock)
			{
				if (_chunkCache.ContainsKey(Thread.CurrentThread))
				{
					var cache = _chunkCache[Thread.CurrentThread];
					if (cache != null && chunkX == cache.Coordinates.X && chunkZ == cache.Coordinates.Z)
						chunk = cache;
					else
					{
						cache = GetChunk(new Coordinates2D(chunkX, chunkZ), generate);
						_chunkCache[Thread.CurrentThread] = cache;
					}
				}
				else
				{
					var cache = GetChunk(new Coordinates2D(chunkX, chunkZ), generate);
					_chunkCache[Thread.CurrentThread] = cache;
				}
			}

			chunk = GetChunk(new Coordinates2D(chunkX, chunkZ), generate);
			position = new Coordinates3D(
				(coordinates.X % Chunk.Width + Chunk.Width) % Chunk.Width,
				coordinates.Y,
				(coordinates.Z % Chunk.Depth + Chunk.Depth) % Chunk.Depth);
			return true;
		}

		public bool IsValidPosition(Coordinates3D position)
		{
			return position.Y >= 0 && position.Y < Chunk.Height;
		}

		public IEnumerator<IChunk> GetEnumerator()
		{
			return new ChunkEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static World LoadWorld(string baseDirectory)
		{
			if (!Directory.Exists(baseDirectory))
				throw new DirectoryNotFoundException();

			var world = new World(Path.GetFileName(baseDirectory))
			{
				BaseDirectory = baseDirectory
			};

			if (File.Exists(Path.Combine(baseDirectory, "manifest.nbt")))
			{
				var file = new NbtFile(Path.Combine(baseDirectory, "manifest.nbt"));
				world.SpawnPoint = new Coordinates3D(file.RootTag["SpawnPoint"]["X"].IntValue,
					file.RootTag["SpawnPoint"]["Y"].IntValue,
					file.RootTag["SpawnPoint"]["Z"].IntValue);
				world.Seed = file.RootTag["Seed"].IntValue;
				var providerName = file.RootTag["ChunkProvider"].StringValue;
				var provider = (IChunkProvider) Activator.CreateInstance(Type.GetType(providerName) ?? throw new InvalidOperationException($"invalid chunk provider '{providerName}'"));
				provider.Initialize(world);
				if (file.RootTag.Contains("Name"))
					world.Name = file.RootTag["Name"].StringValue;
				world.ChunkProvider = provider;
			}

			return world;
		}

		public void GenerateChunk(Coordinates2D coordinates)
		{
			var regionX = coordinates.X / Region.Width - (coordinates.X < 0 ? 1 : 0);
			var regionZ = coordinates.Z / Region.Depth - (coordinates.Z < 0 ? 1 : 0);

			var region = LoadOrGenerateRegion(new Coordinates2D(regionX, regionZ));
			region.GenerateChunk(new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32));
		}

		public void SetChunk(Coordinates2D coordinates, Chunk chunk)
		{
			var regionX = coordinates.X / Region.Width - (coordinates.X < 0 ? 1 : 0);
			var regionZ = coordinates.Z / Region.Depth - (coordinates.Z < 0 ? 1 : 0);

			var region = LoadOrGenerateRegion(new Coordinates2D(regionX, regionZ));
			lock (region)
			{
				chunk.IsModified = true;
				region.SetChunk(new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32), chunk);
			}
		}

		public void UnloadRegion(Coordinates2D coordinates)
		{
			lock (Regions)
			{
				Regions[coordinates].Save(Path.Combine(BaseDirectory, Region.GetRegionFileName(coordinates)));
				Regions.Remove(coordinates);
			}
		}

		public void UnloadChunk(Coordinates2D coordinates)
		{
			var regionX = coordinates.X / Region.Width - (coordinates.X < 0 ? 1 : 0);
			var regionZ = coordinates.Z / Region.Depth - (coordinates.Z < 0 ? 1 : 0);

			var regionPosition = new Coordinates2D(regionX, regionZ);
			if (!Regions.ContainsKey(regionPosition))
				throw new ArgumentOutOfRangeException(nameof(coordinates));

			Regions[regionPosition]
				.UnloadChunk(new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32));
		}

		private BlockDescriptor GetBlockDataFromChunk(Coordinates3D adjustedCoordinates, IChunk chunk,
			Coordinates3D coordinates)
		{
			return new BlockDescriptor
			{
				ID = chunk.GetBlockID(adjustedCoordinates),
				Metadata = chunk.GetMetadata(adjustedCoordinates),
				BlockLight = chunk.GetBlockLight(adjustedCoordinates),
				SkyLight = chunk.GetSkyLight(adjustedCoordinates),
				Coordinates = coordinates
			};
		}

		private Region LoadOrGenerateRegion(Coordinates2D coordinates, bool generate = true)
		{
			if (Regions.ContainsKey(coordinates))
				return (Region) Regions[coordinates];
			if (!generate)
				return null;
			Region region;
			if (BaseDirectory != null)
			{
				var file = Path.Combine(BaseDirectory, Region.GetRegionFileName(coordinates));

				region = File.Exists(file) ? new Region(coordinates, this, file) : new Region(coordinates, this);
			}
			else
				region = new Region(coordinates, this);

			lock (Regions)
				Regions[coordinates] = region;
			return region;
		}

		protected internal void OnChunkGenerated(ChunkLoadedEventArgs e)
		{
			ChunkGenerated?.Invoke(this, e);
		}

		protected internal void OnChunkLoaded(ChunkLoadedEventArgs e)
		{
			ChunkLoaded?.Invoke(this, e);
		}

		public class ChunkEnumerator : IEnumerator<IChunk>
		{
			public ChunkEnumerator(World world)
			{
				World = world;
				Index = -1;
				var regions = world.Regions.Values.ToList();
				var chunks = new List<IChunk>();
				foreach (var region in regions)
					chunks.AddRange(region.Chunks.Values);
				Chunks = chunks;
			}

			public World World { get; set; }
			private int Index { get; set; }
			private IList<IChunk> Chunks { get; }

			public bool MoveNext()
			{
				Index++;
				return Index < Chunks.Count;
			}

			public void Reset()
			{
				Index = -1;
			}

			public void Dispose()
			{
			}

			public IChunk Current => Chunks[Index];

			object IEnumerator.Current => Current;
		}
	}
}