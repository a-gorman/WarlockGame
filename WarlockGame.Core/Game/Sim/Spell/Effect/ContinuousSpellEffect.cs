using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell.Effect;

class ContinuousSpellEffect : IEffect {
    public bool IsExpired { get; set; }
    public SpellContext Context { get; }
    public Vector2 Location { get; }
    public IReadOnlyCollection<ILocationSpellComponent> Components { get; }
    public GameTimer Timer { get; private set; }
    public int RepeatEvery { get; }

    public ContinuousSpellEffect( SpellContext context,  
        Vector2 location, 
        SimTime duration, 
        IReadOnlyCollection<ILocationSpellComponent> components,
        SimTime? repeatTime = null) {
        Context = context;
        Components = components;
        Location = location;
        Timer = duration.ToTimer();
        RepeatEvery = repeatTime?.Ticks ?? 1;
    }
    
    public void Update() {
        Timer = Timer.Decremented();
        IsExpired |= Timer.IsExpired;

        // TODO: This Doesn't consistently start on the first tick
        if (Timer.TicksRemaining % RepeatEvery == 0) {
            foreach (var component in Components) {
                component.Invoke(Context, Location);
            }
        }
    }

    public void Draw(Vector2 viewOffset, SpriteBatch spriteBatch) { }
}