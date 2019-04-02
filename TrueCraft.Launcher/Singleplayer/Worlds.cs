﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrueCraft.Core;
using TrueCraft.Core.Logic;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.World;

namespace TrueCraft.Launcher.Singleplayer
{
	public class Worlds
	{
		public static Worlds Local { get; set; }

		public BlockRepository BlockRepository { get; set; }
		public World[] Saves { get; set; }

		public void Load()
		{
			if (!Directory.Exists(Paths.Worlds))
				Directory.CreateDirectory(Paths.Worlds);
			BlockRepository = new BlockRepository();
			BlockRepository.DiscoverBlockProviders();
			var directories = Directory.GetDirectories(Paths.Worlds);
			var saves = new List<World>();
			foreach (var d in directories)
				try
				{
					var w = World.LoadWorld(d);
					saves.Add(w);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					/* Who cares */
				}

			Saves = saves.ToArray();
		}

		public World CreateNewWorld(string name, string seed)
		{
			int s;
			if (!int.TryParse(seed, out s)) s = MathHelper.Random.Next();
			var world = new World(name, s, new StandardGenerator());
			world.BlockRepository = BlockRepository;
			var safeName = name;
			foreach (var c in Path.GetInvalidFileNameChars())
				safeName = safeName.Replace(c.ToString(), "");
			world.Name = name;
			world.Save(Path.Combine(Paths.Worlds, safeName));
			Saves = Saves.Concat(new[] {world}).ToArray();
			return world;
		}
	}
}