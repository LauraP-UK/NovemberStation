
[ObjectAction("Grab", 0)]
public interface IGrabbable : IObjectAction {
    public void Grab(ActorBase actorBase, IEventBase ev);
}