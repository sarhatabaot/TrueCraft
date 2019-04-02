﻿using System;
using NUnit.Framework;
using TrueCraft.Core.World;
using TrueCraft.API;
using System.IO;
using System.Reflection;

namespace TrueCraft.Core.Test.World
{
    [TestFixture]
    public class RegionTest
    {
        public Region Region { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            var world = new TrueCraft.Core.World.World();
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Region = new Region(Coordinates2D.Zero, world,
                Path.Combine(assemblyDir, "Files", "r.0.0.mca"));
        }

        [Test]
        public void TestGetChunk()
        {
            var chunk = Region.GetChunk(Coordinates2D.Zero);
            Assert.AreEqual(Coordinates2D.Zero, chunk.Coordinates);
            Assert.Throws(typeof(ArgumentException), () =>
                Region.GetChunk(new Coordinates2D(31, 31)));
        }

        [Test]
        public void TestUnloadChunk()
        {
            var chunk = Region.GetChunk(Coordinates2D.Zero);
            Assert.AreEqual(Coordinates2D.Zero, chunk.Coordinates);
            Assert.IsTrue(Region.Chunks.ContainsKey(Coordinates2D.Zero));
            Region.UnloadChunk(Coordinates2D.Zero);
            Assert.IsFalse(Region.Chunks.ContainsKey(Coordinates2D.Zero));
        }

        [Test]
        public void TestGetRegionFileName()
        {
            Assert.AreEqual("r.0.0.mca", Region.GetRegionFileName(Region.Position));
        }
    }
}