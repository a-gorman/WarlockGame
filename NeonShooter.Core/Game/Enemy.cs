//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Display;
using NeonShooter.Core.Game.Effect;

namespace NeonShooter.Core.Game
{
    internal class Enemy : Entity
    {
        public static Random Rand = new();

        private readonly List<IEnumerator<int>> _behaviours = new();
        private int _timeUntilStart = 60;
        public bool IsActive => _timeUntilStart <= 0;
        public int PointValue { get; private set; }

        public Enemy(Texture2D image, Vector2 position) :
            base(new Sprite(image) { Color = Color.White })
        {
            Position = position;
            Radius = image.Width / 2f;
            PointValue = 1;
        }

        public static Enemy CreateSeeker(Vector2 position)
        {
            var enemy = new Enemy(Art.Seeker, position);
            enemy.AddBehaviour(enemy.FollowPlayer(0.9f));
            enemy.PointValue = 2;

            EffectManager.Add(new SpawnIn(Art.Seeker, enemy.Position, enemy.Orientation));

            return enemy;
        }

        public static Enemy CreateWanderer(Vector2 position)
        {
            var enemy = new Enemy(Art.Wanderer, position);
            enemy.AddBehaviour(enemy.MoveRandomly());

            EffectManager.Add(new SpawnIn(Art.Wanderer, enemy.Position, enemy.Orientation));

            return enemy;
        }

        public override void Update()
        {
            if (IsActive)
                ApplyBehaviours();
            else
            {
                _timeUntilStart--;
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, _sprite.Size / 2, NeonShooterGame.ScreenSize - _sprite.Size / 2);

            Velocity *= 0.8f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
            }       
        }

        private void AddBehaviour(IEnumerable<int> behaviour)
        {
            _behaviours.Add(behaviour.GetEnumerator());
        }

        private void ApplyBehaviours()
        {
            for (int i = 0; i < _behaviours.Count; i++)
            {
                if (!_behaviours[i].MoveNext())
                    _behaviours.RemoveAt(i--); // Is this correct?
            }
        }

        public void HandleCollision(Enemy other)
        {
            var d = Position - other.Position;
            Velocity += 10 * d / (d.LengthSquared() + 1);
        }

        public void WasShot()
        {
            IsExpired = true;
            PlayerStatus.AddPoints(PointValue);
            PlayerStatus.IncreaseMultiplier();

            float hue1 = Rand.NextFloat(0, 6);
            float hue2 = (hue1 + Rand.NextFloat(0, 2)) % 6f;
            Color color1 = ColorUtil.HsvToColor(hue1, 0.5f, 1);
            Color color2 = ColorUtil.HsvToColor(hue2, 0.5f, 1);

            for (int i = 0; i < 120; i++)
            {
                float speed = 18f * (1f - 1 / Rand.NextFloat(1, 10));
                var state = new ParticleState()
                {
                    Velocity = Rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1
                };

                Color color = Color.Lerp(color1, color2, Rand.NextFloat(0, 1));
                NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
            }

            Sound.Explosion.Play(0.5f, Rand.NextFloat(-0.2f, 0.2f), 0);
        }

        #region Behaviours

        private IEnumerable<int> FollowPlayer(float acceleration)
        {
            while (true)
            {
                if (!PlayerShip.Instance.IsDead)
                    Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);

                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();

                yield return 0;
            }
        }

        private IEnumerable<int> MoveRandomly()
        {
            float direction = Rand.NextFloat(0, MathHelper.TwoPi);

            while (true)
            {
                direction += Rand.NextFloat(-0.1f, 0.1f);
                direction = MathHelper.WrapAngle(direction);

                for (int i = 0; i < 6; i++)
                {
                    Velocity += MathUtil.FromPolar(direction, 0.4f);
                    Orientation -= 0.05f;

                    var bounds = NeonShooterGame.Viewport.Bounds;
                    bounds.Inflate(-_sprite.Size.X / 2 - 1, -_sprite.Size.Y / 2 - 1);

                    // if the enemy is outside the bounds, make it move away from the edge
                    if (!bounds.Contains(Position.ToPoint()))
                        direction = (NeonShooterGame.ScreenSize / 2 - Position).ToAngle() +
                                    Rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

                    yield return 0;
                }
            }
        }

        #endregion
    }
}