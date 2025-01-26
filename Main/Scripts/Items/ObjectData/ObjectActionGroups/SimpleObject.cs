
using Godot;
using NovemberStation.Main.Scripts.Items.ObjectData;

public class SimpleObject<T> : ObjectDataBase<T> where T : Node3D {
    
    protected override void SetupActions() {
        AddAction(ObjectActions.GRAB_ACTION);
    }
}