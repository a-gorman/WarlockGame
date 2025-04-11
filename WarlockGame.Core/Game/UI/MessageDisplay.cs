using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.UI.Basic;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace WarlockGame.Core.Game.UI;

public class MessageDisplay : IInterfaceComponent
{
    public static MessageDisplay Instance { get; } = new();
    
    public int Layer { get; }
    public bool IsExpired => false;
    public bool Visible { get; set; } = true;
 
    private readonly TextDisplay _messageDisplay = new() { Bounds = new Rectangle(50, 600, 400, 100), TextScale = 0.5f };
    
    public IEnumerable<IInterfaceComponent> Components { get; }

    private readonly LinkedList<Message> _messages = [];

    public MessageDisplay()
    {
        Components = [_messageDisplay];
    }

    public void AddMessage(string message)
    {
        var wrapper = new Message
        {
            Text = message,
            FramesRemaining = 5 * 60
        };
        
        _messages.AddFirst(wrapper);

        if (_messages.Count > 5)
        {
            _messages.RemoveLast();
        }

        Recalculate();
    }

    public void Draw(SpriteBatch spriteBatch)
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
        _messageDisplay.Text = string.Join("\n\n", _messages.Select(x => x.Text));
    }
    
    private class Message
    {
        public required string Text { get; init; }
        public required int FramesRemaining { get; set; }
    }
}