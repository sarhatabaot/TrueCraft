using System.Linq;
using Microsoft.Xna.Framework;
using TrueCraft.Extensions;
using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;
using TrueCraft.Networking;
using TrueCraft.Networking.Packets;
using TrueCraft.Physics;

namespace TrueCraft.Entities
{
	public class FallingSandEntity : ObjectEntity, IAABBEntity
	{
		public FallingSandEntity(Vector3 position) => _Position = position + new Vector3(0.5f);

		public override byte EntityType => 70;

		public override IPacket SpawnPacket =>
			new SpawnGenericEntityPacket(EntityId, (sbyte) EntityType,
				MathHelper.CreateAbsoluteInt(Position.X), MathHelper.CreateAbsoluteInt(Position.Y),
				MathHelper.CreateAbsoluteInt(Position.Z), 0, null, null, null);

		public override int Data => 1;

		public override Size Size => new Size(0.98);

		public void TerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection)
		{
			if (Despawned)
				return;
			if (collisionDirection == Vector3.Down)
			{
				var Id = SandBlock.BlockId;
				if (EntityType == 71)
					Id = GravelBlock.BlockId;
				EntityManager.DespawnEntity(this);
				var position = (Coordinates3D) collisionPoint + Coordinates3D.Up;
				var hit = World.BlockRepository.GetBlockProvider(World.GetBlockId(position));
				if (hit.BoundingBox == null && BlockProvider.Overwritable.All(o => o != hit.Id))
					EntityManager.SpawnEntity(new ItemEntity(position.AsVector3() + new Vector3(0.5f), new ItemStack(Id)));
				else
					World.SetBlockId(position, Id);
			}
		}

		public BoundingBox BoundingBox => new BoundingBox(Position - Size.AsVector3() / 2, Position + Size.AsVector3() / 2);

		public bool BeginUpdate()
		{
			EnablePropertyChange = false;
			return true;
		}

		public void EndUpdate(Vector3 newPosition)
		{
			EnablePropertyChange = true;
			Position = newPosition;
		}

		public float AccelerationDueToGravity => 0.8f;

		public float Drag => 0.40f;

		public float TerminalVelocity => 39.2f;
	}
}