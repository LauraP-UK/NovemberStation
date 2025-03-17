using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class SingleInventoryDisplayForm : FormBase {
    private readonly ControlElement _mainContainer, _centreContainer;
    private readonly VBoxContainerElement _mainInfoContainer;
    private readonly ScrollDisplayList _mainScroll, _actionsScroll;

    private readonly Action<Key, SingleInventoryDisplayForm, bool> _keyboardBehaviour;
    private readonly InvActionButton _placeActionBtn, _dropActionBtn, _closeActionBtn;

    private IContainer _mainOwner;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/SingleInventoryDisplayForm.tscn",
        MAIN_CONTAINER = "MarginContainer/HBoxContainer/MainInv",
        CENTRE_CONTAINER = "MarginContainer/HBoxContainer/Controls",
        MAIN_INFO_CONTAINER = "MarginContainer/HBoxContainer/MainInv/MainInfoContainer";

    public SingleInventoryDisplayForm(string formName, Action<Key, SingleInventoryDisplayForm, bool> keyboardBehaviour = null) : base(formName, FORM_PATH) {
        Control mainContainer = FindNode<Control>(MAIN_CONTAINER);
        Control centreContainer = FindNode<Control>(CENTRE_CONTAINER);
        VBoxContainer mainInfoContainer = FindNode<VBoxContainer>(MAIN_INFO_CONTAINER);

        _mainContainer = new ControlElement(mainContainer);
        _centreContainer = new ControlElement(centreContainer);
        _mainInfoContainer = new VBoxContainerElement(mainInfoContainer);

        _mainScroll = new ScrollDisplayList("main_inv");
        _actionsScroll = new ScrollDisplayList("actions_container");

        mainContainer.AddChild(_mainScroll.GetNode());
        centreContainer.AddChild(_actionsScroll.GetNode());
        _actionsScroll.GetScrollContainer().SetVScrollMode(ScrollContainer.ScrollMode.Disabled);

        _menuElement = new ControlElement(_menu);

        _keyboardBehaviour = keyboardBehaviour;
        SetRegisterListenerOnReady(true);
        
        Player player = GameManager.I().GetPlayer();
        
        _placeActionBtn = new InvActionButton();
        ButtonElement placeBtn = _placeActionBtn.GetButton();
        placeBtn.OnPressed(_ => {
            InvItemDisplay selected = GetSelectedItem();
            RaycastResult result = player.GetLookingAt(2.5f);
            Vector3 rotation = player.GetCamera().GetGlobalRotation();

            Vector3 spawn = result.HasHit() ? result.GetClosestHit().HitAtPosition + (result.GetClosestHit().HitNormal * 0.1f) : result.GetEnd();
            string itemJson = GetItemJson(selected, _mainOwner.GetInventory());
            PlaceItemIntoWorld(itemJson, spawn, rotation);
            
            Refresh();
        });
        _placeActionBtn.Disable();
        
        _dropActionBtn = new InvActionButton();
        ButtonElement dropBtn = _dropActionBtn.GetButton();
        dropBtn.OnPressed(btn => {
            InvItemDisplay selected = GetSelectedItem();
            Vector3 rotation = player.GetCamera().GetGlobalRotation();
            Vector3 spawn = player.GetPosition() + new Vector3(0f, 0.1f, 0f);
            string itemJson = GetItemJson(selected, _mainOwner.GetInventory());
            PlaceItemIntoWorld(itemJson, spawn, rotation);
            
            Refresh();
        });
        _dropActionBtn.Disable();

        _closeActionBtn = new InvActionButton();
        ButtonElement closeBtn = _closeActionBtn.GetButton();
        closeBtn.OnPressed(_ => KeyboardBehaviour(Key.Escape, true));
        _closeActionBtn.SetMisc("Close");

        _actionsScroll.GetDisplayList().SetChildren(_placeActionBtn, _dropActionBtn, _closeActionBtn);
    }

    protected override List<IFormObject> GetAllElements() =>
        new() { _mainContainer, _centreContainer, _mainInfoContainer, _mainScroll, _actionsScroll, _closeActionBtn, _placeActionBtn };

    protected override void OnDestroy() {
        _mainScroll.Destroy();
        _actionsScroll.Destroy();
    }

    public ScrollDisplayList GetMainScroll() => _mainScroll;
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
        foreach (InvItemDisplay button in btns) button.Select(button.Equals(btn));

        if (btn == null) {
            _placeActionBtn.Disable();
            _dropActionBtn.Disable();
        }
        else {
            _placeActionBtn.SetMisc("Place");
            _dropActionBtn.SetMisc("Drop");
        }
    }

    private void SelectFirstItemOf(ItemType itemType) {
        InvItemDisplay btn = GetViaItem(itemType, GetMainScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).ToList());
        SelectButton(btn);
    }

    private InvItemDisplay GetSelectedItem() {
        return GetMainScroll().GetDisplayList().GetDisplayObjects().Select(b => (InvItemDisplay)b).FirstOrDefault(button => button.IsSelected());
    }

    public override void KeyboardBehaviour(Key key, bool isPressed) => _keyboardBehaviour?.Invoke(key, this, isPressed);
    public override bool LockMovement() => true;
    public override bool PausesGame() => false;

    private void Refresh(InvItemDisplay selectedBtn = null) {
        SetMainInv(_mainOwner);
        SelectFirstItemOf(selectedBtn?.GetItemType());
    }

    private bool PlaceItemIntoWorld(string objectJson, Vector3 spawn, Vector3 rotation = default) {
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
        _mainOwner.RemoveItem(objectJson);
        return true;
    }
}