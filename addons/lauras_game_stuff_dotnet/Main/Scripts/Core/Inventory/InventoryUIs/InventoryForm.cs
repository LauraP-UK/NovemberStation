using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class InventoryForm : FormBase {
    public enum Mode {
        SINGLE,
        DUAL
    }

    public enum InventorySide {
        PRIMARY,
        OTHER,
        NONE
    }

    protected readonly Mode _mode;
    protected IContainer _primaryOwner, _otherOwner;

    protected ControlElement _primaryContainer, _actionsContainer;
    protected ControlElement _otherContainer; // Optional
    protected ScrollDisplayList _actionsList, _primaryList;
    protected ScrollDisplayList _otherList; // Optional
    protected VBoxContainerElement _primaryHeader;
    protected VBoxContainerElement _otherHeader; // Optional

    protected Action<Key, InventoryForm, bool> _keyboardBehaviour;

    private SelectedInfo
        _primarySelectedItem = SelectedInfo.Empty(),
        _otherSelectedItem = SelectedInfo.Empty();

    protected InventoryForm(string formName, string formPath, Mode mode) : base(formName, formPath) {
        _mode = mode;
        InitElements();
        List<InvActionButton> actions = InitActionsList();
        _actionsList.GetDisplayList().SetChildren(actions);

        _menuElement = new ControlElement(_menu);
        SetTopLevelLayout(this);
        SetCaptureInput(true);
    }

    protected override void OnDestroy() {
        _actionsList.Destroy();
        _primaryList.Destroy();
        _otherList?.Destroy();
        _primaryHeader.Destroy();
        _otherHeader?.Destroy();
        CustomOnDestroy();
    }

    public override bool LockMovement() => true;
    public override bool PausesGame() => false;
    public override void KeyboardBehaviour(Key key, bool isPressed) => _keyboardBehaviour?.Invoke(key, this, isPressed);

    protected abstract void CustomOnDestroy();
    protected abstract void InitElements();
    protected abstract List<InvActionButton> InitActionsList();
    protected abstract void UpdateActionsList();

    public Mode GetMode() => _mode;

    protected InvActionButton GetCloseButton() {
        InvActionButton closeActionBtn = new();
        ButtonElement closeBtn = closeActionBtn.GetButton();
        closeBtn.OnPressed(_ => KeyboardBehaviour(Key.Escape, true));
        closeActionBtn.SetMisc("Close");
        return closeActionBtn;
    }

    public void UpdateSelectedItem(InventorySide side, InvItemDisplay newDisplay, InvItemSummary newSummary) {
        if (side == InventorySide.PRIMARY) {
            _primarySelectedItem = SelectedInfo.From(newDisplay, newSummary, _primarySelectedItem.IsLastSelected());
            SetInventory(_primaryOwner, side);
            _primaryList.GetNode().QueueRedraw();
        } else if (side == InventorySide.OTHER) {
            _otherSelectedItem = SelectedInfo.From(newDisplay, newSummary, _otherSelectedItem.IsLastSelected());
            SetInventory(_otherOwner, side);
            _otherList.GetNode().QueueRedraw();
        }

        UpdateActionsList();
        _actionsList.GetNode().QueueRedraw();
    }

    public void SetSelectedItem(InvItemDisplay selectedItem, InvItemSummary selectedSummary, InventorySide side = InventorySide.NONE) {
        if (side == InventorySide.NONE) side = GetSide(selectedItem);

        if (side == InventorySide.OTHER) {
            if (!_primarySelectedItem.IsEmpty()) _primarySelectedItem.SetSelected(false);
            _otherSelectedItem = SelectedInfo.From(selectedItem, selectedSummary, true);
        } else if (side == InventorySide.PRIMARY) {
            if (!_otherSelectedItem.IsEmpty()) _otherSelectedItem.SetSelected(false);
            _primarySelectedItem = SelectedInfo.From(selectedItem, selectedSummary, true);
        }

        if (side == InventorySide.PRIMARY) {
            SetPrimaryInventory(_primaryOwner);
        } else if (side == InventorySide.OTHER) {
            SetOtherInventory(_otherOwner);
        }

        HandleHighlights(InventorySide.PRIMARY);
        if (GetMode() == Mode.DUAL) HandleHighlights(InventorySide.OTHER);
        
        _primaryList.GetNode().QueueRedraw();
        if (GetMode() == Mode.DUAL) _otherList.GetNode().QueueRedraw();
        UpdateActionsList();
        _actionsList.GetNode().QueueRedraw();
    }

    public SelectedInfo GetSelectedItem(InventorySide side) {
        if (side == InventorySide.NONE) return SelectedInfo.Empty();
        return side == InventorySide.PRIMARY ? _primarySelectedItem : _otherSelectedItem;
    }

    public ScrollDisplayList GetInventory(InventorySide side) => side == InventorySide.PRIMARY ? _primaryList : _otherList;

    private void SetInventory(IContainer container, InventorySide side) {
        SelectedInfo selectedInfo = GetSelectedItem(side);
        InvItemDisplay selected = selectedInfo.GetItem();
        InvItemSummary selectedSummary = selectedInfo.GetSummary();
        bool isLastSelected = selectedInfo.IsLastSelected();

        ItemType selectedItemType = selected?.GetItemType();
        bool selectedIsExpanded = selected?.IsExpanded() ?? false;

        switch (side) {
            case InventorySide.PRIMARY:
                _primaryOwner = container;
                break;
            case InventorySide.OTHER:
                _otherOwner = container;
                break;
            case InventorySide.NONE:
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }

        IInventory inventory = container.GetInventory();
        ScrollDisplayList list = GetInventory(side);

        List<InvItemDisplay> oldBtns = list.GetDisplayObjects().Cast<InvItemDisplay>().ToList();
        List<InvItemDisplay> newBtns = GetButtons(inventory);
        InvHeaderInfo invHeaderInfo = new(container.GetName(), new Vector2(100, 40));

        if (inventory is IVolumetricInventory volInv)
            invHeaderInfo.SetWeightInfo(volInv.GetUsedSize(), volInv.GetMaxSize());
        else if (inventory is IQuantitativeInventory qInv)
            invHeaderInfo.SetCapacityInfo(qInv.GetUsedQuantity(), qInv.GetMaxQuantity());
        else
            invHeaderInfo.GetInfoLabel().SetAlpha(0.0f);

        (side == InventorySide.PRIMARY ? _primaryHeader : _otherHeader).SetChildren(invHeaderInfo);

        if (ArrayUtils.ExactMatch<InvItemDisplay>(oldBtns, newBtns)) {
            newBtns.ForEach(btn => btn.Destroy());
        } else {
            list.GetDisplayList().SetChildren(newBtns);
            InvItemDisplay toSelect = newBtns.FirstOrDefault(btn => btn.GetItemType().Equals(selectedItemType));
            toSelect?.SetExpanded(selectedIsExpanded);
            if (isLastSelected) toSelect?.Select(true);
            if (selectedSummary != null && toSelect != null) {
                int index = selectedSummary.GetIndex();
                foreach (InvItemSummary summary in toSelect.GetSummaries().Where(summary => summary.GetIndex() == index)) {
                    if (isLastSelected) summary.SetSelected(true);
                    break;
                }
            }
        }
    }

    private void HandleHighlights(InventorySide side) {
        SelectedInfo selectedItem = GetSelectedItem(side);
        ItemType itemType = selectedItem.GetItem()?.GetItemType();
        int index = selectedItem.GetSummary() == null ? -1 : selectedItem.GetSummary().GetIndex();
        bool isLastSelected = selectedItem.IsLastSelected();

        List<InvItemDisplay> btns = GetInventory(side).GetDisplayObjects().Cast<InvItemDisplay>().ToList();
        foreach (InvItemDisplay btn in btns) {
            if (!btn.GetItemType().Equals(itemType)) continue;
            if (!isLastSelected) {
                btn.Highlight(true);
                if (index == -1) continue;
                foreach (InvItemSummary summary in btn.GetSummaries()) {
                    if (summary.GetIndex() != index) continue;
                    summary.Highlight(false);
                    break;
                }
            }
            else if (index != -1) btn.Highlight(true);
        }
    }

    public void SetPrimaryInventory(IContainer container) => SetInventory(container, InventorySide.PRIMARY);
    public void SetOtherInventory(IContainer container) => SetInventory(container, InventorySide.OTHER);

    protected InvItemDisplay GetNewBtnOf(InvItemDisplay item, InventorySide side) => GetInventory(side)
        .GetDisplayObjects()
        .Cast<InvItemDisplay>()
        .FirstOrDefault(i => i.GetItemType().Equals(item.GetItemType()));


    private List<InvItemDisplay> GetButtons(IInventory inv) {
        List<string> contents = inv.GetContents();
        List<InvItemDisplay> displayButtons = new();

        foreach (string itemJson in contents) {
            string itemID = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, itemJson);
            IObjectBase objData = ObjectAtlas.DeserialiseDataWithoutNode(itemJson);
            ItemType itemType = Items.GetViaID(itemID);
            InvItemDisplay itemBtn = GetViaItem(itemType, displayButtons);
            bool isNew = false;

            if (itemBtn == null) {
                isNew = true;
                itemBtn = new InvItemDisplay(itemType, this);
            }

            itemBtn.AddCount(1);
            itemBtn.AddItemJson(itemJson);
            if (objData is IVolumetricObject volObj) itemBtn.AddWeight(volObj.GetSize());
            if (isNew) displayButtons.Add(itemBtn);
        }

        return displayButtons;
    }

    private InvItemDisplay GetViaItem(ItemType itemType, IEnumerable<InvItemDisplay> list) => list.FirstOrDefault(btn => btn.GetItemType().Equals(itemType));

    protected bool PlaceItemIntoWorld(IContainer owner, string objectJson, Vector3 spawn, Vector3 rotation = default) {
        ObjectAtlas.CreatedObject obj = ObjectAtlas.CreatedObjectFromJson(objectJson);
        if (!obj.Success) {
            obj.Node?.QueueFree();
            return false;
        }

        Node3D objNode = (Node3D)obj.Node;
        GameManager.I().GetSceneObjects().AddChild(obj.Node);
        objNode.SetGlobalPosition(spawn);
        if (rotation != default) objNode.SetGlobalRotation(new Vector3(0, rotation.Y, 0));
        GameManager.I().RegisterObject(objNode, obj.Object);
        owner.RemoveItem(objectJson);
        return true;
    }

    public InventorySide GetSide(InvItemDisplay btn) {
        if (GetMode() == Mode.SINGLE) return InventorySide.PRIMARY;

        if (_primaryList.GetDisplayList().GetDisplayObjects().Contains(btn)) return InventorySide.PRIMARY;
        if (_otherList.GetDisplayList().GetDisplayObjects().Contains(btn)) return InventorySide.OTHER;
        return InventorySide.NONE;
    }

    public class SelectedInfo {
        private readonly InvItemDisplay _item;
        private readonly InvItemSummary _summary;
        private readonly bool _immutable;
        private bool _lastSelected;

        private static readonly SelectedInfo EMPTY = new(null, null, false, true);

        private SelectedInfo(InvItemDisplay item, InvItemSummary summary, bool lastSelected, bool immutable = false) {
            _item = item;
            _summary = summary;
            _lastSelected = lastSelected;
            _immutable = immutable;
        }

        public InvItemDisplay GetItem() => _item;
        public InvItemSummary GetSummary() => _summary;
        public bool IsLastSelected() => _lastSelected;

        public void SetSelected(bool selected) {
            if (_immutable) return;
            _lastSelected = selected;
        }

        public bool IsEmpty() => Equals(EMPTY);

        public override string ToString() =>
            IsEmpty() ? "[SelectedInfo: EMPTY]" : $"[SelectedInfo: {_item.GetItemType().GetItemName()}, {(_summary == null ? "NO SUMMARY" : _summary.GetJson())}, {_lastSelected}]";

        public static SelectedInfo Empty() => EMPTY;
        public static SelectedInfo From(InvItemDisplay item, InvItemSummary summary, bool lastSelected) => item == null ? EMPTY : new SelectedInfo(item, summary, lastSelected);
    }
}