using System;
using Godot;

public class TextEditElement : FormElement<TextEdit>  {
    public TextEditElement(TextEdit element = null, Action<TextEdit> onReady = null) : base(element, onReady) {}
    public TextEditElement(string text, Action<TextEdit> onReady = null) : base(text, onReady) {}
    public void SetText(string text) => GetElement().Text = text;
    public void SetAlpha(float alpha) => GetElement().Modulate = new Color(1, 1, 1, alpha);
    public string GetText() => GetElement().Text;
    public string[] GetLines() => GetElement().Text.Split("\n");
    public Vector2 GetCaretPosition() => new(GetElement().GetCaretLine(), GetElement().GetCaretColumn());
    public void SetCaretPosition(Vector2 position) => SetCaretPosition((int)position.X, (int)position.Y);
    public void SetCaretPosition(int line, int column) {
        GetElement().SetCaretLine(line);
        GetElement().SetCaretColumn(column);
    }

    public void OnTextChanged(Action<IFormObject> action) => AddAction(TextEdit.SignalName.TextChanged, action);
}