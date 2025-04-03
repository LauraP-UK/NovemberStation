using System;
using System.Collections.Generic;
using System.Linq;

public class QuantitativeInventory : InventoryBase, IQuantitativeInventory, IOwnableInventory, IFilteredInventory {
    private int _maxQuantity;
    private IContainer _owner;
    
    private readonly SmartSet<Func<string, bool>> _filters = new();
    
    public QuantitativeInventory(int maxQuantity, IContainer owner) {
        _maxQuantity = maxQuantity;
        _owner = owner;
    }
    
    public override string GetName() => "Quantitative Inventory";
    protected override AddItemFailCause CanAddInternal(IObjectBase item) => GetRemainingSlots() > 0 ? AddItemFailCause.SUCCESS : AddItemFailCause.SUBCLASS_FAIL;
    protected override AddItemFailCause CanAddInternal(string jsonData) => GetRemainingSlots() > 0 ? AddItemFailCause.SUCCESS : AddItemFailCause.SUBCLASS_FAIL;
    public int GetMaxQuantity() => _maxQuantity;
    public void SetMaxQuantity(int maxQuantity) => _maxQuantity = maxQuantity;
    public int GetUsedQuantity() => GetContents().Count;
    public int GetRemainingSlots() => Math.Max(_maxQuantity - GetUsedQuantity(), 0);
    public IContainer GetOwner() => _owner;
    public void SetOwner(IContainer owner) => _owner = owner;
    
    public void AddFilter(Func<object, bool> predicate) => _filters.Add(predicate);
    public void AddFilters(IEnumerable<Func<object, bool>> predicates) => _filters.UnionWith(predicates);
    public void RemoveFilter(Func<object, bool> predicate) => _filters.Remove(predicate);
    public void ClearFilters() => _filters.Clear();
    public bool PassesFilters(object obj) => _filters.Cast<Func<object, bool>>().All(filter => filter(obj));
}