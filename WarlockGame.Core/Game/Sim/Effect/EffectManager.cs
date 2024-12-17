using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Effect
{
	internal static class EffectManager
	{
		private static List<IEffect> _effects = new();

		private static bool _isUpdating;
		private static readonly List<IEffect> _addedEffects = new();

		public static void AddDelayedEffect(Action action, GameTimer timer) {
			AddEffect(new DelayedEffect { Action = action, Timer = timer});
		}
		
		public static void Add(IEffect effect)
		{
			if (!_isUpdating)
				AddEffect(effect);
			else
				_addedEffects.Add(effect);
		}

		private static void AddEffect(IEffect effect)
		{
			_effects.Add(effect);
		}

		public static void Update()
		{
			_isUpdating = true;

			foreach (var entity in _effects)
				entity.Update();

			_isUpdating = false;

			foreach (var entity in _addedEffects)
				AddEffect(entity);

			_addedEffects.Clear();

			_effects = _effects.Where(x => !x.IsExpired).ToList();
		}
		
		public static void Draw(SpriteBatch spriteBatch)
		{
			foreach (var effect in _effects)
				effect.Draw(spriteBatch);
		}

		public static void Clear() {
			_effects.Clear();
			_addedEffects.Clear();
		}

		private class DelayedEffect : IEffect {
			public required GameTimer Timer { get; init; }
			public required Action Action { get; init; }
			public bool IsExpired => Timer.IsExpired;
			public void Update() {
				if (Timer.Update()) {
					Action.Invoke();
				}
			}

			public void Draw(SpriteBatch spriteBatch) { }
		}
	}
}