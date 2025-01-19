using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class SignalNode : Node {
    private readonly string _signalName;
    private readonly Action<IFormObject, object[]> _callback;
    private readonly IFormObject _formObject;

    public SignalNode(string signalName, Action<IFormObject, object[]> callback, IFormObject formObject) {
        _signalName = signalName;
        _callback = callback;
        _formObject = formObject;
    }

    public override void _Ready() => GetControlNode().Connect(_signalName, GetCallable());

    public void RunActionNoArgs() {
        try {
            _callback?.Invoke(_formObject, System.Array.Empty<object>());
        }
        catch (Exception e) {
            GD.PrintErr($"Error in ActionNode callback for {_signalName}: {e.Message}\n{e.StackTrace}");
        }
    }

    public void RunActionArgs(params object[] args) {
        try {
            _callback?.Invoke(_formObject, args);
        }
        catch (Exception e) {
            GD.PrintErr($"Error in ActionNode callback for {_signalName}: {e.Message}\n{e.StackTrace}");
        }
    }

    public Callable GetCallable() {
        int paramsCount = -1;

        Control node = GetControlNode();

        foreach (Dictionary dictionary in node.GetSignalList()) {
            if ((string)dictionary["name"] != _signalName) continue;
            paramsCount = ((Array)dictionary["args"]).Count;
            break;
        }

        if (paramsCount == -1)
            throw new InvalidOperationException($"Signal '{_signalName}' not found on node '{node.Name}'.");

        return paramsCount == 0 ? Callable.From(RunActionNoArgs) : Callable.From((Action<object[]>)RunActionArgs);
    }

    private Control GetControlNode() => _formObject.GetNode();
}