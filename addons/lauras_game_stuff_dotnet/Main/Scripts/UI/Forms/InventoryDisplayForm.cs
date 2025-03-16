using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class InventoryDisplayForm : FormBase {
    
    private readonly ControlElement _mainContainer, _otherContainer, _centreContainer, _closeButtonContainer;
    private readonly ScrollDisplayList _mainScroll, _otherScroll, _actionsScroll;
    
    private readonly Action<Key, InventoryDisplayForm, bool> _keyboardBehaviour;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/InventoryDisplayForm.tscn",
        MAIN_CONTAINER = "MarginContainer/HBoxContainer/MainInv",
        OTHER_CONTAINER = "MarginContainer/HBoxContainer/OtherInv",
        CENTRE_CONTAINER = "MarginContainer/HBoxContainer/CentreControls",
        CLOSE_BUTTON_CONTAINER = "MarginContainer/HBoxContainer/CentreControls/CloseContainer";


    public InventoryDisplayForm(string formName, Action<Key, InventoryDisplayForm, bool> keyboardBehaviour = null) : base(formName, FORM_PATH) {
        Control mainContainer = FindNode<Control>(MAIN_CONTAINER);
        Control otherContainer = FindNode<Control>(OTHER_CONTAINER);
        Control centreContainer = FindNode<Control>(CENTRE_CONTAINER);
        Control closeButtonContainer = FindNode<Control>(CLOSE_BUTTON_CONTAINER);
        
        _mainContainer = new ControlElement(mainContainer);
        _otherContainer = new ControlElement(otherContainer);
        _centreContainer = new ControlElement(centreContainer);
        _closeButtonContainer = new ControlElement(closeButtonContainer);

        _mainScroll = new ScrollDisplayList("main_inv");
        _otherScroll = new ScrollDisplayList("other_inv");
        _actionsScroll = new ScrollDisplayList("actions_container");
        
        mainContainer.AddChild(_mainScroll.GetNode());
        otherContainer.AddChild(_otherScroll.GetNode());
        centreContainer.AddChild(_actionsScroll.GetNode());

        _menuElement = new ControlElement(_menu);
        
        _keyboardBehaviour = keyboardBehaviour;
        SetRegisterListenerOnReady(true);
    }
    protected override List<IFormObject> GetAllElements() => new() { _mainContainer, _otherContainer, _centreContainer, _closeButtonContainer };
    protected override void OnDestroy() {
        _mainScroll.Destroy();
        _otherScroll.Destroy();
        _actionsScroll.Destroy();
    }
    
    public ScrollDisplayList GetMainScroll() => _mainScroll;
    public ScrollDisplayList GetOtherScroll() => _otherScroll;
    public ScrollDisplayList GetActionsScroll() => _actionsScroll;

    public void SetMainInv(IInventory inv) => GetMainScroll().GetDisplayList().SetChildren(GetButtons(inv));
    public void SetOtherInv(IInventory inv) => GetOtherScroll().GetDisplayList().SetChildren(GetButtons(inv));

    private InvItemDisplay GetViaItem(ItemType itemType, List<InvItemDisplay> list) => list.FirstOrDefault(invItemDisplay => invItemDisplay.GetItemType().Equals(itemType));

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
    }
    
    private InvItemDisplay GetSelectedItem() {
        List<InvItemDisplay> btns = GetMainScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList();
        btns.AddRange(GetOtherScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList());
        return btns.FirstOrDefault(button => button.IsSelected());
    }

    public override void KeyboardBehaviour(Key key, bool isPressed) => _keyboardBehaviour?.Invoke(key, this, isPressed);
    public override bool LockMovement() => true;
    public override bool PausesGame() => false;
}