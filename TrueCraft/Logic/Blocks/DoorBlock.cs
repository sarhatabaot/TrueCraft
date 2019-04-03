using TrueCraft.Logic.Items;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public abstract class DoorBlock : BlockProvider
	{
		public abstract short ItemID { get; }

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server,
			IWorld world)
		{
			var upper = ((DoorItem.DoorFlags) descriptor.Metadata & DoorItem.DoorFlags.Upper) ==
			            DoorItem.DoorFlags.Upper;
			var other = upper ? Coordinates3D.Down : Coordinates3D.Up;
			if (world.GetBlockID(descriptor.Coordinates + other) != ID)
				world.SetBlockID(descriptor.Coordinates, 0);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(ItemID)};
		}
	}
}