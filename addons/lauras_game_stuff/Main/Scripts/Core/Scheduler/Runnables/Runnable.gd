extends Node
class_name Runnable

var _timer: Timer = null;
var _is_active: bool = false;
var _callable: Callable = Callable.create(null, "-");

func run() -> Runnable:
	Scheduler.i().get_tree().root.add_child(_timer);
	_timer.start();
	_is_active = true;
	return self;

func _execute() -> void:
	if (_is_active && _callable.is_valid()):
		_callable.call(self);
	_is_active = false;
	_timer.queue_free();

func cancel() -> void:
	if (_is_active):
		_timer.stop();
		_is_active = false;
		_timer.queue_free();

func is_active() -> bool:
	return _is_active;
