extends Node
class_name VectorUtils

static func get_null() -> Vector2:
	return Vector2(NAN,NAN);

static func is_null(vector:Vector2) -> bool:
	return is_nan(vector.x) || is_nan(vector.y);
