
using Godot;
using NovemberStation.Main;

public class QuitGameAction : GameActionBase {
    public QuitGameAction(GameAction.Action action) : base(action) { }
    
    [EventListener]
    private void OnQuitKeyPress(KeyPressEvent ev, Key key) {
        if (!IsValidKey(key)) return;
        TestScript.I().Quit();
    }
}