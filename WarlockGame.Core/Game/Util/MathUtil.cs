using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace WarlockGame.Core.Game.Util; 

internal static class MathUtil {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 FromPolar(float angle, float magnitude) {
		return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Squared<T>(this T number) where T: INumber<T> {
		return number * number;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Doubled<T>(this T number) where T: INumber<T> {
		return number + number;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNan(this float number) {
		return float.IsNaN(number);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNan(this double number) {
		return double.IsNaN(number);
	}
}