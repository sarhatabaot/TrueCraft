namespace TrueCraft.API
{
	/// <summary>
	///  Enumerates the different types of containment between two bounding boxes.
	/// </summary>
	public enum ContainmentType
	{
		/// <summary>
		///  The two bounding boxes are disjoint.
		/// </summary>
		Disjoint,

		/// <summary>
		///  One bounding box contains the other.
		/// </summary>
		Contains,

		/// <summary>
		///  The two bounding boxes intersect.
		/// </summary>
		Intersects
	}
}