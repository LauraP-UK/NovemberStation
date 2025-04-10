using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class PaperSheetForm : FormBase {
    private readonly TextEditElement _textEdit;

    private readonly Action<Key, PaperSheetForm, bool> _keyboardBehaviour;
    private SmartSet<Action<string[]>> _onTextChanged = new();

    private const int MAX_LINES = 25;
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/Forms/PaperSheetForm.tscn",
        TEXT_BOX_PATH = "CenterContainer/Control/TextEdit";

    public PaperSheetForm(string formName, string text = "", Action<Key, PaperSheetForm, bool> keyboardBehaviour = null) : base(formName, FORM_PATH) {
        _keyboardBehaviour = keyboardBehaviour;
        TextEdit textEdit = FindNode<TextEdit>(TEXT_BOX_PATH);
        _textEdit = new TextEditElement(textEdit);
        _textEdit.SetText(text);
        
        _textEdit.OnTextChanged(
            _ => {
                int maxLines = GetMaxLines();
                int currentLineCount = GetCurrentLines(); 
                if (currentLineCount + 1 > maxLines) {
                    Vector2 caretPosition = _textEdit.GetCaretPosition();
                    List<string> allowedText = _textEdit.GetLines().ToList().GetRange(0, maxLines);
                    _textEdit.SetText(string.Join("\n", allowedText));
                    _textEdit.SetCaretPosition(caretPosition);
                }

                string[] lines = _textEdit.GetLines();

                _onTextChanged.ForEach(action => action.Invoke(lines));
            });
        
        _menuElement = new ControlElement(_menu);
        SetListener(FormListener.Default(this));
    }

    public override bool LockMovement() => true;
    public override bool PausesGame() => false;
    protected override List<IFormObject> GetAllElements() => new() { _textEdit };
    protected override void OnDestroy() { }
    public int GetMaxLines() => MAX_LINES;
    public int GetCurrentLines() => _textEdit.GetElement().GetLineCount();
    public string GetText() => _textEdit.GetText();
    public void SetText(string text) => _textEdit.SetText(text);
    public override void KeyboardBehaviour(Key key, bool isPressed) => _keyboardBehaviour?.Invoke(key, this, isPressed);
    public void ClearOnTextChanged() => _onTextChanged.Clear();
    public void AddOnTextChanged(Action<string[]> action) {
        if (action == null) return;
        _onTextChanged.Add(action);
    }
}