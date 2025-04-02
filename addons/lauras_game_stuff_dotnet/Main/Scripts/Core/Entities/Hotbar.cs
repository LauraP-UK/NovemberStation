using System;
using System.Collections.Generic;
using Godot;

public class Hotbar {
    public const int HOTBAR_SIZE = 9;
    
    private readonly SmartDictionary<int, Guid> _hotbarGuids = new();
    private readonly IHotbarActor _owner;
    private int _hotbarIndex;
    
    public Hotbar(IHotbarActor owner) => _owner = owner;
    
    public void ChangeIndex(int change) {
        _hotbarIndex += change;
        _hotbarIndex = Mathf.Wrap(_hotbarIndex, 0, GetHotbarSize());
    }

    public void UpdateOwnerHeldItem() {
        Guid hotbarItem = GetHotbarItem();
        if (hotbarItem == Guid.Empty) {
            _owner.ClearHeldItem();
            return;
        }
        
        string hotbarJson = _owner.GetInventory().GetViaGUID(hotbarItem);
        _owner.SetHeldItem(hotbarJson);
    }

    public bool AddToHotbar(Guid guid) {
        if (_hotbarGuids.Values.Contains(guid)) {
            GD.PrintErr($"WARN: Hotbar.AddToHotbar() : Item already exists in hotbar. Cannot add: {guid}");
            return false;
        }
        
        int index = GetNextFreeIndex();
        if (index == -1) {
            GD.PrintErr($"WARN: Hotbar.AddToHotbar() : Hotbar is full. Cannot add: {guid}");
            return false;
        }
        
        _hotbarGuids.Add(index, guid);
        ResyncInventory();
        return true;
    }
    
    public Guid RemoveFromHotbar(int index) {
        if (index < 0 || index >= HOTBAR_SIZE)
            throw new IndexOutOfRangeException($"Hotbar index out of range. Got: {index}, Expected: 0-{HOTBAR_SIZE - 1}");
        
        if (!_hotbarGuids.TryGetValue(index, out Guid guid))
            return Guid.Empty;

        bool remove = _hotbarGuids.Remove(index);
        if (!remove) {
            GD.PrintErr($"WARN: Hotbar.RemoveFromHotbar() : Failed to remove item at index {index}. Cannot remove.");
            return Guid.Empty;
        }

        for (int i = index + 1; i < HOTBAR_SIZE; i++) {
            if (!_hotbarGuids.ContainsKey(i)) continue;
            _hotbarGuids[i - 1] = _hotbarGuids[i];
            _hotbarGuids.Remove(i);
        }
        
        ResyncInventory();
        return guid;
    }
    
    public int RemoveFromHotbar(Guid guid) {
        foreach (KeyValuePair<int,Guid> entry in _hotbarGuids) {
            if (!entry.Value.Equals(guid)) continue;
            RemoveFromHotbar(entry.Key);
            return entry.Key;
        }
        return -1;
    }
    
    public Guid GetHotbarItem(int index = -1) {
        if (index == -1) index = _hotbarIndex;
        
        if (index < 0 || index >= HOTBAR_SIZE) {
            GD.PrintErr($"WARN: Player.GetHotbarItem(): Index out of range. Clamping to 0-{HOTBAR_SIZE - 1}");
            index = Mathsf.Clamp(index, 0, HOTBAR_SIZE - 1);
        }
        
        return _hotbarGuids.GetOrDefault(index, Guid.Empty);
    }

    public void MoveHotbarItem(int index, bool up) {
        if (index < 0 || index >= HOTBAR_SIZE)
            throw new IndexOutOfRangeException($"Hotbar index out of range. Got: {index}, Expected: 0-{HOTBAR_SIZE - 1}");
        
        int target = index + (up ? -1 : 1);
        if (target < 0 || target >= HOTBAR_SIZE || !_hotbarGuids.ContainsKey(target)) return;
        
        Guid first = GetHotbarItem(index);
        Guid second = GetHotbarItem(target);
        
        _hotbarGuids[index] = second;
        _hotbarGuids[target] = first;
    }
    
    private int GetNextFreeIndex() {
        for (int i = 0; i < HOTBAR_SIZE; i++)
            if (!_hotbarGuids.ContainsKey(i)) return i;
        return -1;
    }
    
    public (bool up, bool down) GetHotbarItemMovement(int index) {
        if (GetHotbarSize() <= 1) return (false, false);
        if (index == 0) return (false, true);
        if (index == GetHotbarSize() - 1) return (true, false);
        return (true, true);
    }

    public void ResyncInventory() {
        IObjectBase handItem = _owner.GetHandItem();
        if (handItem == null) return;
        _owner.GetInventory().UpdateItem(handItem.Serialise());
    }

    public SmartDictionary<int, Guid> GetHotbarItems() => _hotbarGuids.Clone();
    public int GetHotbarSize() => _hotbarGuids.Count;
    public bool IsFull() => GetHotbarSize() >= HOTBAR_SIZE;
    public IDictionary<int, Guid> ClearHotbar() => _hotbarGuids.ClearAndReturn();
}