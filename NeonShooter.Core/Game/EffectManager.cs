//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Graphics.Effect;

namespace NeonShooter.Core.Game
{
	internal static class EffectManager
	{
		private static List<IEffect> _effects = new();

		private static bool _isUpdating;
		private static readonly List<IEffect> _addedEffects = new();

		public static int Count => _effects.Count;

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
	}
}