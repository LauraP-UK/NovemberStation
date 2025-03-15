public interface ICollectable : IObjectAction {
    public void Collect(ActorBase actorBase, IEventBase ev);
}