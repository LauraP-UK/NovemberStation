using Godot;

public class HeldDisplaySettings {
    private readonly Vector3 _positionOffset, _rotationOffset;
    private readonly float _scaleOffset;
    
    private static readonly HeldDisplaySettings DEFAULT = new(Vector3.Zero, Vector3.Zero, 1.0f);

    private HeldDisplaySettings(Vector3 positionOffset, Vector3 rotationOffset, float scaleOffset) {
        _positionOffset = positionOffset;
        _rotationOffset = rotationOffset;
        _scaleOffset = scaleOffset;
    }
    
    public Vector3 GetPositionOffset() => _positionOffset;
    public Vector3 GetRotationOffset() => _rotationOffset;
    public float GetScaleOffset() => _scaleOffset;
    public bool IsDefault() => Equals(DEFAULT);

    public void ApplyTo(Node3D node) {
        if (IsDefault()) return;
        node.SetPosition(GetPositionOffset());
        node.SetRotationDegrees(GetRotationOffset());
        node.SetScale(Vector3.One * GetScaleOffset());
    }
    
    public static HeldDisplaySettings Create(Vector3 positionOffset, Vector3 rotationOffset, float scaleOffset) => new(positionOffset, rotationOffset, scaleOffset);
    public static HeldDisplaySettings Default() => DEFAULT;
}