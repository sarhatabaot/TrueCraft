﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Client.Events;
using TrueCraft.Client.Rendering;
using TrueCraft.Core.Lighting;
using TrueCraft.Core.World;
using BoundingBox = TrueCraft.API.BoundingBox;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TrueCraft.Client.Modules
{
	public class ChunkModule : IGraphicalModule
	{
		private static readonly Coordinates2D[] AdjacentCoordinates =
		{
			Coordinates2D.North, Coordinates2D.South,
			Coordinates2D.East, Coordinates2D.West
		};

		private static readonly BlendState ColorWriteDisable = new BlendState
		{
			ColorWriteChannels = ColorWriteChannels.None
		};

		public ChunkModule(TrueCraftGame game)
		{
			Game = game;

			ChunkRenderer = new ChunkRenderer(Game.Client.World, Game, Game.BlockRepository);
			Game.Client.ChunkLoaded += Game_Client_ChunkLoaded;
			Game.Client.ChunkUnloaded += (sender, e) => UnloadChunk(e.Chunk);
			Game.Client.ChunkModified += Game_Client_ChunkModified;
			Game.Client.BlockChanged += Game_Client_BlockChanged;
			ChunkRenderer.MeshCompleted += MeshCompleted;
			ChunkRenderer.Start();
			WorldLighting = new WorldLighting(Game.Client.World.World, Game.BlockRepository);

			OpaqueEffect = new BasicEffect(Game.GraphicsDevice);
			OpaqueEffect.TextureEnabled = true;
			OpaqueEffect.Texture = Game.TextureMapper.GetTexture("terrain.png");
			OpaqueEffect.FogEnabled = true;
			OpaqueEffect.FogStart = 0;
			OpaqueEffect.FogEnd = Game.Camera.Frustum.Far.D * 0.8f;
			OpaqueEffect.VertexColorEnabled = true;
			OpaqueEffect.LightingEnabled = true;

			TransparentEffect = new AlphaTestEffect(Game.GraphicsDevice);
			TransparentEffect.AlphaFunction = CompareFunction.Greater;
			TransparentEffect.ReferenceAlpha = 127;
			TransparentEffect.Texture = Game.TextureMapper.GetTexture("terrain.png");
			TransparentEffect.VertexColorEnabled = true;
			OpaqueEffect.LightingEnabled = true;

			ChunkMeshes = new List<ChunkMesh>();
			IncomingChunks = new ConcurrentBag<Mesh>();
			ActiveMeshes = new HashSet<Coordinates2D>();
		}

		public TrueCraftGame Game { get; set; }
		public ChunkRenderer ChunkRenderer { get; set; }
		public int ChunksRendered { get; set; }

		private HashSet<Coordinates2D> ActiveMeshes { get; }
		private List<ChunkMesh> ChunkMeshes { get; }
		private ConcurrentBag<Mesh> IncomingChunks { get; }
		private WorldLighting WorldLighting { get; }

		private BasicEffect OpaqueEffect { get; }
		private AlphaTestEffect TransparentEffect { get; }

		private void Game_Client_BlockChanged(object sender, BlockChangeEventArgs e)
		{
			var position = e.Position.AsVector3();

			WorldLighting.EnqueueOperation(new BoundingBox(
				position, position + Coordinates3D.One.AsVector3()), false);
			WorldLighting.EnqueueOperation(new BoundingBox(
				position, position + Coordinates3D.One.AsVector3()), true);
			var posA = e.Position;
			posA.Y = 0;
			var posB = e.Position;
			posB.Y = World.Height;
			posB.X++;
			posB.Z++;
			WorldLighting.EnqueueOperation(new BoundingBox(posA.AsVector3(), posB.AsVector3()), true);
			WorldLighting.EnqueueOperation(new BoundingBox(posA.AsVector3(), posB.AsVector3()), false);
			for (var i = 0; i < 100; i++)
				if (!WorldLighting.TryLightNext())
					break;
		}

		private void Game_Client_ChunkModified(object sender, ChunkEventArgs e)
		{
			ChunkRenderer.Enqueue(e.Chunk, true);
		}

		private void Game_Client_ChunkLoaded(object sender, ChunkEventArgs e)
		{
			ChunkRenderer.Enqueue(e.Chunk);
			for (var i = 0; i < AdjacentCoordinates.Length; i++)
			{
				ReadOnlyChunk adjacent = Game.Client.World.GetChunk(
					AdjacentCoordinates[i] + e.Chunk.Coordinates);
				if (adjacent != null)
					ChunkRenderer.Enqueue(adjacent);
			}
		}

		private void MeshCompleted(object sender, RendererEventArgs<ReadOnlyChunk> e)
		{
			IncomingChunks.Add(e.Result);
		}

		private void UnloadChunk(ReadOnlyChunk chunk)
		{
			Game.Invoke(() =>
			{
				ActiveMeshes.Remove(chunk.Coordinates);
				ChunkMeshes.RemoveAll(m => m.Chunk.Coordinates == chunk.Coordinates);
			});
		}

		private void HandleClientPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Position":
					var sorter = new ChunkRenderer.ChunkSorter(new Coordinates3D(
						(int) Game.Client.Position.X, 0, (int) Game.Client.Position.Z));
					Game.Invoke(() => ChunkMeshes.Sort(sorter));
					break;
			}
		}

		public void Update(GameTime gameTime)
		{
			var any = false;
			Mesh _mesh;
			while (IncomingChunks.TryTake(out _mesh))
			{
				any = true;
				var mesh = _mesh as ChunkMesh;
				if (ActiveMeshes.Contains(mesh.Chunk.Coordinates))
				{
					var existing = ChunkMeshes.FindIndex(m => m.Chunk.Coordinates == mesh.Chunk.Coordinates);
					ChunkMeshes[existing] = mesh;
				}
				else
				{
					ActiveMeshes.Add(mesh.Chunk.Coordinates);
					ChunkMeshes.Add(mesh);
				}
			}

			if (any)
				Game.FlushMainThreadActions();
			WorldLighting.TryLightNext();
		}

		public void Draw(GameTime gameTime)
		{
			OpaqueEffect.FogColor = Game.SkyModule.WorldFogColor.ToVector3();
			Game.Camera.ApplyTo(OpaqueEffect);
			Game.Camera.ApplyTo(TransparentEffect);
			OpaqueEffect.AmbientLightColor = TransparentEffect.DiffuseColor = Color.White.ToVector3()
			                                                                  * new Vector3(
				                                                                  0.25f + Game.SkyModule
					                                                                  .BrightnessModifier);

			var chunks = 0;
			Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			for (var i = 0; i < ChunkMeshes.Count; i++)
				if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
				{
					chunks++;
					ChunkMeshes[i].Draw(OpaqueEffect, 0);
					if (!ChunkMeshes[i].IsReady || ChunkMeshes[i].Submeshes != 2)
						Console.WriteLine("Warning: rendered chunk that was not ready");
				}

			Game.GraphicsDevice.BlendState = ColorWriteDisable;
			for (var i = 0; i < ChunkMeshes.Count; i++)
				if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
					ChunkMeshes[i].Draw(TransparentEffect, 1);

			Game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			for (var i = 0; i < ChunkMeshes.Count; i++)
				if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
					ChunkMeshes[i].Draw(TransparentEffect, 1);

			ChunksRendered = chunks;
		}
	}
}