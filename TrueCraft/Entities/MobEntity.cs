using System;
using Microsoft.Xna.Framework;
using TrueCraft.API;
using TrueCraft.API.AI;
using TrueCraft.API.Entities;
using TrueCraft.API.Networking;
using TrueCraft.API.Physics;
using TrueCraft.API.Server;
using TrueCraft.Core.AI;
using TrueCraft.Core.Networking.Packets;
using BoundingBox = TrueCraft.API.BoundingBox;
using Matrix = TrueCraft.API.Matrix;

namespace TrueCraft.Core.Entities
{
	public abstract class MobEntity : LivingEntity, IAABBEntity, IMobEntity
	{
		protected MobEntity()
		{
			Speed = 4;
			CurrentState = new WanderState();
		}

		public abstract sbyte MobType { get; }

		public virtual bool Friendly => true;

		/// <summary>
		///  Mob's current speed in m/s.
		/// </summary>
		public virtual double Speed { get; set; }

		public virtual void TerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection)
		{
			// This space intentionally left blank
		}

		public BoundingBox BoundingBox => new BoundingBox(Position, Position + Size.AsVector3());

		public virtual bool BeginUpdate()
		{
			EnablePropertyChange = false;
			return true;
		}

		public virtual void EndUpdate(Vector3 newPosition)
		{
			EnablePropertyChange = true;
			Position = newPosition;
		}

		public float AccelerationDueToGravity => 1.6f;

		public float Drag => 0.40f;

		public float TerminalVelocity => 78.4f;

		public event EventHandler PathComplete;

		public override IPacket SpawnPacket =>
			new SpawnMobPacket(EntityID, MobType,
				MathHelper.CreateAbsoluteInt(Position.X),
				MathHelper.CreateAbsoluteInt(Position.Y),
				MathHelper.CreateAbsoluteInt(Position.Z),
				MathHelper.CreateRotationByte(Yaw),
				MathHelper.CreateRotationByte(Pitch),
				Metadata);

		public PathResult CurrentPath { get; set; }

		public IMobState CurrentState { get; set; }

		public void Face(Vector3 target)
		{
			var diff = target - Position;
			Yaw = (float) MathHelper.RadiansToDegrees(-(Math.Atan2(diff.X, diff.Z) - Math.PI) +
			                                          Math.PI); // "Flip" over the 180 mark
		}

		public bool AdvancePath(TimeSpan time, bool faceRoute = true)
		{
			var modifier = time.TotalSeconds * Speed;
			if (CurrentPath != null)
			{
				// Advance along path
				var target = CurrentPath.Waypoints[CurrentPath.Index].AsVector3();
				target += new Vector3((float) (Size.Width / 2), 0, (float) (Size.Depth / 2)); // Center it
				target.Y = Position.Y; // TODO: Find better way of doing this
				if (faceRoute)
					Face(target);
				var lookAt = Directions.Forwards.Transform(Matrix.CreateRotationY(MathHelper.ToRadians(-(Yaw - 180) + 180)));
				lookAt *= (float) modifier;
				Velocity = new Vector3(lookAt.X, Velocity.Y, lookAt.Z);
				if (Position.DistanceTo(target) < Velocity.Distance())
				{
					Position = target;
					Velocity = Vector3.Zero;
					CurrentPath.Index++;
					if (CurrentPath.Index >= CurrentPath.Waypoints.Count)
					{
						CurrentPath = null;
						if (PathComplete != null)
							PathComplete(this, null);
						return true;
					}
				}
			}

			return false;
		}

		public override void Update(IEntityManager entityManager)
		{
			if (CurrentState != null)
				CurrentState.Update(this, entityManager);
			else
				AdvancePath(entityManager.TimeSinceLastUpdate);
			base.Update(entityManager);
		}
	}
}