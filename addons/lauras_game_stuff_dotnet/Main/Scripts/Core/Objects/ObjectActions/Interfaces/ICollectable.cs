[ObjectAction("Collect", 2)]
public interface ICollectable : IObjectAction {
    public void Collect(ActorBase actorBase, IEventBase ev);
}