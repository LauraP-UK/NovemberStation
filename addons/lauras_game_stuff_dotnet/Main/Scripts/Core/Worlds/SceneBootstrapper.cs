using Godot;

public abstract partial class SceneBootstrapper : Node3D {
    private bool _pause;
    protected abstract void OnPhysicsProcess(double delta);
    protected abstract void OnProcess(double delta);
    protected abstract void OnReady();
    public abstract SmartDictionary<ulong, IObjectBase> GetObjects();
    public abstract T GetObjectClass<T>(ulong id) where T : IObjectBase;
    public void Pause(bool pause) {
        _pause = pause;
        PhysicsServer3D.SetActive(!pause);
    }

    public bool IsPaused() => _pause;
    public override void _PhysicsProcess(double delta) {
        if (IsPaused()) return;
        OnPhysicsProcess(delta);
    }

    public override void _Process(double delta) {
        if (IsPaused()) return;
        OnProcess(delta);
    }

    public override void _Ready() => OnReady();
}