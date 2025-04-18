[ObjectAction("Drink", 4)]
public interface IDrinkable : IObjectAction {
    public void Drink(ActorBase actorBase, IEventBase ev);
}