using System;
using Godot;

public static class EnvironmentManager {
    
    private static WorldEnvironment _worldEnvironment;
    private static EnvironmentType _lastEnvironment = Environments.MORNING_CLEAR, _lastNext;

    private static readonly EaseType _ease = Easing.IN;
    
    private const long SUN_FADE_TIME = 5000L;
    private const double DAY_START = 0.34D, SUN_RISE = 0.25D, SUN_SET = 0.75D;

    private static long _dayTime = Mathsf.Lerp(0L, EnvironmentSchedule.GetDayLength(0), DAY_START);
    private static bool _forcePause;
    private static float _daySpeed = 1.0f;
    private static int _dayIndex;
    private static SchedulerTask _midnightEvent;
    
    private static Node3D _sunContainer, _sunObj;
    private static DirectionalLight3D _sunLight;
    private static EnvListeners _envListeners;
    
    public static void Init(WorldEnvironment worldEnvironment, Node3D sunContainer, Node3D sunObj, DirectionalLight3D sunLight) {
        if (_envListeners == null && GameManager.IsDebugMode()) EventManager.RegisterListeners(_envListeners = new EnvListeners());
        _worldEnvironment = worldEnvironment;
        _sunContainer = sunContainer;
        _sunObj = sunObj;
        _sunLight = sunLight;
    }
    
    public static void Process(double delta) {
        long dayLength = EnvironmentSchedule.GetDayLength(GetDay());
        Player player = GameManager.I().GetPlayer();

        if (!_forcePause) _dayTime += (long)(delta * 1000L * _daySpeed);
        if (_dayTime >= dayLength) {
            if (GameManager.IsDebugMode()) Toast.Error(player, "Day cycle reset!");
            _dayTime = 0L;
            _dayIndex++;
            if (Randf.RandomChanceIn(1, 10)) {
                _daySpeed = 0.0f;
                _midnightEvent = Scheduler.ScheduleOnce(30000L, _ => _daySpeed = 1.0f);
            }
            EnvironmentSchedule.SetSchedule(GetDay() + 1, EnvironmentSchedule.GetRandomPhases(EnvironmentSchedule.GetSchedule(_dayIndex)[^1].Item2));
            dayLength = EnvironmentSchedule.GetDayLength(GetDay());
        }
        
        long sunRiseStart = Mathsf.Lerp(0L, dayLength, SUN_RISE) - SUN_FADE_TIME;
        long sunRiseEnd = Mathsf.Lerp(0L, dayLength, SUN_RISE) + SUN_FADE_TIME;
        long sunSetStart = Mathsf.Lerp(0L, dayLength, SUN_SET) - SUN_FADE_TIME;
        long sunSetEnd = Mathsf.Lerp(0L, dayLength, SUN_SET) + SUN_FADE_TIME;

        EnvironmentType currentEnvironment = EnvironmentSchedule.GetEnvironmentFromTime(GetDay(), _dayTime);
        EnvironmentType nextEnvironment = EnvironmentSchedule.GetNextEnvironment(GetDay(), _dayTime);
        float ratio = EnvironmentSchedule.GetDistanceThroughCurrentEnvironment(GetDay(), _dayTime);
        EnvironmentType blend = currentEnvironment.Equals(nextEnvironment) ? currentEnvironment : currentEnvironment.BlendWith(nextEnvironment, ratio, _ease);

        if (GameManager.IsDebugMode() && (!currentEnvironment.Equals(_lastEnvironment) || !nextEnvironment.Equals(_lastNext)))
            Toast.Info(player, $"Time: {currentEnvironment.GetName()} : Blending with {nextEnvironment.GetName()}");
        _lastEnvironment = currentEnvironment;
        _lastNext = nextEnvironment;
        
        if (_worldEnvironment != null) blend.Apply(_worldEnvironment);

        float sunRotationDegs = Mathsf.Remap(0L, dayLength, _dayTime, -180.0f, 180.0f);
        Vector3 sunRot = new(sunRotationDegs, 180.0f, 0.0f);
        _sunContainer?.SetRotationDegrees(sunRot);
        _sunObj?.SetRotationDegrees(sunRot);
        _sunObj?.SetPosition(player.GetPosition());
        
        if (_sunLight == null) return;
        
        if (_dayTime >= sunRiseStart && _dayTime < sunRiseEnd) _sunLight.LightEnergy = Mathsf.Remap(sunRiseStart, sunRiseEnd, _dayTime, 0.0f, 1.0f);
        else if (_dayTime >= sunRiseEnd && _dayTime < sunSetStart) _sunLight.LightEnergy = 1.0f;
        else if (_dayTime >= sunSetStart && _dayTime < sunSetEnd) _sunLight.LightEnergy = Mathsf.Remap(sunSetStart, sunSetEnd, _dayTime, 1.0f, 0.0f);
        else _sunLight.LightEnergy = 0.0f;
    }

    private static void AddTime(long time) {
        if (GameManager.IsDebugMode()) Toast.Info(GameManager.I().GetPlayer(), $"{(time >= 0 ? "Adding" : "Subtracting")} {Mathsf.Round(time / 1000L, 2)} seconds");
        _dayTime = (long)Mathf.Wrap(_dayTime + time, 0L, EnvironmentSchedule.GetDayLength(GetDay()));
    }
    
    public static int GetDay() => _dayIndex;
    public static (int hours, int minutes) GetTimeAs24H() {
        double normalized = (double)_dayTime / EnvironmentSchedule.GetDayLength(GetDay());
        normalized = Math.Clamp(normalized, 0.0, 1.0); // Just in case

        double totalMinutes = normalized * 24 * 60;
        int hour = (int)(totalMinutes / 60);
        int minute = (int)(totalMinutes % 60);

        return (hour, minute);
    }

    private class EnvListeners {
        private const long TIME_ADD = 15000L;
        
        [EventListener]
        public void OnKeyPressed(KeyPressEvent ev, Key key) {
            if (ev.IsCaptured()) return;
            switch (key) {
                case Key.R:
                    AddTime(TIME_ADD);
                    break;
                case Key.T:
                    AddTime(-TIME_ADD);
                    break;
                case Key.Y:
                    _forcePause = !_forcePause;
                    _midnightEvent?.Cancel();
                    if (GameManager.IsDebugMode()) Toast.Info(GameManager.I().GetPlayer(), $"Time {( _forcePause ? "paused" : "resumed")}");
                    break;
                case Key.G:
                    _daySpeed += 1.0f;
                    _midnightEvent?.Cancel();
                    if (GameManager.IsDebugMode()) Toast.Info(GameManager.I().GetPlayer(), $"Day Speed: {_daySpeed}");
                    break;
                case Key.H:
                    _daySpeed -= 1.0f;
                    _midnightEvent?.Cancel();
                    if (GameManager.IsDebugMode()) Toast.Info(GameManager.I().GetPlayer(), $"Day Speed: {_daySpeed}");
                    break;
                case Key.J:
                    _daySpeed = 1.0f;
                    _midnightEvent?.Cancel();
                    if (GameManager.IsDebugMode()) Toast.Info(GameManager.I().GetPlayer(), $"Day Speed: {_daySpeed}");
                    break;
            }
        }
    }
}