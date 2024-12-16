extends Node

class_name MathsF

static func round_to_significant(value: float, significantDigits: int) -> float:
	if value == 0:
		return 0;
	var scale = pow(10.0, significantDigits - floor(log10(abs(value))) - 1);
	return round(value * scale) / scale;

static func mid(a: float, b: float) -> float:
	return lerpf(a, b, 0.5);

static func lerp_clamped(a: float, b: float, t: float) -> float:
	return lerp(a, b, clampf(t, 0.0, 1.0));

static func ilerp(a: float, b: float, v: float) -> float:
	if (b == a): return 0; # Catch DivBy0 error
	return (v - a) / (b - a);

static func ilerp_clamped(a: float, b: float, v: float) -> float:
	if (b == a): return 0; # Catch DivBy0 error
	return clamp(ilerp(a, b, v), 0.0, 1.0);

static func delta_angle(from: float, to: float) -> float:
	var delta = fmod((to - from + 180.0), 360.0) - 180.0;
	return delta + 360.0 if delta < -180.0 else delta;

static func max_F(args: Array[float]) -> float:
	return args.max();

static func min_F(args: Array[float]) -> float:
	return args.min();

static func max_I(args: Array[int]) -> int:
	return args.max();

static func min_I(args: Array[int]) -> int:
	return args.min();

static func log10(v: float) -> float:
	return log(v) / log(10);

static func closest_to_F(target: float, array: Array[float]) -> float:
	if array == null || array.size() == 0: return -1;
	if array.size() == 1: return array[0];
	var closest_value = array[0];
	var smallest_difference = abs(closest_value - target);
	for i in range(1, array.size()):
		var difference = abs(array[i] - target);
		if difference < smallest_difference:
			smallest_difference = difference;
			closest_value = array[i];
	return closest_value;

static func closest_to_I(target: int, array: Array[int]) -> int:
	if array == null || array.size() == 0: return -1;
	if array.size() == 1: return array[0];
	var closest_value = array[0];
	var smallest_difference = abs(closest_value - target);
	for i in range(1, array.size()):
		var difference = abs(array[i] - target);
		if difference < smallest_difference:
			smallest_difference = difference;
			closest_value = array[i];
	return closest_value;
