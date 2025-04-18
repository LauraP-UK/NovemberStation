using Godot;

public partial class BackdropBootstrapper : Bootstrapper {
    
    private Camera3D _camera;
    public void SetCameraRotation(Vector3 globalRotation) {
        _camera ??= MainLauncher.FindNode<Camera3D>("Main/Camera3D", true);
        _camera.GlobalRotation = globalRotation;
    }
}