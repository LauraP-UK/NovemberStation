using System;
using System.Collections.Generic;
using Godot;

public class HotbarForm : FormBase, IProcess {
    private readonly HBoxContainerElement _hbContainer;
    private readonly ColorRectElement _bgColour;
    
    private readonly SmartDictionary<int, HotbarIcon> _hotbarIcons = new();
    
    private IHotbarActor _owner;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/Hotbar.tscn",
        HBOX_PATH = "Sizer/HBoxContainer",
        BACKGROUND = "Sizer/BGColour";

    public HotbarForm(string formName) : base(formName, FORM_PATH) {
        HBoxContainer hboxContainer = FindNode<HBoxContainer>(HBOX_PATH);
        ColorRect bgColour = FindNode<ColorRect>(BACKGROUND);
        
        _hbContainer = new HBoxContainerElement(hboxContainer);
        _bgColour = new ColorRectElement(bgColour);

        _menuElement = new ControlElement(_menu);

        for (int i = 0; i < Hotbar.HOTBAR_SIZE; i++) {
            HotbarIcon hotbarIcon = new();
            _hotbarIcons.Add(i, hotbarIcon);
            _hbContainer.GetElement().AddChild(hotbarIcon.GetMenu());
        }
        
        _bgColour.GetElement().SetCustomMinimumSize(new Vector2(80 * Hotbar.HOTBAR_SIZE, 80));
        _bgColour.SetAlpha(0.5f);
    }

    protected override List<IFormObject> GetAllElements() => new() { _hbContainer };
    protected override void OnDestroy() { }

    public void SetOwner(IHotbarActor owner) => _owner = owner;

    public void Process(float delta) {
        if (_owner == null) return;

        IInventory inventory = _owner.GetInventory();
        int selectedIndex = _owner.GetHotbar().GetIndex();

        SmartDictionary<int,Guid> items = _owner.GetHotbar().GetHotbarItems();
        foreach (KeyValuePair<int,HotbarIcon> icon in _hotbarIcons) {
            Guid guid = items.GetOrDefault(icon.Key, Guid.Empty);
            if (Guid.Empty.Equals(guid)) {
                icon.Value.SetIcon(null);
                icon.Value.Highlight(false);
                continue;
            }
            string itemJson = inventory.GetViaGUID(guid);
            string itemTag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, itemJson);
            icon.Value.SetIcon(Items.GetViaID(itemTag));
            icon.Value.Highlight(icon.Key == selectedIndex);
        }
    }
    public override bool LockMovement() => false;
    public override bool PausesGame() => false;
}