using Godot;

public interface IFormObject {
    public ILayoutElement GetTopLevelLayout();
    public void SetTopLevelLayout(ILayoutElement layout);
    public Control GetNode();
}