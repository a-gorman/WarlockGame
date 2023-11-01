using System;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace NeonShooter.Core.Game.Util; 

internal static class MathUtil {
	public static Vector2 FromPolar(float angle, float magnitude) {
		return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
	}

	public static T Squared<T>(this T number) where T: INumber<T> {
		return number * number;
	}
	
	public static T Doubled<T>(this T number) where T: INumber<T> {
		return number + number;
	}
}