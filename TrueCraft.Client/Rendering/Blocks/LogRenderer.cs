﻿using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class LogRenderer : BlockRenderer
	{
		private static readonly Vector2 BaseEndsTexture = new Vector2(5, 1);
		private static readonly Vector2 BaseSidesTexture = new Vector2(4, 1);
		private static readonly Vector2 SpruceSidesTexture = new Vector2(4, 7);
		private static readonly Vector2 BirchSidesTexture = new Vector2(5, 7);

		private static readonly Vector2[] BaseTexture =
		{
			// Positive Z
			BaseSidesTexture + Vector2.UnitX + Vector2.UnitY,
			BaseSidesTexture + Vector2.UnitY,
			BaseSidesTexture,
			BaseSidesTexture + Vector2.UnitX,
			// Negative Z
			BaseSidesTexture + Vector2.UnitX + Vector2.UnitY,
			BaseSidesTexture + Vector2.UnitY,
			BaseSidesTexture,
			BaseSidesTexture + Vector2.UnitX,
			// Positive X
			BaseSidesTexture + Vector2.UnitX + Vector2.UnitY,
			BaseSidesTexture + Vector2.UnitY,
			BaseSidesTexture,
			BaseSidesTexture + Vector2.UnitX,
			// Negative X
			BaseSidesTexture + Vector2.UnitX + Vector2.UnitY,
			BaseSidesTexture + Vector2.UnitY,
			BaseSidesTexture,
			BaseSidesTexture + Vector2.UnitX,
			// Positive Y
			BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
			BaseEndsTexture + Vector2.UnitY,
			BaseEndsTexture,
			BaseEndsTexture + Vector2.UnitX,
			// Negative Y
			BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
			BaseEndsTexture + Vector2.UnitY,
			BaseEndsTexture,
			BaseEndsTexture + Vector2.UnitX
		};

		private static readonly Vector2[] SpruceTexture =
		{
			// Positive Z
			SpruceSidesTexture + Vector2.UnitX + Vector2.UnitY,
			SpruceSidesTexture + Vector2.UnitY,
			SpruceSidesTexture,
			SpruceSidesTexture + Vector2.UnitX,
			// Negative Z
			SpruceSidesTexture + Vector2.UnitX + Vector2.UnitY,
			SpruceSidesTexture + Vector2.UnitY,
			SpruceSidesTexture,
			SpruceSidesTexture + Vector2.UnitX,
			// Positive X
			SpruceSidesTexture + Vector2.UnitX + Vector2.UnitY,
			SpruceSidesTexture + Vector2.UnitY,
			SpruceSidesTexture,
			SpruceSidesTexture + Vector2.UnitX,
			// Negative X
			SpruceSidesTexture + Vector2.UnitX + Vector2.UnitY,
			SpruceSidesTexture + Vector2.UnitY,
			SpruceSidesTexture,
			SpruceSidesTexture + Vector2.UnitX,
			// Positive Y
			BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
			BaseEndsTexture + Vector2.UnitY,
			BaseEndsTexture,
			BaseEndsTexture + Vector2.UnitX,
			// Negative Y
			BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
			BaseEndsTexture + Vector2.UnitY,
			BaseEndsTexture,
			BaseEndsTexture + Vector2.UnitX
		};

		private static readonly Vector2[] BirchTexture =
		{
			// Positive Z
			BirchSidesTexture + Vector2.UnitX + Vector2.UnitY,
			BirchSidesTexture + Vector2.UnitY,
			BirchSidesTexture,
			BirchSidesTexture + Vector2.UnitX,
			// Negative Z
			BirchSidesTexture + Vector2.UnitX + Vector2.UnitY,
			BirchSidesTexture + Vector2.UnitY,
			BirchSidesTexture,
			BirchSidesTexture + Vector2.UnitX,
			// Positive X
			BirchSidesTexture + Vector2.UnitX + Vector2.UnitY,
			BirchSidesTexture + Vector2.UnitY,
			BirchSidesTexture,
			BirchSidesTexture + Vector2.UnitX,
			// Negative X
			BirchSidesTexture + Vector2.UnitX + Vector2.UnitY,
			BirchSidesTexture + Vector2.UnitY,
			BirchSidesTexture,
			BirchSidesTexture + Vector2.UnitX,
			// Positive Y
			BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
			BaseEndsTexture + Vector2.UnitY,
			BaseEndsTexture,
			BaseEndsTexture + Vector2.UnitX,
			// Negative Y
			BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
			BaseEndsTexture + Vector2.UnitY,
			BaseEndsTexture,
			BaseEndsTexture + Vector2.UnitX
		};

		static LogRenderer()
		{
			RegisterRenderer(WoodBlock.BlockID, new LogRenderer());
			for (var i = 0; i < BaseTexture.Length; i++)
			{
				BaseTexture[i] *= new Vector2(16f / 256f);
				SpruceTexture[i] *= new Vector2(16f / 256f);
				BirchTexture[i] *= new Vector2(16f / 256f);
			}
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

			switch ((WoodBlock.WoodType) descriptor.Metadata)
			{
				case WoodBlock.WoodType.Spruce:
					return CreateUniformCube(offset, SpruceTexture, faces, indiciesOffset, out indicies, Color.White,
						lighting);
				case WoodBlock.WoodType.Birch:
					return CreateUniformCube(offset, BirchTexture, faces, indiciesOffset, out indicies, Color.White,
						lighting);
				case WoodBlock.WoodType.Oak:
				default:
					return CreateUniformCube(offset, BaseTexture, faces, indiciesOffset, out indicies, Color.White,
						lighting);
			}
		}
	}
}