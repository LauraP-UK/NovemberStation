using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Timer = System.Timers.Timer;

public class Scheduler {
    private static readonly HashSet<SchedulerTask> _tasks = new();

    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private static long _lastUpdateTime;

    static Scheduler() {
        Timer timer = new(1);
        timer.Elapsed += (_, _) => Update();
        timer.AutoReset = true;
        timer.Start();
    }

    private static void Update() {
        long currentTime = _stopwatch.ElapsedMilliseconds;
        long deltaMillis = currentTime - _lastUpdateTime;
        _lastUpdateTime = currentTime;
        
        List<SchedulerTask> toRemove = new();
        foreach (SchedulerTask schedulerTask in _tasks) {
            schedulerTask.Update(deltaMillis);

            if (schedulerTask.IsComplete()) toRemove.Add(schedulerTask);
        }
        foreach (SchedulerTask schedulerTask in toRemove) _tasks.Remove(schedulerTask);
    }

    public static SchedulerTask ScheduleOnce(long delayMillis, Action<SchedulerTask> action) {
        SchedulerTask task = new(delayMillis, action);
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