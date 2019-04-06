using Microsoft.Xna.Framework;

namespace TrueCraft.Extensions
{
	public static class BoundingBoxExtensions
	{
		public static BoundingBox OffsetBy(this BoundingBox b, Vector3 offset) => new BoundingBox(b.Min + offset, b.Max + offset);
		public static Vector3 Center(this BoundingBox b) => (b.Min + b.Max) / 2;
		public static double Height(this BoundingBox b) => b.Max.Y - b.Min.Y;
		public static double Width(this BoundingBox b) => b.Max.X - b.Min.X;
		public static double Depth(this BoundingBox b) => b.Max.Z - b.Min.Z;
		public static double Volume(this BoundingBox b) => b.Width() * b.Height() * b.Depth();
	}
}