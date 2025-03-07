
using Godot;

public static class EnvironmentManager {
    
    private static WorldEnvironment _worldEnvironment;
    private static EnvironmentType _currentEnvironment = Environments.MORNING;
    private static readonly long DAY_LENGTH = 120000L * Environments.GetCount();
    private const long SUN_FADE_TIME = 10000L / 2L;
	
    private static long _dayTime;
    private static bool _forcePause;
    private static float _daySpeed = 1.0f;
    
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
        if (!_forcePause) _dayTime += (long)(delta * 1000L * _daySpeed);
        if (_dayTime >= DAY_LENGTH) _dayTime = 0L;

        Player player = GameManager.I().GetPlayer();

        long phaseLength = DAY_LENGTH / Environments.GetCount();
        int phaseIndex = (int)Mathsf.Remap(0L, DAY_LENGTH, _dayTime, 0, Environments.GetCount());
        float ratio = _dayTime % phaseLength / (float)phaseLength;

        float sunRotationDegs = Mathsf.Remap(0L, DAY_LENGTH, _dayTime, 360.0f, 0.0f) + 90.0f; // 90 degrees offset because we start at morning
        Vector3 sunRot = new(Mathf.DegToRad(sunRotationDegs), 0.0f, 0.0f);
        _sunContainer?.SetRotation(sunRot);
        _sunObj?.SetRotation(sunRot);
        _sunObj?.SetPosition(player.GetPosition());

        EnvironmentType currentEnvironment = _currentEnvironment;
        _currentEnvironment = Environments.GetAll()[phaseIndex];
			
        if (!currentEnvironment.Equals(_currentEnvironment)) Toast.Info(player, $"Time: {_currentEnvironment.GetName()}");
        EnvironmentType next = currentEnvironment.GetNext();
        EnvironmentType blend = _currentEnvironment.BlendWith(next, ratio);
        if (_worldEnvironment != null) blend.Apply(_worldEnvironment);

        if (_sunLight == null) return;
        if (_dayTime >= (DAY_LENGTH / 2L) - SUN_FADE_TIME && _dayTime <= (DAY_LENGTH / 2L) + SUN_FADE_TIME) {
            _sunLight.LightEnergy = Mathsf.Remap(DAY_LENGTH / 2L - SUN_FADE_TIME, DAY_LENGTH / 2L + SUN_FADE_TIME, _dayTime, 1.0f, 0.0f);
        } else if (_dayTime >= (DAY_LENGTH / 2L) + SUN_FADE_TIME && _dayTime <= DAY_LENGTH - SUN_FADE_TIME) {
            _sunLight.LightEnergy = 0.0f;
        } else if (_dayTime >= DAY_LENGTH - SUN_FADE_TIME || _dayTime <= SUN_FADE_TIME) {
            
            float startTime = DAY_LENGTH - SUN_FADE_TIME;
            float endTime = SUN_FADE_TIME + DAY_LENGTH;
            float adjustedDayTime = _dayTime < SUN_FADE_TIME ? _dayTime + DAY_LENGTH : _dayTime;

            _sunLight.LightEnergy = Mathsf.Remap(startTime, endTime, adjustedDayTime, 0.0f, 1.0f);
        } else {
            _sunLight.LightEnergy = 1.0f;
        }
    }

    private static void AddTime(long time) {
        Toast.Info(GameManager.I().GetPlayer(), $"{(time >= 0 ? "Adding" : "Subtracting")} {Mathsf.Round(time / 1000L, 2)} seconds");
        _dayTime = (long)Mathf.Wrap(_dayTime + time, 0L, DAY_LENGTH);
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
                    Toast.Info(GameManager.I().GetPlayer(), $"Time {( _forcePause ? "paused" : "resumed")}");
                    break;
                case Key.G:
                    _daySpeed += 0.1f;
                    Toast.Info(GameManager.I().GetPlayer(), $"Day Speed: {_daySpeed:0.0}");
                    break;
                case Key.H:
                    _daySpeed -= 0.1f;
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