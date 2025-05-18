using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PS4Mono;
using WarlockGame.Core.Game;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core;

public class WarlockGame: Microsoft.Xna.Framework.Game
{
    public Configurations Config { get; }
    public static WarlockGame Instance { get; private set; }  = null!;
    public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
    public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);
    public static GameTime GameTime { get; private set; } = null!;
    public static ParticleManager<ParticleState> ParticleManager { get; private set; } = null!;
    public static Grid Grid { get; private set; } = null!;
    public static bool IsLocal => !NetworkManager.IsConnected;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private readonly BloomComponent _bloom;
    internal Simulation Simulation { get; } = new();
    private readonly Queue<ServerTickProcessed> _serverTicks = new();
    // Map of player Ids to most recent tick processed
    private readonly Dictionary<int, int> _clientTicksProcessed = new();

    public enum GameState { WaitingToStart, Running }
    public enum ClientTypeState { Local, Client, Server }
    
    public GameState State { get; set; } = GameState.WaitingToStart;
    public static ClientTypeState ClientType => !NetworkManager.IsConnected ? ClientTypeState.Local 
        : NetworkManager.IsClient ? ClientTypeState.Client : ClientTypeState.Server;

    public WarlockGame(params string[] args) {
        Config = ParseArgs(new ConfigurationBuilder().AddIniFile("settings.ini").AddCommandLine(args).Build());

        Instance = this;
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = Config.ScreenWidth;
        _graphics.PreferredBackBufferHeight = Config.ScreenHeight;

        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.25f, 4, 2, 1, 1.5f, 1);
        _bloom.Visible = false;

        Window.Position = Vector2.Zero.ToPoint();
    }
    
    protected override void Initialize()
    {
        Content.RootDirectory = "Content";

        ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);

        const int maxGridPoints = 1600;
        Vector2 gridSpacing = new Vector2((float)Math.Sqrt(Viewport.Width * Viewport.Height / maxGridPoints));
        Grid = new Grid(Viewport.Bounds, gridSpacing);

        Ps4Input.Initialize(this);
        InputManager.Initialize(Config.KeyMappings);

        Window.TextInput += (_, textArgs) => InputManager.OnTextInput(textArgs);
        
        base.Initialize();
        
        UIManager.AddComponent(LogDisplay.Instance);
        UIManager.AddComponent(MessageDisplay.Instance);
        UIManager.AddComponent(new SpellDisplay(Config.KeyMappings));
        UIManager.AddComponent(new HealthBarManager());

#if DEBUG
        LogDisplay.Instance.DisplayLevel = Logger.Level.DEBUG;
#endif
        
        MessageDisplay.Display("Game Started");
        Logger.Info("Game initialized");

        if (Config.Client) {
            NetworkManager.ConnectToServer(Config.JoinIp, () => NetworkManager.JoinGame(Config.Name ?? "Default"));
        }
        
        if (Config.Server) {
            PlayerManager.AddLocalPlayer(Config.Name ?? "Default");
            NetworkManager.StartServer();
        }
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
        CommandManager.Update();

        switch (ClientType)
        {
            case ClientTypeState.Local:
                if (State == GameState.Running)
                {
                    Simulation.Update(CommandManager.SimulationCommands);
                    CommandManager.SimulationCommands.Clear();
                }

                break;
            case ClientTypeState.Server:
            {
                if (State != GameState.Running || _clientTicksProcessed.Count != 0 && Simulation.Tick > _clientTicksProcessed.Values.Max() + 30)
                    break;
                
                var result = Simulation.Update(CommandManager.SimulationCommands);
                
                NetworkManager.SendSerializable(new ServerTickProcessed
                {
                    Tick = Simulation.Tick, 
                    Checksum = result.Checksum,
                    ServerCommands = CommandManager.ProcessedServerCommands.ToList(),
                    PlayerCommands = CommandManager.SimulationCommands.ToList()
                });
                
                CommandManager.SimulationCommands.Clear();
            }
                break;
            case ClientTypeState.Client:
            {
                if (!_serverTicks.Any())
                    break;
                
                var tick = _serverTicks.Dequeue();

                foreach (var command in tick.ServerCommands)
                {
                    CommandManager.IssueServerCommand(command);
                }

                if (State != GameState.Running)
                {
                    break;
                }

                var result = Simulation.Update(tick.PlayerCommands);
                
                var checksumMatched = tick.Checksum == result.Checksum;
                if (!checksumMatched) {
                    Logger.Warning($"Checksum does not match. Actual: '{result.Checksum}' Expected: '{tick.Checksum}'");
                }

                NetworkManager.Send(new ClientTickProcessed
                {
                    Tick = Simulation.Tick, 
                    ChecksumMatched = checksumMatched
                });
                
                CommandManager.SimulationCommands.Clear();
                break;
            }
        }
        
        UIManager.Update();
        ParticleManager.Update();
            
        Grid.Update();
            
        base.Update(gameTime);
    }

    public void RestartGame(int seed) {
        Logger.Info("Restarting game");
        
        CommandManager.Clear();
        ParticleManager.Clear();
        Simulation.Restart(seed);
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
        Simulation.EntityManager.Draw(_spriteBatch);
        Simulation.EffectManager.Draw(_spriteBatch);
        _spriteBatch.End();

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        Grid.Draw(_spriteBatch);
        ParticleManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);

        UIManager.Draw(_spriteBatch);
    }

    public void ClientTickProcessed(int playerId, ClientTickProcessed clientTickProcessed) {
        _clientTicksProcessed[playerId] = clientTickProcessed.Tick;
        if (!clientTickProcessed.ChecksumMatched) {
            Logger.Error($"Client {playerId} checksum has diverged.");
        }
    }

    public void OnServerTickProcessed(ServerTickProcessed serverTickProcessed) {
        _serverTicks.Enqueue(serverTickProcessed);
    }

    private Configurations ParseArgs(IConfigurationRoot args) {
        return new Configurations {
            Client = args["autoStartClient"]?.Let(bool.Parse) ?? false,
            Server = args["autoStartServer"]?.Let(bool.Parse) ?? false,
            RestartOnJoin = args["autoRestartOnJoin"]?.Let(bool.Parse) ?? false,
            Name = args["playerName"],
            JoinIp = args["joinIp"] ?? "localhost",
            ScreenHeight = args["screenHeight"]?.Let(int.Parse) ?? 1080,
            ScreenWidth = args["screenWidth"]?.Let(int.Parse) ?? 1920,
            KeyMappings = new Dictionary<Keys, InputAction> {
                { ParseKey(args["keymap:spell1"], Keys.Q), InputAction.Spell1 },
                { ParseKey(args["keymap:spell2"], Keys.W), InputAction.Spell2 },
                { ParseKey(args["keymap:spell3"], Keys.E), InputAction.Spell3 },
                { ParseKey(args["keymap:spell4"], Keys.R), InputAction.Spell4 },
                { ParseKey(args["keymap:spell5"], Keys.T), InputAction.Spell5 },
                { ParseKey(args["keymap:spell6"], Keys.D), InputAction.Spell6 },
                { ParseKey(args["keymap:spell7"], Keys.F), InputAction.Spell7 },
                { ParseKey(args["keymap:spell8"], Keys.G), InputAction.Spell8 },
                { ParseKey(args["keymap:spell9"], Keys.C), InputAction.Spell9 },
                { ParseKey(args["keymap:spell10"], Keys.V), InputAction.Spell10 },
                { ParseKey(args["keymap:exit"], Keys.Escape), InputAction.Exit },
                { ParseKey(args["keymap:pause"], Keys.P), InputAction.Pause },
                { ParseKey(args["keymap:openCommandInput"], Keys.Enter), InputAction.OpenCommandInput },
            }
        };
    }

    private Keys ParseKey(string? str, Keys defaultValue) {
        return Enum.TryParse(str, true, out Keys key) ? key : defaultValue;
    }

    public class Configurations {
        public required bool Server { get; init; }
        public required bool Client { get; init; }
        public required string? Name { get; init; }
        public required string JoinIp { get; init; }
        public required int ScreenWidth { get; init; }
        public required int ScreenHeight { get; init; }
        public required Dictionary<Keys, InputAction> KeyMappings { get; init; }
        public bool RestartOnJoin { get; set; }
    }
}