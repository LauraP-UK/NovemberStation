extends Node

class_name Pair

var _key = null;
var _value = null;

func _init(key, value):
	_key = key;
	_value = value;

func set_key(key):
	self._key = key;

func set_value(value):
	self._value = value;

func key():
	return _key;

func value():
	return _value;

func is_empty():
	return _key == null && _value == null;

func equals(pair: Pair) -> bool:
	var keys_equal = Compare.compare_values(_key, pair._key);
	var values_equal = Compare.compare_values(_value, pair._value);
	return keys_equal and values_equal

func as_string() -> String:
	return "Pair(" + str(_key) + ", " + str(_value) + ")";

func hash() -> int:
	var key_hash = _key.hash() if _key != null else 0;
	var value_hash = _value.hash() if _value != null else 0;
	return key_hash ^ value_hash;

static func of(key, value) -> Pair:
	return Pair.new(key, value);

static func empty() -> Pair:
	return Pair.new(null, null);
