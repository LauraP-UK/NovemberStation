
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class EnvironmentSchedule {
    
    private static readonly List<EnvironmentPhase> _schedule = new(), _nextSchedule = new();

    static EnvironmentSchedule() {
        SetSchedule(GetDefaultPhases());
        SetNextSchedule(GetRandomSchedule());
    }

    private static EnvironmentPhase GetPhaseFromTime(long time) {
        time = (long) Mathf.Wrap(time, 0L, GetDayLength());
        foreach (EnvironmentPhase phase in _schedule) {
            time -= phase.GetTimeLength();
            if (time <= 0) return phase;
        }
        throw new UICException();
    }

    public static EnvironmentType GetEnvironmentFromTime(long time) => GetPhaseFromTime(time).GetEnvironment();
    public static long GetDayLength() => _schedule.Sum(environmentPhase => environmentPhase.GetTimeLength());
    private static long GetAccumulatedTime(EnvironmentPhase environmentPhase) => _schedule.TakeWhile(phase => !phase.Equals(environmentPhase)).Sum(phase => phase.GetTimeLength());
    public static float GetDistanceThroughCurrentEnvironment(long time) {
        EnvironmentPhase environmentPhase = GetPhaseFromTime(time);
        long accumulatedTime = GetAccumulatedTime(environmentPhase);
        long nextTime = accumulatedTime + environmentPhase.GetTimeLength();
        return Mathsf.Remap(accumulatedTime, nextTime, time, 0.0f, 1.0f);
    }
    public static EnvironmentType GetNextEnvironment(long time) {
        int index = _schedule.IndexOf(GetPhaseFromTime(time));
        if (index == _schedule.Count - 1) return _nextSchedule[0].GetEnvironment();
        return _schedule[index + 1].GetEnvironment();
    }
    
    public static void ProgressSchedule() {
        _schedule.Clear();
        foreach (EnvironmentPhase phase in _nextSchedule) _schedule.Add(phase);
        _nextSchedule.Clear();
        SetNextSchedule(GetRandomSchedule());
    }
    
    public static void SetSchedule(List<(long, EnvironmentType)> schedule) {
        _schedule.Clear();
        foreach ((long phaseLength, EnvironmentType environment) in schedule) _schedule.Add(new EnvironmentPhase(phaseLength, environment));
    }

    public static void SetNextSchedule(List<(long, EnvironmentType)> schedule) {
        _nextSchedule.Clear();
        foreach ((long phaseLength, EnvironmentType environment) in schedule) _nextSchedule.Add(new EnvironmentPhase(phaseLength, environment));
    }

    private static List<(long, EnvironmentType)> GetDefaultPhases() {
        return new List<(long, EnvironmentType)> {
            (2L * 60000L, Environments.MORNING_CLEAR), // Morning fade to Day
            (12L * 60000L, Environments.DAY_CLEAR), // Day remains Day
            (2L * 60000L, Environments.DAY_CLEAR), // Day fade to Evening
            (2L * 60000L, Environments.EVENING_CLEAR), // Evening fade to Night
            (12L * 60000L, Environments.NIGHT_CLEAR), // Night remains Night
            (2L * 60000L, Environments.NIGHT_CLEAR) // Night fade to Morning
        };
    }

    private static List<(long, EnvironmentType)> GetRandomSchedule() {
        EnvironmentType morning = Randf.Random(Environments.GetMorningTypes());
        EnvironmentType day = Randf.Random(Environments.GetDayTypes());
        EnvironmentType evening = Randf.Random(Environments.GetEveningTypes());
        EnvironmentType night = Randf.Random(Environments.GetNightTypes());
        
        GD.Print($"Day {EnvironmentManager.GetDay() + 1}'s schedule: {morning.GetName()} -> {day.GetName()} -> {evening.GetName()} -> {night.GetName()}");
        
        return new List<(long, EnvironmentType)> {
            (2L * 60000L, morning),
            (12L * 60000L, day),
            (2L * 60000L, day),
            (2L * 60000L, evening),
            (12L * 60000L, night),
            (2L * 60000L, night)
        };
    }

    private class EnvironmentPhase {
        private readonly long _phaseLength;
        private readonly EnvironmentType _type;
        public EnvironmentPhase(long phaseLength, EnvironmentType type) {
            _phaseLength = phaseLength;
            _type = type;
        }
        public long GetTimeLength() => _phaseLength;
        public EnvironmentType GetEnvironment() => _type;
    }
}