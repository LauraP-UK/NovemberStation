public class CollectActionDefault : BaseActionDefault {
    public static void Invoke(ActorBase actor, IObjectBase obj, IEventBase ev, MouseType mouseType = MouseType.NONE, KeyType keyType = KeyType.DOWN) {
        if (!CanRun(ev, mouseType) && !CanRun(ev, keyType)) return;
        AddItemFailCause result = ((IContainer)actor).StoreItem(obj, obj.GetBaseNode3D());
        if (result == AddItemFailCause.SUBCLASS_FAIL) Toast.Error((Player)actor, "Your inventory is full!");
        else if (result == AddItemFailCause.FILTER_FAIL) Toast.Error((Player)actor, "You can't pick up this item!");
        else GameManager.WakeObjects();
    }
}