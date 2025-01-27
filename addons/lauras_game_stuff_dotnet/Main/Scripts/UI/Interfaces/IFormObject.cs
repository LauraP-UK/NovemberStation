using System;
using Godot;

public interface IFormObject {
    public Guid GetId();
    public IFormObject GetTopLevelLayout();
    public void SetTopLevelLayout(IFormObject layout);
    public bool CaptureInput();
    public bool RequiresProcess();
    public Control GetNode();
    public void Destroy();
}