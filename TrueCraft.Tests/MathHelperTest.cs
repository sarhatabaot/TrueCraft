﻿using NUnit.Framework;
using TrueCraft.API;

namespace TrueCraft.Core.Test
{
	[TestFixture]
	public class MathHelperTest
	{
		[Test]
		public void TestCreateRotationByte()
		{
			var a = (byte) MathHelper.CreateRotationByte(0);
			var b = (byte) MathHelper.CreateRotationByte(180);
			var c = (byte) MathHelper.CreateRotationByte(359);
			var d = (byte) MathHelper.CreateRotationByte(360);
			Assert.AreEqual(0, a);
			Assert.AreEqual(128, b);
			Assert.AreEqual(255, c);
			Assert.AreEqual(0, d);
		}

		[Test]
		public void TestGetCollisionPoint()
		{
			var inputs = new[]
			{
				Directions.Down,
				Directions.Up,
				Directions.Left,
				Directions.Right,
				Directions.Forwards,
				Directions.Backwards
			};
			var results = new[]
			{
				MathHelper.GetCollisionPoint(inputs[0]),
				MathHelper.GetCollisionPoint(inputs[1]),
				MathHelper.GetCollisionPoint(inputs[2]),
				MathHelper.GetCollisionPoint(inputs[3]),
				MathHelper.GetCollisionPoint(inputs[4]),
				MathHelper.GetCollisionPoint(inputs[5])
			};
			var expected = new[]
			{
				CollisionPoint.NegativeY,
				CollisionPoint.PositiveY,
				CollisionPoint.NegativeX,
				CollisionPoint.PositiveX,
				CollisionPoint.PositiveZ,
				CollisionPoint.NegativeZ
			};
			for (var i = 0; i < expected.Length; i++) Assert.AreEqual(expected[i], results[i]);
		}
	}
}