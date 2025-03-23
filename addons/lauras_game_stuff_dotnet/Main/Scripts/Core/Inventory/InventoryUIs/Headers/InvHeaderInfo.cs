using System.Collections.Generic;
using Godot;

public class InvHeaderInfo : FormBase {
    private readonly LabelElement _nameLabel, _infoLabel;
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/InvHeaderInfo.tscn",
        NAME_LABEL = "NameLabel",
        INFO_LABEL = "InfoLabel";
    public InvHeaderInfo(string invName, Vector2 size) : base(invName + "_header_info", FORM_PATH) {
        Label nameLabel = FindNode<Label>(NAME_LABEL);
        Label infoLabel = FindNode<Label>(INFO_LABEL);
        
        _nameLabel = new LabelElement(nameLabel);
        _infoLabel = new LabelElement(infoLabel);
        
        _nameLabel.SetText(invName);
        
        _menuElement = new ControlElement(_menu);
        _menu.SetCustomMinimumSize(size);
    }
    protected override List<IFormObject> GetAllElements() => new() { _nameLabel, _infoLabel };
    protected override void OnDestroy() { }
    public void SetWeightInfo(float weight, float maxWeight) => _infoLabel.SetText($"{weight} / {maxWeight} kg");
    public void SetCapacityInfo(int current, int max) => _infoLabel.SetText($"{current} / {max}");
    public LabelElement GetNameLabel() => _nameLabel;
    public LabelElement GetInfoLabel() => _infoLabel;
}