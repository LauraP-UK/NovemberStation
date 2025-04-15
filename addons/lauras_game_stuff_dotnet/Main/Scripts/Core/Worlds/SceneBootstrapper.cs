using Godot;

public abstract partial class SceneBootstrapper : Node3D {
    protected abstract void OnPhysicsProcess(double delta);
    protected abstract void OnProcess(double delta);
    protected abstract void OnReady();
    public abstract SmartDictionary<ulong, IObjectBase> GetObjects();
    public abstract T GetObjectClass<T>(ulong id) where T : IObjectBase;
    public override void _PhysicsProcess(double delta) {
        if (GameManager.IsPaused()) return;
        OnPhysicsProcess(delta);
    }

    public override void _Process(double delta) {
        if (GameManager.IsPaused()) return;
        OnProcess(delta);
    }

    public override void _Ready() => OnReady();
}