extends Node

class_name RandF

static func rand_F(min: float, max: float, bias: float = 1.0) -> float:
	return (max - min)  * pow(randf_range(0.0, 0.99999999), bias) + min;

static func rand_I(min: int, max: int, bias: float = 1.0) -> int:
	return int(floor((max + 1 - min) * pow(randf_range(0.0, 0.99999999), bias) + min));


static func rand_chance_in_F(chance: float, chanceIn: float) -> bool:
	return rand_F(1.0, abs(chanceIn)) <= abs(chance);

static func rand_chance_in_I(chance: float, chanceIn: float) -> bool:
	return rand_I(1, abs(chanceIn)) <= abs(chance);


static func random() -> bool:
	return rand_I(0, 1) == 0;


static func rand_from(array: Array):
	if (array == null || array.size() == 0): return null;
	if (array.size() == 1): return array[0];
	return array[rand_I(0, array.size() - 1)];

static func rand_from_dict(dict: Dictionary) -> Pair:
	if (dict == null || dict.size() == 0): return null;
	var randKey = rand_from(dict.keys());
	return Pair.of(randKey, dict.get(randKey));

static func random_and_remove(array: Array) -> Pair:
	if (array == null || array.size() == 0): return null;
	var element = rand_from(array);
	if (element == null): return null;
	var copy: Array = array.duplicate();
	copy.remove_at(copy.find(element));
	return Pair.of(element, copy);

static func random_and_remove_dict(dict: Dictionary) -> Pair:
	if (dict == null || dict.size() == 0): return null;
	var selected: Pair = rand_from_dict(dict);
	if (selected == null || selected.isEmpty()): return null;
	var copy: Dictionary = dict.duplicate();
	copy.erase(selected.key());
	return Pair.of(selected, copy);

static func copy_and_shuffle(array: Array) -> Array:
	if (array == null || array.is_empty()): return array;
	var copy: Array = array.duplicate();
	copy.shuffle();
	return copy;

static func rand_subset(array: Array, length: int) -> Array:
	if (array == null || array.is_empty()): return array;
	if (length >= array.size()): return copy_and_shuffle(array);
	return copy_and_shuffle(array).slice(0, length);

static func rand_subset_dict(dict: Dictionary, length: int) -> Dictionary:
	if (dict == null || dict.is_empty()): return dict;
	if (length >= dict.size()): return dict.duplicate();
	var returnDict: Dictionary = {};
	var editDict: Dictionary = dict.duplicate();
	for i in range(length):
		var selected: Pair = random_and_remove_dict(editDict);
		returnDict[selected.key().key()] = selected.key().value();
		editDict = selected.value();
	return returnDict;
