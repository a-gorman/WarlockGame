//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.UX; 

static class Input
{
	private static MouseState _mouseState, _lastMouseState;
	private static GamePadState _gamepadState, _lastGamepadState;

	public static InputType InputType { get; private set; } = InputType.MouseMove;

	public static int ActiveSpellIndex = -1;

	public static Vector2 MousePosition => new Vector2(_mouseState.X, _mouseState.Y);

	private static InputAction[] _inputActions = { InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4 };
	
	public static void Update() {

		KeyboardInput.Update();
		_lastMouseState = _mouseState;
		_lastGamepadState = _gamepadState;

		_mouseState = Mouse.GetState();
		_gamepadState = GamePad.GetState(PlayerIndex.One);

		// If the player pressed one of the arrow keys or is using a gamepad to aim, we want to disable mouse aiming. Otherwise,
		// if the player moves the mouse, enable mouse aiming.
		if (new[] { InputAction.MoveLeft, InputAction.MoveRight, InputAction.MoveUp, InputAction.MoveDown }.Any(KeyboardInput.IsActionKeyDown))
			InputType = InputType.KeyboardMove;
		else if (MousePosition != new Vector2(_lastMouseState.X, _lastMouseState.Y))
			InputType = InputType.MouseMove;
		else if(_gamepadState.ThumbSticks.Right != Vector2.Zero) {
			InputType = InputType.Gamepad;
		}
		
		HandleSpellButtonPressed();

		if (ActiveSpellIndex != -1 && InputType != InputType.Gamepad && WasLeftMousePressed()) {
			PlayerManager.Players.First().Warlock.CastSpell(ActiveSpellIndex); // Assume the left mouse button is always player 1
			ActiveSpellIndex = -1;
		}
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

		if (KeyboardInput.IsActionKeyDown(InputAction.MoveLeft))
			direction.X -= 1;
		if (KeyboardInput.IsActionKeyDown(InputAction.MoveRight))
			direction.X += 1;
		if (KeyboardInput.IsActionKeyDown(InputAction.MoveUp))
			direction.Y -= 1;
		if (KeyboardInput.IsActionKeyDown(InputAction.MoveDown))
			direction.Y += 1;

		// Clamp the length of the vector to a maximum of 1.
		if (direction.LengthSquared() > 1)
			direction.Normalize();

		return direction;
	}

	public static Vector2 GetAimDirection(Vector2 relativeTo)
	{
		if (InputType == InputType.Gamepad) {
			Vector2 direction = _gamepadState.ThumbSticks.Right;
			direction.Y *= -1;
			return direction.ToNormalizedOrZero();
		}

		return GetMouseAimDirection(relativeTo);
	}

	private static Vector2 GetMouseAimDirection(Vector2 relativeTo)
	{
		Vector2 direction = MousePosition - relativeTo;
		return direction.ToNormalizedOrZero();
	}

	public static void HandleSpellButtonPressed()
	{
		switch (InputType) {
			case InputType.MouseMove:
			case InputType.KeyboardMove:

				for (var i = 0; i < _inputActions.Length; i++) {
					if (KeyboardInput.WasActionKeyPressed(_inputActions[i])) {
						ActiveSpellIndex = i;
						break;
					}
				}

				break;
			case InputType.Gamepad:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}

enum InputType {
	MouseMove,
	KeyboardMove,
	Gamepad
}
