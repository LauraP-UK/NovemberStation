using System.Collections.Generic;
using Godot;

public class TimeDisplayForm : FormBase {
    private readonly LabelElement _hoursLabel, _minutesLabel, _dividerLabel;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/TimeDisplayForm.tscn",
        HOURS_PATH = "CContainer/HContainer/Hours",
        MINUTES_PATH = "CContainer/HContainer/Minutes",
        DIVIDER_PATH = "CContainer/HContainer/Divider";
    
    public TimeDisplayForm() : base("time_display_form", FORM_PATH) {
        Label hoursLabel = FindNode<Label>(HOURS_PATH);
        Label minutesLabel = FindNode<Label>(MINUTES_PATH);
        Label dividerLabel = FindNode<Label>(DIVIDER_PATH);
        
        _hoursLabel = new LabelElement(hoursLabel);
        _minutesLabel = new LabelElement(minutesLabel);
        _dividerLabel = new LabelElement(dividerLabel);
        
        _menuElement = new ControlElement(_menu);
        SetCaptureInput(false);
    }
    protected override List<IFormObject> GetAllElements() => new() { _hoursLabel, _minutesLabel, _dividerLabel };
    protected override void OnDestroy() {}
    
    public void SetTime(int hours, int minutes) {
        _hoursLabel.SetText($"{hours:00}");
        _minutesLabel.SetText($"{minutes:00}");
    }
    
    public void ShowDivider(bool show) => _dividerLabel.SetAlpha(show ? 1.0f : 0.0f);
}