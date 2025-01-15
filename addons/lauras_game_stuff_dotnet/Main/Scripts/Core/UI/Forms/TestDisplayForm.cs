
using System;
using System.Collections.Generic;
using Godot;

public class TestDisplayForm : FormBase {
    
    private readonly ControlLayout _container;
    private readonly ScrollDisplayList _scrollDisplay;
    private Action<TestDisplayForm> _onReady;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/TestDisplayForm.tscn",
        DISPLAY_CONTAINER = "Display";

    public TestDisplayForm(string formName) : base(formName, FORM_PATH) {
        Control container = FindNode<Control>(DISPLAY_CONTAINER);
        
        _container = new ControlLayout(container);
        _scrollDisplay = new ScrollDisplayList("ScrollDisplayList");
        
        _scrollDisplay.SetKeyboardEnabled(true);
        
        _menuLayout = new ControlLayout(_menu, _ => {
            _container?.GetContainer().AddChild(_scrollDisplay.GetMenu());
            _onReady?.Invoke(this);
        });

        _menuLayout.Build();
    }
    protected override List<IFormObject> getAllElements() => new() {_container};
    public ScrollDisplayList GetScrollDisplay() => _scrollDisplay;
    
    public void SetOnReady(Action<TestDisplayForm> onReady) => _onReady = onReady;
}