
using System.Collections.Generic;
using Godot;
using NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class ContextMenuForm : FormBase {

    private readonly NinePatchRectElement _mainFrame, _actionsContainerFrame;
    private readonly ControlElement _actionsContainer;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ContextMenu.tscn",
        MAIN_FRAME = "Frame",
        ACTIONS_CONTAINER = "ActionsContainer",
        ACTIONS_CONTAINER_FRAME = "ActionsContainer/ActionsFrame";

    public ContextMenuForm(string formName) : base(formName, FORM_PATH) {
        NinePatchRect mainFrame = FindNode<NinePatchRect>(MAIN_FRAME);
        NinePatchRect actionsContainerFrame = FindNode<NinePatchRect>(ACTIONS_CONTAINER_FRAME);
        Control actionsContainer = FindNode<Control>(ACTIONS_CONTAINER);
        
        _mainFrame = new NinePatchRectElement(mainFrame);
        _actionsContainerFrame = new NinePatchRectElement(actionsContainerFrame);
        _actionsContainer = new ControlElement(actionsContainer);
        
        _menuElement = new ControlElement(_menu);
        SetCaptureInput(false);
    }
    
    
    protected override List<IFormObject> GetAllElements() => new(){_mainFrame, _actionsContainerFrame, _actionsContainer};

    protected override void OnDestroy() { }
    public override bool LockMovement() => false;

    public void SetNWCorner(Vector2 position) => _menuElement.GetElement().SetPosition(position);
    public void SetSECorner(Vector2 position) => _menuElement.GetElement().SetSize(position);
    
    public void Show() => _menuElement.GetElement().Show();
    public void Hide() => _menuElement.GetElement().Hide();
}