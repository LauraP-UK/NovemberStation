
using System.Linq;
using Godot;

public abstract class ObjectDataBase<T> : IObjectData where T : Node3D {
    
    protected readonly SmartSet<ActionBase> _actions = new();
    protected readonly T _object;

    protected abstract void SetupActions();

    public Node3D GetObject() => _object;
    protected void AddAction(ActionBase action) => _actions.Add(action);
    public void InvokeAction(int index, ActorBase actorBase) => _actions.First(a => a.GetIndex() == index).Invoke(actorBase, this);

}