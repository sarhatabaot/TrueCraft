using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueCraft.Logic
{
	public class ItemRepository : IItemRepository
	{
		private readonly List<IItemProvider> ItemProviders = new List<IItemProvider>();

		public ItemRepository() => ItemProviders = new List<IItemProvider>();

		public IItemProvider GetItemProvider(short Id)
		{
			// TODO: Binary search
			for (var i = 0; i < ItemProviders.Count; i++)
				if (ItemProviders[i].Id == Id)
					return ItemProviders[i];
			return null;
		}

		public void RegisterItemProvider(IItemProvider provider)
		{
			int i;
			for (i = ItemProviders.Count - 1; i >= 0; i--)
			{
				if (provider.Id == ItemProviders[i].Id)
				{
					ItemProviders[i] = provider; // Override
					return;
				}

				if (ItemProviders[i].Id < provider.Id)
					break;
			}

			ItemProviders.Insert(i + 1, provider);
		}

		public void DiscoverItemProviders()
		{
			var providerTypes = new List<Type>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			foreach (var type in assembly.GetTypes().Where(t =>
				typeof(IItemProvider).IsAssignableFrom(t) && !t.IsAbstract))
				providerTypes.Add(type);

			providerTypes.ForEach(t =>
			{
				var instance = (IItemProvider) Activator.CreateInstance(t);
				RegisterItemProvider(instance);
			});
		}
	}
}