
public class NoOpGameAction : GameActionBase {
    
    public NoOpGameAction(GameAction.Action action) : base(action) { }
    
    public override void RegisterListeners() { }
}