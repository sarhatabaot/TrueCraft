using System;
using TrueCraft.Entities;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft.Logic
{
	public interface IItemProvider
	{
		short ID { get; }
		sbyte MaximumStack { get; }
		string DisplayName { get; }
		void ItemUsedOnNothing(ItemStack item, IWorld world, IRemoteClient user);
		void ItemUsedOnEntity(ItemStack item, IEntity usedOn, IWorld world, IRemoteClient user);
		void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user);

		Tuple<int, int> GetIconTexture(byte metadata);
	}
}