using System;

public interface IFilteredInventory : IInventory {
    public void AddFilter(Func<object, bool> predicate);
    public void RemoveFilter(Func<object, bool> predicate);
    public void ClearFilters();
    public bool PassesFilters(object obj);
}