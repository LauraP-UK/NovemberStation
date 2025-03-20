using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class DualInventoryForm : InventoryForm {

    private InvActionButton _moveActionBtn;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/InventoryDisplayForm.tscn",
        MAIN_CONTAINER = "MarginContainer/HBoxContainer/MainInv",
        OTHER_CONTAINER = "MarginContainer/HBoxContainer/OtherInv",
        CENTRE_CONTAINER = "MarginContainer/HBoxContainer/CentreControls",
        MAIN_INFO_CONTAINER = "MarginContainer/HBoxContainer/MainInv/MainInfoContainer",
        OTHER_INFO_CONTAINER = "MarginContainer/HBoxContainer/OtherInv/OtherInfoContainer";

    public DualInventoryForm(string formName, Action<Key, InventoryForm, bool> keyboardBehaviour) : base(formName, FORM_PATH, Mode.DUAL) {
        _keyboardBehaviour = keyboardBehaviour;
        SetRegisterListenerOnReady(true);
    }

    protected override List<IFormObject> GetAllElements() => new() {
        _primaryContainer, _otherContainer, _actionsContainer, _primaryHeader, _otherHeader, _primaryList, _otherList, _actionsList
    };

    protected override void CustomOnDestroy() { }

    protected override void InitElements() {
        Control primaryContainer = FindNode<Control>(MAIN_CONTAINER);
        Control otherContainer = FindNode<Control>(OTHER_CONTAINER);
        Control centreContainer = FindNode<Control>(CENTRE_CONTAINER);
        VBoxContainer mainInfoContainer = FindNode<VBoxContainer>(MAIN_INFO_CONTAINER);
        VBoxContainer otherInfoContainer = FindNode<VBoxContainer>(OTHER_INFO_CONTAINER);

        _primaryContainer = new ControlElement(primaryContainer);
        _otherContainer = new ControlElement(otherContainer);
        _actionsContainer = new ControlElement(centreContainer);
        _primaryHeader = new VBoxContainerElement(mainInfoContainer);
        _otherHeader = new VBoxContainerElement(otherInfoContainer);

        _primaryList = new ScrollDisplayList("primary_scroll_list");
        _otherList = new ScrollDisplayList("other_scroll_list");
        _actionsList = new ScrollDisplayList("actions_scroll_list");

        primaryContainer.AddChild(_primaryList.GetNode());
        otherContainer.AddChild(_otherList.GetNode());
        centreContainer.AddChild(_actionsList.GetNode());
        _actionsList.GetScrollContainer().SetVScrollMode(ScrollContainer.ScrollMode.Disabled);
    }

    protected override List<InvActionButton> InitActionsList() {
        _moveActionBtn = new InvActionButton();
        ButtonElement moveBtn = _moveActionBtn.GetButton();

        moveBtn.OnPressed(_ => {
            (InvItemDisplay selected, InventorySide side) = GetSelectedItem();
            if (selected == null) return;

            if (side == InventorySide.PRIMARY) {
                string itemJson = GetItemJson(selected, _primaryOwner.GetInventory());
                string metaTag = Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.META_TAG, itemJson);

                AddItemFailCause result = _otherOwner.GetInventory().CanAddItem(itemJson);
                switch (result) {
                    case AddItemFailCause.SUBCLASS_FAIL:
                        Toast.Error(GameManager.I().GetPlayer(), $"{_otherOwner.GetName()} is full!");
                        return;
                    case AddItemFailCause.FILTER_FAIL:
                        Toast.Error(GameManager.I().GetPlayer(), "You can't put that item in there!");
                        return;
                    case AddItemFailCause.SUCCESS:
                    default:
                        _otherOwner.StoreItem(metaTag, itemJson);
                        _primaryOwner.RemoveItem(itemJson);
                        break;
                }
            }
            else {
                string itemJson = GetItemJson(selected, _otherOwner.GetInventory());
                string metaTag = Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.META_TAG, itemJson);

                AddItemFailCause result = _primaryOwner.GetInventory().CanAddItem(itemJson);
                switch (result) {
                    case AddItemFailCause.SUBCLASS_FAIL:
                        Toast.Error(GameManager.I().GetPlayer(), "Your inventory is full!");
                        return;
                    case AddItemFailCause.FILTER_FAIL:
                        Toast.Error(GameManager.I().GetPlayer(), "You can't pick this up!");
                        return;
                    case AddItemFailCause.SUCCESS:
                    default:
                        _primaryOwner.StoreItem(metaTag, itemJson);
                        _otherOwner.RemoveItem(itemJson);
                        break;
                }
            }

            RefreshFromButton(selected.GetCount() > 1 ? selected : null);
        });

        return new List<InvActionButton> { _moveActionBtn, GetCloseButton() };
    }

    protected override void SelectItem(InventoryFormState state) {
        InvItemDisplay target = state.Target;
        GetAllItemDisplays().ForEach(i => i.Select(false));

        List<InvItemDisplay> items = (state.Side == InventorySide.PRIMARY ? _primaryList : _otherList).GetDisplayObjects().Cast<InvItemDisplay>().ToList();

        foreach (InvItemDisplay item in items) {
            bool thisSelected = item.GetItemType().Equals(target?.GetItemType());
            item.Select(thisSelected);
            if (thisSelected) item.SetExpanded(state.Expanded);
        }
        
        if (target == null)
            _moveActionBtn?.Disable();
        else {
            switch (state.Side) {
                case InventorySide.PRIMARY:
                    _moveActionBtn.SetStore();
                    break;
                case InventorySide.OTHER:
                    _moveActionBtn.SetTake();
                    break;
                default:
                    _moveActionBtn.Disable();
                    break;
            }
        }
    }

    protected override (InvItemDisplay button, InventorySide side) GetSelectedItem() {
        foreach (InvItemDisplay item in GetAllItemDisplays().Where(item => item.IsSelected())) return (item, GetSide(item));
        return (null, InventorySide.NONE);
    }
}