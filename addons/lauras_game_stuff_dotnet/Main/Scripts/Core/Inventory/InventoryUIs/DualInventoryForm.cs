using System;
using System.Collections.Generic;
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

        moveBtn.OnPressed(
            _ => {
                SelectedInfo primarySelected = GetSelectedItem(InventorySide.PRIMARY);
                SelectedInfo otherSelected = GetSelectedItem(InventorySide.OTHER);

                InventorySide side = InventorySide.NONE;
                InvItemDisplay selected = null;
                InvItemSummary summary = null;

                if (primarySelected.IsLastSelected()) {
                    side = InventorySide.PRIMARY;
                    selected = primarySelected.GetItem();
                    summary = primarySelected.GetSummary();
                } else if (otherSelected.IsLastSelected()) {
                    side = InventorySide.OTHER;
                    selected = otherSelected.GetItem();
                    summary = otherSelected.GetSummary();
                }

                if (side == InventorySide.NONE || selected == null) return;
                selected = GetNewBtnOf(selected, side);
                
                string json = summary != null ? summary.GetJson() : selected.GetFirstJson();
                bool success = side == InventorySide.PRIMARY
                    ? TransferItem(json, _primaryOwner, _otherOwner, "You can't put that item in there!", $"{_otherOwner.GetName()} is full!")
                    : TransferItem(json, _otherOwner, _primaryOwner, "You can't pick this up!", "Your inventory is full!");

                if (!success) return;
                
                selected.AddCount(-1);
                InvItemDisplay refreshSelected = selected.GetCount() > 0 ? selected : null;
                InventorySide otherSide = side == InventorySide.PRIMARY ? InventorySide.OTHER : InventorySide.PRIMARY;

                SelectedInfo otherSideItem = GetSelectedItem(otherSide);

                UpdateSelectedItem(otherSide, otherSideItem.GetItem(), otherSideItem.GetSummary());
                UpdateSelectedItem(side, refreshSelected, null);
                ShowHotbarStars();
            }
        );

        return new List<InvActionButton> { _moveActionBtn, GetCloseButton() };
    }

    private bool TransferItem(string json, IContainer from, IContainer to, string filterFailMessage, string subclassFailMessage) {
        string metaTag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.META_TAG, json);
        AddItemFailCause result = to.GetInventory().CanAddItem(json);
        switch (result) {
            case AddItemFailCause.SUBCLASS_FAIL:
                Toast.Error(GameManager.I().GetPlayer(), subclassFailMessage);
                return false;
            case AddItemFailCause.FILTER_FAIL:
                Toast.Error(GameManager.I().GetPlayer(), filterFailMessage);
                return false;
            case AddItemFailCause.SUCCESS:
            default:
                to.StoreItem(metaTag, json);
                from.RemoveItem(json);
                break;
        }

        return true;
    }

    protected override void UpdateActionsList() {
        SelectedInfo primarySelected = GetSelectedItem(InventorySide.PRIMARY);
        SelectedInfo otherSelected = GetSelectedItem(InventorySide.OTHER);

        if (primarySelected.IsLastSelected()) _moveActionBtn.SetStore();
        else if (otherSelected.IsLastSelected()) _moveActionBtn.SetTake();
        else  _moveActionBtn?.Disable();
    }
}