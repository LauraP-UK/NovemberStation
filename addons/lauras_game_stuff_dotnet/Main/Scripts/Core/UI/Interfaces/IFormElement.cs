

using Godot;

public interface IFormElement : IFormObject {
    public Control GetElement();
    public void ConnectSignals();
}