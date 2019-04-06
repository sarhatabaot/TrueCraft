using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;

namespace TrueCraft._ADDON.Blocks
{
	public abstract class StairsBlock : BlockProvider
	{
		public enum StairDirection
		{
			East = 0,
			West = 1,
			South = 2,
			North = 3
		}

		public override double Hardness => 0;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override byte LightOpacity => 255;

		public virtual bool SignificantMetadata => false;

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			byte meta = 0;
			switch (MathHelper.DirectionByRotationFlat(user.Entity.Yaw))
			{
				case Direction.East:
					meta = (byte) StairDirection.East;
					break;
				case Direction.West:
					meta = (byte) StairDirection.West;
					break;
				case Direction.North:
					meta = (byte) StairDirection.North;
					break;
				case Direction.South:
					meta = (byte) StairDirection.South;
					break;
				default:
					meta = 0; // Should never happen
					break;
			}

			world.SetMetadata(descriptor.Coordinates, meta);
		}
	}
}