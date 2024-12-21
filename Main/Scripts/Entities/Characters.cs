
using Godot;

public static class Characters {
    
    public enum ECharacterType {
        PLAYER,
        NPC1
    }
    
    public static readonly CharacterType PLAYER = new(
        ECharacterType.PLAYER, 
        "Player",
        "res://Main/Prefabs/Actors/Player.tscn",
        model => new Player((CharacterBody3D) model)
        );
    public static readonly CharacterType NPC1 = new(
        ECharacterType.NPC1, 
        "Alice", 
        "res://Main/Prefabs/Actors/Player.tscn"/*TODO: Change to an actor class*/,
        model => new NPC((CharacterBody3D) model)
        );
}