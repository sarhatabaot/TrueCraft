﻿using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class TorchRenderer : BlockRenderer
	{
		private static readonly Vector2
			TextureMap = new Vector2(7, 86); // Note: this is in pixels (torch texture is not a full block)

		private static readonly Vector2[] Texture =
		{
			// Positive Z
			TextureMap + new Vector2(2, 10),
			TextureMap + new Vector2(0, 10),
			TextureMap,
			TextureMap + new Vector2(2, 0),
			// Negative Z
			TextureMap + new Vector2(2, 10),
			TextureMap + new Vector2(0, 10),
			TextureMap,
			TextureMap + new Vector2(2, 0),
			// Positive X
			TextureMap + new Vector2(2, 10),
			TextureMap + new Vector2(0, 10),
			TextureMap,
			TextureMap + new Vector2(2, 0),
			// Negative X
			TextureMap + new Vector2(2, 10),
			TextureMap + new Vector2(0, 10),
			TextureMap,
			TextureMap + new Vector2(2, 0),
			// Positive Y
			TextureMap + new Vector2(2, 2),
			TextureMap + new Vector2(0, 2),
			TextureMap + new Vector2(0, 0),
			TextureMap + new Vector2(2, 0),
			// Negative Y
			TextureMap + new Vector2(2, 4),
			TextureMap + new Vector2(0, 4),
			TextureMap + new Vector2(0, 2),
			TextureMap + new Vector2(2, 2)
		};

		static TorchRenderer()
		{
			RegisterRenderer(TorchBlock.BlockId, new TorchRenderer());
			for (var i = 0; i < Texture.Length; i++)
				Texture[i] /= 256f;
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

			var centerized = new Vector3(7f / 16f, 0, 7f / 16f);
			var cube = CreateUniformCube(Vector3.Zero, Texture, VisibleFaces.All,
				indiciesOffset, out indicies, Color.White, lighting);
			for (var i = 0; i < cube.Length; i++)
			{
				cube[i].Position.X *= 1f / 8f;
				cube[i].Position.Z *= 1f / 8f;
				if (cube[i].Position.Y > 0)
					cube[i].Position.Y *= 5f / 8f;
				switch ((TorchBlock.TorchDirection) descriptor.Metadata)
				{
					case TorchBlock.TorchDirection.West:
						if (cube[i].Position.Y == 0)
							cube[i].Position.X += 8f / 16f;
						else
							cube[i].Position.X += 3f / 16f;
						cube[i].Position.Y += 5f / 16f;
						break;
					case TorchBlock.TorchDirection.East:
						if (cube[i].Position.Y == 0)
							cube[i].Position.X -= 8f / 16f;
						else
							cube[i].Position.X -= 3f / 16f;
						cube[i].Position.Y += 5f / 16f;
						break;
					case TorchBlock.TorchDirection.North:
						if (cube[i].Position.Y == 0)
							cube[i].Position.Z += 8f / 16f;
						else
							cube[i].Position.Z += 3f / 16f;
						cube[i].Position.Y += 5f / 16f;
						break;
					case TorchBlock.TorchDirection.South:
						if (cube[i].Position.Y == 0)
							cube[i].Position.Z -= 8f / 16f;
						else
							cube[i].Position.Z -= 3f / 16f;
						cube[i].Position.Y += 5f / 16f;
						break;
					case TorchBlock.TorchDirection.Ground:
					default:
						// nop
						break;
				}

				cube[i].Position += offset;
				cube[i].Position += centerized;
			}

			return cube;
		}
	}
}