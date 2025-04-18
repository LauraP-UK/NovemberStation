[ObjectAction("Shove", 1)]
public interface IShovable : IObjectAction {
    public void Shove(ActorBase actorBase, IEventBase ev);
}