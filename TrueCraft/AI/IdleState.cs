using System;
using TrueCraft.Entities;
using TrueCraft.Server;

namespace TrueCraft.AI
{
	public class IdleState : IMobState
	{
		public IdleState(IMobState nextState, DateTime? expiry = null)
		{
			NextState = nextState;
			if (expiry != null)
				Expiry = expiry.Value;
			else
				Expiry = DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(5, 15));
		}

		private DateTime Expiry { get; }
		private IMobState NextState { get; }

		public void Update(IMobEntity entity, IEntityManager manager)
		{
			if (DateTime.UtcNow >= Expiry)
				entity.CurrentState = NextState;
		}
	}
}