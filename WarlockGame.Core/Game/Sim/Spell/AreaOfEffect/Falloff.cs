namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

static class Falloff {

    public struct Axis {
        public Vector2 Displacement;
        public float Max;
        public float Min;
        
        public Axis(Vector2 displacement, float max, float min) {
            Displacement = displacement;
            Max = max;
            Min = min;
        }
    }
    
    public delegate float FalloffFactor(Vector2 displacement, float outerRadius, float innerRadius, float targetRadius);
    public delegate float FalloffFactor2Axis(Axis axis1, Axis axis2, float targetRadius);
    
    public static float Linear(Vector2 displacement, float maxDisplacement, float innerRadius, float targetRadius) {
        if (maxDisplacement == 0) return 1;
        
        return 1 - float.Clamp((displacement.Length() - (innerRadius + targetRadius)) / maxDisplacement, 0, 1);
    }
    
    public static float None(Vector2 displacement, float maxDisplacement, float innerRadius, float targetRadius) {
        return 1;
    }
    
    public static float Axis1Linear(Axis axis1, Axis _, float targetRadius) {
        return Linear(axis1.Displacement, axis1.Max, axis1.Min, targetRadius);
    }
    
    public static float None(Axis axis1, Axis axis2, float targetRadius) {
        return 1;
    }
}