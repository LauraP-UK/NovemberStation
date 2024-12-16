extends Node
class_name WeightedSelection

var selections:Array[WeightPair] = [];

func add_selection(weight:float, selection):
	selections.append(WeightPair.of(weight, selection));

func select() -> Variant:
	var total:float = 0.0;
	for selection:WeightPair in selections:
		total += selection.key();
	
	var selected_weight:float = RandF.rand_F(0.0, total);
	var cumulative:float = 0.0;
	
	for selection:WeightPair in selections:
		cumulative += selection.key();
		if (selected_weight <= cumulative):
			return selection.value();
	return null; #Should never trigger, famous last words...
