using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Effect
{
	internal class EffectManager {
		public IReadOnlyList<IEffect> Effects => _effects;
		
		private readonly List<IEffect> _effects = new();

		private bool _isUpdating;
		private readonly List<IEffect> _addedEffects = new();

		public void AddDelayedEffect(Action action, SimTime time) {
			Add(new DelayedEffect { Action = action, Timer = time.ToTimer() });
		}
		
		public void Add(IEffect effect)
		{
			if (!_isUpdating)
				AddEffect(effect);
			else
				_addedEffects.Add(effect);
		}
		
		public void Update()
		{
			_isUpdating = true;

			foreach (var effect in _effects) {
				effect.Update();
			}

			_isUpdating = false;

			foreach (var effect in _addedEffects) {
				AddEffect(effect);
			}

			_addedEffects.Clear();

			_effects.RemoveAll(x => x.IsExpired);
		}

		public void Clear() {
			_effects.Clear();
			_addedEffects.Clear();
		}

		private void AddEffect(IEffect effect) {
			_effects.Add(effect);
		}

		
		private class DelayedEffect : IEffect {
			public required GameTimer Timer { get; set; }
			public required Action Action { get; init; }
			public bool IsExpired => Timer.IsExpired;
			public void Update() {
				Timer = Timer.Decremented();
				if (Timer.IsExpired) {
					Action.Invoke();
				}
			}

			public void Draw(Vector2 viewOffset, SpriteBatch spriteBatch) { }
		}
	}
}