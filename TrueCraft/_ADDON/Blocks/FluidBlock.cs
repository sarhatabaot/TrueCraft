﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TrueCraft.Logic;
using TrueCraft.Networking;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft._ADDON.Blocks
{
	public abstract class FluidBlock : BlockProvider
	{
		private static readonly Coordinates3D[] Neighbors =
		{
			Coordinates3D.North,
			Coordinates3D.South,
			Coordinates3D.East,
			Coordinates3D.West
		};
		// Fluids in Minecraft propegate according to a set of rules as cellular automata.
		// Source blocks start at zero and each block progressively further from the source
		// is one greater than the largest value nearby. When they reach
		// MaximumFluidDepletion, the fluid stops propgetating.

		public abstract override byte Id { get; }

		public override BoundingBox? BoundingBox => null;

		protected abstract double SecondsBetweenUpdates { get; }
		protected abstract byte MaximumFluidDepletion { get; }
		protected abstract byte FlowingID { get; }
		protected abstract byte StillID { get; }

		protected virtual bool AllowSourceCreation => true;

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new ItemStack[0];
		}

		public void ScheduleNextEvent(Coordinates3D coords, IWorld world, IMultiPlayerServer server)
		{
			if (world.GetBlockId(coords) == StillID)
				return;
			var chunk = world.FindChunk(coords);
			server.Scheduler.ScheduleEvent("fluid", chunk,
				TimeSpan.FromSeconds(SecondsBetweenUpdates), _server =>
					AutomataUpdate(_server, world, coords));
		}

		public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
		{
			if (Id == FlowingID)
				ScheduleNextEvent(descriptor.Coordinates, world, user.Server);
		}

		public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiPlayerServer server,
			IWorld world)
		{
			if (Id == StillID)
			{
				var outward = DetermineOutwardFlow(world, descriptor.Coordinates);
				var inward = DetermineInwardFlow(world, descriptor.Coordinates);
				if (outward.Length != 0 || inward != descriptor.Metadata)
				{
					world.SetBlockId(descriptor.Coordinates, FlowingID);
					ScheduleNextEvent(descriptor.Coordinates, world, server);
				}
			}
		}

		public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiPlayerServer server, IWorld world)
		{
			ScheduleNextEvent(coords, world, server);
		}

		private void AutomataUpdate(IMultiPlayerServer server, IWorld world, Coordinates3D coords)
		{
			if (world.GetBlockId(coords) != FlowingID && world.GetBlockId(coords) != StillID)
				return;
			server.BlockUpdatesEnabled = false;
			var again = DoAutomata(server, world, coords);
			server.BlockUpdatesEnabled = true;
			if (again)
			{
				var chunk = world.FindChunk(coords);
				server.Scheduler.ScheduleEvent("fluid", chunk,
					TimeSpan.FromSeconds(SecondsBetweenUpdates), _server =>
						AutomataUpdate(_server, world, coords));
			}
		}

		public bool DoAutomata(IMultiPlayerServer server, IWorld world, Coordinates3D coords)
		{
			var previousLevel = world.GetMetadata(coords);

			var inward = DetermineInwardFlow(world, coords);
			var outward = DetermineOutwardFlow(world, coords);

			if (outward.Length == 1 && outward[0].TargetBlock == coords + Coordinates3D.Down)
			{
				// Exit early if we have placed a fluid block beneath us (and we aren't a source block)
				FlowOutward(world, outward[0], server);
				if (previousLevel != 0)
					return true;
			}

			// Process inward flow
			if (inward > MaximumFluidDepletion)
			{
				world.SetBlockId(coords, 0);
				return true;
			}

			world.SetMetadata(coords, inward);
			if (inward == 0 && previousLevel != 0) return true;

			// Process outward flow
			for (var i = 0; i < outward.Length; i++)
				FlowOutward(world, outward[i], server);
			// Set our block to still fluid if we are done spreading.
			if (outward.Length == 0 && inward == previousLevel)
			{
				world.SetBlockId(coords, StillID);
				return false;
			}

			return true;
		}

		private void FlowOutward(IWorld world, LiquidFlow target, IMultiPlayerServer server)
		{
			// For each block we can flow into, generate an item entity if appropriate
			var provider = world.BlockRepository.GetBlockProvider(world.GetBlockId(target.TargetBlock));
			provider.GenerateDropEntity(new BlockDescriptor {Coordinates = target.TargetBlock, Id = provider.Id}, world,
				server, ItemStack.EmptyStack);
			// And overwrite the block with a new fluid block.
			world.SetBlockId(target.TargetBlock, FlowingID);
			world.SetMetadata(target.TargetBlock, target.Level);
			var chunk = world.FindChunk(target.TargetBlock);
			server.Scheduler.ScheduleEvent("fluid", chunk,
				TimeSpan.FromSeconds(SecondsBetweenUpdates),
				s => AutomataUpdate(s, world, target.TargetBlock));
			if (FlowingID == LavaBlock.BlockId)
				(BlockRepository.GetBlockProvider(FireBlock.BlockId) as FireBlock).ScheduleUpdate(
					server, world, world.GetBlockData(target.TargetBlock));
		}

		/// <summary>
		///  Examines neighboring blocks and determines the new fluid level that this block should adopt.
		/// </summary>
		protected byte DetermineInwardFlow(IWorld world, Coordinates3D coords)
		{
			var currentLevel = world.GetMetadata(coords);
			var up = world.GetBlockId(coords + Coordinates3D.Up);
			if (up == FlowingID || up == StillID) // Check for fluid above us
				return currentLevel;
			if (currentLevel != 0)
			{
				byte highestNeighboringFluid = 15;
				var neighboringSourceBlocks = 0;
				for (var i = 0; i < Neighbors.Length; i++)
				{
					var nId = world.GetBlockId(coords + Neighbors[i]);
					if (nId == FlowingID || nId == StillID)
					{
						var neighborLevel = world.GetMetadata(coords + Neighbors[i]);
						if (neighborLevel < highestNeighboringFluid)
							highestNeighboringFluid = neighborLevel;
						if (neighborLevel == 0)
							neighboringSourceBlocks++;
					}
				}

				if (neighboringSourceBlocks >= 2 && AllowSourceCreation)
					currentLevel = 0;
				if (highestNeighboringFluid > 0)
					currentLevel = (byte) (highestNeighboringFluid + 1);
			}

			return currentLevel;
		}

		/// <summary>
		///  Produces a list of outward flow targets that this block may flow towards.
		/// </summary>
		protected LiquidFlow[] DetermineOutwardFlow(IWorld world, Coordinates3D coords)
		{
			// The maximum distance we will search for lower ground to flow towards
			const int dropCheckDistance = 5;

			var outwardFlow = new List<LiquidFlow>(5);

			var currentLevel = world.GetMetadata(coords);
			var blockBelow = world.BlockRepository.GetBlockProvider(world.GetBlockId(coords + Coordinates3D.Down));
			if (blockBelow.Hardness == 0 && blockBelow.Id != FlowingID && blockBelow.Id != StillID)
			{
				outwardFlow.Add(new LiquidFlow(coords + Coordinates3D.Down, 1));
				if (currentLevel != 0)
					return outwardFlow.ToArray();
			}

			if (currentLevel < MaximumFluidDepletion)
			{
				// This code is responsible for seeking out candidates for flowing towards.
				// Fluid in Minecraft will flow in the direction of the nearest drop-off where
				// there is at least one block removed on the Y axis.
				// It will flow towards several equally strong candidates at once.

				var candidateFlowPoints = new List<Coordinates3D>(4);
				var furthestPossibleCandidate = new Coordinates3D(dropCheckDistance + 1, z: dropCheckDistance + 1) +
				                                Coordinates3D.Down;

				var nearestCandidate = furthestPossibleCandidate;
				for (var x = -dropCheckDistance; x < dropCheckDistance; x++)
				for (var z = -dropCheckDistance; z < dropCheckDistance; z++)
				{
					if (Math.Abs(z) + Math.Abs(x) > dropCheckDistance)
						continue;
					var check = new Coordinates3D(x, z: z) + Coordinates3D.Down;
					var c = world.BlockRepository.GetBlockProvider(world.GetBlockId(check + coords));
					if (c.Hardness == 0)
					{
						if (!LineOfSight(world, check + coords, coords))
							continue;
						if (coords.DistanceTo(check + coords) == coords.DistanceTo(nearestCandidate + coords))
							candidateFlowPoints.Add(check);
						if (coords.DistanceTo(check + coords) < coords.DistanceTo(nearestCandidate + coords))
						{
							candidateFlowPoints.Clear();
							nearestCandidate = check;
						}
					}
				}

				if (nearestCandidate == furthestPossibleCandidate)
				{
					candidateFlowPoints.Add(new Coordinates3D(-dropCheckDistance - 1, z: dropCheckDistance + 1) +
					                        Coordinates3D.Down);
					candidateFlowPoints.Add(new Coordinates3D(dropCheckDistance + 1, z: -dropCheckDistance - 1) +
					                        Coordinates3D.Down);
					candidateFlowPoints.Add(new Coordinates3D(-dropCheckDistance - 1, z: -dropCheckDistance - 1) +
					                        Coordinates3D.Down);
				}

				candidateFlowPoints.Add(nearestCandidate);

				// For each candidate, determine if we are actually capable of flowing towards it.
				// We are able to flow through blocks with a hardness of zero, but no others. We are
				// not able to flow through established fluid blocks.
				for (var i = 0; i < candidateFlowPoints.Count; i++)
				{
					var location = candidateFlowPoints[i];
					location.Clamp(1);

					var xCoordinateCheck = new Coordinates3D(x: location.X) + coords;
					var zCoordinateCheck = new Coordinates3D(z: location.Z) + coords;

					var xID = world.BlockRepository.GetBlockProvider(world.GetBlockId(xCoordinateCheck));
					var zID = world.BlockRepository.GetBlockProvider(world.GetBlockId(zCoordinateCheck));

					if (xID.Hardness == 0 && xID.Id != FlowingID && xID.Id != StillID)
						if (outwardFlow.All(f => f.TargetBlock != xCoordinateCheck))
							outwardFlow.Add(new LiquidFlow(xCoordinateCheck, (byte) (currentLevel + 1)));

					if (zID.Hardness == 0 && zID.Id != FlowingID && zID.Id != StillID)
						if (outwardFlow.All(f => f.TargetBlock != zCoordinateCheck))
							outwardFlow.Add(new LiquidFlow(zCoordinateCheck, (byte) (currentLevel + 1)));
				}

				// Occasionally, there are scenarios where the nearest candidate hole is not acceptable, but
				// there is space immediately next to the block. We should fill that space.
				if (outwardFlow.Count == 0 && blockBelow.Id != FlowingID && blockBelow.Id != StillID)
					for (var i = 0; i < Neighbors.Length; i++)
					{
						var b = world.BlockRepository.GetBlockProvider(world.GetBlockId(coords + Neighbors[i]));
						if (b.Hardness == 0 && b.Id != StillID && b.Id != FlowingID)
							outwardFlow.Add(new LiquidFlow(Neighbors[i] + coords, (byte) (currentLevel + 1)));
					}
			}

			return outwardFlow.ToArray();
		}

		/// <summary>
		///  Returns true if the given candidate coordinate has a line-of-sight to the given target coordinate.
		/// </summary>
		private bool LineOfSight(IWorld world, Coordinates3D candidate, Coordinates3D target)
		{
			candidate += Coordinates3D.Up;
			var direction = target - candidate;
			direction.Clamp(1);
			do
			{
				var z = candidate.Z;
				do
				{
					var p = world.BlockRepository.GetBlockProvider(world.GetBlockId(candidate));
					if (p.Hardness != 0)
						return false;
					candidate.Z += direction.Z;
				} while (target.Z != candidate.Z);

				candidate.Z = z;
				candidate.X += direction.X;
			} while (target.X != candidate.X);

			return true;
		}

		/// <summary>
		///  Represents a block that the currently updating fluid block is able to flow outwards into.
		/// </summary>
		protected struct LiquidFlow
		{
			public LiquidFlow(Coordinates3D targetBlock, byte level)
			{
				TargetBlock = targetBlock;
				Level = level;
			}

			/// <summary>
			///  The block to be filled with fluid.
			/// </summary>
			public Coordinates3D TargetBlock;

			/// <summary>
			///  The fluid level to fill the target block with.
			/// </summary>
			public byte Level;
		}
	}
}