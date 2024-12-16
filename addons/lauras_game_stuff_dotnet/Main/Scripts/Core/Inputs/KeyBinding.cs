using System.Collections.Generic;

public class KeyBinding {
    private readonly AutoDictionary<string, GameAction> _keyToAction = new();

    public void BindKey(string key, GameAction action) {
        _keyToAction.Add(key, action);
    }

    public GameAction? GetAction(string key) {
        return _keyToAction.TryGetValue(key, out GameAction action) ? action : null;
    }

    public IEnumerable<string> GetKeysForAction(GameAction action) {
        foreach (KeyValuePair<string,GameAction> pair in _keyToAction)
            if (pair.Value == action) yield return pair.Key;
    }
}