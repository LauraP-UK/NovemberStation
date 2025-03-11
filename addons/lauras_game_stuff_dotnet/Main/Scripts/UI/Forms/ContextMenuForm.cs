using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class ContextMenuForm : FormBase {
    private readonly NinePatchRectElement _mainFrame, _actionsContainerFrame, _nameContainerFrame;
    private readonly ControlElement _actionsContainer, _nameContainer;
    private readonly VBoxContainerElement _actionListContainer, _nameListContainer;

    private const float ACTION_SIZE_X = 45.0f, ACTION_SIZE_Y = 20.0f;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ContextMenu.tscn",
        MAIN_FRAME = "Frame",
        ACTIONS_CONTAINER = "ActionsContainer",
        ACTIONS_CONTAINER_FRAME = "ActionsContainer/ActionsFrame",
        ACTIONS_CONTAINER_LIST = "ActionsContainer/VList",
        NAME_CONTAINER = "NameContainer",
        NAME_CONTAINER_FRAME = "NameContainer/NameFrame",
        NAME_CONTAINER_LIST = "NameContainer/VList";

    public ContextMenuForm(string formName) : base(formName, FORM_PATH) {
        NinePatchRect mainFrame = FindNode<NinePatchRect>(MAIN_FRAME);
        Control actionsContainer = FindNode<Control>(ACTIONS_CONTAINER);
        NinePatchRect actionsContainerFrame = FindNode<NinePatchRect>(ACTIONS_CONTAINER_FRAME);
        VBoxContainer actionsContainerList = FindNode<VBoxContainer>(ACTIONS_CONTAINER_LIST);
        Control nameContainer = FindNode<Control>(NAME_CONTAINER);
        NinePatchRect nameContainerFrame = FindNode<NinePatchRect>(NAME_CONTAINER_FRAME);
        VBoxContainer nameContainerList = FindNode<VBoxContainer>(NAME_CONTAINER_LIST);

        _mainFrame = new NinePatchRectElement(mainFrame);
        _actionsContainer = new ControlElement(actionsContainer);
        _actionsContainerFrame = new NinePatchRectElement(actionsContainerFrame);
        _actionListContainer = new VBoxContainerElement(actionsContainerList);
        _actionListContainer.SetUniquesOnly(true);
        _nameContainer = new ControlElement(nameContainer);
        _nameContainerFrame = new NinePatchRectElement(nameContainerFrame);
        _nameListContainer = new VBoxContainerElement(nameContainerList);

        _menuElement = new ControlElement(_menu);
        SetCaptureInput(false);

        // TODO: Capture number key input to select actions
    }


    protected override List<IFormObject> GetAllElements() => new()
        { _mainFrame, _actionsContainerFrame, _actionsContainer, _actionListContainer, _nameContainer, _nameContainerFrame, _nameListContainer };

    protected override void OnDestroy() => GetListContainer().ClearChildren();
    public override bool LockMovement() => false;

    public NinePatchRectElement GetMainFrame() => _mainFrame;
    public NinePatchRectElement GetActionsContainerFrame() => _actionsContainerFrame;
    public NinePatchRectElement GetNameContainerFrame() => _nameContainerFrame;
    public VBoxContainerElement GetListContainer() => _actionListContainer;
    public VBoxContainerElement GetNameListContainer() => _nameListContainer;

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
        VBoxContainerElement nameListContainer = GetNameListContainer();

        Player player = GameManager.I().GetPlayer();

        if (objData != null) {
            string displayName = objData.GetDisplayName();
            List<IFormObject> displayNames = nameListContainer.GetDisplayObjects();

            string existingName = displayNames.Count == 0 ? "" : ((NameDisplay)displayNames.First())?.GetDisplayName();
            if (displayName != "" && displayName != existingName) {
                NameDisplay nameDisplay = new(displayName, ACTION_SIZE_Y);
                nameListContainer.SetChildren(new List<NameDisplay>{nameDisplay});

                Vector2 size = new(nameDisplay.GetMinimumWidth() + 20, ACTION_SIZE_Y);
                _nameContainer.GetElement().SetCustomMinimumSize(size);
            }

            List<ActionDisplayButton> currentButtons = listContainer.GetDisplayObjects().Select(obj => (ActionDisplayButton)obj).ToList();

            List<Type> validActions = objData.GetValidActions(player, null);
            List<Type> currentActions = currentButtons.Select(obj => obj.GetAction()).ToList();

            float minimumWidth = 0;

            if (!ArrayUtils.ExactMatch<Type>(validActions, currentActions)) {
                List<ActionDisplayButton> buttons = new();

                foreach (Type action in validActions) {
                    ActionDisplayButton button = new(action);
                    button.SetActionName(ActionAtlas.GetActionName(action));
                    button.SetActionNum(1 + buttons.Count + ".");
                    button.GetNode().SetCustomMinimumSize(new Vector2(0, ACTION_SIZE_Y));
                    button.SetAlpha(actionsAlpha);
                    minimumWidth = Math.Max(minimumWidth, button.GetMinimumWidth());
                    buttons.Add(button);
                }

                listContainer.SetChildren(buttons);
            }
            else
                minimumWidth = currentButtons.Select(button => button.GetMinimumWidth()).Prepend(minimumWidth).Max();

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
        return (ActionDisplayButton)listContainer.GetDisplayObjects()[wrappedI];
    }
}