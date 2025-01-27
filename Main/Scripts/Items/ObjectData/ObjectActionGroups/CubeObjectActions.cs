
public class CubeObjectActions : ObjectData {
    public CubeObjectActions() {
        AddAction(ObjectActions.GRAB_ACTION);
        AddAction(ObjectActions.SHOVE_ACTION);
    }
    public override string GetMetaTag() => "cube_obj";
}