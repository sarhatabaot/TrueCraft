using TrueCraft.Windows;

namespace TrueCraft.Logic
{
	public interface ICraftingRepository
	{
		ICraftingRecipe GetRecipe(IWindowArea craftingArea);
		bool TestRecipe(IWindowArea craftingArea, ICraftingRecipe recipe, int x, int y);
	}
}