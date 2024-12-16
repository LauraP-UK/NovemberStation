extends Node
class_name Scheduler

static var _SCHEDULER:Scheduler = null;

func _init():
	if (_SCHEDULER == null): _SCHEDULER = self;

static func i() -> Scheduler:
	return _SCHEDULER;

static func run_later(millis: int, callable: Callable) -> RunOnce:
	return RunOnce.new().schedule(millis, callable);

static func repeat(run_in: int, repeat_interval, callable: Callable, repeat_limit: int = -1) -> RunRepeat:
	return RunRepeat.new().schedule(run_in, repeat_interval, callable, repeat_limit);
