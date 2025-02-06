using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using PS4Mono;
using WarlockGame.Core.Game;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Effect;
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
    private readonly Simulation _simulation = Simulation.Instance;
    private readonly Queue<ServerProcessedTick> _serverTicks = new();
    // Map of player Ids to most recent tick processed
    private readonly Dictionary<int, int> _clientTicksProcessed = new();
    public enum GameState { WaitingToStart, Running }

    public GameState State { get; set; } = GameState.WaitingToStart;

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
        
        UIManager.AddComponent(LogDisplay.Instance);

#if DEBUG
        LogDisplay.Instance.Visible = true;
        LogDisplay.Instance.DisplayLevel = Logger.Level.DEBUG;
#endif
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

        NetworkManager.Update();
        InputManager.Update();
        CommandManager.Update(_simulation.Tick);
        if (State == GameState.Running && !IsStutterRequired()) {
            _simulation.Update(CommandManager.CurrentSimulationCommands);
            var checksum = _simulation.Checksum;

            if (NetworkManager.IsServer) {
                NetworkManager.Send(new ServerTickProcessed
                {
                    Tick = Simulation.Instance.Tick, 
                    Checksum = checksum,
                    ServerCommands = CommandManager.ProcessedServerCommands.ToList(),
                    PlayerCommands = CommandManager.CurrentSimulationCommands.ToList()
                });
            }
            else if (NetworkManager.IsClient) {
                var serverProcessedTick = _serverTicks.Dequeue();
                var checksumMatched = serverProcessedTick.Checksum == checksum;
                if (!checksumMatched) {
                    Logger.Warning($"Checksum does not match. Actual: '{checksum}' Expected: '{serverProcessedTick.Checksum}'");
                }

                NetworkManager.SendReusable(new ClientTickProcessed
                {
                    Tick = Simulation.Instance.Tick, 
                    ChecksumMatched = checksumMatched
                });
            }
        }
        UIManager.Update();
        ParticleManager.Update();
            
        Grid.Update();
            
        base.Update(gameTime);
    }

    public void RestartGame(int seed) {
        Logger.Info("Restarting game");
        
        ParticleManager.Clear();
        CommandManager.ClearSimulationCommands();
        _simulation.Restart(seed);
        State = GameState.Running;
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
    }
}