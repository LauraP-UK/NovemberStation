
using System;
using Godot;

public abstract class ActorBase : IActor {
    private readonly Guid _uuid = Guid.NewGuid();
    private readonly CharacterBody3D _model;

    private ControllerBase _controller;
    private string _name;

    protected ActorBase(CharacterBody3D model) {
        _model = model;
    }
    
    public Guid GetUUID() => _uuid;
    public string GetName() => _name;
    public Vector3 GetPosition() => _model.Position;
    public CharacterBody3D GetModel() => _model;
    public Node3D GetVisualModel() => GetModel().GetNode<Node3D>("WorldModel");
    public CollisionShape3D GetCollisionShape() => GetModel().GetNode<CollisionShape3D>("CapsuleCollider");
    public RayCast3D GetBelowRaycast() => GetModel().GetNode<RayCast3D>("StairsRaycasts/BelowCast");
    public RayCast3D GetFrontRaycast() => GetModel().GetNode<RayCast3D>("StairsRaycasts/FrontCast");
    public ControllerBase GetController() => _controller;
    public T GetController<T>() where T : ControllerBase => (T) _controller;
    public void SetController(ControllerBase controller) => _controller = controller;
    public void SetName(string name) => _name = name;
    public void SetPosition(Vector3 position, Vector3 rotation = default) {
        _model.Position = position;
        if (rotation != default) _model.RotationDegrees = rotation;
    }

    public override int GetHashCode() => _uuid.GetHashCode();

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return _uuid == ((ActorBase) obj)._uuid;
    }
}