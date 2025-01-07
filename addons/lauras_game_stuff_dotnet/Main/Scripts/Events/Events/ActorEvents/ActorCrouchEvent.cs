
public class ActorCrouchEvent : ActorEventBase {
    
    private bool _startCrouch;
    
    public ActorCrouchEvent(ActorBase actor) => SetActor(actor);
    
    public void SetStartCrouch(bool startCrouch) => _startCrouch = startCrouch;
    public bool IsStartCrouch() => _startCrouch;
}