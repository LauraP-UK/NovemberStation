using System;
using System.Collections.Generic;
using Godot;

public static class Scheduler {
    private static readonly SmartSet<SchedulerTask> _tasks = new();
    private static ulong _lastUpdateTime = Time.GetTicksMsec();

    public static void Process() {
        ulong currentTime = Time.GetTicksMsec();
        ulong deltaMillis = currentTime - _lastUpdateTime;
        _lastUpdateTime = currentTime;

        List<SchedulerTask> toRemove = new();
        SmartSet<SchedulerTask> clonedTasks = _tasks.Clone();
        foreach (SchedulerTask schedulerTask in clonedTasks) {
            schedulerTask.Update((long) deltaMillis);
            if (schedulerTask.IsComplete()) toRemove.Add(schedulerTask);
        }
        clonedTasks.Clear();
        
        _tasks.RemoveAll(toRemove);
    }

    public static SchedulerTask ScheduleOnce(long delayMillis, Action<SchedulerTask> action) {
        SchedulerTask task = new(delayMillis, action, 0L, 1);
        _tasks.Add(task);
        return task;
    }

    public static SchedulerTask ScheduleRepeating(long delayMillis, long repeatIntervalMillis, Action<SchedulerTask> action, int repeatLimit = -1) {
        SchedulerTask task = new(delayMillis, action, repeatIntervalMillis, repeatLimit);
        _tasks.Add(task);
        return task;
    }

    public static void CancelAll() {
        foreach (SchedulerTask schedulerTask in _tasks) schedulerTask.Cancel();
        _tasks.Clear();
    }
}