
using System.Linq;
using Godot;

public abstract class ObjectData {
    private readonly SmartSet<ActionBase> _actions = new();
    protected ObjectData() => ObjectActionRegister.Register(this);
    protected void AddAction(ActionBase action) => _actions.Add(action);
    public void InvokeAction<T>(int index, ActorBase actorBase, T node) where T : Node3D => _actions.First(a => a.GetIndex() == index).Invoke(actorBase, node, this);
    public SmartSet<ActionBase> GetActions() => new(_actions);
    public abstract string GetMetaTag();
}