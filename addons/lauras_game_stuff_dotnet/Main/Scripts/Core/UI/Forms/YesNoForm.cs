
using System;
using System.Collections.Generic;
using Godot;

public class YesNoForm {
    
    private readonly ButtonElement _yesButton, _noButton;
    private readonly LabelElement _label, _title;
    private readonly Control _menu;
    private readonly ControlLayout _menuLayout;
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/YesNoFormTest.tscn",
        TITLE_PATH = "Content/CenterContainer/Control/VBoxContainer/Info/InfoList/Title",
        DESCRIPTION_PATH = "Content/CenterContainer/Control/VBoxContainer/Info/InfoList/DecriptionControl/Description",
        YES_BUTTON_PATH = "Content/CenterContainer/Control/VBoxContainer/Buttons/ButtonsList/Accept_btn",
        NO_BUTTON_PATH = "Content/CenterContainer/Control/VBoxContainer/Buttons/ButtonsList/Decline_btn";

    public YesNoForm(string nodeName) {
        _menu = Loader.SafeInstantiate<Control>(FORM_PATH, true);
        _menu.Name = nodeName;
        
        Label titleNode = _menu.GetNode<Label>(TITLE_PATH);
        Label labelNode = _menu.GetNode<Label>(DESCRIPTION_PATH);
        Button yesButton = _menu.GetNode<Button>(YES_BUTTON_PATH);
        Button noButton = _menu.GetNode<Button>(NO_BUTTON_PATH);
        
        _title = new LabelElement(titleNode);
        _label = new LabelElement(labelNode);
        _yesButton = new ButtonElement(yesButton);
        _noButton = new ButtonElement(noButton);
        
        _menuLayout = new ControlLayout(_menu, _ => {
            foreach (IFormElement element in GetElements()) {
                element.ConnectSignals();
                element.SetTopLevelLayout(_menuLayout);
            }
        });
        _menuLayout.Build();
    }
    
    private List<IFormElement> GetElements() => new(){ _title, _label, _yesButton, _noButton };
    public void SetTitle(string title) => _title.GetElement().SetText(title);
    public void SetDescription(string description) => _label.GetElement().SetText(description);
    public void SetYesText(string text) => _yesButton.GetElement().SetText(text);
    public void SetNoText(string text) => _noButton.GetElement().SetText(text);
    public void OnYes(Action<IFormObject> onYes) => _yesButton.OnPressed(onYes);
    public void OnNo(Action<IFormObject> onNo) => _noButton.OnPressed(onNo);
    public Control GetMenu() => _menu;
}