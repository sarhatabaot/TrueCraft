using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using TrueCraft.Lighting;
using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;
using TrueCraft.TerrainGen;
using TrueCraft.World;

namespace TrueCraft.Tests.Lighting
{
	[TestFixture]
	public class WorldLighterTest
	{
		[Test]
		public void TestBasicLighting()
		{
			var repository = new BlockRepository();
			repository.RegisterBlockProvider(new GrassBlock());
			repository.RegisterBlockProvider(new DirtBlock());
			repository.RegisterBlockProvider(new AirBlock());
			repository.RegisterBlockProvider(new BedrockBlock());
			var world = new TrueCraft.World.World("TEST", new FlatlandGenerator());
			world.BlockRepository = repository;
			var lighter = new WorldLighting(world, repository);
			world.GetBlockId(Coordinates3D.Zero); // Generate a chunk
			lighter.InitialLighting(world.GetChunk(Coordinates2D.Zero));

			for (var y = 5; y >= 0; y--)
			{
				Console.Write("Y: {0} ", y);
				Console.Write(world.GetBlockId(new Coordinates3D(0, y, 0)));
				Console.Write(" -> ");
				Console.WriteLine(world.GetSkyLight(new Coordinates3D(0, y, 0)));
			}

			// Validate behavior
			for (var y = 0; y < Chunk.Height; y++)
			for (var x = 0; x < Chunk.Width; x++)
			for (var z = 0; z < Chunk.Depth; z++)
			{
				var coords = new Coordinates3D(x, y, z);
				var sky = world.GetSkyLight(coords);
				if (y < 4)
					Assert.AreEqual(0, sky, coords.ToString());
				else
					Assert.AreEqual(15, sky, coords.ToString());
			}
		}

		[Test]
		public void TestFarPropegation()
		{
			var repository = new BlockRepository();
			repository.RegisterBlockProvider(new GrassBlock());
			repository.RegisterBlockProvider(new DirtBlock());
			repository.RegisterBlockProvider(new AirBlock());
			repository.RegisterBlockProvider(new BedrockBlock());
			var world = new TrueCraft.World.World("TEST", new FlatlandGenerator());
			world.BlockRepository = repository;
			var lighter = new WorldLighting(world, repository);
			world.GetBlockId(Coordinates3D.Zero); // Generate a chunk
			lighter.InitialLighting(world.GetChunk(Coordinates2D.Zero));

			world.SetBlockId(new Coordinates3D(5, 3, 5), 0); // Create area that looks like so:
			world.SetBlockId(new Coordinates3D(5, 2, 5), 0); // x x  Light goes like so: |
			world.SetBlockId(new Coordinates3D(5, 1, 5), 0); // x x                      |
			world.SetBlockId(new Coordinates3D(4, 1, 5), 0); //   x                     -/

			for (var x = 0; x < 4; x++)
				world.SetBlockId(new Coordinates3D(x, 1, 5), 0); // Dig a tunnel
			// xxxxx x ish
			// x     x
			// xxxxxxx

			lighter.EnqueueOperation(new BoundingBox(new Vector3(5, 2, 5),
				new Vector3(6, 4, 6)), true);

			while (lighter.TryLightNext()) // Test lighting
			{
			}

			Console.WriteLine("Testing {0}", new Coordinates3D(5, 3, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 3, 5)));
			Console.WriteLine("Testing {0}", new Coordinates3D(5, 2, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 2, 5)));
			Console.WriteLine("Testing {0}", new Coordinates3D(5, 1, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 1, 5)));

			byte expected = 15;
			for (var x = 5; x >= 0; x--)
			{
				Console.WriteLine("Testing {0}", new Coordinates3D(x, 1, 5));
				Assert.AreEqual(expected, world.GetSkyLight(new Coordinates3D(x, 1, 5)));
				expected--;
			}
		}

		[Test]
		public void TestFarPropegationx2()
		{
			var repository = new BlockRepository();
			repository.RegisterBlockProvider(new GrassBlock());
			repository.RegisterBlockProvider(new DirtBlock());
			repository.RegisterBlockProvider(new AirBlock());
			repository.RegisterBlockProvider(new BedrockBlock());
			var world = new TrueCraft.World.World("TEST", new FlatlandGenerator());
			world.BlockRepository = repository;
			var lighter = new WorldLighting(world, repository);
			world.GetBlockId(Coordinates3D.Zero); // Generate a chunk
			lighter.InitialLighting(world.GetChunk(Coordinates2D.Zero));

			// Test this layout:
			// xxx x    y=3
			// x   x    y=2
			// x   x    y=1
			// xxxxx    y=0
			//
			//    ^ x,z = 5

			for (var y = 1; y <= 3; y++) // Dig hole
				world.SetBlockId(new Coordinates3D(5, y, 5), 0);

			for (var x = 0; x <= 4; x++) // Dig outwards
			{
				world.SetBlockId(new Coordinates3D(x, 2, 5), 0); // Dig a tunnel
				world.SetBlockId(new Coordinates3D(x, 1, 5), 0); // Dig a tunnel
			}

			var watch = new Stopwatch();
			watch.Start();

			lighter.EnqueueOperation(new BoundingBox(new Vector3(5, 2, 5),
				new Vector3(6, 4, 6)), true);

			while (lighter.TryLightNext()) // Test lighting
			{
			}

			watch.Stop();

			// Output lighting
			for (var y = 3; y >= 0; y--)
			{
				for (var x = 0; x <= 5; x++)
					Console.Write(world.GetBlockId(new Coordinates3D(x, y, 5)).ToString("D2") + " ");
				Console.WriteLine();
			}

			Console.WriteLine();
			for (var y = 3; y >= 0; y--)
			{
				for (var x = 0; x <= 5; x++)
					Console.Write(world.GetSkyLight(new Coordinates3D(x, y, 5)).ToString("D2") + " ");
				Console.WriteLine();
			}

			Console.WriteLine("Testing {0}", new Coordinates3D(5, 3, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 3, 5)));
			Console.WriteLine("Testing {0}", new Coordinates3D(5, 2, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 2, 5)));
			Console.WriteLine("Testing {0}", new Coordinates3D(5, 1, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 1, 5)));

			byte expected = 15;
			for (var x = 5; x >= 0; x--)
			{
				Console.WriteLine("Testing {0}", new Coordinates3D(x, 2, 5));
				Assert.AreEqual(expected, world.GetSkyLight(new Coordinates3D(x, 2, 5)));
				expected--;
			}

			expected = 15;
			for (var x = 5; x >= 0; x--)
			{
				Console.WriteLine("Testing {0}", new Coordinates3D(x, 1, 5));
				Assert.AreEqual(expected, world.GetSkyLight(new Coordinates3D(x, 1, 5)));
				expected--;
			}

			Console.WriteLine("{0}ms", watch.ElapsedMilliseconds);
		}

		[Test]
		public void TestLeavesAndEtc()
		{
			var repository = new BlockRepository();
			repository.RegisterBlockProvider(new GrassBlock());
			repository.RegisterBlockProvider(new DirtBlock());
			repository.RegisterBlockProvider(new AirBlock());
			repository.RegisterBlockProvider(new BedrockBlock());
			repository.RegisterBlockProvider(new LeavesBlock());
			var world = new TrueCraft.World.World("TEST", new FlatlandGenerator());
			world.BlockRepository = repository;
			var lighter = new WorldLighting(world, repository);
			world.GetBlockId(Coordinates3D.Zero); // Generate a chunk

			for (var y = 1; y <= 16; y++)
			{
				var coords = new Coordinates3D(5, y, 5);
				world.SetBlockId(coords, 0);
				world.SetBlockId(coords + Coordinates3D.East, DirtBlock.BlockID);
				world.SetBlockId(coords + Coordinates3D.West, DirtBlock.BlockID);
				world.SetBlockId(coords + Coordinates3D.North, DirtBlock.BlockID);
				world.SetBlockId(coords + Coordinates3D.South, DirtBlock.BlockID);
			}

			world.GetChunk(Coordinates2D.Zero).UpdateHeightMap();

			lighter.InitialLighting(world.GetChunk(Coordinates2D.Zero));

			// Test this layout:
			// xox      o == leaves
			// x x
			// xox
			// x x
			// xox ...

			for (var y = 1; y <= 16; y++)
				if (y % 2 == 1)
					world.SetBlockId(new Coordinates3D(5, y, 5), LeavesBlock.BlockID);
			world.GetChunk(Coordinates2D.Zero).UpdateHeightMap();

			lighter.EnqueueOperation(new BoundingBox(new Vector3(5, 0, 5),
				new Vector3(6, 16, 6)), true);

			while (lighter.TryLightNext()) // Test lighting
			{
			}

			// Output lighting
			for (var y = 16; y >= 0; y--)
			{
				Console.Write(world.GetBlockId(new Coordinates3D(5, y, 5)).ToString("D2"));
				Console.Write(" " + world.GetSkyLight(new Coordinates3D(5, y, 5)).ToString("D2"));
				Console.WriteLine("   Y={0}", y);
			}

			var expected = new byte[]
			{
				15, // air
				13, // leaves
				12, // air
				10, // leaves
				9, // air
				7, // leaves
				6, // air
				4, // leaves
				3, // air
				1, // leaves
				0, // air
				0 // leaves
			};

			for (int y = 16, i = 0; y >= 0; y--, i++)
			{
				byte ex;
				if (i < expected.Length)
					ex = expected[i];
				else
					ex = 0;
				Assert.AreEqual(ex, world.GetSkyLight(new Coordinates3D(5, y, 5)));
			}
		}

		[Test]
		public void TestShortPropegation()
		{
			var repository = new BlockRepository();
			repository.RegisterBlockProvider(new GrassBlock());
			repository.RegisterBlockProvider(new DirtBlock());
			repository.RegisterBlockProvider(new AirBlock());
			repository.RegisterBlockProvider(new BedrockBlock());
			var world = new TrueCraft.World.World("TEST", new FlatlandGenerator());
			world.BlockRepository = repository;
			var lighter = new WorldLighting(world, repository);
			world.GetBlockId(Coordinates3D.Zero); // Generate a chunk
			lighter.InitialLighting(world.GetChunk(Coordinates2D.Zero));

			world.SetBlockId(new Coordinates3D(5, 3, 5), 0); // Create area that looks like so:
			world.SetBlockId(new Coordinates3D(5, 2, 5), 0); // x x  Light goes like so: |
			world.SetBlockId(new Coordinates3D(5, 1, 5), 0); // x x                      |
			world.SetBlockId(new Coordinates3D(4, 1, 5), 0); //   x                     -/

			lighter.EnqueueOperation(new BoundingBox(new Vector3(5, 2, 5),
				new Vector3(6, 4, 6)), true);

			while (lighter.TryLightNext()) // Test lighting
			{
			}

			Console.WriteLine("Testing {0}", new Coordinates3D(5, 3, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 3, 5)));
			Console.WriteLine("Testing {0}", new Coordinates3D(5, 2, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 2, 5)));
			Console.WriteLine("Testing {0}", new Coordinates3D(5, 1, 5));
			Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 1, 5)));
			Console.WriteLine("Testing {0}", new Coordinates3D(4, 1, 5));
			Assert.AreEqual(14, world.GetSkyLight(new Coordinates3D(4, 1, 5)));
		}
	}
}