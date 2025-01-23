
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class BinaryChoiceForm : FormBase {
    
    public enum BackgroundType {
        IMAGE,
        NINE_PATCH
    }
    
    private readonly ButtonElement _upperButton, _lowerButton;
    private readonly LabelElement _label, _title;
    private readonly TextureRectElement _backgroundImg;
    private readonly NinePatchRectElement _backgroundNinePatch;
    private Action<Key, BinaryChoiceForm, bool> _keyboardBehaviour;
    private BackgroundType _backgroundType = BackgroundType.IMAGE;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/YesNoFormTest.tscn",
        TITLE_PATH = "Content/CenterContainer/Control/VBoxContainer/Info/InfoList/Title",
        DESCRIPTION_PATH = "Content/CenterContainer/Control/VBoxContainer/Info/InfoList/DecriptionControl/Description",
        UPPER_BUTTON_PATH = "Content/CenterContainer/Control/VBoxContainer/Buttons/ButtonsList/Accept_btn",
        LOWER_BUTTON_PATH = "Content/CenterContainer/Control/VBoxContainer/Buttons/ButtonsList/Decline_btn",
        BACKGROUND_IMG_PATH = "Background/BackgroundTexture",
        BACKGROUND_NINEPATCH_PATH = "Background/BackgroundNinePatch";

    public BinaryChoiceForm(string nodeName, Action<Key, BinaryChoiceForm, bool> keyboardBehaviour = null) : base(nodeName, FORM_PATH) {
        Label titleNode = FindNode<Label>(TITLE_PATH);
        Label labelNode = FindNode<Label>(DESCRIPTION_PATH);
        Button upperButton = FindNode<Button>(UPPER_BUTTON_PATH);
        Button lowerButton = FindNode<Button>(LOWER_BUTTON_PATH);
        TextureRect background = FindNode<TextureRect>(BACKGROUND_IMG_PATH);
        NinePatchRect backgroundNinePatch = FindNode<NinePatchRect>(BACKGROUND_NINEPATCH_PATH);
        
        _keyboardBehaviour = keyboardBehaviour;
        
        _title = new LabelElement(titleNode);
        _label = new LabelElement(labelNode);
        _upperButton = new ButtonElement(upperButton);
        _lowerButton = new ButtonElement(lowerButton);
        _backgroundImg = new TextureRectElement(background);
        _backgroundNinePatch = new NinePatchRectElement(backgroundNinePatch);
        
        _menuElement = new ControlElement(_menu, _ => {
            foreach (IFormObject element in GetAllElements())
                element.SetTopLevelLayout(_menuElement);
        });
    }
    
    protected override List<IFormObject> GetAllElements() => new(){ _title, _label, _upperButton, _lowerButton, _backgroundImg, _backgroundNinePatch };
    public List<ButtonElement> GetButtons() => new(){ _upperButton, _lowerButton };
    
    public ButtonElement GetUpperButton() => _upperButton;
    public ButtonElement GetLowerButton() => _lowerButton;
    public LabelElement GetTitleLabel() => _title;
    public LabelElement GetDescriptionLabel() => _label;
    public TextureRectElement GetBackgroundImg() => _backgroundImg;
    public NinePatchRectElement GetBackgroundNinePatch() => _backgroundNinePatch;
    
    public void SetBackgroundType(BackgroundType backgroundType) {
        _backgroundType = backgroundType;
        switch (backgroundType) {
            case BackgroundType.IMAGE:
                _backgroundImg.GetElement().Show();
                _backgroundNinePatch.GetElement().Hide();
                break;
            case BackgroundType.NINE_PATCH:
                _backgroundImg.GetElement().Hide();
                _backgroundNinePatch.GetElement().Show();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(backgroundType), backgroundType, null);
        }
    }
    public void SetTitle(string title) => _title.GetElement().SetText(title);
    public void SetDescription(string description) => _label.GetElement().SetText(description);
    public void SetBackgroundTexture(Texture2D texture, BackgroundType backgroundType = BackgroundType.IMAGE) {
        SetBackgroundType(backgroundType);
        switch (_backgroundType) {
            case BackgroundType.IMAGE:
                _backgroundImg.SetTexture(texture);
                break;
            case BackgroundType.NINE_PATCH:
                _backgroundNinePatch.SetTexture(texture);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public void SetBackgroundAlpha(float alpha) {
        switch (_backgroundType) {
            case BackgroundType.IMAGE:
                _backgroundImg.SetAlpha(alpha);
                break;
            case BackgroundType.NINE_PATCH:
                _backgroundNinePatch.SetAlpha(alpha);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetBackgroundTexture(string path, BackgroundType backgroundType = BackgroundType.IMAGE) {
        Texture2D texture2D = ResourceLoader.Load<Texture2D>(path);
        if (texture2D == null) GD.PrintErr($"ERROR: BinaryChoiceForm.SetBackgroundTexture() : Texture at path '{path}' not found.");
        else SetBackgroundTexture(texture2D, backgroundType);
    }

    public void SetUpperText(string text) => _upperButton.GetElement().SetText(text);
    public void SetLowerText(string text) => _lowerButton.GetElement().SetText(text);
    public void OnUpperButton(Action<IFormObject> onUpper) => _upperButton.OnPressed(onUpper);
    public void OnLowerButton(Action<IFormObject> onLower) => _lowerButton.OnPressed(onLower);
    
    public void SetKeyboardBehaviour(Action<Key, BinaryChoiceForm, bool> keyboardBehaviour) => _keyboardBehaviour = keyboardBehaviour;
    public static void DefaultKeyboardBehaviour(Key key, BinaryChoiceForm form, bool isPressed) {
        if (!isPressed) return;
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

    protected override void OnDestroy() { }

    protected override void KeyboardBehaviour(Key key, bool isPressed) {
        if (_keyboardBehaviour != null) {
            _keyboardBehaviour(key, this, isPressed);
            return;
        }
        DefaultKeyboardBehaviour(key, this, isPressed);
    }
}