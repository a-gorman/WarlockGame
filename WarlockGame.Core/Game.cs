global using Vector2 = Microsoft.Xna.Framework.Vector2;
global using OneOf;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
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
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.UI.Components;

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
    private SpriteBatch _spriteBatch = null!;
    private readonly BloomComponent _bloom;
    internal Simulation Simulation { get; private set; } = null!;
    private readonly Queue<ServerTickProcessed> _serverTicks = new();
    // Map of player Ids to most recent tick processed
    private readonly Dictionary<int, int> _clientTicksProcessed = new();

    public enum GameState { WaitingToStart, Running }
    public enum ClientTypeState { Local, Client, Server }
    
    public GameState State { get; set; } = GameState.WaitingToStart;
    public static ClientTypeState ClientType => !NetworkManager.IsConnected ? ClientTypeState.Local 
        : NetworkManager.IsClient ? ClientTypeState.Client : ClientTypeState.Server;

    public WarlockGame(params string[] args) {
        Configuration.ParseArgs(new ConfigurationBuilder().AddIniFile("settings.ini").AddCommandLine(args).Build());

        Logger.Info($"Settings loaded with command line arguments: {args}", Logger.LogType.Program);
        
        Instance = this;
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = Configuration.ScreenWidth;
        _graphics.PreferredBackBufferHeight = Configuration.ScreenHeight;

        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.25f, 4, 2, 1, 1.5f, 1);
        _bloom.Visible = false;

        Window.Position = Vector2.Zero.ToPoint();
        Window.IsBorderless = true;
        Window.Title = Configuration.WindowName;
    }

    protected override void BeginRun() {
        Simulation = new Simulation();
        
        Ps4Input.Initialize(this);
        InputManager.Initialize(Configuration.KeyMappings);

        Window.TextInput += (_, textArgs) => InputManager.OnTextInput(textArgs);
        
        UIManager.AddComponent(LogDisplay.Instance);
        UIManager.AddComponent(MessageDisplay.Instance);
        UIManager.AddComponent(new SpellDisplay(Configuration.KeyMappings));
        UIManager.AddComponent(new MainView(Simulation));

        LogDisplay.Instance.SetDisplayLevel(Configuration.LogDisplayLevel);
        LogDisplay.Instance.Visible = Configuration.LogDisplayVisible;
        Logger.DedupeLevel = Configuration.LogDedupeLevel;
        
        MessageDisplay.Display("Game Started");
        Logger.Info("Game initialized", Logger.LogType.Program);

        if (Configuration.Client) {
            NetworkManager.ConnectToServer(Configuration.JoinIp, 
                () => NetworkManager.JoinGame(Configuration.PlayerName ?? "Default", Configuration.PreferredColor));
        }
        
        if (Configuration.Server) {
            PlayerManager.AddLocalPlayer(Configuration.PlayerName ?? "Default", Configuration.PreferredColor);
            NetworkManager.StartServer();
        }
        
        Simulation.Initialize();
        
        UIManager.AddComponent(new Scoreboard(Simulation.GameRules));
        UIManager.AddComponent(new SpellPicker(3));
    }

    protected override void Initialize()
    {
        Content.RootDirectory = "Content";

        ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);

        const int maxGridPoints = 1600;
        Vector2 gridSpacing = new Vector2((float)Math.Sqrt(Viewport.Width * Viewport.Height / maxGridPoints));
        Grid = new Grid(Viewport.Bounds, gridSpacing);
        
        base.Initialize();
    }
    
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
        var inputState = InputManager.Update();
        UIManager.Update(inputState);
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
                    Logger.Error($"Checksum does not match. Actual: '{result.Checksum}' Expected: '{tick.Checksum}'", Logger.LogType.Network);
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
        
        ParticleManager.Update();
            
        Grid.Update();
            
        base.Update(gameTime);
    }

    public void RestartGame(int seed) {
        Logger.Info("Restarting game", Logger.LogType.Program);
        
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
        // Simulation.EntityManager.Draw(_spriteBatch);
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
            Logger.Error($"Client {playerId} checksum has diverged.", Logger.LogType.Network);
        }
    }

    public void OnServerTickProcessed(ServerTickProcessed serverTickProcessed) {
        _serverTicks.Enqueue(serverTickProcessed);
    }
}