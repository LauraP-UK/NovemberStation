using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class SingleInventoryForm : InventoryForm {
    private InvActionButton _placeActionBtn, _dropActionBtn, _addToHotbarBtn;
    private ScrollDisplayList _hotbarList;
    private ControlElement _hotbarContainer;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/SingleInventoryDisplayForm.tscn",
        MAIN_CONTAINER = "MarginContainer/HBoxContainer/MainInv",
        CENTRE_CONTAINER = "MarginContainer/HBoxContainer/Controls",
        MAIN_INFO_CONTAINER = "MarginContainer/HBoxContainer/MainInv/MainInfoContainer",
        HOTBAR_CONTAINER = "MarginContainer/HBoxContainer/Hotbar";

    public SingleInventoryForm(string formName, Action<Key, InventoryForm, bool> keyboardBehaviour) : base(formName, FORM_PATH, Mode.SINGLE) {
        _keyboardBehaviour = keyboardBehaviour;
        SetRegisterListenerOnReady(true);
    }

    protected override List<IFormObject> GetAllElements() => new() { _primaryContainer, _actionsContainer, _hotbarContainer, _primaryHeader, _primaryList, _actionsList, _hotbarList };

    protected override void CustomOnDestroy() { }

    protected override void InitElements() {
        Control primaryContainer = FindNode<Control>(MAIN_CONTAINER);
        Control actionsContainer = FindNode<Control>(CENTRE_CONTAINER);
        VBoxContainer primaryHeaderContainer = FindNode<VBoxContainer>(MAIN_INFO_CONTAINER);
        Control hotbarContainer = FindNode<Control>(HOTBAR_CONTAINER);

        _primaryContainer = new ControlElement(primaryContainer);
        _actionsContainer = new ControlElement(actionsContainer);
        _primaryHeader = new VBoxContainerElement(primaryHeaderContainer);
        _hotbarContainer = new ControlElement(hotbarContainer);

        _primaryList = new ScrollDisplayList("primary_scroll_list");
        _actionsList = new ScrollDisplayList("actions_scroll_list");
        _hotbarList = new ScrollDisplayList("hotbar_scroll_list");

        primaryContainer.AddChild(_primaryList.GetNode());
        actionsContainer.AddChild(_actionsList.GetNode());
        hotbarContainer.AddChild(_hotbarList.GetNode());

        _actionsList.GetScrollContainer().SetVScrollMode(ScrollContainer.ScrollMode.Disabled);

        List<ItemType> holdables = Items.GetItems().Where(i => i.IsHoldable()).ToList();
        holdables.AddRange(holdables);
        List<ItemType> shuffledItems = Randf.CopyAndShuffleList(holdables).GetRange(0, 9);
        List<HotbarItem> toDisplay = shuffledItems.Select(holdable => new HotbarItem("", holdable)).ToList();

        _hotbarList.GetDisplayList().SetChildren(toDisplay);
    }

    protected override List<InvActionButton> InitActionsList() {
        Player player = GameManager.I().GetPlayer();

        _placeActionBtn = new InvActionButton();
        ButtonElement placeBtn = _placeActionBtn.GetButton();
        placeBtn.OnPressed(
            _ => {
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
            }
        );
        _placeActionBtn.Disable();

        _dropActionBtn = new InvActionButton();
        ButtonElement dropBtn = _dropActionBtn.GetButton();
        dropBtn.OnPressed(
            _ => {
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
            }
        );
        _dropActionBtn.Disable();

        _addToHotbarBtn = new InvActionButton();
        ButtonElement hotbarBtn = _addToHotbarBtn.GetButton();
        hotbarBtn.OnPressed(
            _ => {
                SelectedInfo selectedItem = GetSelectedItem(InventorySide.PRIMARY);
                InvItemDisplay selected = selectedItem.GetItem();

                if (selected == null) return;
                selected = GetNewBtnOf(selected, InventorySide.PRIMARY);
                
                GD.Print($"Adding {selected.GetItemType().GetItemName()} to hotbar!");
            }
        );
        _addToHotbarBtn.Disable();
        
        

        return new List<InvActionButton> { _placeActionBtn, _dropActionBtn, _addToHotbarBtn, GetCloseButton() };
    }

    protected override void UpdateActionsList() {
        SelectedInfo primarySelected = GetSelectedItem(InventorySide.PRIMARY);
        if (primarySelected.IsEmpty()) {
            _placeActionBtn?.Disable();
            _dropActionBtn?.Disable();
            _addToHotbarBtn?.Disable();
        } else {
            _placeActionBtn?.SetMisc("Place");
            _dropActionBtn?.SetMisc("Drop");
            _addToHotbarBtn?.SetMisc("Add to Hotbar");
        }
    }
}