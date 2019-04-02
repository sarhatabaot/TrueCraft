using System;

namespace TrueCraft.Client.Rendering
{
	[Flags]
	public enum VisibleFaces
	{
		None = 0,
		North = 1,
		South = 2,
		East = 4,
		West = 8,
		Top = 16,
		Bottom = 32,
		All = North | South | East | West | Top | Bottom
	}
}