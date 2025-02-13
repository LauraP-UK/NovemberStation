
using System.Collections.Generic;
using Godot;

public class RoundCursor : FormBase, ICursor {
    
    private readonly TextureRectElement _cursor;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/CursorRound.tscn",
        CURSOR = "Texture";

    public RoundCursor(string formName) : base(formName, FORM_PATH) {
        TextureRect cursor = FindNode<TextureRect>(CURSOR);
        
        _cursor = new TextureRectElement(cursor);
        _menuElement = new ControlElement(_menu);
    }
    protected override List<IFormObject> GetAllElements() => new() { _cursor };
    protected override void OnDestroy() {}
    public void SetPosition(Vector2 position) => _menu.SetPosition(position);
    public ControlElement GetCursorElement() => _menuElement;
}