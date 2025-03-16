using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class ContextMenuForm : FormBase {
    private readonly NinePatchRectElement _mainFrame, _actionsContainerFrame, _contextFrame;
    private readonly ControlElement _actionsContainer, _nameContainer, _contextContainer;
    private readonly VBoxContainerElement _actionListContainer, _nameListContainer, _contextListContainer;

    private const float ACTION_SIZE_X = 45.0f, INFO_BOX_SIZE_Y = 20.0f;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ContextMenu.tscn",
        MAIN_FRAME = "Frame",
        ACTIONS_CONTAINER = "ActionsContainer",
        ACTIONS_CONTAINER_FRAME = "ActionsContainer/ActionsFrame",
        ACTIONS_CONTAINER_LIST = "ActionsContainer/VList",
        NAME_CONTAINER = "NameContainer",
        NAME_CONTAINER_LIST = "NameContainer/VList",
        CONTEXT_CONTAINER = "ContextContainer",
        CONTEXT_FRAME = "ContextContainer/ContextFrame",
        CONTEXT_LIST = "ContextContainer/VList";

    public ContextMenuForm(string formName) : base(formName, FORM_PATH) {
        NinePatchRect mainFrame = FindNode<NinePatchRect>(MAIN_FRAME);

        Control actionsContainer = FindNode<Control>(ACTIONS_CONTAINER);
        NinePatchRect actionsContainerFrame = FindNode<NinePatchRect>(ACTIONS_CONTAINER_FRAME);
        VBoxContainer actionsContainerList = FindNode<VBoxContainer>(ACTIONS_CONTAINER_LIST);

        Control nameContainer = FindNode<Control>(NAME_CONTAINER);
        VBoxContainer nameContainerList = FindNode<VBoxContainer>(NAME_CONTAINER_LIST);

        Control contextContainer = FindNode<Control>(CONTEXT_CONTAINER);
        NinePatchRect contextContainerFrame = FindNode<NinePatchRect>(CONTEXT_FRAME);
        VBoxContainer contextList = FindNode<VBoxContainer>(CONTEXT_LIST);

        _mainFrame = new NinePatchRectElement(mainFrame);

        _actionsContainer = new ControlElement(actionsContainer);
        _actionsContainerFrame = new NinePatchRectElement(actionsContainerFrame);
        _actionListContainer = new VBoxContainerElement(actionsContainerList);
        _actionListContainer.SetUniquesOnly(true);

        _nameContainer = new ControlElement(nameContainer);
        _nameListContainer = new VBoxContainerElement(nameContainerList);

        _contextContainer = new ControlElement(contextContainer);
        _contextFrame = new NinePatchRectElement(contextContainerFrame);
        _contextListContainer = new VBoxContainerElement(contextList);

        _menuElement = new ControlElement(_menu);
        SetCaptureInput(false);

        // TODO: Capture number key input to select actions
    }


    protected override List<IFormObject> GetAllElements() => new()
        { _mainFrame, _actionsContainerFrame, _actionsContainer, _actionListContainer, _nameContainer, _nameListContainer, _contextContainer, _contextFrame, _contextListContainer };

    protected override void OnDestroy() => GetListContainer().ClearChildren();
    public override bool LockMovement() => false;

    public NinePatchRectElement GetMainFrame() => _mainFrame;
    public NinePatchRectElement GetActionsContainerFrame() => _actionsContainerFrame;
    public NinePatchRectElement GetContextFrame() => _contextFrame;
    public VBoxContainerElement GetListContainer() => _actionListContainer;
    public VBoxContainerElement GetNameListContainer() => _nameListContainer;
    public VBoxContainerElement GetContextListContainer() => _contextListContainer;

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

    public void Draw(int actionIndex, Vector2 minPos, Vector2 maxPos, float mainAlpha, float titleAlpha, float actionsAlpha, IObjectBase objData = null) {
        SetNWCorner(minPos);
        SetSECorner(maxPos);
        VBoxContainerElement listContainer = GetListContainer();

        if (objData != null) {
            HandleDisplayName(objData, titleAlpha);
            HandleContextInfo(objData, titleAlpha);
            actionsAlpha = HandleActions(objData, actionsAlpha);
        }
        else if (listContainer.IsEmpty()) actionsAlpha = 0.0f;

        if (!listContainer.IsEmpty() && !UIManager.IsPrimaryMenuOpen()) GetAction(actionIndex)?.GrabFocus();

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

    private void HandleDisplayName(IObjectBase objData, float titleAlpha) {
        VBoxContainerElement nameListContainer = GetNameListContainer();

        string displayName = objData.GetDisplayName();
        List<IFormObject> displayNames = nameListContainer.GetDisplayObjects();

        NameDisplay existingNameDisplay = displayNames.Count == 0 ? null : (NameDisplay)displayNames.First();
        string existingName = existingNameDisplay == null ? "" : existingNameDisplay.GetDisplayName();
        if (displayName != "" && displayName != existingName) {
            NameDisplay newNameDisplay = new(displayName, INFO_BOX_SIZE_Y);
            nameListContainer.SetChildren(new List<NameDisplay> { newNameDisplay });

            Vector2 size = new(newNameDisplay.GetMinimumWidth() + 20, INFO_BOX_SIZE_Y);
            _nameContainer.GetElement().SetCustomMinimumSize(size);
        }

        if (displayNames.Count == 0) return;
        NameDisplay nameDisplay = (NameDisplay)displayNames.First();
        nameDisplay.HandleAlpha(titleAlpha);
    }

    private void HandleContextInfo(IObjectBase objData, float titleAlpha) {
        VBoxContainerElement contextListContainer = GetContextListContainer();

        string context = objData.GetContext();

        if (context == "") {
            contextListContainer.ClearChildren();
            GetContextFrame().SetAlpha(0.0f);
            return;
        }

        List<string> contexts = ProcessContexts(context);
        List<string> existingContexts = contextListContainer.GetDisplayObjects().Select(o => ((ContextInfoElement)o).GetContext()).ToList();

        if (!ArrayUtils.ExactMatch<string>(existingContexts, contexts)) {
            List<ContextInfoElement> elements = contexts.Select(c => new ContextInfoElement(c, ACTION_SIZE_X)).ToList();
            contextListContainer.SetChildren(elements);

            float width = elements.Select(c => c.GetMinimumWidth()).Prepend(0).Max() + 20;
            Vector2 size = new(width, INFO_BOX_SIZE_Y * elements.Count);
            GetContextFrame().GetNode().SetCustomMinimumSize(size);
            contextListContainer.GetNode().SetCustomMinimumSize(size);
        }

        List<ContextInfoElement> contextInfoElements = contextListContainer.GetDisplayObjects().Select(o => (ContextInfoElement)o).ToList();
        foreach (ContextInfoElement c in contextInfoElements) {
            c.HandleAlpha(titleAlpha);
            GetContextFrame().SetAlpha(titleAlpha);
        }
    }

    private float HandleActions(IObjectBase objData, float actionsAlpha) {
        Player player = GameManager.I().GetPlayer();

        VBoxContainerElement listContainer = GetListContainer();
        List<ActionDisplayButton> currentButtons = listContainer.GetDisplayObjects().Select(obj => (ActionDisplayButton)obj).ToList();

        List<ActionKey> validActions = objData.GetValidActions(player, null);
        List<ActionKey> currentActions = currentButtons.Where(obj => obj.GetAction() != null).Select(obj => (ActionKey)obj.GetAction()).ToList();

        float minimumWidth = 0;

        if (!ArrayUtils.ExactMatch<ActionKey>(validActions, currentActions)) {
            List<ActionDisplayButton> buttons = new();

            foreach (ActionKey action in validActions) {
                ActionDisplayButton button = new(action);
                button.SetActionName(ActionAtlas.GetActionName(action));
                button.SetActionNum(1 + buttons.Count + ".");
                button.GetNode().SetCustomMinimumSize(new Vector2(0, INFO_BOX_SIZE_Y));
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
            Vector2 size = new(ACTION_SIZE_X + minimumWidth, INFO_BOX_SIZE_Y * displayItems);
            GetActionsContainerFrame().GetNode().SetSize(size);
            listContainer.GetNode().SetSize(size);
        }

        return actionsAlpha;
    }

    private List<string> ProcessContexts(string context) => context.Split('\n').Select(line => line.Trim()).Where(trimmed => trimmed != "").ToList();
}