using System;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Util
{
	internal static class MathUtil
	{
		public static Vector2 FromPolar(float angle, float magnitude)
		{
			return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
		}

		public static Vector2 GetClosetPointOnLineSegment(Vector2 A, Vector2 B, Vector2 point) {
			// Convert to local coordinates
			var AC = point - A;
			var AB = B - A;

			// Get point D by taking the projection of AC onto AB then adding the offset of A
			var D = AC.ProjectedOnto(AB) + A;

			var AD = D - A;
			// D might not be on AB so calculate k of D down AB (aka solve AD = k * AB)
			// We can use either component, but choose larger value to reduce the chance of dividing by zero
			var k = AB.X != 0 ? AD.X / AB.X : AD.Y / AB.Y;

			// Check if D is off either end of the line segment
			if (k <= 0.0) {
				return A;
			} if (k >= 1.0) {
				return B;
			}

			return D;
		}
	}
}