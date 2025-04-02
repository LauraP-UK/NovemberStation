public interface IHotbarActor : IContainer {
    public Hotbar GetHotbar();
    public IObjectBase GetHandItem();
    public IObjectBase SetHeldItem(string json);
    public IObjectBase SetHeldItem(ItemType itemType);
    public void ClearHeldItem();
}