using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class InventoryDisplayForm : FormBase {
    private enum InventorySide {
        MAIN, OTHER, NONE
    }
    
    private readonly ControlElement _mainContainer, _otherContainer, _centreContainer;
    private readonly VBoxContainerElement _mainInfoContainer, _otherInfoContainer;
    private readonly ScrollDisplayList _mainScroll, _otherScroll, _actionsScroll;

    private readonly Action<Key, InventoryDisplayForm, bool> _keyboardBehaviour;
    private readonly InvActionButton _moveActionBtn, _closeActionBtn;

    private IContainer _mainOwner, _otherOwner;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/InventoryDisplayForm.tscn",
        MAIN_CONTAINER = "MarginContainer/HBoxContainer/MainInv",
        OTHER_CONTAINER = "MarginContainer/HBoxContainer/OtherInv",
        CENTRE_CONTAINER = "MarginContainer/HBoxContainer/CentreControls",
        MAIN_INFO_CONTAINER = "MarginContainer/HBoxContainer/MainInv/MainInfoContainer",
        OTHER_INFO_CONTAINER = "MarginContainer/HBoxContainer/OtherInv/OtherInfoContainer";


    public InventoryDisplayForm(string formName, Action<Key, InventoryDisplayForm, bool> keyboardBehaviour = null) : base(formName, FORM_PATH) {
        
        Control mainContainer = FindNode<Control>(MAIN_CONTAINER);
        Control otherContainer = FindNode<Control>(OTHER_CONTAINER);
        Control centreContainer = FindNode<Control>(CENTRE_CONTAINER);
        VBoxContainer mainInfoContainer = FindNode<VBoxContainer>(MAIN_INFO_CONTAINER);
        VBoxContainer otherInfoContainer = FindNode<VBoxContainer>(OTHER_INFO_CONTAINER);

        _mainContainer = new ControlElement(mainContainer);
        _otherContainer = new ControlElement(otherContainer);
        _centreContainer = new ControlElement(centreContainer);
        _mainInfoContainer = new VBoxContainerElement(mainInfoContainer);
        _otherInfoContainer = new VBoxContainerElement(otherInfoContainer);

        _mainScroll = new ScrollDisplayList("main_inv");
        _otherScroll = new ScrollDisplayList("other_inv");
        _actionsScroll = new ScrollDisplayList("actions_container");

        mainContainer.AddChild(_mainScroll.GetNode());
        otherContainer.AddChild(_otherScroll.GetNode());
        centreContainer.AddChild(_actionsScroll.GetNode());
        _actionsScroll.GetScrollContainer().SetVScrollMode(ScrollContainer.ScrollMode.Disabled);

        _menuElement = new ControlElement(_menu);

        _keyboardBehaviour = keyboardBehaviour;
        SetRegisterListenerOnReady(true);

        _closeActionBtn = new InvActionButton();
        ButtonElement closeBtn = _closeActionBtn.GetButton();
        closeBtn.OnPressed(_ => KeyboardBehaviour(Key.Escape, true));
        _closeActionBtn.SetMisc("Close");
        
        _moveActionBtn = new InvActionButton();
        ButtonElement moveBtn = _moveActionBtn.GetButton();

        moveBtn.OnPressed(_ => {
            InvItemDisplay selected = GetSelectedItem();
            if (selected == null) return;

            InventorySide side = GetSide(selected);

            if (side == InventorySide.MAIN) {
                string itemJson = GetItemJson(selected, _mainOwner.GetInventory());
                string metaTag = Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.META_TAG, itemJson);

                if (!_otherOwner.GetInventory().CanAddItem(itemJson)) {
                    Toast.Error(GameManager.I().GetPlayer(), $"{_otherOwner.GetName()} is full!");
                    return;
                }

                _otherOwner.StoreItem(metaTag, itemJson);
                _mainOwner.RemoveItem(itemJson);
            }
            else {
                string itemJson = GetItemJson(selected, _otherOwner.GetInventory());
                string metaTag = Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.META_TAG, itemJson);

                if (!_mainOwner.GetInventory().CanAddItem(itemJson)) {
                    Toast.Error(GameManager.I().GetPlayer(), "Your inventory is full!");
                    return;
                }

                _mainOwner.StoreItem(metaTag, itemJson);
                _otherOwner.RemoveItem(itemJson);
            }
            
            
            Refresh(selected.GetCount() > 1 ? selected : null, side);
        });

        _actionsScroll.GetDisplayList().SetChildren(_moveActionBtn, _closeActionBtn);
    }

    protected override List<IFormObject> GetAllElements() => new() {
        _mainContainer, _otherContainer, _centreContainer, _mainInfoContainer, _otherInfoContainer, _mainScroll, _otherScroll, _actionsScroll
    };

    protected override void OnDestroy() {
        _mainScroll.Destroy();
        _otherScroll.Destroy();
        _actionsScroll.Destroy();
    }

    public ScrollDisplayList GetMainScroll() => _mainScroll;
    public ScrollDisplayList GetOtherScroll() => _otherScroll;
    public ScrollDisplayList GetActionsScroll() => _actionsScroll;

    public void SetMainInv(IContainer container) {
        _mainOwner = container;
        IInventory inventory = container.GetInventory();
        GetMainScroll().GetDisplayList().SetChildren(GetButtons(inventory));
        InvHeaderInfo invHeaderInfo = new(container.GetName(), new Vector2(100, 40));

        if (inventory is IVolumetricInventory volInv) invHeaderInfo.SetWeight(volInv.GetUsedSize(), volInv.GetMaxSize());
        else invHeaderInfo.GetWeightLabel().SetAlpha(0.0f);
        
        _mainInfoContainer.SetChildren(invHeaderInfo);
    }

    public void SetOtherInv(IContainer container) {
        _otherOwner = container;
        GetOtherScroll().GetDisplayList().SetChildren(GetButtons(container.GetInventory()));
        InvHeaderInfo invHeaderInfo = new(container.GetName(), new Vector2(100, 40));
        
        if (container.GetInventory() is IVolumetricInventory volInv) invHeaderInfo.SetWeight(volInv.GetUsedSize(), volInv.GetMaxSize());
        else invHeaderInfo.GetWeightLabel().SetAlpha(0.0f);
        
        _otherInfoContainer.SetChildren(invHeaderInfo);
    }

    private InventorySide GetSide(InvItemDisplay btn) {
        if (GetMainScroll().GetDisplayList().GetDisplayObjects().Contains(btn)) return InventorySide.MAIN;
        if (GetOtherScroll().GetDisplayList().GetDisplayObjects().Contains(btn)) return InventorySide.OTHER;
        return InventorySide.NONE;
    }

    private InvItemDisplay GetViaItem(ItemType itemType, List<InvItemDisplay> list) => list.FirstOrDefault(invItemDisplay => invItemDisplay.GetItemType().Equals(itemType));
    private string GetItemJson(InvItemDisplay invItemDisplay, IInventory inv) => inv.GetContents()
        .FirstOrDefault(c => Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.TYPE_ID, c) == invItemDisplay.GetItemType().GetTypeID());

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
                invItemDisplay = new InvItemDisplay(itemType);
            }

            invItemDisplay.AddCount(1);
            if (objData is IVolumetricObject volObj) invItemDisplay.AddWeight(volObj.GetSize());
            if (isNew) {
                invItemDisplay.OnPressed(SelectButton);
                displayButtons.Add(invItemDisplay);
            }
        }

        return displayButtons;
    }

    private void SelectButton(InvItemDisplay btn) {
        List<InvItemDisplay> btns = GetMainScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList();
        btns.AddRange(GetOtherScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList());
        foreach (InvItemDisplay button in btns) button.Select(button.Equals(btn));

        if (btn == null) _moveActionBtn.Disable();
        else if (GetSide(btn) == InventorySide.MAIN) _moveActionBtn.SetStore();
        else _moveActionBtn.SetTake();
    }

    private void SelectFirstItemOf(ItemType itemType, InventorySide side) {
        if (side == InventorySide.MAIN) {
            InvItemDisplay btn = GetViaItem(itemType, GetMainScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList());
            SelectButton(btn);
        }
        else if (side == InventorySide.OTHER) {
            InvItemDisplay btn = GetViaItem(itemType, GetOtherScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList());
            SelectButton(btn);
        }
    }

    private InvItemDisplay GetSelectedItem() {
        List<InvItemDisplay> btns = GetMainScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList();
        btns.AddRange(GetOtherScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList());
        return btns.FirstOrDefault(button => button.IsSelected());
    }

    public override void KeyboardBehaviour(Key key, bool isPressed) => _keyboardBehaviour?.Invoke(key, this, isPressed);
    public override bool LockMovement() => true;
    public override bool PausesGame() => false;

    private void Refresh(InvItemDisplay selectedBtn = null, InventorySide side = InventorySide.NONE) {
        SetMainInv(_mainOwner);
        SetOtherInv(_otherOwner);
        SelectFirstItemOf(selectedBtn?.GetItemType(), side);
    }
}