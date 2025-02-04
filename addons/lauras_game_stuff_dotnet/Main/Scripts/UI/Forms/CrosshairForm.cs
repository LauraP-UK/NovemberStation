using System.Collections.Generic;
using Godot;

public class CrosshairForm : FormBase {
    
    private readonly TextureRectElement _crosshair;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/Crosshair.tscn",
        CROSSHAIR = "CenterContainer/Control/Image";

    public CrosshairForm(string formName) : base(formName, FORM_PATH) {
        TextureRect crosshair = FindNode<TextureRect>(CROSSHAIR);
        _crosshair = new TextureRectElement(crosshair);
        
        SetCaptureInput(false);
        
        _menuElement = new ControlElement(_menu);
    }
    
    protected override List<IFormObject> GetAllElements() => new() { _crosshair };
    protected override void OnDestroy() { }
    public override bool LockMovement() => false;
    public void SetCrosshairImage(string path) => _crosshair.SetTexture(path);
}