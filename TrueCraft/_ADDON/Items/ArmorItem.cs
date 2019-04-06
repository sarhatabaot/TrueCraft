using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public abstract class ArmorItem : ItemProvider
	{
		public abstract ArmorMaterial Material { get; }

		public virtual short BaseDurability => 0;

		public abstract float BaseArmor { get; }

		public override sbyte MaximumStack => 1;
	}
}