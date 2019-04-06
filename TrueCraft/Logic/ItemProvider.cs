using System;
using TrueCraft.Entities;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic
{
	public abstract class ItemProvider : IItemProvider
	{
		public abstract short Id { get; }

		public abstract Tuple<int, int> GetIconTexture(byte metadata);

		public virtual sbyte MaximumStack => 64;

		public virtual string DisplayName => string.Empty;

		public virtual void ItemUsedOnEntity(ItemStack item, IEntity usedOn, IWorld world, IRemoteClient user)
		{
			// This space intentionally left blank
		}

		public virtual void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			// This space intentionally left blank
		}

		public virtual void ItemUsedOnNothing(ItemStack item, IWorld world, IRemoteClient user)
		{
			// This space intentionally left blank
		}
	}
}