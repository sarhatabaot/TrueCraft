namespace TrueCraft.Logic
{
	/// <summary>
	///  Providers item providers for a server.
	/// </summary>
	public interface IItemRepository
	{
		/// <summary>
		///  Gets this repository's item provider for the specified item Id. This may return null
		///  if the item Id in question has no corresponding block provider.
		/// </summary>
		IItemProvider GetItemProvider(short Id);

		/// <summary>
		///  Registers a new item provider. This overrides any existing item providers that use the
		///  same item Id.
		/// </summary>
		void RegisterItemProvider(IItemProvider provider);
	}
}