[ObjectAction("Write On", 5)]
public interface IWritable : IObjectAction {
    public void OpenWritingUI(ActorBase actorBase, IEventBase ev);
}