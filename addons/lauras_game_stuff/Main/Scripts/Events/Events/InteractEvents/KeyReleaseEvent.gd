extends Event

class_name KeyReleaseEvent

var key_released: String;

func _init(key_released: String):
	self.key_released = key_released;


static func get_event_id() -> String:
	return "KeyReleaseEvent";
