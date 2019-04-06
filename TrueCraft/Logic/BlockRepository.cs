using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TrueCraft.Entities;
using TrueCraft.World;

namespace TrueCraft.Logic
{
	public class BlockRepository : IBlockRepository, IBlockPhysicsProvider
	{
		private readonly IBlockProvider[] BlockProviders = new IBlockProvider[0x100];

		public BoundingBox? GetBoundingBox(IWorld world, Coordinates3D coordinates)
		{
			// TODO: Block-specific bounding boxes
			var Id = world.GetBlockId(coordinates);
			if (Id == 0) return null;
			var provider = BlockProviders[Id];
			return provider.BoundingBox;
		}

		public IBlockProvider GetBlockProvider(byte Id)
		{
			return BlockProviders[Id];
		}

		public void RegisterBlockProvider(IBlockProvider provider)
		{
			BlockProviders[provider.Id] = provider;
		}

		public void DiscoverBlockProviders()
		{
			var providerTypes = new List<Type>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			foreach (var type in assembly.GetTypes().Where(t =>
				typeof(IBlockProvider).IsAssignableFrom(t) && !t.IsAbstract))
				providerTypes.Add(type);

			providerTypes.ForEach(t =>
			{
				var instance = (IBlockProvider) Activator.CreateInstance(t);
				RegisterBlockProvider(instance);
			});
		}
	}
}