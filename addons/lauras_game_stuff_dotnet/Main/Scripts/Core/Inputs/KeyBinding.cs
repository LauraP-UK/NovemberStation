using System.Collections.Generic;
using Godot;

public class KeyBinding {
    private readonly AutoDictionary<Key, GameAction.Action> _keyToAction = new();

    public void BindKey(Key key, GameAction.Action action) {
        _keyToAction.Add(key, action);
    }

    public GameAction.Action? GetAction(Key key) {
        return _keyToAction.TryGetValue(key, out GameAction.Action action) ? action : null;
    }

    public IEnumerable<Key> GetKeysForAction(GameAction.Action action) {
        foreach (KeyValuePair<Key, GameAction.Action> pair in _keyToAction)
            if (pair.Value == action) yield return pair.Key;
    }
}