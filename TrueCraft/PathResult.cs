using System.Collections.Generic;

namespace TrueCraft
{
	public class PathResult
	{
		public int Index;

		public IList<Coordinates3D> Waypoints;

		public PathResult() => Index = 0;
	}
}