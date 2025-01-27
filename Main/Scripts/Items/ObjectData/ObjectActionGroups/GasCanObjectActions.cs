
public class GasCanObjectActions : ObjectData {
    public GasCanObjectActions() {
        AddAction(ObjectActions.GRAB_ACTION);
        AddAction(ObjectActions.SHOVE_ACTION);
        AddAction(ObjectActions.USE_ACTION);
    }
    public override string GetMetaTag() => "gascan_obj";
}