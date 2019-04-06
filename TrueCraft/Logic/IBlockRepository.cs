namespace TrueCraft.Logic
{
	/// <summary>
	///  Providers block providers for a server.
	/// </summary>
	public interface IBlockRepository
	{
		/// <summary>
		///  Gets this repository's block provider for the specified block Id. This may return null
		///  if the block Id in question has no corresponding block provider.
		/// </summary>
		IBlockProvider GetBlockProvider(byte Id);

		/// <summary>
		///  Registers a new block provider. This overrides any existing block providers that use the
		///  same block Id.
		/// </summary>
		void RegisterBlockProvider(IBlockProvider provider);
	}
}