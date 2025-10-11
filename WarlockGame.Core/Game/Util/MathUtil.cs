using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

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

	public static Rectangle WithOffset(this Rectangle source, Vector2 offset) {
		source.Offset(offset);
		return source;
	}
	
	public static Rectangle WithOffset(this Rectangle source, Point offset) {
		source.Offset(offset);
		return source;
	}
	
	public static Rectangle WithOffset(this Rectangle source, int x, int y) {
		source.Offset(x, y);
		return source;
	}
	
	public static Rectangle AtLocation(this Rectangle source, Vector2 location) {
		source.Location = location.ToPoint();
		return source;
	}
	
	public static Rectangle AtLocation(this Rectangle source, Point location) {
		source.Location = location;
		return source;
	}
	
	public static Rectangle AtLocation(this Rectangle source, int x, int y) {
		source.Location = new Point(x, y);
		return source;
	}
}