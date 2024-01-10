using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NeonShooter.Core.Game;
using NeonShooter.Core.Game.Display;
using NeonShooter.Core.Game.Graphics;
using NeonShooter.Core.Game.Graphics.UI;
using NeonShooter.Core.Game.Log;
using NeonShooter.Core.Game.Networking;
using NeonShooter.Core.Game.Util;
using NeonShooter.Core.Game.UX;
using NeonShooter.Core.Game.UX.InputDevices;
using PS4Mono;

namespace NeonShooter.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NeonShooterGame: Microsoft.Xna.Framework.Game
    {
		// some helpful static properties
		public static NeonShooterGame Instance { get; private set; }  = null!;
		public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
        public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);
        public static GameTime GameTime { get; private set; } = null!;
		public static ParticleManager<ParticleState> ParticleManager { get; private set; } = null!;
		public static Grid Grid { get; private set; } = null!;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly BloomComponent _bloom;

        private bool _paused = false;

        
        public NeonShooterGame()
        {
			Instance = this;
			_graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;

            _bloom = new BloomComponent(this);
			Components.Add(_bloom);
			_bloom.Settings = new BloomSettings(null, 0.25f, 4, 2, 1, 1.5f, 1);
            _bloom.Visible = false;
        }

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";

            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);

            const int maxGridPoints = 1600;
            Vector2 gridSpacing = new Vector2((float)Math.Sqrt(Viewport.Width * Viewport.Height / maxGridPoints));
            Grid = new Grid(Viewport.Bounds, gridSpacing);

            Ps4Input.Initialize(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			Art.Load(Content);
			Sound.Load(Content);

            PlayerManager.AddPlayer("Alex", PlayerManager.DeviceType.MouseAndKeyboard);
            PlayerManager.AddPlayer("John", PlayerManager.DeviceType.PlayStation1);
 
            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = true;
                // MediaPlayer.Play(Sound.Music);
            }
            catch { }

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Input.Update();

            // Allows the game to exit
            if (Input.WasButtonPressed(Buttons.Back) || StaticKeyboardInput.WasKeyPressed(Keys.Escape))
                Exit();

            if (StaticKeyboardInput.WasKeyPressed(Keys.P))
                _paused = !_paused;
            
            if (StaticKeyboardInput.WasKeyPressed(Keys.C))
                NetworkManager.ConnectToServer();
            
            if (StaticKeyboardInput.WasKeyPressed(Keys.V))
                NetworkManager.StartServer();

            Debug.Visualize(Logger.Log.TakeLast(5), Vector2.Zero);
            
            if (!_paused)
            {
                InputDeviceManager.Update();
                PlayerManager.Update();
                EntityManager.Update();
                EffectManager.Update();
                // EnemySpawner.Update();
                ParticleManager.Update();
                
                Grid.Update();
            }
            
            NetworkManager.Update();
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _bloom.BeginDraw();

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            EntityManager.Draw(_spriteBatch);
            EffectManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            Grid.Draw(_spriteBatch);
            ParticleManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);

            // Draw the user interface without bloom
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            var activePlayer = PlayerManager.Players.First();
            
            // DrawTitleSafeAlignedString("Lives: " + activePlayer.Status.Lives, 5);
            DrawTitleSafeRightAlignedString("Score: " + activePlayer.Status.Score, 5);
            DrawTitleSafeRightAlignedString("Multiplier: " + activePlayer.Status.Multiplier, 35);
            // draw the custom mouse cursor
            _spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
            
            SpellDisplay.Draw(_spriteBatch);

            DrawDebugInfo();
            
            if (GameStatus.IsGameOver)
            {
                string text = "Game Over\n" +
                    "Your Score: " + activePlayer.Status.Score + "\n" +
                    "High Score: " + activePlayer.Status.HighScore;

                Vector2 textSize = Art.Font.MeasureString(text);
                _spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
            }

            _spriteBatch.End();
        }

        private void DrawRightAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            _spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
        }

        private void DrawTitleSafeAlignedString(string text, int pos)
        {
            _spriteBatch.DrawString(Art.Font, text, new Vector2(Viewport.TitleSafeArea.X + pos), Color.White);
        }

        private void DrawTitleSafeRightAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            _spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5 - Viewport.TitleSafeArea.X, Viewport.TitleSafeArea.Y + y), Color.White);
        }

        private void DrawDebugInfo() {
            DrawRightAlignedString($"Mouse POS: {Input.MousePosition}", 65);
        }
        
        public void DrawDebugString(string text, Vector2 position) {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            _spriteBatch.DrawString(Art.Font, text, position, Color.White);
            _spriteBatch.End();
        }
    }
}
