using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public class InteractionZoneBuilder<TNode, TObject> where TNode : Node3D where TObject : IObjectBase {
    private readonly string _identifier;
    private readonly TNode _containingNode;
    private readonly TObject _objectBase;
    private readonly List<PendingAction> _actions = [];
    private readonly List<PendingArbitraryAction> _arbitraryActions = [];
    private Func<string> _getDisplayName = () => "";
    private Func<string> _getContext = () => "";
    private Func<bool> _isActive = () => true;

    private InteractionZoneBuilder(string identifier, TNode containingNode, TObject objectBase) {
        _identifier = identifier;
        _containingNode = containingNode;
        _objectBase = objectBase;
    }
    
    public InteractionZoneBuilder<TNode, TObject> WithIsActive(Func<bool> isActive) {
        _isActive = isActive;
        return this;
    }
    
    public InteractionZoneBuilder<TNode, TObject> WithDisplayName(Func<string> getDisplayName) {
        _getDisplayName = getDisplayName;
        return this;
    }
    
    public InteractionZoneBuilder<TNode, TObject> WithDisplayName(string displayName) => WithDisplayName(() => displayName);

    public InteractionZoneBuilder<TNode, TObject> WithContext(Func<string> getContext) {
        _getContext = getContext;
        return this;
    }
    
    public InteractionZoneBuilder<TNode, TObject> WithContext(string context) => WithContext(() => context);

    public InteractionZoneBuilder<TNode, TObject> WithAction<TAction>(Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> run) where TAction : IObjectAction {
        _actions.Add(
            new PendingAction {
                ActionType = typeof(TAction),
                Test = test,
                Run = run
            }
        );
        return this;
    }

    public InteractionZoneBuilder<TNode, TObject> WithArbitraryAction(string name, int index, Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> action) {
        _arbitraryActions.Add(
            new PendingArbitraryAction {
                Name = name,
                Index = index,
                Test = test,
                Action = action
            }
        );
        return this;
    }

    public InteractionZone<TNode, TObject> Build() {
        return new InteractionZone<TNode, TObject>(
            _identifier, _containingNode, _objectBase, _getDisplayName, _getContext, _isActive,
            zone => {
                foreach (PendingAction action in _actions) {
                    try {
                        MethodInfo registerMethod = typeof(ActionHolder).GetMethod("RegisterAction", BindingFlags.Instance | BindingFlags.NonPublic);
                        MethodInfo genericRegister = registerMethod.MakeGenericMethod(action.ActionType);
                        genericRegister.Invoke(zone, [action.Test, action.Run]);
                    } catch (Exception e) {
                        GD.PrintErr($"ERROR: InteractionZoneBuilder<TNode, TObject>.Build() : Failed to register action '{action.ActionType.Name}'. Exception: {e.Message}");
                    }
                }

                foreach (PendingArbitraryAction action in _arbitraryActions) {
                    try {
                        MethodInfo registerMethod = typeof(ActionHolder).GetMethod("RegisterArbitraryAction", BindingFlags.Instance | BindingFlags.NonPublic);
                        registerMethod.Invoke(zone, [action.Name, action.Index, action.Test, action.Action]);
                    } catch (Exception e) {
                        GD.PrintErr($"ERROR: InteractionZoneBuilder<TNode, TObject>.Build() : Failed to register arbitrary action '{action.Name}'. Exception: {e.Message}");
                    }
                }
            }
        );
    }

    private struct PendingAction {
        public Type ActionType;
        public Func<ActorBase, IEventBase, bool> Test;
        public Action<ActorBase, IEventBase> Run;
    }

    private struct PendingArbitraryAction {
        public string Name;
        public int Index;
        public Func<ActorBase, IEventBase, bool> Test;
        public Action<ActorBase, IEventBase> Action;
    }

    public static InteractionZoneBuilder<TNode, TObject> Builder(string identifier, TNode containingNode, TObject objectBase) => new(identifier, containingNode, objectBase);
}