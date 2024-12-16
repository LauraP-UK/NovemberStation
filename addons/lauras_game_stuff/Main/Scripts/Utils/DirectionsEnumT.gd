extends Resource

class_name DirectionsType

var name: String;
var offset: Vector2;
var simple_direction: Directions.SIMPLE_DIRECTION = Directions.SIMPLE_DIRECTION.SELF;

func _init(name: String, offset: Vector2, simple_enum: Directions.SIMPLE_DIRECTION):
	self.name = name;
	self.simple_direction = simple_enum;
	self.offset = offset;

func get_relative(coord: Vector2) -> Vector2:
	return Vector2(coord.x + offset.x, coord.y + offset.y);

func get_opposite() -> DirectionsType:
	return Directions.get_opposite(self);

func get_cw() -> DirectionsType:
	return Directions.get_cw(self);

func get_ccw() -> DirectionsType:
	return Directions.get_ccw(self);

func is_cardinal() -> bool:
	return Directions.get_cardinal().has(self);

static func create(name: String, offset: Vector2, simple_enum: Directions.SIMPLE_DIRECTION) -> DirectionsType:
	return DirectionsType.new(name, offset, simple_enum);

static func combine(name: String, directionA: DirectionsType, directionB: DirectionsType, simple_enum: Directions.SIMPLE_DIRECTION):
	return DirectionsType.new(name, Vector2(directionA.offset.x + directionB.offset.x, directionA.offset.y + directionB.offset.y), simple_enum);
