using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class EnvironmentSchedule {
    private static readonly SmartDictionary<int, List<EnvironmentPhase>> _schedule = new();

    static EnvironmentSchedule() {
        SetSchedule(0, GetDefaultPhases());
        SetSchedule(1, GetRandomPhases(Environments.NIGHT_CLEAR));
    }

    private static EnvironmentPhase GetPhaseFromTime(int day, long time) {
        time = (long)Mathf.Wrap(time, 0L, GetDayLength(day));
        List<EnvironmentPhase> dailyPhases = GetSchedulePhases(day);
        if (dailyPhases.Count == 0) throw new InvalidOperationException($"Schedule for day {day} is empty!");

        foreach (EnvironmentPhase phase in dailyPhases) {
            time -= phase.GetTimeLength();
            if (time <= 0) return phase;
        }

        throw new UICException();
    }

    public static EnvironmentType GetEnvironmentFromTime(int day, long time) => GetPhaseFromTime(day, time).GetEnvironment();
    public static long GetDayLength(int day) => GetSchedulePhases(day).Sum(environmentPhase => environmentPhase.GetTimeLength());

    private static long GetAccumulatedTime(int day, EnvironmentPhase environmentPhase) =>
        GetSchedulePhases(day).TakeWhile(phase => !phase.Equals(environmentPhase)).Sum(phase => phase.GetTimeLength());

    public static float GetDistanceThroughCurrentEnvironment(int day, long time) {
        EnvironmentPhase environmentPhase = GetPhaseFromTime(day, time);
        long accumulatedTime = GetAccumulatedTime(day, environmentPhase);
        long nextTime = accumulatedTime + environmentPhase.GetTimeLength();
        return Mathsf.Remap(accumulatedTime, nextTime, time, 0.0f, 1.0f);
    }

    public static EnvironmentType GetNextEnvironment(int day, long time) {
        EnvironmentPhase phaseFromTime = GetPhaseFromTime(day, time);
        List<EnvironmentPhase> environmentPhases = GetSchedulePhases(day);
        if (environmentPhases[^1].Equals(phaseFromTime))
            return GetSchedulePhases(day + 1).First().GetEnvironment();

        int index = environmentPhases.IndexOf(phaseFromTime);
        if (index == -1)
            throw new InvalidOperationException(
                $"Could not find phase for time {time} on day {day}!  Schedule length: {_schedule.Count}  |  That day's schedule: {environmentPhases}");
        return environmentPhases[index + 1].GetEnvironment();
    }

    public static void SetSchedule(int day, List<(long, EnvironmentType)> schedule) {
        List<EnvironmentPhase> daySchedule = GetSchedulePhases(day);
        daySchedule.Clear();
        foreach ((long phaseLength, EnvironmentType environment) in schedule) daySchedule.Add(new EnvironmentPhase(phaseLength, environment));
    }
    
    public static List<(long, EnvironmentType)> GetSchedule(int day) => GetSchedulePhases(day).Select(phase => (phase.GetTimeLength(), phase.GetEnvironment())).ToList();
    private static List<EnvironmentPhase> GetSchedulePhases(int day) => _schedule.GetOrCompute(day, () => new List<EnvironmentPhase>());

    private static List<(long, EnvironmentType)> GetDefaultPhases() {
        return new List<(long, EnvironmentType)> {
            (6L * 60000L, Environments.NIGHT_CLEAR),
            (2L * 60000L, Environments.NIGHT_CLEAR),
            (2L * 60000L, Environments.MORNING_CLEAR),
            (12L * 60000L, Environments.DAY_CLEAR),
            (2L * 60000L, Environments.DAY_CLEAR),
            (2L * 60000L, Environments.EVENING_CLEAR),
            (6L * 60000L, Environments.NIGHT_CLEAR)
        };
    }

    public static List<(long, EnvironmentType)> GetRandomPhases(EnvironmentType nightLeadIn = null) {
        EnvironmentType nightStart = Randf.Random(Environments.GetNightTypes());
        EnvironmentType morning = Randf.Random(Environments.GetMorningTypes());
        EnvironmentType day = Randf.Random(Environments.GetDayTypes());
        EnvironmentType evening = Randf.Random(Environments.GetEveningTypes());
        EnvironmentType nightEnd = Randf.Random(Environments.GetNightTypes());

        List<(long, EnvironmentType)> randomPhases = new() {
            (6L * 60000L, nightLeadIn ?? nightStart),
            (2L * 60000L, nightLeadIn ?? nightStart),
            (2L * 60000L, morning),
            (12L * 60000L, day),
            (2L * 60000L, day),
            (2L * 60000L, evening),
            (6L * 60000L, nightEnd)
        };

        return randomPhases;
    }
    
    public static string SchedToString(List<(long, EnvironmentType)> sched) {
        string result = "";
        foreach ((long phaseLength, EnvironmentType environment) in sched) result += $"[{environment.GetName()}, {phaseLength}]  ";
        return result;
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
        public override string ToString() => $"[{_type.GetName()}, {_phaseLength}]";
    }
}