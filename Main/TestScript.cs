using System;
using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {

    private static TestScript instance;
    private static readonly Scheduler scheduler = new();

    public TestScript() {
        instance = this;
    }

    public static TestScript I() {
        return instance;
    }

    public void PrintSomethingStupid() {
        GD.Print("Start");
    }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        I().PrintSomethingStupid();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}