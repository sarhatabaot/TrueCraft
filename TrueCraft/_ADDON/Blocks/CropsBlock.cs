using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic.Items;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft.Logic.Blocks
{
	public class CropsBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x3B;

		public override byte Id => 0x3B;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Crops";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

		public override BoundingBox? BoundingBox => null;

		public override BoundingBox? InteractiveBoundingBox =>
			new BoundingBox(Vector3.Zero, new Vector3(1, 3 / 16.0f, 1));

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(8, 5);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			if (descriptor.Metadata >= 7)
				return new[]
				{
					new ItemStack(WheatItem.ItemID), new ItemStack(SeedsItem.ItemID, (sbyte) MathHelper.Random.Next(3))
				};
			return new[] {new ItemStack(SeedsItem.ItemID)};
		}

		private void GrowBlock(IMultiPlayerServer server, IWorld world, Coordinates3D coords)
		{
			if (world.GetBlockId(coords) != BlockId)
				return;
			var meta = world.GetMetadata(coords);
			meta++;
			world.SetMetadata(coords, meta);
			if (meta < 7)
			{
				var chunk = world.FindChunk(coords);
				server.Scheduler.ScheduleEvent("crops",
					chunk, TimeSpan.FromSeconds(MathHelper.Random.Next(30, 60)),
					_server => GrowBlock(_server, world, coords));
			}
		}

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server,
			IWorld world)
		{
			if (world.GetBlockId(descriptor.Coordinates + Coordinates3D.Down) != FarmlandBlock.BlockId)
			{
				GenerateDropEntity(descriptor, world, server, ItemStack.EmptyStack);
				world.SetBlockId(descriptor.Coordinates, 0);
			}
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			var chunk = world.FindChunk(descriptor.Coordinates);
			user.Server.Scheduler.ScheduleEvent("crops", chunk,
				TimeSpan.FromSeconds(MathHelper.Random.Next(30, 60)),
				server => GrowBlock(server, world, descriptor.Coordinates + MathHelper.BlockFaceToCoordinates(face)));
		}

		public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiPlayerServer server, IWorld world)
		{
			var chunk = world.FindChunk(coords);
			server.Scheduler.ScheduleEvent("crops", chunk,
				TimeSpan.FromSeconds(MathHelper.Random.Next(30, 60)),
				s => GrowBlock(s, world, coords));
		}
	}
}