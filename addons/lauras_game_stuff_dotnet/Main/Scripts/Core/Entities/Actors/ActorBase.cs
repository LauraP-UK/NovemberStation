
using System;
using Godot;

public abstract class ActorBase : IActor {
    private readonly Guid _uuid = Guid.NewGuid();
    
    private ControllerBase _controller;
    
    private string _name;
    private Vector3 _position;
    
    public Guid GetUUID() => _uuid;
    public string GetName() => _name;
    public Vector3 GetPosition() => _position;
    
    public void SetController(ControllerBase controller) {
        _controller = controller;
    }
    
    protected void SetName(string name) {
        _name = name;
    }
    
    public void SetPosition(Vector3 position) {
        _position = position;
    }
}