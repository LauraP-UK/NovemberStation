extends Runnable
class_name RunRepeat

var _repeat_interval:int;
var _initial_delay: int;

var _repeat_count: int = 0;
var _repeat_limit: int = -1;

func schedule(delay: int, interval: int, callable: Callable, repeat_limit:int = -1) -> RunRepeat:
	#assert(callable.get_argument_count() == 1, "ERROR: RunRepeat.schedule() : Callable passed to RunRepeat must accept a Runnable parameter!");
	_callable = callable;
	_repeat_interval = interval;
	_initial_delay = delay;
	_repeat_limit = repeat_limit;
	_timer = Timer.new();
	_timer.autostart = false;
	_timer.one_shot = false;
	_timer.connect("timeout", Callable.create(self, "_execute"));
	return self;

func run() -> Runnable:
	Scheduler.i().get_tree().root.add_child(_timer);
	_timer.start((_initial_delay if _initial_delay > 0 else 1) / 1000.0);
	_timer.wait_time = (_repeat_interval / 1000.0);
	_is_active = true;
	return self;

func _execute() -> void:
	if (_is_active && _callable.is_valid()):
		_repeat_count += 1;
		_callable.call(self);
		if (_repeat_limit > 0 && _repeat_count >= _repeat_limit):
			cancel();
