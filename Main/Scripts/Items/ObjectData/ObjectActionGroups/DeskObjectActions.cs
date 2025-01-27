
public class DeskObjectActions : ObjectData  {
    public DeskObjectActions() {
        AddAction(ObjectActions.GRAB_ACTION);
    }
    public override string GetMetaTag() => "desk_obj";
}