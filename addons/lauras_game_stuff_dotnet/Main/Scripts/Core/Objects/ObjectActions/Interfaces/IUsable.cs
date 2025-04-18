[ObjectAction("Use", 3)]
public interface IUsable : IObjectAction {
    public void Use(ActorBase actorBase, IEventBase ev);
}