using System;
using Godot;

public static class EnvironmentManager {
    private static WorldEnvironment _worldEnvironment, _backdropEnvironment;
    private static EnvironmentType _lastEnvironment = Environments.MORNING_CLEAR, _lastNext;

    private static readonly EaseType _ease = Easing.IN;
    private static readonly long _standardDayLength = EnvironmentSchedule.GetDayLength(0);

    private const long SUN_FADE_TIME = 5000L;
    private const double DAY_START = 0.34D, LUNAR_CYCLE = 29.53D;
    private const double SUN_RISE = 0.25D, SUN_SET = 0.75D;
    private const double STAR_APPEARANCE_START = SUN_SET - 0.01D, STAR_APPEARANCE_END = SUN_SET + 0.05D;
    private const double STAR_FADE_START = SUN_RISE - 0.05D, STAR_FADE_END = SUN_RISE + 0.01D;

    private const string
        WORLD_ENVIRONMENT_PATH = "Main/WorldEnvironment",
        SUN_CONTAINER_PATH = "Main/SunContainer",
        SUN_LIGHT_PATH = "Main/SunContainer/Sun",
        STARFIELD_PATH = "Main/Starfield";

    private static long _dayTime = Mathsf.Lerp(0L, EnvironmentSchedule.GetDayLength(0), DAY_START);
    private static bool _forcePause;
    private static float _daySpeed = 1.0f;
    private static int _dayIndex;

    private static SchedulerTask _midnightEvent;
    private static Node3D _worldSunContainer, _backdropSunContainer, _backdropSunObj, _backdropMoonObj;
    private static DirectionalLight3D _worldSunLight, _backdropSunLight;
    private static MeshInstance3D _starfield;
    private static EnvListeners _envListeners;

    public static void Init() {
        if (_envListeners == null && GameManager.IsDebugMode()) EventManager.RegisterListeners(_envListeners = new EnvListeners());

        _worldEnvironment = MainLauncher.FindNode<WorldEnvironment>(WORLD_ENVIRONMENT_PATH);
        _backdropEnvironment = MainLauncher.FindNode<WorldEnvironment>(WORLD_ENVIRONMENT_PATH, true);

        _worldSunContainer = MainLauncher.FindNode<Node3D>(SUN_CONTAINER_PATH);
        _backdropSunContainer = MainLauncher.FindNode<Node3D>(SUN_CONTAINER_PATH, true);

        _worldSunLight = MainLauncher.FindNode<DirectionalLight3D>(SUN_LIGHT_PATH);
        _backdropSunLight = MainLauncher.FindNode<DirectionalLight3D>(SUN_LIGHT_PATH, true);

        _starfield = MainLauncher.FindNode<MeshInstance3D>(STARFIELD_PATH, true);

        Node3D sunObj = Loader.SafeInstantiate<Node3D>("res://Main/Prefabs/Sandbox/Sun.tscn");
        _backdropSunObj = sunObj;

        Node3D moonObj = Loader.SafeInstantiate<Node3D>("res://Main/Prefabs/Sandbox/Moon.tscn");
        _backdropMoonObj = moonObj;

        MainLauncher.GetBackdropBootstrapper().AddChild(sunObj);
        MainLauncher.GetBackdropBootstrapper().AddChild(moonObj);
    }

    public static void Process(double delta) {
        long dayLength = EnvironmentSchedule.GetDayLength(GetDay());
        Player player = GameManager.GetPlayer();

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
        if (_backdropEnvironment != null) blend.Apply(_backdropEnvironment, 0.25f);

        float sunRotationDegs = Mathsf.Remap(0L, dayLength, _dayTime, -180.0f, 180.0f);
        Vector3 sunRot = new(sunRotationDegs, 180.0f, 0.0f);

        _worldSunContainer?.SetRotationDegrees(sunRot);
        _backdropSunContainer?.SetRotationDegrees(sunRot);
        _backdropSunObj?.SetRotationDegrees(sunRot);

        float moonRotationDegs = (float)(GetAccumulatedTime() / (_standardDayLength * LUNAR_CYCLE) % 1.0f * 360.0f) + sunRotationDegs + 120.0f;
        Vector3 moonRot = new(moonRotationDegs, 180.0f, 0.0f);
        _backdropMoonObj?.SetRotationDegrees(moonRot);

        if (_starfield != null) {
            long starAppearStart = Mathsf.Lerp(0L, dayLength, STAR_APPEARANCE_START);
            long starAppearEnd = Mathsf.Lerp(0L, dayLength, STAR_APPEARANCE_END);
            long starFadeStart = Mathsf.Lerp(0L, dayLength, STAR_FADE_START);
            long starFadeEnd = Mathsf.Lerp(0L, dayLength, STAR_FADE_END);

            float starfieldAlpha;
            if (_dayTime >= starAppearStart && _dayTime < starAppearEnd)
                starfieldAlpha = Mathsf.Remap(starAppearStart, starAppearEnd, _dayTime, 0.0f, 1.0f);
            else if ((_dayTime >= starAppearEnd && _dayTime < dayLength) || (_dayTime >= 0.0f && _dayTime < starFadeStart))
                starfieldAlpha = 1.0f;
            else if (_dayTime >= starFadeStart && _dayTime < starFadeEnd)
                starfieldAlpha = Mathsf.Remap(starFadeStart, starFadeEnd, _dayTime, 1.0f, 0.0f);
            else
                starfieldAlpha = 0.0f;

            _starfield.SetRotationDegrees(new Vector3(sunRotationDegs + 190.0f, 180.0f, 90.0f));
            StandardMaterial3D mat = _starfield.GetActiveMaterial(0) as StandardMaterial3D;
            Color albedo = mat.GetAlbedo();
            albedo.A = starfieldAlpha;
            mat.SetAlbedo(albedo);
        }

        float sunLightEnergy;

        if (_dayTime >= sunRiseStart && _dayTime < sunRiseEnd) sunLightEnergy = Mathsf.Remap(sunRiseStart, sunRiseEnd, _dayTime, 0.0f, 1.0f);
        else if (_dayTime >= sunRiseEnd && _dayTime < sunSetStart) sunLightEnergy = 1.0f;
        else if (_dayTime >= sunSetStart && _dayTime < sunSetEnd)  sunLightEnergy = Mathsf.Remap(sunSetStart, sunSetEnd, _dayTime, 1.0f, 0.0f);
        else sunLightEnergy = 0.0f;

        if (_worldSunLight != null) _worldSunLight.LightEnergy = sunLightEnergy;
    }

    private static void AddTime(long time) {
        if (GameManager.IsDebugMode()) Toast.Info(GameManager.GetPlayer(), $"{(time >= 0 ? "Adding" : "Subtracting")} {Mathsf.Round(time / 1000L, 2)} seconds");
        _dayTime = (long)Mathf.Wrap(_dayTime + time, 0L, EnvironmentSchedule.GetDayLength(GetDay()));
    }

    public static int GetDay() => _dayIndex;

    public static long GetAccumulatedTime() {
        long time = 0L;
        for (int i = 0; i < GetDay(); i++) time += EnvironmentSchedule.GetDayLength(i);
        return time + _dayTime;
    }

    public static (int hours, int minutes) GetTimeAs24H() {
        double normalized = (double)_dayTime / EnvironmentSchedule.GetDayLength(GetDay());
        normalized = Math.Clamp(normalized, 0.0, 1.0);

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
            Player player = GameManager.GetPlayer();
            switch (key) {
                case Key.R: AddTime(TIME_ADD); break;
                case Key.T: AddTime(-TIME_ADD); break;
                case Key.Y:
                    _forcePause = !_forcePause;
                    _midnightEvent?.Cancel();
                    if (GameManager.IsDebugMode()) Toast.Info(player, $"Time {(_forcePause ? "paused" : "resumed")}");
                    break;
                case Key.G:
                    _daySpeed += 1.0f;
                    _midnightEvent?.Cancel();
                    if (GameManager.IsDebugMode()) Toast.Info(player, $"Day Speed: {_daySpeed}");
                    break;
                case Key.H:
                    _daySpeed -= 1.0f;
                    _midnightEvent?.Cancel();
                    if (GameManager.IsDebugMode()) Toast.Info(player, $"Day Speed: {_daySpeed}");
                    break;
                case Key.J:
                    _daySpeed = 1.0f;
                    _midnightEvent?.Cancel();
                    if (GameManager.IsDebugMode()) Toast.Info(player, $"Day Speed: {_daySpeed}");
                    break;
            }
        }
    }
}