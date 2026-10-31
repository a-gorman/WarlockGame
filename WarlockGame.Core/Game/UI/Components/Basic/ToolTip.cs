using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class ToolTip: InterfaceComponent {
    private bool _toolTipDisplayed;
    private readonly InterfaceComponent _wrapper;
    
    
    public ToolTip(InterfaceComponent[] components) {
        ZIndex = ZLayer.Tooltip;
        
        _wrapper = new InterfaceComponent {
            Disabled = true
        };
        
        foreach (var component in components) {
            _wrapper.AddComponent(component);
        }
        
        AddComponent(_wrapper);
    }

    protected override void OnUpdate(in UIManager.UpdateArgs args) {
        if (Visible && _toolTipDisplayed && args.MousePosition == null) {
            _toolTipDisplayed = false;
            _wrapper.Disabled = true;
        }

        if (!Visible || !_toolTipDisplayed && args.MousePosition != null) {
            _toolTipDisplayed = true;
            _wrapper.Disabled = false;
        }
    }
}