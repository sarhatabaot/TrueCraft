﻿using System;
using Microsoft.Xna.Framework;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class CraftingTableRenderer : BlockRenderer
	{
		private static readonly Vector2 TopTexture = new Vector2(11, 2);
		private static readonly Vector2 BottomTexture = new Vector2(4, 0);
		private static readonly Vector2 SideATexture = new Vector2(11, 3);
		private static readonly Vector2 SideBTexture = new Vector2(12, 3);

		private static readonly Vector2[] Texture =
		{
			// Positive Z
			SideATexture + Vector2.UnitX + Vector2.UnitY,
			SideATexture + Vector2.UnitY,
			SideATexture,
			SideATexture + Vector2.UnitX,
			// Negative Z
			SideATexture + Vector2.UnitX + Vector2.UnitY,
			SideATexture + Vector2.UnitY,
			SideATexture,
			SideATexture + Vector2.UnitX,
			// Positive X
			SideBTexture + Vector2.UnitX + Vector2.UnitY,
			SideBTexture + Vector2.UnitY,
			SideBTexture,
			SideBTexture + Vector2.UnitX,
			// Negative X
			SideBTexture + Vector2.UnitX + Vector2.UnitY,
			SideBTexture + Vector2.UnitY,
			SideBTexture,
			SideBTexture + Vector2.UnitX,
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

		static CraftingTableRenderer()
		{
			RegisterRenderer(CraftingTableBlock.BlockID, new CraftingTableRenderer());
			for (var i = 0; i < Texture.Length; i++)
				Texture[i] *= new Vector2(16f / 256f);
		}

		public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
			VisibleFaces faces, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
		{
			var lighting = new int[6];
			for (var i = 0; i < 6; i++)
			{
				var coords = descriptor.Coordinates + FaceCoords[i];
				lighting[i] = GetLight(descriptor.Chunk, coords);
			}

			return CreateUniformCube(offset, Texture, faces, indiciesOffset, out indicies, Color.White, lighting);
		}
	}
}