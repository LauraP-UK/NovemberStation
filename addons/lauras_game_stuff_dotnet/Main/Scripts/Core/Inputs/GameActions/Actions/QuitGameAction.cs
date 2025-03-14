
using Godot;

public class QuitGameAction : GameActionBase {
    public QuitGameAction(GameAction.Action action) : base(action) { }
    
    [EventListener(PriorityLevels.TERMINUS)]
    private void OnQuitKeyPress(KeyPressEvent ev, Key key) {
        if (!IsValidInput(key) || ev.IsCaptured()) return;
        GameManager.I().PopPauseMenu();
    }
}