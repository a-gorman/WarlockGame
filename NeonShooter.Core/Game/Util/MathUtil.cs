using System;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Util; 

internal static class MathUtil {
	public static Vector2 FromPolar(float angle, float magnitude) {
		return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
	}
}