using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using PS4Mono;
using WarlockGame.Core.Game;
using WarlockGame.Core.Game.Display;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;
using Warlock = WarlockGame.Core.Game.Entity.Warlock;

namespace WarlockGame.Core;

public class WarlockGame: Microsoft.Xna.Framework.Game
{
    public static WarlockGame Instance { get; private set; }  = null!;
    public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
    public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);
    public static Vector2 ArenaSize => new Vector2(1900, 1000);
    public static GameTime GameTime { get; private set; } = null!;
    public static int Frame { get; set; }
    public static ParticleManager<ParticleState> ParticleManager { get; private set; } = null!;
    public static Grid Grid { get; private set; } = null!;
    public static bool IsLocal => !NetworkManager.IsConnected;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private readonly BloomComponent _bloom;
 
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
        UIManager.AddComponent(new SpellDisplay());
        UIManager.AddComponent(new HealthBarManager());

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
        Debug.Visualize($"Frame: {Frame}", new Vector2(1500, 0));

        if (!NetworkManager.StutterRequired) {
            Frame++;
        }
        GameTime = gameTime;
        StaticInput.Update();

        var logs = Logger.Logs.Select(x => String.Join(": ", x.LevelString(), x.Tick, x.Message)).Take(5);
        Debug.Visualize(logs, Vector2.Zero);
            
        NetworkManager.Update();

        if (!NetworkManager.StutterRequired)
        {
            InputManager.Update();
            UIManager.Update();
            CommandProcessor.Update(Frame);
            PlayerManager.Update();
            EntityManager.Update();
            EffectManager.Update();
            ParticleManager.Update();
                
            Grid.Update();
        }

        if (NetworkManager.IsServer) {
            NetworkManager.Send(new ServerHeartbeat
            {
                Tick = Frame, 
                Checksum = (int) EntityManager.Warlocks.Sum(x => x.Position.X + x.Position.Y),
                ServerCommands = CommandProcessor.ProcessedServerCommands.ToList(),
                PlayerCommands = CommandProcessor.ProcessedPlayerCommands.ToList()
            });
        }
        else if (NetworkManager.IsClient)
        {
            NetworkManager.SendReusable(new ClientHeartbeat { Frame = Frame });
        }
            
        base.Update(gameTime);
    }

    public void RestartGame() {
        ClearGameState();
        foreach (var player in PlayerManager.Players) {
            EntityManager.Add(new Warlock(player.Id));
        }
    }
    
    private void ClearGameState() {
        CommandProcessor.Clear();
        EntityManager.Clear();
        EffectManager.Clear();
        ParticleManager.Clear();
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