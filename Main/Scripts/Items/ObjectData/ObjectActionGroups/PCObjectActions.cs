
public class PCObjectActions : ObjectData {
    public PCObjectActions() {
        AddAction(ObjectActions.USE_ACTION);
    }
    public override string GetMetaTag() => "pc_obj";
}