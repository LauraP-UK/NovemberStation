
using System;
using Godot;

public class InteractionZone<TNode, TObject> : ActionHolder, IInteractionZone where TNode : Node3D where TObject : IObjectBase {
    
    private readonly string _identifier;
    private readonly TNode _containingNode;
    private readonly TObject _objectBase;
    private readonly Func<string> _getDisplayName;
    private readonly Func<string> _getContext;
    private Node3D _bboxNode;
    
    public InteractionZone(string identifier, TNode containingNode, TObject objectBase, Func<string> getDisplayName, Func<string> getContext, Action<InteractionZone<TNode, TObject>> registerActions) {
        _identifier = identifier;
        _containingNode = containingNode;
        _objectBase = objectBase;
        _getDisplayName = getDisplayName;
        _getContext = getContext;
        registerActions?.Invoke(this);
    }

    public Node3D GetBoundingBoxNode() {
        if (_bboxNode != null) return _bboxNode;
        Node bbox = _containingNode.FindChild("BBox");
        switch (bbox) {
            case Node3D bboxNode: {
                _bboxNode = bboxNode;
                return bboxNode;
            }
            case null:
                GD.PrintErr($"ERROR: InteractionZone<TNode, TObject>.GetBoundingBoxNode() : BBox node not found in {_containingNode.Name}");
                break;
            default:
                GD.PrintErr($"ERROR: InteractionZone<TNode, TObject>.GetBoundingBoxNode() : BBox node is not a Node3D in {_containingNode.Name}");
                break;
        }
        return null;
    }
    
    public string GetIdentifier() => _identifier;
    public Node3D GetContainingNode() => _containingNode;
    public IObjectBase GetObjectBase() => _objectBase;
    public TNode GetContainingNodeTyped() => _containingNode;
    public TObject GetObjectBaseTyped() => _objectBase;
    public string GetDisplayName() => _getDisplayName?.Invoke() ?? _objectBase.GetDisplayName();
    public string GetContext() => _getContext?.Invoke() ?? _objectBase.GetContext();
}