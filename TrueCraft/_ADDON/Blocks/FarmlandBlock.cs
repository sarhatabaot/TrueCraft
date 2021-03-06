using System;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft._ADDON.Blocks
{
	public class FarmlandBlock : BlockProvider
	{
		public enum MoistureLevel : byte
		{
			Dry = 0x0,

			// Any value less than 0x7 is considered 'dry'

			Moist = 0x7
		}

		public static readonly int UpdateIntervalSeconds = 30;

		public static readonly byte BlockId = 0x3C;

		public override byte Id => 0x3C;

		public override double BlastResistance => 3;

		public override double Hardness => 0.6;

		public override byte Luminance => 0;

		public override bool Opaque => true;

		public override byte LightOpacity => 255;

		public override string DisplayName => "Farmland";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Gravel;

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(DirtBlock.BlockId)};
		}

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(7, 5);
		}

		public bool IsHydrated(Coordinates3D coordinates, IWorld world)
		{
			var min = new Coordinates3D(-6 + coordinates.X, coordinates.Y, -6 + coordinates.Z);
			var max = new Coordinates3D(6 + coordinates.X, coordinates.Y + 1, 6 + coordinates.Z);
			for (var x = min.X; x < max.X; x++)
			for (var y = min.Y;
				y < max.Y;
				y++) // TODO: This does not check one above the farmland block for some reason
			for (var z = min.Z; z < max.Z; z++)
			{
				var Id = world.GetBlockId(new Coordinates3D(x, y, z));
				if (Id == WaterBlock.BlockId || Id == StationaryWaterBlock.BlockId)
					return true;
			}

			return false;
		}

		private void HydrationCheckEvent(IMultiPlayerServer server, Coordinates3D coords, IWorld world)
		{
			if (world.GetBlockId(coords) != BlockId)
				return;
			if (MathHelper.Random.Next(3) == 0)
			{
				var meta = world.GetMetadata(coords);
				if (IsHydrated(coords, world) && meta != 15)
					meta++;
				else
				{
					meta--;
					if (meta == 0)
					{
						world.SetBlockId(coords, BlockId);
						return;
					}
				}

				world.SetMetadata(coords, meta);
			}

			var chunk = world.FindChunk(coords);
			server.Scheduler.ScheduleEvent("farmland", chunk,
				TimeSpan.FromSeconds(UpdateIntervalSeconds),
				_server => HydrationCheckEvent(_server, coords, world));
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			if (IsHydrated(descriptor.Coordinates, world)) world.SetMetadata(descriptor.Coordinates, 1);
			var chunk = world.FindChunk(descriptor.Coordinates);
			user.Server.Scheduler.ScheduleEvent("farmland", chunk,
				TimeSpan.FromSeconds(UpdateIntervalSeconds),
				server => HydrationCheckEvent(server, descriptor.Coordinates, world));
		}

		public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiPlayerServer server, IWorld world)
		{
			var chunk = world.FindChunk(coords);
			server.Scheduler.ScheduleEvent("farmland", chunk,
				TimeSpan.FromSeconds(UpdateIntervalSeconds),
				s => HydrationCheckEvent(s, coords, world));
		}
	}
}