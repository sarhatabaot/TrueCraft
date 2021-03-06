﻿using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class TntRenderer : BlockRenderer
	{
		private static readonly Vector2 TopTexture = new Vector2(9, 0);
		private static readonly Vector2 BottomTexture = new Vector2(10, 0);
		private static readonly Vector2 SideTexture = new Vector2(8, 0);

		private static readonly Vector2[] Texture =
		{
			// Positive Z
			SideTexture + Vector2.UnitX + Vector2.UnitY,
			SideTexture + Vector2.UnitY,
			SideTexture,
			SideTexture + Vector2.UnitX,
			// Negative Z
			SideTexture + Vector2.UnitX + Vector2.UnitY,
			SideTexture + Vector2.UnitY,
			SideTexture,
			SideTexture + Vector2.UnitX,
			// Positive X
			SideTexture + Vector2.UnitX + Vector2.UnitY,
			SideTexture + Vector2.UnitY,
			SideTexture,
			SideTexture + Vector2.UnitX,
			// Negative X
			SideTexture + Vector2.UnitX + Vector2.UnitY,
			SideTexture + Vector2.UnitY,
			SideTexture,
			SideTexture + Vector2.UnitX,
			// Negative Y
			TopTexture + Vector2.UnitX + Vector2.UnitY,
			TopTexture + Vector2.UnitY,
			TopTexture,
			TopTexture + Vector2.UnitX,
			// Negative Y
			BottomTexture + Vector2.UnitX + Vector2.UnitY,
			BottomTexture + Vector2.UnitY,
			BottomTexture,
			BottomTexture + Vector2.UnitX
		};

		static TntRenderer()
		{
			RegisterRenderer(TNTBlock.BlockId, new TntRenderer());
			for (var i = 0; i < Texture.Length; i++)
				Texture[i] *= new Vector2(16f / 256f);
		}

		public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
			VisibleFaces faces, Tuple<int, int> textureMap, int indicesOffset, out int[] indices)
		{
			var lighting = new int[6];
			for (var i = 0; i < 6; i++)
			{
				var coords = descriptor.Coordinates + FaceCoords[i];
				lighting[i] = GetLight(descriptor.Chunk, coords);
			}

			return CreateUniformCube(offset, Texture, faces, indicesOffset, out indices, Color.White, lighting);
		}
	}
}