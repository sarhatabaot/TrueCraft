﻿using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class LadderRenderer : BlockRenderer
	{
		private static readonly Vector2 TextureMap = new Vector2(3, 5);

		private static readonly Vector2[] Texture =
		{
			TextureMap + Vector2.UnitX + Vector2.UnitY,
			TextureMap + Vector2.UnitY,
			TextureMap,
			TextureMap + Vector2.UnitX
		};

		static LadderRenderer()
		{
			RegisterRenderer(LadderBlock.BlockId, new LadderRenderer());
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

			VertexPositionNormalColorTexture[] verticies;
			Vector3 correction;
			var faceCorrection = 0;
			switch ((LadderBlock.LadderDirection) descriptor.Metadata)
			{
				case LadderBlock.LadderDirection.North:
					verticies = CreateQuad(CubeFace.PositiveZ, offset, Texture, 0, indiciesOffset, out indicies,
						Color.White);
					correction = Vector3.Forward;
					faceCorrection = (int) CubeFace.PositiveZ * 4;
					break;
				case LadderBlock.LadderDirection.South:
					verticies = CreateQuad(CubeFace.NegativeZ, offset, Texture, 0, indiciesOffset, out indicies,
						Color.White);
					correction = Vector3.Backward;
					faceCorrection = (int) CubeFace.NegativeZ * 4;
					break;
				case LadderBlock.LadderDirection.East:
					verticies = CreateQuad(CubeFace.NegativeX, offset, Texture, 0, indiciesOffset, out indicies,
						Color.White);
					correction = Vector3.Right;
					faceCorrection = (int) CubeFace.NegativeX * 4;
					break;
				case LadderBlock.LadderDirection.West:
					verticies = CreateQuad(CubeFace.PositiveX, offset, Texture, 0, indiciesOffset, out indicies,
						Color.White);
					correction = Vector3.Left;
					faceCorrection = (int) CubeFace.PositiveX * 4;
					break;
				default:
					// Should never happen
					verticies = CreateUniformCube(offset, Texture, VisibleFaces.All,
						indiciesOffset, out indicies, Color.White);
					correction = Vector3.Zero;
					break;
			}

			for (var i = 0; i < verticies.Length; i++)
				verticies[i].Position += correction;
			for (var i = 0; i < indicies.Length; i++)
				indicies[i] -= faceCorrection;
			return verticies;
		}
	}
}