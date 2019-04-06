using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public abstract class FoodItem : ItemProvider
	{
		/// <summary>
		///  The amount of health this food restores.
		/// </summary>
		public abstract float Restores { get; }

		//Most foods aren't stackable
		public override sbyte MaximumStack => 1;
	}
}