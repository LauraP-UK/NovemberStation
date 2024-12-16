extends Runnable
class_name RunOnce

func schedule(millis: int, callable: Callable) -> RunOnce:
	assert(callable.get_argument_count() == 1, "ERROR: RunRepeat.schedule() : Callable passed to RunOnce must accept a Runnable parameter!");
	_callable = callable;
	_timer = Timer.new();
	_timer.autostart = false;
	_timer.one_shot = true;
	_timer.wait_time = (millis if millis > 0 else 1) / 1000.0;
	_timer.connect("timeout", Callable.create(self, "_execute"));
	return self;
