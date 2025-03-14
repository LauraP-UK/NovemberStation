public interface IOwnableInventory {
    public IContainer GetOwner();
    public void SetOwner(IContainer owner);
}