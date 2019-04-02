﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.TerrainGen.Biomes
{
    public class SavannaBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Savanna; }
        }

        public override double Temperature
        {
            get { return 1.2f; }
        }

        public override double Rainfall
        {
            get { return 0.0f; }
        }

        public override TreeSpecies[] Trees
        {
            get
            {
                return new[] { TreeSpecies.Oak };
            }
        }

        public override PlantSpecies[] Plants
        {
            get
            {
                return new[] { PlantSpecies.Deadbush, PlantSpecies.Fern };
            }
        }

        public override double TreeDensity
        {
            get
            {
                return 50;
            }
        }
    }
}