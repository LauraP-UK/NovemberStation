using System.Collections.Generic;
using Godot;

public class InvHeaderInfo : FormBase {

    private readonly LabelElement _nameLabel, _weightLabel;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/InvHeaderInfo.tscn",
        NAME_LABEL = "NameLabel",
        WEIGHT_LABEL = "WeightLabel";

    public InvHeaderInfo(string invName, Vector2 size) : base(invName + "_header_info", FORM_PATH) {
        Label nameLabel = FindNode<Label>(NAME_LABEL);
        Label weightLabel = FindNode<Label>(WEIGHT_LABEL);
        
        _nameLabel = new LabelElement(nameLabel);
        _weightLabel = new LabelElement(weightLabel);
        
        _nameLabel.SetText(invName);
        
        _menuElement = new ControlElement(_menu);
        _menu.SetCustomMinimumSize(size);
    }
    protected override List<IFormObject> GetAllElements() => new() { _nameLabel, _weightLabel };
    protected override void OnDestroy() { }
    
    public void SetWeight(float weight, float maxWeight) => _weightLabel.SetText($"{weight:0.00}/{maxWeight:0.00} kg");
    
    public LabelElement GetNameLabel() => _nameLabel;
    public LabelElement GetWeightLabel() => _weightLabel;
}