namespace TrueCraft.World
{
	public interface IDecoration
	{
		bool ValidLocation(Coordinates3D location);
		bool GenerateAt(IWorld world, IChunk chunk, Coordinates3D location);
	}
}