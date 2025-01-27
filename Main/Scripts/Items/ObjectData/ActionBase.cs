
using Godot;

public abstract class ActionBase {
    private ObjectActions.ActionType _actionType;
    
    private readonly string _actionName;
    private readonly int _index;

    protected ActionBase(ObjectActions.ActionType actionType, string actionName, int index) {
        _actionType = actionType;
        _actionName = actionName;
        _index = index;
    }

    public abstract void Invoke<T>(ActorBase actorBase, T node, ObjectData objectData) where T : Node3D;
    public int GetIndex() => _index;
    public string GetActionName() => _actionName;
}