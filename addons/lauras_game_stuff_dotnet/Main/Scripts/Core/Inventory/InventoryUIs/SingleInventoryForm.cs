using System;
using System.Collections.Generic;
using Godot;

public class SingleInventoryForm : InventoryForm {
    
    private InvActionButton _placeActionBtn, _dropActionBtn;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/SingleInventoryDisplayForm.tscn",
        MAIN_CONTAINER = "MarginContainer/HBoxContainer/MainInv",
        CENTRE_CONTAINER = "MarginContainer/HBoxContainer/Controls",
        MAIN_INFO_CONTAINER = "MarginContainer/HBoxContainer/MainInv/MainInfoContainer";
    
    public SingleInventoryForm(string formName, Action<Key, InventoryForm, bool> keyboardBehaviour) : base(formName, FORM_PATH, Mode.SINGLE) {
        _keyboardBehaviour = keyboardBehaviour;
        SetRegisterListenerOnReady(true);
    }
    protected override List<IFormObject> GetAllElements() => new() {_primaryContainer, _actionsContainer, _primaryHeader, _primaryList, _actionsList};

    protected override void CustomOnDestroy() { }

    protected override void InitElements() {
        Control primaryContainer = FindNode<Control>(MAIN_CONTAINER);
        Control actionsContainer = FindNode<Control>(CENTRE_CONTAINER);
        VBoxContainer primaryHeaderContainer = FindNode<VBoxContainer>(MAIN_INFO_CONTAINER);
        
        _primaryContainer = new ControlElement(primaryContainer);
        _actionsContainer = new ControlElement(actionsContainer);
        _primaryHeader = new VBoxContainerElement(primaryHeaderContainer);
        
        _primaryList = new ScrollDisplayList("primary_scroll_list");
        _actionsList = new ScrollDisplayList("actions_scroll_list");

        primaryContainer.AddChild(_primaryList.GetNode());
        actionsContainer.AddChild(_actionsList.GetNode());
        _actionsList.GetScrollContainer().SetVScrollMode(ScrollContainer.ScrollMode.Disabled);
    }

    protected override List<InvActionButton> InitActionsList() {
        Player player = GameManager.I().GetPlayer();

        _placeActionBtn = new InvActionButton();
        ButtonElement placeBtn = _placeActionBtn.GetButton();
        placeBtn.OnPressed(_ => {
            SelectedInfo selectedItem = GetSelectedItem(InventorySide.PRIMARY);
            InvItemDisplay selected = selectedItem.GetItem();
            InvItemSummary summary = selectedItem.GetSummary();
            if (selected == null) return;
            selected = GetNewBtnOf(selected, InventorySide.PRIMARY);
            string itemJson = summary != null ? summary.GetJson() : selected.GetFirstJson();

            RaycastResult result = player.GetLookingAt(2.5f);
            Vector3 rotation = player.GetCamera().GetGlobalRotation();
            Vector3 spawn = result.HasHit() ? result.GetClosestHit().HitAtPosition + (result.GetClosestHit().HitNormal * 0.1f) : result.GetEnd();
            bool success = PlaceItemIntoWorld(_primaryOwner, itemJson, spawn, rotation);
            if (!success) return;
            
            selected.AddCount(-1);
            
            InvItemDisplay refreshSelected = selected.GetCount() > 0 ? selected : null;
            UpdateSelectedItem(InventorySide.PRIMARY, refreshSelected, null);
        });
        _placeActionBtn.Disable();
        
        _dropActionBtn = new InvActionButton();
        ButtonElement dropBtn = _dropActionBtn.GetButton();
        dropBtn.OnPressed(_ => {
            SelectedInfo selectedItem = GetSelectedItem(InventorySide.PRIMARY);
            InvItemDisplay selected = selectedItem.GetItem();
            
            if (selected == null) return;
            selected = GetNewBtnOf(selected, InventorySide.PRIMARY);
            string itemJson = selected.GetJsonFromExpanded();

            Vector3 rotation = player.GetCamera().GetGlobalRotation();
            Vector3 spawn = player.GetPosition() + new Vector3(0f, 0.1f, 0f);
            bool success = PlaceItemIntoWorld(_primaryOwner, itemJson, spawn, rotation);
            if (!success) return;
            
            selected.AddCount(-1);
            
            InvItemDisplay refreshSelected = selected.GetCount() > 0 ? selected : null;
            UpdateSelectedItem(InventorySide.PRIMARY, refreshSelected, null);
        });
        _dropActionBtn.Disable();
        
        return new List<InvActionButton> {_placeActionBtn, _dropActionBtn, GetCloseButton()};
    }

    protected override void UpdateActionsList() {
        SelectedInfo primarySelected = GetSelectedItem(InventorySide.PRIMARY);
        if (primarySelected.IsEmpty()) {
            _placeActionBtn?.Disable();
            _dropActionBtn?.Disable();
        } else {
            _placeActionBtn?.SetMisc("Place");
            _dropActionBtn?.SetMisc("Drop");
        }
    }
}