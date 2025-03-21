using System;
using System.Collections.Generic;
using System.Linq;
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
            InvItemDisplay selected = GetSelectedItem().button;
            RaycastResult result = player.GetLookingAt(2.5f);
            Vector3 rotation = player.GetCamera().GetGlobalRotation();

            Vector3 spawn = result.HasHit() ? result.GetClosestHit().HitAtPosition + (result.GetClosestHit().HitNormal * 0.1f) : result.GetEnd();
            string itemJson = selected.GetJsonFromExpanded();
            PlaceItemIntoWorld(_primaryOwner, itemJson, spawn, rotation);
            
            Refresh(selected.GetCount() > 1 ? selected : null);
        });
        _placeActionBtn.Disable();
        
        _dropActionBtn = new InvActionButton();
        ButtonElement dropBtn = _dropActionBtn.GetButton();
        dropBtn.OnPressed(_ => {
            InvItemDisplay selected = GetSelectedItem().button;
            Vector3 rotation = player.GetCamera().GetGlobalRotation();
            Vector3 spawn = player.GetPosition() + new Vector3(0f, 0.1f, 0f);
            string itemJson = selected.GetJsonFromExpanded();
            PlaceItemIntoWorld(_primaryOwner, itemJson, spawn, rotation);
            
            Refresh(selected.GetCount() > 1 ? selected : null);
        });
        _dropActionBtn.Disable();
        
        return new List<InvActionButton> {_placeActionBtn, _dropActionBtn, GetCloseButton()};
    }

    protected override void SelectItem(InventoryFormState state) {
        InvItemDisplay target = state.Target;
        GetAllItemDisplays().ForEach(i => {
            i.Select(false);
            i.SetExpanded(false);
        });
        
        foreach (InvItemDisplay item in GetAllItemDisplays()) {
            bool thisSelected = item.GetItemType().Equals(target?.GetItemType());
            item.Select(thisSelected);
            if (thisSelected) item.SetExpanded(state.Expanded);
        }
        
        if (target == null) {
            _placeActionBtn?.Disable();
            _dropActionBtn?.Disable();
        } else {
            _placeActionBtn?.SetMisc("Place");
            _dropActionBtn?.SetMisc("Drop");
        }
    }

    protected override (InvItemDisplay button, InventorySide side) GetSelectedItem() {
        InvItemDisplay selected = GetAllItemDisplays().FirstOrDefault(btn => btn.IsSelected());
        return (selected, InventorySide.PRIMARY);
    }
}