﻿using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Rendering
{
	/// <summary>
	/// </summary>
	public class ChunkMesh : Mesh
	{
		/// <summary>
		/// </summary>
		/// <param name="chunk"></param>
		/// <param name="device"></param>
		/// <param name="vertices"></param>
		/// <param name="indices"></param>
		public ChunkMesh(ReadOnlyChunk chunk, TrueCraftGame game, VertexPositionNormalColorTexture[] vertices,
			int[] indices)
			: base(game, 1, true)
		{
			Chunk = chunk;
			Vertices = vertices;
			SetSubmesh(0, indices);
		}

		/// <summary>
		/// </summary>
		/// <param name="chunk"></param>
		/// <param name="device"></param>
		/// <param name="vertices"></param>
		/// <param name="opaqueIndices"></param>
		/// <param name="transparentIndices"></param>
		public ChunkMesh(ReadOnlyChunk chunk, TrueCraftGame game, VertexPositionNormalColorTexture[] vertices,
			int[] opaqueIndices, int[] transparentIndices)
			: base(game, 2, true)
		{
			Chunk = chunk;
			Vertices = vertices;
			SetSubmesh(0, opaqueIndices);
			SetSubmesh(1, transparentIndices);
		}

		/// <summary>
		/// </summary>
		public ReadOnlyChunk Chunk { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="vertices"></param>
		/// <returns></returns>
		protected override BoundingBox RecalculateBounds(VertexPositionNormalColorTexture[] vertices)
		{
			return new BoundingBox(
				new Vector3(Chunk.X * Core.World.Chunk.Width, 0, Chunk.Z * Core.World.Chunk.Depth),
				new Vector3(Chunk.X * Core.World.Chunk.Width
				            + Core.World.Chunk.Width, Core.World.Chunk.Height,
					Chunk.Z * Core.World.Chunk.Depth + Core.World.Chunk.Depth));
		}
	}
}