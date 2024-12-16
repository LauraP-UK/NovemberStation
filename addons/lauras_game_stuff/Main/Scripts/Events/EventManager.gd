extends Node
# Dictionary to store event types and their respective callbacks
var listeners = {}
var counter: float = 0.0;

# Registers a listener for a specific event
func register_listener(event_class: GDScript, callback: Callable, owner_obj: Object = null) -> void:
	var event_class_ID: String = event_class.get_event_id();
	var owning_obj: Object = owner_obj if (owner_obj != null) else callback.get_object();
	if !listeners.has(event_class_ID):
		listeners[event_class_ID] = [];
	listeners[event_class_ID].append(Pair.of(callback, owning_obj));

# Unregisters a listener for a specific event
func unregister_listener(event_class: GDScript, callback: Callable) -> void:
	var event_class_ID: String = event_class.get_event_id();
	if listeners.has(event_class_ID):
		listeners[event_class_ID].erase(callback);

# Fires the event, calling all registered listeners
func fire_event(event: Event) -> void:
	var event_class_ID: String = event.get_event_id();
	for event_ID in listeners:
		if (event_ID != event_class_ID): continue;
		for pair: Pair in listeners[event_ID].duplicate():
			var callback: Callable = pair.key();
			var owner_obj: Object = pair.value();
			if is_instance_valid(owner_obj):
				callback.call(event);
			else:
				listeners[event_ID].erase(pair);

func _process(delta):
	counter += delta;
	if (counter >= 5.0):
		counter = 0.0;
		cleanup_listeners();

# Garbage collector to clean up invalid listeners
func cleanup_listeners() -> void:
	for event_ID in listeners.keys():
		for pair: Pair in listeners[event_ID].duplicate():  # Duplicate to avoid modifying the list while iterating
			var callback: Callable = pair.key();
			var owner_obj: Object = pair.value();
			if !is_instance_valid(owner_obj) or !owner_obj.is_inside_tree():  # Check if the object is valid and still in the scene tree
				listeners[event_ID].erase(pair);
