using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class ToolTip: InterfaceComponent {
    private bool _toolTipDisplayed;
    private readonly InterfaceComponent _wrapper;
    
    
    public ToolTip(InterfaceComponent[] wrappedElements) {
        ZIndex = ZLayer.Tooltip;
        
        _wrapper = new InterfaceComponent {
            Layout = Layout.WithUnrestrictedBounds(154, -130, 150, 150, Alignment.TopRight),
            Disabled = true
        };
        
        foreach (var component in wrappedElements) {
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