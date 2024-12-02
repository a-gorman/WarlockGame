using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using PS4Mono;
using WarlockGame.Core.Game;
using WarlockGame.Core.Game.Effect;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Util;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core;

public class WarlockGame: Microsoft.Xna.Framework.Game
{
    public static WarlockGame Instance { get; private set; }  = null!;
    public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
    public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);
    public static GameTime GameTime { get; private set; } = null!;
    public static ParticleManager<ParticleState> ParticleManager { get; private set; } = null!;
    public static Grid Grid { get; private set; } = null!;
    public static bool IsLocal => !NetworkManager.IsConnected;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private readonly BloomComponent _bloom;
    private Simulation _simulation = Simulation.Instance;

    public WarlockGame() {
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
        InputManager.Initialize();
        _simulation.Initialize();

        Window.TextInput += (_, textArgs) => InputManager.OnTextInput(textArgs);
        
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

        MediaPlayer.IsRepeating = true;
        // MediaPlayer.Play(Sound.Music);
    }

    protected override void Update(GameTime gameTime)
    {
        GameTime = gameTime;
        StaticInput.Update();

        var logs = Logger.Logs.Select(x => String.Join(": ", x.LevelString(), x.Tick, x.Message)).Take(5);
        Debug.Visualize(logs, Vector2.Zero);
            
        NetworkManager.Update();
        InputManager.Update();
        if (!NetworkManager.StutterRequired) {
            _simulation.Update();
        }
        UIManager.Update();
        ParticleManager.Update();
            
        Grid.Update();

        if (NetworkManager.IsServer) {
            NetworkManager.Send(new ServerTickProcessed
            {
                Tick = Simulation.Instance.Tick, 
                Checksum = SimUtils.CalculateChecksum(),
                ServerCommands = CommandProcessor.ProcessedServerCommands.ToList(),
                PlayerCommands = CommandProcessor.ProcessedPlayerCommands.ToList()
            });
        }
        else if (NetworkManager.IsClient)
        {
            NetworkManager.SendReusable(new ClientTickProcessed { Tick = Simulation.Instance.Tick });
        }
            
        base.Update(gameTime);
    }

    public void RestartGame(int seed) {
        Logger.Info("Restarting game");
        
        ParticleManager.Clear();
        _simulation.Restart(seed);
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

        UIManager.Draw(_spriteBatch);
        
        // DrawDebugInfo();
    }

    private void DrawRightAlignedString(string text, float y)
    {
        var textWidth = Art.Font.MeasureString(text).X;
        _spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
    }

    private void DrawDebugInfo() {
        DrawRightAlignedString($"Mouse POS: {StaticInput.MousePosition}", 65);
    }
}