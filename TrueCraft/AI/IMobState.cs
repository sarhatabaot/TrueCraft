using TrueCraft.Entities;
using TrueCraft.Server;

namespace TrueCraft.AI
{
	public interface IMobState
	{
		void Update(IMobEntity entity, IEntityManager manager);
	}
}