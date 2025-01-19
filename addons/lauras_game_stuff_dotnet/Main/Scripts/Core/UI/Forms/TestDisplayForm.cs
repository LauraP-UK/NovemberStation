
using System;
using System.Collections.Generic;
using Godot;
using NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class TestDisplayForm : FormBase {
    
    private readonly ControlElement _container;
    private readonly ScrollDisplayList _scrollDisplay;
    private Action<TestDisplayForm> _onReady;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/TestDisplayForm.tscn",
        DISPLAY_CONTAINER = "Display";

    public TestDisplayForm(string formName) : base(formName, FORM_PATH) {
        Control container = FindNode<Control>(DISPLAY_CONTAINER);
        
        _container = new ControlElement(container);
        _scrollDisplay = new ScrollDisplayList("ScrollDisplayList");
        
        _scrollDisplay.SetKeyboardEnabled(true);
        
        _menuElement = new ControlElement(_menu, _ => {
            _container?.GetElement().AddChild(_scrollDisplay.GetMenu());
            _onReady?.Invoke(this);
        });
    }
    protected override List<IFormObject> GetAllElements() => new() {_container, _scrollDisplay};
    public ScrollDisplayList GetScrollDisplay() => _scrollDisplay;
    
    public void SetOnReady(Action<TestDisplayForm> onReady) => _onReady = onReady;

    protected override void OnDestroy() {
        foreach (IFormObject displayObject in _scrollDisplay.GetDisplayObjects())
            ((ShopItemDisplayButton) displayObject).Destroy();
    }
}