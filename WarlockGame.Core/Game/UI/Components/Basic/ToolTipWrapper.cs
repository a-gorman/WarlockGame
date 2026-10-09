namespace WarlockGame.Core.Game.UI.Components.Basic;

class ToolTipWrapper: InterfaceComponent {
    private bool _toolTipDisplayed;
    private readonly InterfaceComponent _toolTip;
    
    
    public ToolTipWrapper(InterfaceComponent tooltipComponent, InterfaceComponent wrappedComponent) {
        _toolTip = tooltipComponent;
        Clickable = ClickableState.PassThrough;
        _toolTip.Disabled = true;
        SiblingLayer = 9;
        
        AddComponent(tooltipComponent);
        AddComponent(wrappedComponent);
    }

    protected override void OnUpdate(in UIManager.UpdateArgs args) {
        if (Visible && _toolTipDisplayed && args.MousePosition == null) {
            _toolTipDisplayed = false;
            _toolTip.Disabled = true;
        }

        if (!Visible || !_toolTipDisplayed && args.MousePosition != null) {
            _toolTipDisplayed = true;
            _toolTip.Disabled = false;
        }
    }
}