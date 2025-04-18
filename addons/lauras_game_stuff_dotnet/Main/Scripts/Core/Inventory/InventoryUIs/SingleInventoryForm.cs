﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class SingleInventoryForm : InventoryForm, IProcess {
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
        RefreshHotbarIcons();
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

        for (int i = 0; i < Hotbar.HOTBAR_SIZE; i++) _hotbarList.GetDisplayList().AddChild(new HotbarItem(i, this));
    }

    protected override List<InvActionButton> InitActionsList() {
        Player player = GameManager.GetPlayer();

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

                string guid = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, itemJson);
                player.GetHotbar().RemoveFromHotbar(Guid.Parse(guid));
                RefreshHotbarIcons();
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

                string guid = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, itemJson);
                player.GetHotbar().RemoveFromHotbar(Guid.Parse(guid));
                RefreshHotbarIcons();
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
                string json = selected.GetJsonFromExpanded();

                string guid = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, json);
                
                player.GetHotbar().AddToHotbar(Guid.Parse(guid));
                RefreshHotbarIcons();
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

        if (_primaryOwner is Player player) {
            if (player.GetHotbar().IsFull()) {
                _addToHotbarBtn?.Disable("Hotbar is full");
            }
        }
    }

    public void RefreshHotbarIcons() {
        if (_primaryOwner is not IHotbarActor owner) return;
        SmartDictionary<int,Guid> items = owner.GetHotbar().GetHotbarItems();
        List<HotbarItem> icons = _hotbarList.GetDisplayObjects().Cast<HotbarItem>().ToList();
        

        SelectedInfo selectedItem = GetSelectedItem(InventorySide.PRIMARY);
        Guid toHighlight = Guid.Empty;
        if (selectedItem?.GetSummary() != null) {
            InvItemSummary summary = selectedItem.GetSummary();
            toHighlight = Guid.Parse(Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, summary.GetJson()));
        }

        foreach (HotbarItem hotbarItem in icons) {
            Guid guid = items.GetOrDefault(hotbarItem.GetIndex(), Guid.Empty);
            if (guid == Guid.Empty) {
                hotbarItem.SetFromItem("");
                continue;
            }
            string json = _primaryOwner.GetInventory().GetViaGUID(guid);
            if (string.IsNullOrEmpty(json)) {
                GD.PrintErr($"ERROR: SingleInventoryForm.RefreshHotbarIcons() : Hotbar item {guid} not found in inventory.");
                hotbarItem.SetFromItem("");
                continue;
            }
            hotbarItem.SetFromItem(json);
            hotbarItem.Highlight(hotbarItem.GetItemGUID().Equals(toHighlight));
        }
        
        owner.GetHotbar().UpdateOwnerHeldItem();
        ShowHotbarStars();
    }

    protected override void OnSetInventory(IContainer container, InventorySide side) {
        foreach (HotbarItem hotbarItem in _hotbarList.GetDisplayObjects().Cast<HotbarItem>()) hotbarItem.SetOwner(container);
        RefreshHotbarIcons();
    }

    public void Process(float delta) {
        if (_primaryOwner is not IHotbarActor owner) return;
        IObjectBase handItem = owner.GetHandItem();
        if (handItem == null) return;
        string heldJson = owner.GetInventory().GetViaGUID(handItem.GetGUID());

        if (string.IsNullOrEmpty(heldJson)) return;
        
        string itemTypeTag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, heldJson);
        ItemType itemType = Items.GetViaID(itemTypeTag);

        List<InvItemDisplay> displayButtons = _primaryList.GetDisplayObjects().Cast<InvItemDisplay>().ToList();
        foreach (InvItemDisplay btn in displayButtons) {
            if (!btn.GetItemType().Equals(itemType)) continue;
            List<InvItemSummary> summaries = btn.GetSummaries();
            foreach (InvItemSummary summary in summaries) {
                string summaryGuid = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, summary.GetJson());
                if (summaryGuid != handItem.GetGUID().ToString()) continue;
                summary.RefreshSummaryText(heldJson);
                return;
            }
        }
    }
}