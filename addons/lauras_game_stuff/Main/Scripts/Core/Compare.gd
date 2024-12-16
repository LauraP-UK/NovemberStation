extends Node

class_name Compare

static func compare_values(a, b) -> bool:
	# Check if both are null
	if a == null and b == null:
		return true
	
	# If one is null and the other isn't, they are not equal
	if a == null or b == null:
		return false

	# If they both have an equals method, use it
	if typeof(a) == TYPE_OBJECT and typeof(b) == TYPE_OBJECT:
		if "equals" in a and "equals" in b:
			return a.equals(b)
	
	# Fallback to using the == operator
	return a == b
