using System;
using System.Linq;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.World;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class TorchBlock : BlockProvider, ICraftingRecipe
	{
		public enum TorchDirection
		{
			East = 0x01,
			West = 0x02,
			South = 0x03,
			North = 0x04,
			Ground = 0x05
		}

		public static readonly byte BlockId = 0x32;

		public override byte Id => 0x32;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 13;

		public override bool Opaque => false;

		public override bool RenderOpaque => true;

		public override string DisplayName => "Torch";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public override BoundingBox? BoundingBox => null;

		public override BoundingBox? InteractiveBoundingBox => new BoundingBox(new Vector3(4 / 16.0f, 0, 4 / 16.0f),
			new Vector3(12 / 16.0f, 7.0f / 16.0f, 12 / 16.0f));

		public virtual ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(CoalItem.ItemId)},
				{new ItemStack(StickItem.ItemId)}
			};

		public virtual ItemStack Output => new ItemStack(BlockId, 4);

		public virtual bool SignificantMetadata => false;

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			TorchDirection[] preferredDirections =
			{
				TorchDirection.West, TorchDirection.East,
				TorchDirection.North, TorchDirection.South,
				TorchDirection.Ground
			};
			TorchDirection direction;
			switch (face)
			{
				case BlockFace.PositiveZ:
					direction = TorchDirection.South;
					break;
				case BlockFace.NegativeZ:
					direction = TorchDirection.North;
					break;
				case BlockFace.PositiveX:
					direction = TorchDirection.East;
					break;
				case BlockFace.NegativeX:
					direction = TorchDirection.West;
					break;
				default:
					direction = TorchDirection.Ground;
					break;
			}

			var i = 0;
			descriptor.Metadata = (byte) direction;
			while (!IsSupported(descriptor, user.Server, world) && i < preferredDirections.Length)
			{
				direction = preferredDirections[i++];
				descriptor.Metadata = (byte) direction;
			}

			world.SetBlockData(descriptor.Coordinates, descriptor);
		}

		public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world,
			IRemoteClient user)
		{
			coordinates += MathHelper.BlockFaceToCoordinates(face);
			var old = world.GetBlockData(coordinates);
			byte[] overwritable =
			{
				AirBlock.BlockId,
				WaterBlock.BlockId,
				StationaryWaterBlock.BlockId,
				LavaBlock.BlockId,
				StationaryLavaBlock.BlockId
			};
			if (overwritable.Any(b => b == old.Id))
			{
				var data = world.GetBlockData(coordinates);
				data.Id = Id;
				data.Metadata = (byte) item.Metadata;

				BlockPlaced(data, face, world, user);

				if (!IsSupported(world.GetBlockData(coordinates), user.Server, world))
					world.SetBlockData(coordinates, old);
				else
				{
					item.Count--;
					user.Inventory[user.SelectedSlot] = item;
				}
			}
		}

		public override Coordinates3D GetSupportDirection(BlockDescriptor descriptor)
		{
			switch ((TorchDirection) descriptor.Metadata)
			{
				case TorchDirection.Ground:
					return Coordinates3D.Down;
				case TorchDirection.East:
					return Coordinates3D.West;
				case TorchDirection.West:
					return Coordinates3D.East;
				case TorchDirection.North:
					return Coordinates3D.South;
				case TorchDirection.South:
					return Coordinates3D.North;
			}

			return Coordinates3D.Zero;
		}

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 5);
		}
	}
}