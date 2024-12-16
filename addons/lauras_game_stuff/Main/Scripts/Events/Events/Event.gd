extends Node

class_name Event


# Abstract class for general events
func _init():
	pass  # Abstract init, can be extended by subclasses.


static func get_event_id() -> String:
	return "Event";
