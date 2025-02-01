using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class ContextMenuForm : FormBase {
    private readonly NinePatchRectElement _mainFrame, _actionsContainerFrame;
    private readonly ControlElement _actionsContainer;
    private readonly VBoxContainerElement _listContainer;
    
    private const float ACTION_SIZE_X = 45.0f, ACTION_SIZE_Y = 20.0f;

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

    private void SetActionsAlpha(float alpha) {
        GetActionsContainerFrame().SetAlpha(alpha);
        GetListContainer().GetDisplayObjects().ForEach(displayObject => {
            if (displayObject is not ActionDisplayButton button) return;
            button.SetAlpha(alpha);
        });
    }

    private void SetMainAlpha(float alpha) => GetMainFrame().SetAlpha(alpha);

    private void SetNWCorner(Vector2 position) => _menuElement.GetElement().SetPosition(position);
    private void SetSECorner(Vector2 position) => _menuElement.GetElement().SetSize(position);

    private void Show() => _menuElement.GetElement().Show();

    public void Hide() {
        if (!_menuElement.GetElement().Visible) return;
        GetListContainer().ClearChildren();
        _menuElement.GetElement().Hide();
    }

    public void Draw(int actionIndex, Vector2 minPos, Vector2 maxPos, float mainAlpha, float actionsAlpha, IObjectBase objData = null) {
        SetNWCorner(minPos);
        SetSECorner(maxPos);
        VBoxContainerElement listContainer = GetListContainer();

        Player player = GameManager.I().GetPlayer();

        if (objData != null) {
            List<Type> validActions = objData.GetValidActions(player, null);

            float minimumWidth = 0;
            List<ActionDisplayButton> buttons = new();
            
            foreach (Type action in validActions) {
                ActionDisplayButton button = new(action);
                button.SetActionName(ActionAtlas.GetActionName(action));
                button.SetActionNum(1 + buttons.Count + ".");
                button.GetNode().SetCustomMinimumSize(new Vector2(0, ACTION_SIZE_Y));
                button.SetAlpha(actionsAlpha);
                minimumWidth = button.GetMinimumWidth() > minimumWidth ? button.GetMinimumWidth() : minimumWidth;
                buttons.Add(button);
            }
            
            listContainer.SetChildren(buttons);

            if (listContainer.IsEmpty())
                actionsAlpha = 0.0f;
            else {
                int displayItems = listContainer.GetChildCount();
                Vector2 size = new(ACTION_SIZE_X + minimumWidth, ACTION_SIZE_Y * displayItems);
                GetActionsContainerFrame().GetNode().SetSize(size);
                listContainer.GetNode().SetSize(size);
            }
        }
        else if (listContainer.IsEmpty()) actionsAlpha = 0.0f;

        if (!listContainer.IsEmpty()) GetAction(actionIndex)?.GrabFocus();

        Show();
        
        SetMainAlpha(mainAlpha);
        SetActionsAlpha(actionsAlpha);
    }

    public ActionDisplayButton GetAction(int index) {
        VBoxContainerElement listContainer = GetListContainer();
        if (listContainer.IsEmpty()) return null;
        int wrappedI = Mathf.Wrap(index, 0, listContainer.GetDisplayObjects().Count);
        return (ActionDisplayButton) listContainer.GetDisplayObjects()[wrappedI];
    }
}