
using Godot;

public static class EnvironmentManager {
    
    private static WorldEnvironment _worldEnvironment;
    private static EnvironmentType _lastEnvironment = Environments.MORNING_CLEAR, _lastNext;
    private const long SUN_FADE_TIME = 10000L / 2L;
	
    private static long _dayTime;
    private static bool _forcePause;
    private static float _daySpeed = 1.0f;
    private static int _dayIndex;
    
    private static Node3D _sunContainer, _sunObj;
    private static DirectionalLight3D _sunLight;
    private static EnvListeners _envListeners;
    
    public static void Init(WorldEnvironment worldEnvironment, Node3D sunContainer, Node3D sunObj, DirectionalLight3D sunLight) {
        if (_envListeners == null) EventManager.RegisterListeners(_envListeners = new EnvListeners());
        _worldEnvironment = worldEnvironment;
        _sunContainer = sunContainer;
        _sunObj = sunObj;
        _sunLight = sunLight;
    }
    
    public static void Process(double delta) {
        long dayLength = EnvironmentSchedule.GetDayLength();
        Player player = GameManager.I().GetPlayer();

        if (!_forcePause) _dayTime += (long)(delta * 1000L * _daySpeed);
        if (_dayTime >= dayLength) {
            Toast.Error(player, "Day cycle reset!");
            _dayTime = 0L;
            _dayIndex++;
            EnvironmentSchedule.ProgressSchedule();
        }

        EnvironmentType currentEnvironment = EnvironmentSchedule.GetEnvironmentFromTime(_dayTime);
        EnvironmentType nextEnvironment = EnvironmentSchedule.GetNextEnvironment(_dayTime);
        float ratio = EnvironmentSchedule.GetDistanceThroughCurrentEnvironment(_dayTime);
        GD.Print($"Ratio: {ratio}");
        EnvironmentType blend = currentEnvironment.BlendWith(nextEnvironment, ratio);

        if (!_lastEnvironment.Equals(currentEnvironment) || !nextEnvironment.Equals(_lastNext)) {
            Toast.Info(player, $"Time: {currentEnvironment.GetName()} : Blending with {nextEnvironment.GetName()}");
            GD.Print($"Day {GetDay()} : {currentEnvironment.GetName()}");
        }
        _lastEnvironment = currentEnvironment;
        _lastNext = nextEnvironment;
        
        if (_worldEnvironment != null) blend.Apply(_worldEnvironment);

        float sunRotationDegs = Mathsf.Remap(0L, dayLength, _dayTime, 360.0f, 0.0f) + 90.0f; // 90 degrees offset because we start at morning
        Vector3 sunRot = new(Mathf.DegToRad(sunRotationDegs), 0.0f, 0.0f);
        _sunContainer?.SetRotation(sunRot);
        _sunObj?.SetRotation(sunRot);
        _sunObj?.SetPosition(player.GetPosition());
        
        if (_sunLight == null) return;
        if (_dayTime >= dayLength / 2L - SUN_FADE_TIME && _dayTime <= dayLength / 2L + SUN_FADE_TIME) {
            _sunLight.LightEnergy = Mathsf.Remap(dayLength / 2L - SUN_FADE_TIME, dayLength / 2L + SUN_FADE_TIME, _dayTime, 1.0f, 0.0f);
        } else if (_dayTime >= dayLength / 2L + SUN_FADE_TIME && _dayTime <= dayLength - SUN_FADE_TIME) {
            _sunLight.LightEnergy = 0.0f;
        } else if (_dayTime >= dayLength - SUN_FADE_TIME || _dayTime <= SUN_FADE_TIME) {
            
            float startTime = dayLength - SUN_FADE_TIME;
            float endTime = SUN_FADE_TIME + dayLength;
            float adjustedDayTime = _dayTime < SUN_FADE_TIME ? _dayTime + dayLength : _dayTime;

            _sunLight.LightEnergy = Mathsf.Remap(startTime, endTime, adjustedDayTime, 0.0f, 1.0f);
        } else {
            _sunLight.LightEnergy = 1.0f;
        }
    }

    private static void AddTime(long time) {
        Toast.Info(GameManager.I().GetPlayer(), $"{(time >= 0 ? "Adding" : "Subtracting")} {Mathsf.Round(time / 1000L, 2)} seconds");
        _dayTime = (long)Mathf.Wrap(_dayTime + time, 0L, EnvironmentSchedule.GetDayLength());
    }
    
    public static int GetDay() => _dayIndex;

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
                    Toast.Info(GameManager.I().GetPlayer(), $"Time {( _forcePause ? "paused" : "resumed")}");
                    break;
                case Key.G:
                    _daySpeed += 1.0f;
                    Toast.Info(GameManager.I().GetPlayer(), $"Day Speed: {_daySpeed:0.0}");
                    break;
                case Key.H:
                    _daySpeed -= 1.0f;
                    Toast.Info(GameManager.I().GetPlayer(), $"Day Speed: {_daySpeed:0.0}");
                    break;
                case Key.J:
                    _daySpeed = 1.0f;
                    Toast.Info(GameManager.I().GetPlayer(), $"Day Speed: {_daySpeed:0.0}");
                    break;
            }
        }
    }
}