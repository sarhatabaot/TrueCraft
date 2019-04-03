using Microsoft.Xna.Framework;

namespace TrueCraft
{
	public static class Directions
	{
		/// <summary>
		///  A vector that points upward.
		/// </summary>
		public static readonly Vector3 Up = new Vector3(0, 1, 0);

		/// <summary>
		///  A vector that points downward.
		/// </summary>
		public static readonly Vector3 Down = new Vector3(0, -1, 0);

		/// <summary>
		///  A vector that points to the left.
		/// </summary>
		public static readonly Vector3 Left = new Vector3(-1, 0, 0);

		/// <summary>
		///  A vector that points to the right.
		/// </summary>
		public static readonly Vector3 Right = new Vector3(1, 0, 0);

		/// <summary>
		///  A vector that points backward.
		/// </summary>
		public static readonly Vector3 Backwards = new Vector3(0, 0, -1);

		/// <summary>
		///  A vector that points forward.
		/// </summary>
		public static readonly Vector3 Forwards = new Vector3(0, 0, 1);
		
		/// <summary>
		///  A vector that points to the east.
		/// </summary>
		public static readonly Vector3 East = new Vector3(1, 0, 0);

		/// <summary>
		///  A vector that points to the west.
		/// </summary>
		public static readonly Vector3 West = new Vector3(-1, 0, 0);

		/// <summary>
		///  A vector that points to the north.
		/// </summary>
		public static readonly Vector3 North = new Vector3(0, 0, -1);

		/// <summary>
		///  A vector that points to the south.
		/// </summary>
		public static readonly Vector3 South = new Vector3(0, 0, 1);
	}
}