using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OneOf;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell.Effect;

class ContinuousSpellEffect : IEffect {
    public bool IsExpired { get; set; }
    public required SpellContext Context { get; init; }
    public required OneOf<Vector2, Func<ContinuousSpellEffect, Vector2>> Location { private get; init; }
    public required IReadOnlyCollection<ILocationSpellComponent> Components { get; init; }
    public required GameTimer Timer { get; init; }
    public int RepeatEvery { get; init; } = 1;
    
    public void Update() {
        Timer.Decrement();
        IsExpired |= Timer.IsExpired;

        // TODO: This Doesn't consistently start on the first from
        if (Timer.FramesRemaining % RepeatEvery == 0) {
            var currentLocation = Location.Match(x => x, x => x.Invoke(this));

            foreach (var component in Components) {
                component.Invoke(Context, currentLocation);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch) {
        // TODO: Draw
    }
}