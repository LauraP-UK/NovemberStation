using System;
using Godot;

public class CharacterType {
    private readonly Characters.ECharacterType _characterType;
    private readonly string _name, _resource;
    private readonly Func<Node3D, ActorBase> _factory;

    public CharacterType(Characters.ECharacterType characterType, string name, string resource, Func<Node3D, ActorBase> factory) {
        _characterType = characterType;
        _name = name;
        _resource = resource;
        _factory = factory;
    }
    
    public Characters.ECharacterType GetCharacterType() => _characterType;
    public string GetName() => _name;
    public string GetResource() => _resource;

    public ActorBase CreateActor() {
        PackedScene actorScene = GD.Load<PackedScene>(GetResource());
        if (actorScene == null)
            throw new InvalidOperationException($"ERROR: CharacterType.CreateActor() : Failed to load scene from resource {GetResource()}  |  CharacterType: {GetCharacterType()}, Name: {GetName()}");

        Node3D model = actorScene.Instantiate<Node3D>();
        model.Position = Vector3.Zero;
        ActorBase actor = _factory(model);
        
        if (actor == null)
            throw new InvalidOperationException($"ERROR: CharacterType.CreateActor() : Failed to create actor from scene {GetResource()}  |  CharacterType: {GetCharacterType()}, Name: {GetName()}");
        
        actor.SetName(GetName());
        actor.SetPosition(Vector3.Zero);
        actor.GetModel().ProcessMode = Node.ProcessModeEnum.Pausable;
        return actor;
    }

}