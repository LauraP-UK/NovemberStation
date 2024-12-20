
using Godot;

public class MovementGameAction : GameActionBase {
    public MovementGameAction(GameAction.Action action) : base(action) {}
    public override void RegisterListeners() {}

    private void test() {
        ActorMoveEvent actorMoveEvent = GetEvent<ActorMoveEvent, string>();
        actorMoveEvent.SetActorName("Player");
        actorMoveEvent.SetDummyLocation(new Vector3(1, 0, 0));
        actorMoveEvent.Fire();
    }
}