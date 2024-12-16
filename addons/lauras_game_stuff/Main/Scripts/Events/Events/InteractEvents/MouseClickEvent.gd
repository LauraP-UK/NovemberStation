extends Event

class_name MouseClickEvent

enum ButtonType { LEFT, RIGHT, MIDDLE }
enum ActionType { PRESS, RELEASE, DOUBLE_CLICK }

var button_type: ButtonType
var action_type: ActionType
var position: Vector2

func _init(button_type:ButtonType, action_type:ActionType, position:Vector2):
	self.button_type = button_type;
	self.action_type = action_type;
	self.position = position;

static func get_event_id() -> String:
	return "MouseClickEvent";
