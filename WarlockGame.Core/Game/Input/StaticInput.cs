using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Input.Devices;

namespace WarlockGame.Core.Game.Input; 

static class StaticInput
{
	private static MouseState _mouseState, _lastMouseState;
	private static GamePadState _gamepadState, _lastGamepadState;

	public static Vector2 MousePosition => new Vector2(_mouseState.X, _mouseState.Y);
	
	public static void Update() {

		StaticKeyboardInput.Update();
		_lastMouseState = _mouseState;
		_lastGamepadState = _gamepadState;

		_mouseState = Mouse.GetState();
		_gamepadState = GamePad.GetState(PlayerIndex.One);
	}

	// public static bool WasLeftMousePressed()
	// {
	// 	return _mouseState.LeftButton == ButtonState.Pressed &&  _lastMouseState.LeftButton == ButtonState.Released;
	// }
	// 	
	// public static bool WasRightMousePressed()
	// {
	// 	return _mouseState.RightButton == ButtonState.Pressed &&  _lastMouseState.RightButton == ButtonState.Released;
	// }

	public static bool WasButtonPressed(Buttons button)
	{
		return _lastGamepadState.IsButtonUp(button) && _gamepadState.IsButtonDown(button);
	}
}
