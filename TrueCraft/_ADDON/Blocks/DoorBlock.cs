using TrueCraft.Logic.Items;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public abstract class DoorBlock : BlockProvider
	{
		public abstract short ItemId { get; }

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server,
			IWorld world)
		{
			var upper = ((DoorItem.DoorFlags) descriptor.Metadata & DoorItem.DoorFlags.Upper) ==
			            DoorItem.DoorFlags.Upper;
			var other = upper ? Coordinates3D.Down : Coordinates3D.Up;
			if (world.GetBlockId(descriptor.Coordinates + other) != Id)
				world.SetBlockId(descriptor.Coordinates, 0);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(ItemId)};
		}
	}
}