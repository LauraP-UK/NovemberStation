using System;
using System.Collections.Generic;
using Godot;

public class Hotbar {
    public const int HOTBAR_SIZE = 9;
    public const ulong DISPLAY_TIME = 2500, FADE_TIME = 500;

    private readonly SmartDictionary<int, Guid> _hotbarGuids = new(), _priorGUIDs = new();
    private readonly IHotbarActor _owner;
    private int _hotbarIndex;
    private bool _changed;
    private ulong _lastPoppedTime;

    public Hotbar(IHotbarActor owner) => _owner = owner;

    public void ChangeIndex(int change) {
        int oldIndex = _hotbarIndex;
        _hotbarIndex += change;
        _hotbarIndex = Mathf.Wrap(_hotbarIndex, 0, HOTBAR_SIZE);
        _changed = oldIndex != _hotbarIndex;
    }

    public int GetIndex() => _hotbarIndex;

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
        if (index < 0 || index >= HOTBAR_SIZE) throw new IndexOutOfRangeException($"Hotbar index out of range. Got: {index}, Expected: 0-{HOTBAR_SIZE - 1}");

        if (!_hotbarGuids.TryGetValue(index, out Guid guid)) return Guid.Empty;

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
        foreach (KeyValuePair<int, Guid> entry in _hotbarGuids) {
            if (!entry.Value.Equals(guid)) continue;
            RemoveFromHotbar(entry.Key);
            UpdateOwnerHeldItem();
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
        if (index < 0 || index >= HOTBAR_SIZE) throw new IndexOutOfRangeException($"Hotbar index out of range. Got: {index}, Expected: 0-{HOTBAR_SIZE - 1}");

        int target = index + (up ? -1 : 1);
        if (target < 0 || target >= HOTBAR_SIZE || !_hotbarGuids.ContainsKey(target)) return;

        Guid first = GetHotbarItem(index);
        Guid second = GetHotbarItem(target);

        _hotbarGuids[index] = second;
        _hotbarGuids[target] = first;
    }

    private int GetNextFreeIndex() {
        for (int i = 0; i < HOTBAR_SIZE; i++)
            if (!_hotbarGuids.ContainsKey(i))
                return i;
        return -1;
    }

    public (bool up, bool down) GetHotbarItemMovement(int index) {
        int count = _hotbarGuids.Count;
        if (count <= 1) return (false, false);
        if (index == 0) return (false, true);
        if (index == count - 1) return (true, false);
        return (true, true);
    }

    public void ResyncInventory() {
        SmartDictionary<int, Guid> guids = GetHotbarItems();
        IInventory inventory = _owner.GetInventory();

        if (_owner is Player player) {
            if (!_changed && !ArrayUtils.ExactMatchSorted(
                    (a, b) => a.Key.CompareTo(b.Key),
                    _priorGUIDs,
                    guids
                ))
                _changed = true;

            ulong currentTime = Time.GetTicksMsec();

            if (_changed) {
                _priorGUIDs.ClearAndReturn();
                foreach (KeyValuePair<int, Guid> entry in guids) _priorGUIDs.Add(entry.Key, entry.Value);
                _changed = false;
                _lastPoppedTime = currentTime;
            }

            float alpha =
                currentTime - _lastPoppedTime < DISPLAY_TIME
                    ? 1.0f
                    : Math.Clamp(Mathsf.Remap(_lastPoppedTime + DISPLAY_TIME, _lastPoppedTime + DISPLAY_TIME + FADE_TIME, currentTime, 1.0f, 0.0f), 0.0f, 1.0f);

            HotbarMenu hotbarMenu = player.GetController<PlayerController>().GetHotbarMenu();
            Control menuNode = hotbarMenu.GetForm().GetMenu();
            Color modulate = menuNode.GetModulate();
            modulate.A = alpha;
            menuNode.SetModulate(modulate);
        }

        IObjectBase handItem = _owner.GetHandItem();
        if (handItem == null) return;
        inventory.UpdateItem(handItem.Serialise());
    }

    public SmartDictionary<int, Guid> GetHotbarItems() => _hotbarGuids;
    public int GetHotbarSize() => _hotbarGuids.Count;
    public bool IsFull() => GetHotbarSize() >= HOTBAR_SIZE;
    public IDictionary<int, Guid> ClearHotbar() => _hotbarGuids.ClearAndReturn();
    public bool Contains(Guid guid) => _hotbarGuids.Values.Contains(guid);
}