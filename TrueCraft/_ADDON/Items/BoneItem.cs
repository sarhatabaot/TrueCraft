using System;

namespace TrueCraft.Logic.Items
{
	public class BoneItem : ItemProvider
	{
		public static readonly short ItemID = 0x160;

		public override short Id => 0x160;

		public override string DisplayName => "Bone";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(12, 1);
		}
	}
}