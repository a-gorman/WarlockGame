using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Effect
{
	internal class EffectManager {
		public IReadOnlyList<IEffect> Effects => _effects;
		
		private List<IEffect> _effects = new();

		private bool _isUpdating;
		private readonly List<IEffect> _addedEffects = new();

		public void AddDelayedEffect(Action action, SimTime time) {
			AddEffect(new DelayedEffect { Action = action, Timer = time.ToTimer() });
		}
		
		public void Add(IEffect effect)
		{
			if (!_isUpdating)
				AddEffect(effect);
			else
				_addedEffects.Add(effect);
		}

		private void AddEffect(IEffect effect)
		{
			_effects.Add(effect);
		}

		public void Update()
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

		public void Clear() {
			_effects.Clear();
			_addedEffects.Clear();
		}

		private class DelayedEffect : IEffect {
			public required GameTimer Timer { get; set; }
			public required Action Action { get; init; }
			public bool IsExpired => Timer.IsExpired;
			public void Update() {
				Timer = Timer.Decrement();
				if (Timer.IsExpired) {
					Action.Invoke();
				}
			}

			public void Draw(Vector2 location, SpriteBatch spriteBatch) { }
		}
	}
}