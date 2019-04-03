using System;
using Microsoft.Xna.Framework;
using TrueCraft.API;
using Matrix = TrueCraft.API.Matrix;

namespace TrueCraft
{
	public static class Vector3Extensions
	{
		public static double DistanceTo(this Vector3 v, Vector3 other)
		{
			return Vector3.DistanceSquared(v, other);
		}

		public static double Distance(this Vector3 v) => v.DistanceTo(Vector3.Zero);

		public static Vector3 Round(this Vector3 v)
		{
			return new Vector3((float) Math.Round(v.X), (float) Math.Round(v.Y), (float) Math.Round(v.Z));
		}

		public static Vector3 AsVector3(this Coordinates3D c)
		{
			return new Vector3(c.X, c.Y, c.Z);
		}

		public static Vector3 AsVector3(this Size s)
		{
			return new Vector3((float) s.Width, (float) s.Height, (float) s.Depth);
		}

		public static void Clamp(this Vector3 v, double value)
		{
			if (Math.Abs(v.X) > value)
				v.X = (float) (value * (v.X < 0 ? -1 : 1));
			if (Math.Abs(v.Y) > value)
				v.Y = (float) (value * (v.Y < 0 ? -1 : 1));
			if (Math.Abs(v.Z) > value)
				v.Z = (float) (value * (v.Z < 0 ? -1 : 1));
		}

		public static Vector3 Transform(this Vector3 v, Matrix matrix)
		{
			var x = v.X * matrix.M11 + v.Y * matrix.M21 + v.Z * matrix.M31 + matrix.M41;
			var y = v.X * matrix.M12 + v.Y * matrix.M22 + v.Z * matrix.M32 + matrix.M42;
			var z = v.X * matrix.M13 + v.Y * matrix.M23 + v.Z * matrix.M33 + matrix.M43;
			return new Vector3((float) x, (float) y, (float) z);
		}

		public static Vector3 Floor(this Vector3 v)
		{
			return new Vector3((float) Math.Floor(v.X), (float) Math.Floor(v.Y), (float) Math.Floor(v.Z));
		}
	}
}