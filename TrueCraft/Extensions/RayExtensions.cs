using Microsoft.Xna.Framework;
using TrueCraft.API;

public static class RayExtensions
{
	/// <summary>
	///  Returns the distance along the ray where it intersects the specified bounding box, if it intersects at all.
	/// </summary>
	public static double? Intersects(this Ray r, BoundingBox box, out BlockFace face)
	{
		face = BlockFace.PositiveY;
		//first test if start in box
		if (r.Position.X >= box.Min.X
			&& r.Position.X <= box.Max.X
			&& r.Position.Y >= box.Min.Y
			&& r.Position.Y <= box.Max.Y
			&& r.Position.Z >= box.Min.Z
			&& r.Position.Z <= box.Max.Z)
			return 0.0f; // here we consider whether the cube is full and origin is in cube so intersect at origin

		//Second we check each face
		var maxT = new Vector3(-1.0f);
		//Vector3 minT = new Vector3(-1.0f);
		//calcul intersection with each faces
		if (r.Direction.X != 0.0f)
		{
			if (r.Position.X < box.Min.X)
				maxT.X = (box.Min.X - r.Position.X) / r.Direction.X;
			else if (r.Position.X > box.Max.X)
				maxT.X = (box.Max.X - r.Position.X) / r.Direction.X;
		}

		if (r.Direction.Y != 0.0f)
		{
			if (r.Position.Y < box.Min.Y)
				maxT.Y = (box.Min.Y - r.Position.Y) / r.Direction.Y;
			else if (r.Position.Y > box.Max.Y)
				maxT.Y = (box.Max.Y - r.Position.Y) / r.Direction.Y;
		}

		if (r.Direction.Z != 0.0f)
		{
			if (r.Position.Z < box.Min.Z)
				maxT.Z = (box.Min.Z - r.Position.Z) / r.Direction.Z;
			else if (r.Position.Z > box.Max.Z)
				maxT.Z = (box.Max.Z - r.Position.Z) / r.Direction.Z;
		}

		//get the maximum maxT
		if (maxT.X > maxT.Y && maxT.X > maxT.Z)
		{
			if (maxT.X < 0.0f)
				return null; // ray go on opposite of face
							 //coordonate of hit point of face of cube
			var coord = r.Position.Z + maxT.X * r.Direction.Z;
			// if hit point coord ( intersect face with ray) is out of other plane coord it miss
			if (coord < box.Min.Z || coord > box.Max.Z)
				return null;
			coord = r.Position.Y + maxT.X * r.Direction.Y;
			if (coord < box.Min.Y || coord > box.Max.Y)
				return null;

			if (r.Position.X < box.Min.X)
				face = BlockFace.NegativeX;
			else if (r.Position.X > box.Max.X)
				face = BlockFace.PositiveX;

			return maxT.X;
		}

		if (maxT.Y > maxT.X && maxT.Y > maxT.Z)
		{
			if (maxT.Y < 0.0f)
				return null; // ray go on opposite of face
							 //coordonate of hit point of face of cube
			var coord = r.Position.Z + maxT.Y * r.Direction.Z;
			// if hit point coord ( intersect face with ray) is out of other plane coord it miss
			if (coord < box.Min.Z || coord > box.Max.Z)
				return null;
			coord = r.Position.X + maxT.Y * r.Direction.X;
			if (coord < box.Min.X || coord > box.Max.X)
				return null;

			if (r.Position.Y < box.Min.Y)
				face = BlockFace.NegativeY;
			else if (r.Position.Y > box.Max.Y)
				face = BlockFace.PositiveY;

			return maxT.Y;
		}
		else //Z
		{
			if (maxT.Z < 0.0f)
				return null; // ray go on opposite of face
							 //coordonate of hit point of face of cube
			var coord = r.Position.X + maxT.Z * r.Direction.X;
			// if hit point coord ( intersect face with ray) is out of other plane coord it miss
			if (coord < box.Min.X || coord > box.Max.X)
				return null;
			coord = r.Position.Y + maxT.Z * r.Direction.Y;
			if (coord < box.Min.Y || coord > box.Max.Y)
				return null;

			if (r.Position.Z < box.Min.Z)
				face = BlockFace.NegativeZ;
			else if (r.Position.Z > box.Max.Z)
				face = BlockFace.PositiveZ;

			return maxT.Z;
		}
	}
}