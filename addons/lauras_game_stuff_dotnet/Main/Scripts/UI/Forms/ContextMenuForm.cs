using System;
using System.Collections.Generic;
using Godot;

public class ContextMenuForm : FormBase {
    private readonly NinePatchRectElement _mainFrame, _actionsContainerFrame;
    private readonly ControlElement _actionsContainer;
    private readonly VBoxContainerElement _listContainer;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ContextMenu.tscn",
        MAIN_FRAME = "Frame",
        ACTIONS_CONTAINER = "ActionsContainer",
        ACTIONS_CONTAINER_FRAME = "ActionsContainer/ActionsFrame",
        ACTIONS_CONTAINER_LIST = "ActionsContainer/VList";

    public ContextMenuForm(string formName) : base(formName, FORM_PATH) {
        NinePatchRect mainFrame = FindNode<NinePatchRect>(MAIN_FRAME);
        NinePatchRect actionsContainerFrame = FindNode<NinePatchRect>(ACTIONS_CONTAINER_FRAME);
        Control actionsContainer = FindNode<Control>(ACTIONS_CONTAINER);
        VBoxContainer actionsContainerList = FindNode<VBoxContainer>(ACTIONS_CONTAINER_LIST);

        _mainFrame = new NinePatchRectElement(mainFrame);
        _actionsContainerFrame = new NinePatchRectElement(actionsContainerFrame);
        _actionsContainer = new ControlElement(actionsContainer);
        _listContainer = new VBoxContainerElement(actionsContainerList);
        _listContainer.SetUniquesOnly(true);

        _menuElement = new ControlElement(_menu);
        SetCaptureInput(false);
    }


    protected override List<IFormObject> GetAllElements() => new() { _mainFrame, _actionsContainerFrame, _actionsContainer, _listContainer };

    protected override void OnDestroy() => GetListContainer().ClearChildren();
    public override bool LockMovement() => false;
    public override bool RequiresProcess() => true;

    public override void Process(double delta) { }

    public NinePatchRectElement GetMainFrame() => _mainFrame;
    public NinePatchRectElement GetActionsContainerFrame() => _actionsContainerFrame;
    public VBoxContainerElement GetListContainer() => _listContainer;

    public void SetActionsAlpha(float alpha) {
        GetActionsContainerFrame().SetAlpha(alpha);
        GetListContainer().GetDisplayObjects().ForEach(displayObject => {
            if (displayObject is not ActionDisplayButton button) return;
            button.SetAlpha(alpha);
        });
    }

    public void SetMainAlpha(float alpha) => GetMainFrame().SetAlpha(alpha);

    public void SetNWCorner(Vector2 position) => _menuElement.GetElement().SetPosition(position);
    public void SetSECorner(Vector2 position) => _menuElement.GetElement().SetSize(position);

    public void Show() => _menuElement.GetElement().Show();

    public void Hide() {
        _menuElement.GetElement().Hide();
        GetListContainer().ClearChildren();
    }
}