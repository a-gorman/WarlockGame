//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game
{
	internal static class Input
	{
		private static KeyboardState _keyboardState, _lastKeyboardState;
		private static MouseState _mouseState, _lastMouseState;
		private static GamePadState _gamepadState, _lastGamepadState;

		private static bool _isAimingWithMouse = false;

		public static Vector2 MousePosition => new Vector2(_mouseState.X, _mouseState.Y);

		public static void Update()
		{
			_lastKeyboardState = _keyboardState;
			_lastMouseState = _mouseState;
			_lastGamepadState = _gamepadState;

			_keyboardState = Keyboard.GetState();
			_mouseState = Mouse.GetState();
			_gamepadState = GamePad.GetState(PlayerIndex.One);

			// If the player pressed one of the arrow keys or is using a gamepad to aim, we want to disable mouse aiming. Otherwise,
			// if the player moves the mouse, enable mouse aiming.
			if (new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(x => _keyboardState.IsKeyDown(x)) || _gamepadState.ThumbSticks.Right != Vector2.Zero)
				_isAimingWithMouse = false;
			else if (MousePosition != new Vector2(_lastMouseState.X, _lastMouseState.Y))
				_isAimingWithMouse = true;
		}

		// Checks if a key was just pressed down
		public static bool WasKeyPressed(Keys key)
		{
			return _lastKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key);
		}
		
		public static bool WasLeftMousePressed()
		{
			return _mouseState.LeftButton == ButtonState.Pressed &&  _lastMouseState.LeftButton == ButtonState.Released;
		}
		
		public static bool WasRightMousePressed()
		{
			return _mouseState.RightButton == ButtonState.Pressed &&  _lastMouseState.RightButton == ButtonState.Released;
		}

		public static bool WasButtonPressed(Buttons button)
		{
			return _lastGamepadState.IsButtonUp(button) && _gamepadState.IsButtonDown(button);
		}

		public static Vector2 GetMovementDirection()
		{
			
			Vector2 direction = _gamepadState.ThumbSticks.Left;
			direction.Y *= -1;	// invert the y-axis

			if (_keyboardState.IsKeyDown(Keys.A))
				direction.X -= 1;
			if (_keyboardState.IsKeyDown(Keys.D))
				direction.X += 1;
			if (_keyboardState.IsKeyDown(Keys.W))
				direction.Y -= 1;
			if (_keyboardState.IsKeyDown(Keys.S))
				direction.Y += 1;

			// Clamp the length of the vector to a maximum of 1.
			if (direction.LengthSquared() > 1)
				direction.Normalize();

			return direction;
		}

		public static Vector2 GetAimDirection(Vector2 relativeTo)
		{
			if (_isAimingWithMouse)
				return GetMouseAimDirection(relativeTo);

			Vector2 direction = _gamepadState.ThumbSticks.Right;
			direction.Y *= -1;

			if (_keyboardState.IsKeyDown(Keys.Left))
				direction.X -= 1;
			if (_keyboardState.IsKeyDown(Keys.Right))
				direction.X += 1;
			if (_keyboardState.IsKeyDown(Keys.Up))
				direction.Y -= 1;
			if (_keyboardState.IsKeyDown(Keys.Down))
				direction.Y += 1;

			return direction.ToNormalizedOrZero();
		}

		private static Vector2 GetMouseAimDirection(Vector2 relativeTo)
		{
			Vector2 direction = MousePosition - relativeTo;
			return direction.ToNormalizedOrZero();
		}

		public static bool WasBombButtonPressed()
		{
			return WasButtonPressed(Buttons.LeftTrigger) || WasButtonPressed(Buttons.RightTrigger) || WasKeyPressed(Keys.Space);
		}
	}
}