using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Serialization.Tags;
using TrueCraft.World;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class UprightSignBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x3F;

		public override byte Id => 0x3F;

		public override double BlastResistance => 5;

		public override double Hardness => 1;

		public override byte Luminance => 0;

		public override bool Opaque // This is weird. You can stack signs on signs in Minecraft.
			=>
				true;

		public override string DisplayName => "Sign";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public override BoundingBox? BoundingBox => null;

		public override BoundingBox? InteractiveBoundingBox => new BoundingBox(new Vector3(6 / 16.0f, 0, 6 / 16.0f),
			new Vector3(10 / 16.0f, 10 / 16.0f, 10 / 16.0f));

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(4, 0);
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			double rotation = user.Entity.Yaw + 180 % 360;
			if (rotation < 0)
				rotation += 360;

			world.SetMetadata(descriptor.Coordinates, (byte) (rotation / 22.5));
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(SignItem.ItemId)};
		}

		public override void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			world.SetTileEntity(descriptor.Coordinates, null);
			base.BlockMined(descriptor, face, world, user);
		}

		public override void TileEntityLoadedForClient(BlockDescriptor descriptor, IWorld world, NbtCompound entity,
			IRemoteClient client)
		{
			client.QueuePacket(new UpdateSignPacket
			{
				X = descriptor.Coordinates.X,
				Y = (short) descriptor.Coordinates.Y,
				Z = descriptor.Coordinates.Z,
				Text = new[]
				{
					entity["Text1"].StringValue,
					entity["Text2"].StringValue,
					entity["Text3"].StringValue,
					entity["Text4"].StringValue
				}
			});
		}
	}
}