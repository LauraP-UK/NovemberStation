
using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class ActionNode : Node {
        private readonly string _signalName;
        private readonly Action<IFormObject, object[]> _callback;
        private readonly IFormElement _formElement;

        public ActionNode(string signalName, Action<IFormObject, object[]> callback, IFormElement formElement) {
            _signalName = signalName;
            _callback = callback;
            _formElement = formElement;
        }

        public override void _Ready() => _formElement.GetElement().Connect(_signalName, GetCallable());

        private void RunActionNoArgs() {
            try {
                _callback?.Invoke(_formElement, System.Array.Empty<object>()); // Invoke callback with an empty argument array
            } catch (Exception e) {
                GD.PrintErr($"Error in ActionNode callback for {_signalName}: {e.Message}\n{e.StackTrace}");
            }
        }

        // Overloaded method for signals with arguments
        private void RunActionArgs(params object[] args) {
            try {
                _callback?.Invoke(_formElement, args);
            } catch (Exception e) {
                GD.PrintErr($"Error in ActionNode callback for {_signalName}: {e.Message}\n{e.StackTrace}");
            }
        }

        private Callable GetCallable() {
            int paramsCount = -1;
            foreach (Dictionary dictionary in _formElement.GetElement().GetSignalList()) {
                if ((string) dictionary["name"] != _signalName) continue;
                paramsCount = ((Array) dictionary["args"]).Count;
                break;
            }
            if (paramsCount == -1)
                throw new InvalidOperationException($"Signal '{_signalName}' not found on node '{_formElement.GetElement().Name}'.");
            
            return paramsCount == 0 ? Callable.From(RunActionNoArgs) : Callable.From((Action<object[]>) RunActionArgs);
        }
}