using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using TrueCraft.AI;
using TrueCraft.TerrainGen;
using TrueCraft.World;

namespace TrueCraft.Tests.AI
{
	[TestFixture]
	public class PathFindingTest
	{
		private void DrawGrid(PathResult path, IWorld world)
		{
			for (var z = -8; z < 8; z++)
			{
				for (var x = -8; x < 8; x++)
				{
					var coords = new Coordinates3D(x, 4, z);
					if (path.Waypoints.Contains(coords))
						Console.Write("o");
					else
					{
						var id = world.GetBlockID(coords);
						if (id != 0)
							Console.Write("x");
						else
							Console.Write("_");
					}
				}

				Console.WriteLine();
			}
		}

		[Test]
		public void TestAStarAvoidRoom()
		{
			var world = new TrueCraft.World.World("default", new FlatlandGenerator());
			var astar = new AStarPathFinder();
			var start = new Coordinates3D(-5, 4, 0);
			var end = new Coordinates3D(5, 4, 0);

			// North wall
			for (var x = -4; x < 4; x++)
				world.SetBlockID(new Coordinates3D(x, 4, -4), 1);
			// East wall
			for (var z = -4; z < 4; z++)
				world.SetBlockID(new Coordinates3D(3, 4, z), 1);
			// South wall
			for (var x = -4; x < 4; x++)
				world.SetBlockID(new Coordinates3D(x, 4, 4), 1);

			var watch = new Stopwatch();
			watch.Start();
			var path = astar.FindPath(world, new BoundingBox(), start, end);
			watch.Stop();
			DrawGrid(path, world);
			Console.WriteLine(watch.ElapsedMilliseconds + "ms");

			// Just test the start and end, the exact results need to be eyeballed
			Assert.AreEqual(start, path.Waypoints[0]);
			Assert.AreEqual(end, path.Waypoints[path.Waypoints.Count - 1]);
		}

		[Test]
		public void TestAStarDiagonalPath()
		{
			var world = new TrueCraft.World.World("default", new FlatlandGenerator());
			var astar = new AStarPathFinder();
			var start = new Coordinates3D(0, 4, 0);
			var end = new Coordinates3D(5, 4, 5);

			var watch = new Stopwatch();
			watch.Start();
			var path = astar.FindPath(world, new BoundingBox(), start, end);
			watch.Stop();
			DrawGrid(path, world);
			Console.WriteLine(watch.ElapsedMilliseconds + "ms");

			// Just test the start and end, the exact results need to be eyeballed
			Assert.AreEqual(start, path.Waypoints[0]);
			Assert.AreEqual(end, path.Waypoints[path.Waypoints.Count - 1]);
		}

		[Test]
		public void TestAStarExitRoom()
		{
			var world = new TrueCraft.World.World("default", new FlatlandGenerator());
			var astar = new AStarPathFinder();
			var start = new Coordinates3D(0, 4, 0);
			var end = new Coordinates3D(5, 4, 0);

			// North wall
			for (var x = -4; x < 4; x++)
				world.SetBlockID(new Coordinates3D(x, 4, -4), 1);
			// East wall
			for (var z = -4; z < 4; z++)
				world.SetBlockID(new Coordinates3D(3, 4, z), 1);
			// South wall
			for (var x = -4; x < 4; x++)
				world.SetBlockID(new Coordinates3D(x, 4, 4), 1);

			var watch = new Stopwatch();
			watch.Start();
			var path = astar.FindPath(world, new BoundingBox(), start, end);
			watch.Stop();
			DrawGrid(path, world);
			Console.WriteLine(watch.ElapsedMilliseconds + "ms");

			// Just test the start and end, the exact results need to be eyeballed
			Assert.AreEqual(start, path.Waypoints[0]);
			Assert.AreEqual(end, path.Waypoints[path.Waypoints.Count - 1]);
		}

		[Test]
		public void TestAStarImpossible()
		{
			var world = new TrueCraft.World.World("default", new FlatlandGenerator());
			var astar = new AStarPathFinder();
			var start = new Coordinates3D(0, 4, 0);
			var end = new Coordinates3D(5, 4, 0);

			world.SetBlockID(start + Coordinates3D.East, 1);
			world.SetBlockID(start + Coordinates3D.West, 1);
			world.SetBlockID(start + Coordinates3D.North, 1);
			world.SetBlockID(start + Coordinates3D.South, 1);

			var watch = new Stopwatch();
			watch.Start();
			var path = astar.FindPath(world, new BoundingBox(), start, end);
			watch.Stop();
			Console.WriteLine(watch.ElapsedMilliseconds + "ms");

			Assert.IsNull(path);
		}

		[Test]
		public void TestAStarLinearPath()
		{
			var world = new TrueCraft.World.World("default", new FlatlandGenerator());
			var astar = new AStarPathFinder();

			var watch = new Stopwatch();
			watch.Start();
			var path = astar.FindPath(world, new BoundingBox(),
				new Coordinates3D(0, 4, 0), new Coordinates3D(5, 4, 0));
			watch.Stop();
			DrawGrid(path, world);
			Console.WriteLine(watch.ElapsedMilliseconds + "ms");

			var expected = new[]
			{
				new Coordinates3D(0, 4, 0),
				new Coordinates3D(1, 4, 0),
				new Coordinates3D(2, 4, 0),
				new Coordinates3D(3, 4, 0),
				new Coordinates3D(4, 4, 0),
				new Coordinates3D(5, 4, 0)
			};
			for (var i = 0; i < path.Waypoints.Count; i++)
				Assert.AreEqual(expected[i], path.Waypoints[i]);
		}

		[Test]
		public void TestAStarObstacle()
		{
			var world = new TrueCraft.World.World("default", new FlatlandGenerator());
			var astar = new AStarPathFinder();
			var start = new Coordinates3D(0, 4, 0);
			var end = new Coordinates3D(5, 4, 0);
			world.SetBlockID(new Coordinates3D(3, 4, 0), 1); // Obstacle

			var watch = new Stopwatch();
			watch.Start();
			var path = astar.FindPath(world, new BoundingBox(), start, end);
			watch.Stop();
			DrawGrid(path, world);
			Console.WriteLine(watch.ElapsedMilliseconds + "ms");

			// Just test the start and end, the exact results need to be eyeballed
			Assert.AreEqual(start, path.Waypoints[0]);
			Assert.AreEqual(end, path.Waypoints[path.Waypoints.Count - 1]);
			Assert.IsFalse(path.Waypoints.Contains(new Coordinates3D(3, 4, 0)));
		}
	}
}