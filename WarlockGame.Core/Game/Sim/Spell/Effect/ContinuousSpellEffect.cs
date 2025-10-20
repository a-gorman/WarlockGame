using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using OneOf;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell.Effect;

class ContinuousSpellEffect : IEffect {
    public bool IsExpired { get; set; }
    public required SpellContext Context { get; init; }
    public required OneOf<Vector2, Func<ContinuousSpellEffect, Vector2>> Location { private get; init; }
    public required IReadOnlyCollection<ILocationSpellComponent> Components { get; init; }
    public required GameTimer Timer { get; set; }
    public int RepeatEvery { get; init; } = 1;
    
    public void Update() {
        Timer = Timer.Decrement();
        IsExpired |= Timer.IsExpired;

        // TODO: This Doesn't consistently start on the first tick
        if (Timer.TicksRemaining % RepeatEvery == 0) {
            var currentLocation = Location.Match(x => x, x => x.Invoke(this));

            foreach (var component in Components) {
                component.Invoke(Context, currentLocation);
            }
        }
    }

    public void Draw(Vector2 location, SpriteBatch spriteBatch) { }
}