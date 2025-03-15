public class CollectActionDefault : BaseActionDefault {
    public static void Invoke(ActorBase actor, IObjectBase obj, IEventBase ev, MouseType mouseType = MouseType.NONE, KeyType keyType = KeyType.DOWN) {
        if (!CanRun(ev, mouseType) && !CanRun(ev, keyType)) return;
        bool success = ((IContainer)actor).StoreItem(obj, obj.GetBaseNode3D());
        if (!success) Toast.Error((Player)actor, "Your inventory is full!");
    }
}