using Godot;

public interface IFormObject {
    public IFormObject GetTopLevelLayout();
    public void SetTopLevelLayout(IFormObject layout);
    public bool CaptureInput();
    public Control GetNode();
    public void Destroy();
}