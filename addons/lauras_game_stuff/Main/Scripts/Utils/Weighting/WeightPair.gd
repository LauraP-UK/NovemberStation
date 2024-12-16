extends Pair
class_name WeightPair

func _init(weight:float, selection):
	self._key = weight;
	self._value = selection;

func set_key(weight:float):
	self._key = weight;

static func of(weight:float, selection) -> Pair:
	return WeightPair.new(weight, selection);
