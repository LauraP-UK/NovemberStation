extends Event

class_name KeyPressEvent

var key_pressed: String;

func _init(key_pressed: String):
	self.key_pressed = key_pressed;


static func get_event_id() -> String:
	return "KeyPressEvent";
