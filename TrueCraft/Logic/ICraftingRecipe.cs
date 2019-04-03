namespace TrueCraft.Logic
{
	public interface ICraftingRecipe
	{
		ItemStack[,] Pattern { get; }
		ItemStack Output { get; }
		bool SignificantMetadata { get; }
	}
}