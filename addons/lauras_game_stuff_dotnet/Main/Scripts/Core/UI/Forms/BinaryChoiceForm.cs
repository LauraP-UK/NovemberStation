
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BinaryChoiceForm : FormBase {
    
    private readonly ButtonElement _upperButton, _lowerButton;
    private readonly LabelElement _label, _title;
    private readonly TextureRectElement _background;
    private Action<Key, BinaryChoiceForm> _keyboardBehaviour;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/YesNoFormTest.tscn",
        TITLE_PATH = "Content/CenterContainer/Control/VBoxContainer/Info/InfoList/Title",
        DESCRIPTION_PATH = "Content/CenterContainer/Control/VBoxContainer/Info/InfoList/DecriptionControl/Description",
        UPPER_BUTTON_PATH = "Content/CenterContainer/Control/VBoxContainer/Buttons/ButtonsList/Accept_btn",
        LOWER_BUTTON_PATH = "Content/CenterContainer/Control/VBoxContainer/Buttons/ButtonsList/Decline_btn",
        BACKGROUND_PATH = "Background/BackgroundTexture";

    public BinaryChoiceForm(string nodeName, Action<Key, BinaryChoiceForm> keyboardBehaviour = null) : base(nodeName, FORM_PATH) {
        Label titleNode = _menu.GetNode<Label>(TITLE_PATH);
        Label labelNode = _menu.GetNode<Label>(DESCRIPTION_PATH);
        Button upperButton = _menu.GetNode<Button>(UPPER_BUTTON_PATH);
        Button lowerButton = _menu.GetNode<Button>(LOWER_BUTTON_PATH);
        TextureRect background = _menu.GetNode<TextureRect>(BACKGROUND_PATH);
        
        _keyboardBehaviour = keyboardBehaviour;
        
        _title = new LabelElement(titleNode);
        _label = new LabelElement(labelNode);
        _upperButton = new ButtonElement(upperButton);
        _lowerButton = new ButtonElement(lowerButton);
        _background = new TextureRectElement(background);
        
        _menuLayout = new ControlLayout(_menu, _ => {
            foreach (IFormElement element in GetElements()) {
                element.ConnectSignals();
                element.SetTopLevelLayout(_menuLayout);
            }
        });
        _menuLayout.Build();
    }
    
    protected override List<IFormElement> GetElements() => new(){ _title, _label, _upperButton, _lowerButton };
    public List<ButtonElement> GetButtons() => new(){ _upperButton, _lowerButton };
    
    public ButtonElement GetUpperButton() => _upperButton;
    public ButtonElement GetLowerButton() => _lowerButton;
    public LabelElement GetTitleLabel() => _title;
    public LabelElement GetDescriptionLabel() => _label;
    public TextureRectElement GetBackground() => _background;
    
    public void SetTitle(string title) => _title.GetElement().SetText(title);
    public void SetDescription(string description) => _label.GetElement().SetText(description);
    public void SetBackgroundTexture(Texture2D texture) => _background.GetElement().Texture = texture;
    public void SetBackgroundTexture(string path) {
        Texture2D texture2D = ResourceLoader.Load<Texture2D>(path);
        _background.GetElement().SetTexture(texture2D);
    }

    public void SetUpperText(string text) => _upperButton.GetElement().SetText(text);
    public void SetLowerText(string text) => _lowerButton.GetElement().SetText(text);
    public void OnUpperButton(Action<IFormObject> onUpper) => _upperButton.OnPressed(onUpper);
    public void OnLowerButton(Action<IFormObject> onLower) => _lowerButton.OnPressed(onLower);
    
    public void SetKeyboardBehaviour(Action<Key, BinaryChoiceForm> keyboardBehaviour) => _keyboardBehaviour = keyboardBehaviour;
    public static void DefaultKeyboardBehaviour(Key key, BinaryChoiceForm form) {
        switch (key) {
            case Key.W: {
                form.GetUpperButton().GetElement().GrabFocus();
                return;
            }
            case Key.S: {
                form.GetLowerButton().GetElement().GrabFocus();
                return;
            }
            case Key.Space: {
                foreach (ButtonElement button in form.GetButtons().Where(button => button.GetElement().HasFocus())) {
                    button.ForcePressed();
                    return;
                }
                break;
            }
            case Key.Escape: {
                form.GetLowerButton().ForcePressed();
                return;
            }
        }
    }

    protected override void KeyboardBehaviour(Key key) {
        if (_keyboardBehaviour != null) {
            _keyboardBehaviour(key, this);
            return;
        }
        DefaultKeyboardBehaviour(key, this);
    }
}