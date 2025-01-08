
using System;
using Godot;

public partial class ActionNode : Node {
    private string _signal;
    private readonly Action<object[]> _callback;
    public ActionNode(string signal, Action<object[]> callback) {
        _signal = signal;
        _callback = callback;
    }

    public void RunAction(params object[] args) {
        try {
            _callback?.Invoke(args);
        } catch (Exception e) {
            GD.PrintErr($"Error in ActionNode callback for {_signal} : {e.Message}\n{e.StackTrace}");
        }
    }
}