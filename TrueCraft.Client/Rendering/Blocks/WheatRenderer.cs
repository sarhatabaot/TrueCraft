using System;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class WheatRenderer : BlockRenderer
	{
		private readonly Vector2[][] _textures;

		static WheatRenderer() => RegisterRenderer(CropsBlock.BlockId, new WheatRenderer());

		public WheatRenderer()
		{
			var textureMap = new Vector2(8, 5);
			_textures = new Vector2[8][];
			for (var i = 0; i < 8; i++)
			{
				_textures[i] = new[]
				{
					textureMap + Vector2.UnitX + Vector2.UnitY,
					textureMap + Vector2.UnitY,
					textureMap,
					textureMap + Vector2.UnitX
				};
				for (var j = 0; j < _textures[i].Length; j++)
					_textures[i][j] *= new Vector2(16f / 256f);
				textureMap += new Vector2(1, 0);
			}
		}

		public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
			VisibleFaces faces, Tuple<int, int> textureMap, int indicesOffset, out int[] indices)
		{
			// Wheat is rendered by rendering the four vertical faces of a cube, then moving them
			// towards the middle. We also render a second set of four faces so that you can see
			// each face from the opposite side (to avoid culling)
			var texture = _textures[0];
			if (descriptor.Metadata < _textures.Length)
				texture = _textures[descriptor.Metadata];
			indices = new int[4 * 2 * 6];
			var verticies = new VertexPositionNormalColorTexture[4 * 2 * 6];
			int[] _indicies;
			var center = new Vector3(-0.5f, -0.5f, -0.5f);
			for (var _side = 0;
				_side < 4;
				_side++) // Y faces are the last two in the CubeFace enum, so we can just iterate to 4
			{
				var side = (CubeFace) _side;
				var quad = CreateQuad(side, center, texture, 0, indicesOffset, out _indicies, Color.White);
				if (side == CubeFace.NegativeX || side == CubeFace.PositiveX)
					for (var i = 0; i < quad.Length; i++)
					{
						quad[i].Position.X *= 0.5f;
						quad[i].Position += offset;
					}
				else
					for (var i = 0; i < quad.Length; i++)
					{
						quad[i].Position.Z *= 0.5f;
						quad[i].Position += offset;
					}

				Array.Copy(quad, 0, verticies, _side * 4, 4);
				Array.Copy(_indicies, 0, indices, _side * 6, 6);
			}

			indicesOffset += 4 * 6;
			for (var _side = 0; _side < 4; _side++)
			{
				var side = (CubeFace) _side;
				var quad = CreateQuad(side, center, texture, 0, indicesOffset, out _indicies, Color.White);
				if (side == CubeFace.NegativeX || side == CubeFace.PositiveX)
					for (var i = 0; i < quad.Length; i++)
					{
						quad[i].Position.X *= 0.5f;
						quad[i].Position.X = -quad[i].Position.X;
						quad[i].Position += offset;
					}
				else
					for (var i = 0; i < quad.Length; i++)
					{
						quad[i].Position.Z *= 0.5f;
						quad[i].Position.Z = -quad[i].Position.Z;
						quad[i].Position += offset;
					}

				Array.Copy(quad, 0, verticies, _side * 4 + 4 * 4, 4);
				Array.Copy(_indicies, 0, indices, _side * 6 + 6 * 4, 6);
			}

			for (var i = 0; i < verticies.Length; i++)
			{
				verticies[i].Position.Y -= 1 / 16f;
				verticies[i].Position -= center;
			}

			return verticies;
		}
	}
}