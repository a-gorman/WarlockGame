using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.UI.Components;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace WarlockGame.Core.Game.UI;

public sealed class MessageDisplay : InterfaceComponent
{
    public static MessageDisplay Instance { get; } = new();
 
    private readonly TextDisplay _messageDisplay = new() { TextScale = 0.5f };
    
    private readonly LinkedList<Message> _messages = [];

    public MessageDisplay() {
        BoundingBox = new Rectangle(50, 600, 400, 100);
        AddComponent(_messageDisplay);
    }

    public static void Display(string message)
    {
        Logger.Debug("Displayed Message: " + message);
        
        var wrapper = new Message
        {
            Text = message,
            FramesRemaining = 5 * 60
        };
        
        Instance._messages.AddFirst(wrapper);

        if (Instance._messages.Count > 5)
        {
            Instance._messages.RemoveLast();
        }

        Instance.Recalculate();
    }

    public override void Draw(Vector2 location, SpriteBatch spriteBatch)
    {
        foreach (var message in _messages)
        {
            message.FramesRemaining -= 1;
        }

        if (_messages.Last?.Value is { FramesRemaining: <= 0 })
        {
            _messages.RemoveLast();
            Recalculate();
        }
    }

    private void Recalculate()
    {
        _messageDisplay.Text = string.Join("\n\n", _messages.Reverse().Select(x => x.Text));
    }
    
    private class Message
    {
        public required string Text { get; init; }
        public required int FramesRemaining { get; set; }
    }
}