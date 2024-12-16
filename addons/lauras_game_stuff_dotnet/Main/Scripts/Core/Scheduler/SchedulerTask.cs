using System;

public class SchedulerTask {
    private const long MINIMUM_TIME = 20L;

    private readonly Action<SchedulerTask> _action;
    private readonly long _repeatInterval;
    private readonly int _repeatLimit;
    
    private long _delayRemaining;
    private int _repeatCount;
    private bool _isCancelled;

    public SchedulerTask(long delayMillis, Action<SchedulerTask> action, long repeatIntervalMillis = -1, int repeatLimit = -1) {
        _action = action;
        _delayRemaining = delayMillis <= MINIMUM_TIME ? MINIMUM_TIME : delayMillis;
        _repeatInterval = repeatIntervalMillis <= MINIMUM_TIME ? MINIMUM_TIME : repeatIntervalMillis;
        _repeatLimit = repeatLimit;
        _repeatCount = 0;
        _isCancelled = false;
    }
    
    public bool IsComplete() {
        if (_isCancelled) return true;
        return _repeatLimit > 0 && _repeatCount >= _repeatLimit;
    }

    public void Update(long deltaMillis) {
        if (_isCancelled || IsComplete()) return;

        _delayRemaining -= deltaMillis;

        if (!(_delayRemaining <= 0)) return;
        _action.Invoke(this);

        if (_repeatInterval > 0 && (_repeatLimit == -1 || _repeatCount < _repeatLimit)) {
            _delayRemaining = _repeatInterval;
            _repeatCount++;
        } else {
            _isCancelled = true;
        }
    }

    public void Cancel() {
        _isCancelled = true;
    }

    public int GetRepeatCount() {
        return _repeatCount;
    }

    public int GetRepeatLimit() {
        return _repeatLimit;
    }

    public bool IsCancelled() {
        return _isCancelled;
    }
}