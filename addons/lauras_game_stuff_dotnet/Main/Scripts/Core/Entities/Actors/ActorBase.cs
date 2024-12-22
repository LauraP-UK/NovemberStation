
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
    public ControllerBase GetController() => _controller;
    public void SetController(ControllerBase controller) => _controller = controller;
    public void SetName(string name) => _name = name;
    public void SetPosition(Vector3 position) => _model.Position = position;
}