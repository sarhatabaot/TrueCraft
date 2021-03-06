﻿using System;
using Microsoft.Xna.Framework;
using TrueCraft.Networking;
using TrueCraft.Serialization.Tags;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft.Logic
{
	public interface IBlockProvider : IItemProvider
	{
		new byte Id { get; }
		double BlastResistance { get; }
		double Hardness { get; }
		byte Luminance { get; }
		bool Opaque { get; }
		bool RenderOpaque { get; }
		byte LightOpacity { get; }
		bool DiffuseSkyLight { get; }
		bool Flammable { get; }
		SoundEffectClass SoundEffect { get; }
		ToolMaterial EffectiveToolMaterials { get; }
		ToolType EffectiveTools { get; }
		string DisplayName { get; }
		BoundingBox? BoundingBox { get; } // NOTE: Will this eventually need to be metadata-aware?
		BoundingBox? InteractiveBoundingBox { get; } // NOTE: Will this eventually need to be metadata-aware?
		Tuple<int, int> GetTextureMap(byte metadata);

		void GenerateDropEntity(BlockDescriptor descriptor, IWorld world, IMultiPlayerServer server,
			ItemStack heldItem);

		void BlockLeftClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
		bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
		void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
		void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
		void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server, IWorld world);
		void BlockLoadedFromChunk(Coordinates3D coords, IMultiPlayerServer server, IWorld world);

		void TileEntityLoadedForClient(BlockDescriptor descriptor, IWorld world, NbtCompound compound,
			IRemoteClient client);
	}
}