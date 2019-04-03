using Moq;
using NUnit.Framework;
using TrueCraft.Logic;
using TrueCraft.Windows;

namespace TrueCraft.Tests.Windows
{
	[TestFixture]
	public class CraftingWindowAreaTest
	{
		[Test]
		public void TestCraftingWindowArea()
		{
			var recipe = new Mock<ICraftingRecipe>();
			recipe.Setup(r => r.Output).Returns(new ItemStack(10));
			var repository = new Mock<ICraftingRepository>();
			repository.Setup(r => r.GetRecipe(It.IsAny<IWindowArea>())).Returns(recipe.Object);

			var area = new CraftingWindowArea(repository.Object, 0);
			area[0] = new ItemStack(11);
			Assert.AreEqual(new ItemStack(10), area[CraftingWindowArea.CraftingOutput]);
		}
	}
}