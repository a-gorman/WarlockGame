using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

class MainMenu: InterfaceComponent {
    private readonly Basic.Grid _grid;
    
    public MainMenu() {
        Layout = Layout.WithSize(400, 500, Layout.Alignment.Center);
        _grid = new Basic.Grid(rows: 3) { Clickable = ClickableState.PassThrough };
        Clickable = ClickableState.PassThrough;

        var backgroundTexture = new Texture2D(Art.Pixel.GraphicsDevice, 1, 1);
        backgroundTexture.SetData([Color.DarkSlateGray]);

        var hostButton = new Button(backgroundTexture) {
            Layout = Layout.WithMargin(10),
            LeftClick = _ => {
                if (NetworkManager.IsConnected) {
                    MessageDisplay.Display("Already in game!");
                    return;
                }

                UIManager.OpenTextPrompt("Enter name:", name => {
                    WarlockGame.Instance.Host(name, Configuration.PreferredColor);
                });

                Visible = false;
            }
        }.Also(x => x.AddComponent(new TextDisplay { Text = "Host" }));
        var joinButton = new Button(backgroundTexture) {
            Layout = Layout.WithMargin(10), LeftClick = _ => {
                if (NetworkManager.IsConnected) {
                    MessageDisplay.Display("Already in game!");
                    return;
                }
                
                UIManager.OpenTextPrompt("Enter name:",
                    name => {
                        UIManager.OpenTextPrompt("Enter Host IP Address:",
                            ipAddress => {
                                WarlockGame.Instance.ConnectToServer(
                                    ipAddress.NullOrEmptyTo("localhost"), 
                                    name,
                                    Configuration.PreferredColor);
                            });
                    });

                Visible = false;
            }
        }.Also(x => x.AddComponent(new TextDisplay { Text = "Join" }));
        var exitButton = new Button(backgroundTexture) { 
            Layout = Layout.WithMargin(10), 
            LeftClick = _ => { WarlockGame.Instance.Exit(); } 
        }.Also(x => x.AddComponent(new TextDisplay { Text = "Exit" }));
        
        _grid.AddComponentToCell(hostButton, 0, 0);
        _grid.AddComponentToCell(joinButton, 1, 0);
        _grid.AddComponentToCell(exitButton, 2, 0);
        
        AddComponent(_grid);
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pointTexture.SetData([Color.Gray]);
        spriteBatch.Draw(pointTexture, BoundingBox, Color.White);
    }
}