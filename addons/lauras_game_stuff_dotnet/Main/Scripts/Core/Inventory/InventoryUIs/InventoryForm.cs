
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class InventoryForm : FormBase {
    public enum Mode { SINGLE, DUAL }
    public enum InventorySide { PRIMARY, OTHER, NONE }
    
    protected readonly Mode _mode;
    protected IContainer _primaryOwner, _otherOwner;

    protected ControlElement _primaryContainer, _actionsContainer;
    protected ControlElement _otherContainer; // Optional
    protected ScrollDisplayList _actionsList, _primaryList;
    protected ScrollDisplayList _otherList; // Optional
    protected VBoxContainerElement _primaryHeader;
    protected VBoxContainerElement _otherHeader; // Optional
    
    protected Action<Key, InventoryForm, bool> _keyboardBehaviour;

    protected InventoryForm(string formName, string formPath, Mode mode) : base(formName, formPath) {
        _mode = mode;
        InitElements();
        List<InvActionButton> actions = InitActionsList();
        _actionsList.GetDisplayList().SetChildren(actions);
        
        _menuElement = new ControlElement(_menu);
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
    protected abstract void SelectItem(InventoryFormState state);
    
    public Mode GetMode() => _mode;
    
    protected InvActionButton GetCloseButton() {
        InvActionButton closeActionBtn = new();
        ButtonElement closeBtn = closeActionBtn.GetButton();
        closeBtn.OnPressed(_ => KeyboardBehaviour(Key.Escape, true));
        closeActionBtn.SetMisc("Close");

        return closeActionBtn;
    }
    
    public void SetPrimaryInventory(IContainer container) {
        _primaryOwner = container;
        IInventory inventory = container.GetInventory();
        _primaryList.GetDisplayList().SetChildren(GetButtons(inventory));
        InvHeaderInfo invHeaderInfo = new(container.GetName(), new Vector2(100, 40));

        if (inventory is IVolumetricInventory volInv) invHeaderInfo.SetWeight(volInv.GetUsedSize(), volInv.GetMaxSize());
        else invHeaderInfo.GetWeightLabel().SetAlpha(0.0f);
        
        _primaryHeader.SetChildren(invHeaderInfo);
    }

    public void SetOtherInventory(IContainer container) {
        _otherOwner = container;
        _otherList.GetDisplayList().SetChildren(GetButtons(container.GetInventory()));
        InvHeaderInfo invHeaderInfo = new(container.GetName(), new Vector2(100, 40));

        if (container.GetInventory() is IVolumetricInventory volInv) invHeaderInfo.SetWeight(volInv.GetUsedSize(), volInv.GetMaxSize());
        else invHeaderInfo.GetWeightLabel().SetAlpha(0.0f);

        _otherHeader.SetChildren(invHeaderInfo);
    }
    
    private List<InvItemDisplay> GetButtons(IInventory inv) {
        List<string> contents = inv.GetContents();
        List<InvItemDisplay> displayButtons = new();

        foreach (string itemJson in contents) {
            string itemID = Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.TYPE_ID, itemJson);
            IObjectBase objData = ObjectAtlas.DeserialiseDataWithoutNode(itemJson);
            ItemType itemType = Items.GetViaID(itemID);
            InvItemDisplay invItemDisplay = GetViaItem(itemType, displayButtons);
            bool isNew = false;

            if (invItemDisplay == null) {
                isNew = true;
                invItemDisplay = new InvItemDisplay(itemType, this);
            }

            invItemDisplay.AddCount(1);
            invItemDisplay.AddItemJson(itemJson);
            if (objData is IVolumetricObject volObj) invItemDisplay.AddWeight(volObj.GetSize());
            if (isNew) {
                invItemDisplay.OnPressed(display => {
                    InventoryFormState state = new(display, GetMode(), GetSide(display), display.IsExpanded());
                    SelectItem(state);
                });
                displayButtons.Add(invItemDisplay);
            }
        }

        return displayButtons;
    }
    
    protected List<InvItemDisplay> GetAllItemDisplays() {
        List<InvItemDisplay> displays = _primaryList.GetDisplayList().GetDisplayObjects().Cast<InvItemDisplay>().ToList();
        if (_mode == Mode.DUAL && _otherList != null)
            displays.AddRange(_otherList.GetDisplayList().GetDisplayObjects().Cast<InvItemDisplay>());
        return displays;
    }
    protected abstract (InvItemDisplay button, InventorySide side) GetSelectedItem();// => GetAllItemDisplays().FirstOrDefault(btn => btn.IsSelected());
    
    private InvItemDisplay GetViaItem(ItemType itemType, IEnumerable<InvItemDisplay> list) => list.FirstOrDefault(btn => btn.GetItemType().Equals(itemType));
    protected string GetItemJson(InvItemDisplay invItemDisplay, IInventory inv) {
        if (invItemDisplay == null) return null;
        return inv.GetContents()
            .FirstOrDefault(c => Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.TYPE_ID, c) == invItemDisplay.GetItemType().GetTypeID());
    }

    protected bool PlaceItemIntoWorld(IContainer owner, string objectJson, Vector3 spawn, Vector3 rotation = default) {
        ObjectAtlas.CreatedObject obj = ObjectAtlas.CreatedObjectFromJson(objectJson);
        if (!obj.Success) {
            obj.Node?.QueueFree();
            return false;
        }
        Node3D objNode = (Node3D)obj.Node;
        GameManager.I().GetSceneObjects().AddChild(obj.Node);
        objNode.SetGlobalPosition(spawn);
        if (rotation != default) objNode.SetGlobalRotation(new Vector3(0,rotation.Y,0));
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
    
    public void Refresh(InventoryFormState state) {
        SetPrimaryInventory(_primaryOwner);
        if (GetMode() == Mode.DUAL) SetOtherInventory(_otherOwner);
        SelectItem(state);
    }

    public void Refresh(InvItemDisplay button) {
        Refresh(button == null
            ? new InventoryFormState(null, GetMode(), InventorySide.NONE, false)
            : new InventoryFormState(button, GetMode(), GetSide(button), button.IsExpanded()));
    }
    
    public readonly struct InventoryFormState {
        public readonly InvItemDisplay Target;
        public readonly Mode Mode;
        public readonly InventorySide Side;
        public readonly bool Expanded;

        public InventoryFormState(InvItemDisplay target, Mode mode, InventorySide side, bool expanded) {
            Target = target;
            Mode = mode;
            Side = side;
            Expanded = expanded;
        }
    }
}