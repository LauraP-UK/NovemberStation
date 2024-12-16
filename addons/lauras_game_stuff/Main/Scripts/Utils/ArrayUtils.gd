extends Node
class_name ArrayUtils

static func get_shared(arr1:Array, arr2:Array) -> Array:
	var return_arr:Array = [];
	for val in arr1:
		if (arr2.has(val)): return_arr.append(val);
	return return_arr;
