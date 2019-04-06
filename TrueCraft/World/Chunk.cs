using System;
using System.Collections.Generic;
using TrueCraft.Serialization;
using TrueCraft.Serialization.Serialization;
using TrueCraft.Serialization.Tags;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.World
{
	public class Chunk : INbtSerializable, IChunk
	{
		public const int Width = 16, Height = 128, Depth = 16;

		[NbtIgnore] private bool _LightPopulated;

		public Chunk()
		{
			Biomes = new byte[Width * Depth];
			HeightMap = new int[Width * Depth];
			TileEntities = new Dictionary<Coordinates3D, NbtCompound>();
			TerrainPopulated = false;
			LightPopulated = false;
			MaxHeight = 0;
			const int size = Width * Height * Depth;
			const int halfSize = size / 2;
			Data = new byte[size + halfSize * 3];
			Metadata = new NibbleSlice(Data, size, halfSize);
			BlockLight = new NibbleSlice(Data, size + halfSize, halfSize);
			SkyLight = new NibbleSlice(Data, size + halfSize * 2, halfSize);
		}

		public Chunk(Coordinates2D coordinates) : this()
		{
			X = coordinates.X;
			Z = coordinates.Z;
		}

		public long LastUpdate { get; set; }

		public event EventHandler Disposed;

		public void Dispose()
		{
			if (Disposed != null)
				Disposed(this, null);
		}

		[NbtIgnore] public DateTime LastAccessed { get; set; }

		[NbtIgnore] public bool IsModified { get; set; }

		[NbtIgnore] public byte[] Data { get; set; }

		[NbtIgnore] public NibbleSlice Metadata { get; set; }

		[NbtIgnore] public NibbleSlice BlockLight { get; set; }

		[NbtIgnore] public NibbleSlice SkyLight { get; set; }

		public byte[] Biomes { get; set; }
		public int[] HeightMap { get; set; }
		public int MaxHeight { get; private set; }

		[TagName("xPos")] public int X { get; set; }

		[TagName("zPos")] public int Z { get; set; }

		public bool LightPopulated
		{
			get => _LightPopulated;
			set
			{
				_LightPopulated = value;
				IsModified = true;
			}
		}

		public Dictionary<Coordinates3D, NbtCompound> TileEntities { get; set; }

		public Coordinates2D Coordinates
		{
			get => new Coordinates2D(X, Z);
			set
			{
				X = value.X;
				Z = value.Z;
			}
		}

		public bool TerrainPopulated { get; set; }

		[NbtIgnore] public IRegion ParentRegion { get; set; }

		public byte GetBlockID(Coordinates3D coordinates)
		{
			var index = coordinates.Y + coordinates.Z * Height + coordinates.X * Height * Width;
			return Data[index];
		}

		public byte GetMetadata(Coordinates3D coordinates)
		{
			var index = coordinates.Y + coordinates.Z * Height + coordinates.X * Height * Width;
			return Metadata[index];
		}

		public byte GetSkyLight(Coordinates3D coordinates)
		{
			var index = coordinates.Y + coordinates.Z * Height + coordinates.X * Height * Width;
			return SkyLight[index];
		}

		public byte GetBlockLight(Coordinates3D coordinates)
		{
			var index = coordinates.Y + coordinates.Z * Height + coordinates.X * Height * Width;
			return BlockLight[index];
		}

		/// <summary>
		///  Sets the block Id at specific coordinates relative to this chunk.
		///  Warning: The parent world's BlockChanged event handler does not get called.
		/// </summary>
		public void SetBlockID(Coordinates3D coordinates, byte value)
		{
			IsModified = true;
			if (ParentRegion != null)
				ParentRegion.DamageChunk(Coordinates);
			var index = coordinates.Y + coordinates.Z * Height + coordinates.X * Height * Width;
			Data[index] = value;
			if (value == AirBlock.BlockId)
				Metadata[index] = 0x0;
			var oldHeight = GetHeight((byte) coordinates.X, (byte) coordinates.Z);
			if (value == AirBlock.BlockId)
			{
				if (oldHeight <= coordinates.Y)
					while (coordinates.Y > 0)
					{
						coordinates.Y--;
						if (GetBlockID(coordinates) != 0)
						{
							SetHeight((byte) coordinates.X, (byte) coordinates.Z, coordinates.Y);
							if (coordinates.Y > MaxHeight)
								MaxHeight = coordinates.Y;
							break;
						}
					}
			}
			else
			{
				if (oldHeight < coordinates.Y)
					SetHeight((byte) coordinates.X, (byte) coordinates.Z, coordinates.Y);
			}
		}

		/// <summary>
		///  Sets the metadata at specific coordinates relative to this chunk.
		///  Warning: The parent world's BlockChanged event handler does not get called.
		/// </summary>
		public void SetMetadata(Coordinates3D coordinates, byte value)
		{
			IsModified = true;
			if (ParentRegion != null)
				ParentRegion.DamageChunk(Coordinates);
			var index = coordinates.Y + coordinates.Z * Height + coordinates.X * Height * Width;
			Metadata[index] = value;
		}

		/// <summary>
		///  Sets the sky light at specific coordinates relative to this chunk.
		///  Warning: The parent world's BlockChanged event handler does not get called.
		/// </summary>
		public void SetSkyLight(Coordinates3D coordinates, byte value)
		{
			IsModified = true;
			if (ParentRegion != null)
				ParentRegion.DamageChunk(Coordinates);
			var index = coordinates.Y + coordinates.Z * Height + coordinates.X * Height * Width;
			SkyLight[index] = value;
		}

		/// <summary>
		///  Sets the block light at specific coordinates relative to this chunk.
		///  Warning: The parent world's BlockChanged event handler does not get called.
		/// </summary>
		public void SetBlockLight(Coordinates3D coordinates, byte value)
		{
			IsModified = true;
			if (ParentRegion != null)
				ParentRegion.DamageChunk(Coordinates);
			var index = coordinates.Y + coordinates.Z * Height + coordinates.X * Height * Width;
			BlockLight[index] = value;
		}

		/// <summary>
		///  Gets the tile entity for the given coordinates. May return null.
		/// </summary>
		public NbtCompound GetTileEntity(Coordinates3D coordinates)
		{
			if (TileEntities.ContainsKey(coordinates))
				return TileEntities[coordinates];
			return null;
		}

		/// <summary>
		///  Sets the tile entity at the given coordinates to the given value.
		/// </summary>
		public void SetTileEntity(Coordinates3D coordinates, NbtCompound value)
		{
			if (value == null && TileEntities.ContainsKey(coordinates))
				TileEntities.Remove(coordinates);
			else
				TileEntities[coordinates] = value;
			IsModified = true;
			if (ParentRegion != null)
				ParentRegion.DamageChunk(Coordinates);
		}

		/// <summary>
		///  Gets the height of the specified column.
		/// </summary>
		public int GetHeight(byte x, byte z)
		{
			return HeightMap[x * Width + z];
		}

		public void UpdateHeightMap()
		{
			for (byte x = 0; x < Width; x++)
			for (byte z = 0; z < Depth; z++)
			{
				int y;
				for (y = Height - 1; y >= 0; y--)
				{
					var index = y + z * Height + x * Height * Width;
					if (Data[index] != 0)
					{
						SetHeight(x, z, y);
						if (y > MaxHeight)
							MaxHeight = y;
						break;
					}
				}

				if (y == 0)
					SetHeight(x, z, 0);
			}
		}

		public NbtTag Serialize(string tagName)
		{
			var chunk = new NbtCompound(tagName);
			var entities = new NbtList("Entities", NbtTagType.Compound);
			chunk.Add(entities);
			chunk.Add(new NbtInt("X", X));
			chunk.Add(new NbtInt("Z", Z));
			chunk.Add(new NbtByte("LightPopulated", (byte) (LightPopulated ? 1 : 0)));
			chunk.Add(new NbtByte("TerrainPopulated", (byte) (TerrainPopulated ? 1 : 0)));
			chunk.Add(new NbtByteArray("Blocks", Data));
			chunk.Add(new NbtByteArray("Data", Metadata.ToArray()));
			chunk.Add(new NbtByteArray("SkyLight", SkyLight.ToArray()));
			chunk.Add(new NbtByteArray("BlockLight", BlockLight.ToArray()));

			var tiles = new NbtList("TileEntities", NbtTagType.Compound);
			foreach (var kvp in TileEntities)
			{
				var c = new NbtCompound();
				c.Add(new NbtList("coordinates", new[]
				{
					new NbtInt(kvp.Key.X),
					new NbtInt(kvp.Key.Y),
					new NbtInt(kvp.Key.Z)
				}));
				c.Add(new NbtList("value", new[] {kvp.Value}));
				tiles.Add(c);
			}

			chunk.Add(tiles);

			// TODO: Entities
			return chunk;
		}

		public void Deserialize(NbtTag value)
		{
			var tag = (NbtCompound) value;

			X = tag["X"].IntValue;
			Z = tag["Z"].IntValue;
			if (tag.Contains("TerrainPopulated"))
				TerrainPopulated = tag["TerrainPopulated"].ByteValue > 0;
			if (tag.Contains("LightPopulated"))
				LightPopulated = tag["LightPopulated"].ByteValue > 0;
			const int size = Width * Height * Depth;
			const int halfSize = size / 2;
			Data = new byte[(int) (size * 2.5)];
			Buffer.BlockCopy(tag["Blocks"].ByteArrayValue, 0, Data, 0, size);
			Metadata = new NibbleSlice(Data, size, halfSize);
			BlockLight = new NibbleSlice(Data, size + halfSize, halfSize);
			SkyLight = new NibbleSlice(Data, size + halfSize * 2, halfSize);

			Metadata.Deserialize(tag["Data"]);
			BlockLight.Deserialize(tag["BlockLight"]);
			SkyLight.Deserialize(tag["SkyLight"]);

			if (tag.Contains("TileEntities"))
				foreach (var entity in tag["TileEntities"] as NbtList)
					TileEntities[new Coordinates3D(entity["coordinates"][0].IntValue,
						entity["coordinates"][1].IntValue,
						entity["coordinates"][2].IntValue)] = entity["value"][0] as NbtCompound;
			UpdateHeightMap();

			// TODO: Entities
		}

		private void SetHeight(byte x, byte z, int value)
		{
			IsModified = true;
			HeightMap[x * Width + z] = value;
		}

		public NbtFile ToNbt()
		{
			var serializer = new NbtSerializer(typeof(Chunk));
			var compound = serializer.Serialize(this, "Level") as NbtCompound;
			var file = new NbtFile();
			file.RootTag.Add(compound);
			return file;
		}

		public static Chunk FromNbt(NbtFile nbt)
		{
			var serializer = new NbtSerializer(typeof(Chunk));
			var chunk = (Chunk) serializer.Deserialize(nbt.RootTag["Level"]);
			return chunk;
		}
	}
}