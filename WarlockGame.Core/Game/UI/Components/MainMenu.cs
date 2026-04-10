using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

class MainMenu: InterfaceComponent {
    private readonly Grid _mainGrid;
    private readonly Grid _joinGrid;
    private readonly Grid _hostGrid;
    private MenuState _state = MenuState.Main;

    private readonly Texture2D _buttonTexture;

    public MainMenu() {
        Layout = Layout.WithSize(300, 400, Alignment.Center);
        Clickable = ClickableState.PassThrough;
        Layer = 5;

        _buttonTexture = new Texture2D(Art.Pixel.GraphicsDevice, 1, 1);
        _buttonTexture.SetData([Color.DarkSlateGray]);
        
        _mainGrid = CreateMainGrid();
        AddComponent(_mainGrid);
        
        _joinGrid = CreateJoinGrid();
        AddComponent(_joinGrid);
        
        _hostGrid = CreateHostGrid();
        AddComponent(_hostGrid);
        
        TransitionToMainState();
    }

    private Grid CreateMainGrid() {
        var hostButton = new Button(_buttonTexture) {
            Layout = Layout.WithMargin(20),
            LeftClick = _ => TransitionToHostState()
        }.Also(x => x.AddComponent(new TextDisplay("Host", Alignment.Center)));
        
        var joinButton = new Button(_buttonTexture) {
            Layout = Layout.WithMargin(20), LeftClick = _ => TransitionToJoinState()
        }.Also(x => x.AddComponent(new TextDisplay("Connect", Alignment.Center)));
        
        var exitButton = new Button(_buttonTexture) { 
            Layout = Layout.WithMargin(20), 
            LeftClick = _ => { WarlockGame.Instance.Exit(); } 
        }.Also(x => x.AddComponent(new TextDisplay("Exit", Alignment.Center)));

        return Grid.SingleColumn([hostButton, joinButton, exitButton],
            ClickableState.PassThrough);
    }
    
    private Grid CreateJoinGrid() {
        var playerNameInput = new TextInput(textColor: Color.Black, backgroundColor: Color.White) 
            { Layout = Layout.WithHeight(Art.Font.LineSpacing, alignment: Alignment.Top) };
        
        var joinIpInput = new TextInput(textColor: Color.Black, backgroundColor: Color.White) 
            { Layout = Layout.WithHeight(Art.Font.LineSpacing, alignment: Alignment.Top) };
        
        var connectButton = new Button(_buttonTexture) {
            Layout = Layout.WithMargin(10), 
            LeftClick = _ => WarlockGame.Instance.ConnectToServer(
                joinIpInput.Text.NullOrEmptyTo("localhost"), 
                playerNameInput.Text.NullOrEmptyTo("Default Client"),
                Configuration.PreferredColor)
        }.Also(x => x.AddComponent(new TextDisplay("Connect", Alignment.Center)));
        
        var backButton = new Button(_buttonTexture) {
            Layout = Layout.WithMargin(10), LeftClick = _ => TransitionToMainState()
        }.Also(x => x.AddComponent(new TextDisplay("Back", Alignment.Center)));

        return Grid.SingleColumn([
                Grid.SingleColumn([CreateInputLabel("Player Name:"), playerNameInput], ClickableState.PassThrough),
                Grid.SingleColumn([CreateInputLabel("IP Address:"), joinIpInput], ClickableState.PassThrough),
                connectButton,
                backButton
            ],
            ClickableState.PassThrough,
            Layout.WithMargin(10, 0));
    }
    
    private Grid CreateHostGrid() {
        var playerNameInput = new TextInput(textColor: Color.Black, backgroundColor: Color.White) 
            { Layout = Layout.WithHeight(Art.Font.LineSpacing, alignment: Alignment.Top) };
        
        var startButton = new Button(_buttonTexture) {
            Layout = Layout.WithMargin(10), 
            LeftClick = _ => WarlockGame.Instance.Host(playerNameInput.Text, Configuration.PreferredColor)
        }.Also(x => x.AddComponent(new TextDisplay("Start", Alignment.Center)));
        
        var backButton = new Button(_buttonTexture) {
            Layout = Layout.WithMargin(10), LeftClick = _ => TransitionToMainState()
        }.Also(x => x.AddComponent(new TextDisplay("Back", Alignment.Center)));
        
        return Grid.SingleColumn([
            Grid.SingleColumn([CreateInputLabel("Player name:"), playerNameInput], ClickableState.PassThrough), 
            startButton, 
            backButton],
            ClickableState.PassThrough,
            Layout.WithMargin(10, 0));
    }

    private void TransitionToMainState() {
        _mainGrid.Disabled = false;
        _joinGrid.Disabled = true;
        _hostGrid.Disabled = true;
        _state = MenuState.Main;
    }
    
    private void TransitionToJoinState() {
        _mainGrid.Disabled = true;
        _joinGrid.Disabled = false;
        _hostGrid.Disabled = true;
        _state = MenuState.Join;
    }
    
    private void TransitionToHostState() {
        _mainGrid.Disabled = true;
        _joinGrid.Disabled = true;
        _hostGrid.Disabled = false;
        _state = MenuState.Host;
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pointTexture.SetData([Color.Gray]);
        spriteBatch.Draw(pointTexture, BoundingBox, Color.White);
    }

    private TextDisplay CreateInputLabel(string text) {
        return new TextDisplay(text, Alignment.BottomLeft) {
            TextScale = 0.65f,
            TextColor = Color.Black
        };
    }

    private enum MenuState {
        Main,
        Join,
        Host
    }
}

