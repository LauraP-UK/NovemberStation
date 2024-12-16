@tool
extends Node

class_name Directions

enum SIMPLE_DIRECTION {SELF, NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORTH_WEST}
enum CARDINAL_SIMPLE_DIRECTION {NORTH = SIMPLE_DIRECTION.NORTH, EAST = SIMPLE_DIRECTION.EAST, SOUTH = SIMPLE_DIRECTION.SOUTH, WEST = SIMPLE_DIRECTION.WEST}

static var SELF: DirectionsType = DirectionsType.create("Self", Vector2(0,0), SIMPLE_DIRECTION.SELF);
static var NORTH: DirectionsType = DirectionsType.create("North", Vector2(0,-1), SIMPLE_DIRECTION.NORTH);
static var EAST: DirectionsType = DirectionsType.create("East", Vector2(1,0), SIMPLE_DIRECTION.EAST);
static var SOUTH: DirectionsType = DirectionsType.create("South", Vector2(0,1), SIMPLE_DIRECTION.SOUTH);
static var WEST: DirectionsType = DirectionsType.create("West", Vector2(-1,0), SIMPLE_DIRECTION.WEST);
static var NORTH_EAST: DirectionsType = DirectionsType.combine("North East", NORTH, EAST, SIMPLE_DIRECTION.NORTH_EAST);
static var SOUTH_EAST: DirectionsType = DirectionsType.combine("South East", SOUTH, EAST, SIMPLE_DIRECTION.SOUTH_EAST);
static var SOUTH_WEST: DirectionsType = DirectionsType.combine("South West", SOUTH, WEST, SIMPLE_DIRECTION.SOUTH_WEST);
static var NORTH_WEST: DirectionsType = DirectionsType.combine("North West", NORTH, WEST, SIMPLE_DIRECTION.NORTH_WEST);

static func get_cardinal() -> Array[DirectionsType]:
	return [NORTH, EAST, SOUTH, WEST];
	
static func get_diags() -> Array[DirectionsType]:
	return [NORTH_EAST, SOUTH_EAST, SOUTH_WEST, NORTH_WEST];

static func get_all() -> Array[DirectionsType]:
	return [SELF, NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORTH_WEST];

static func get_relative(pos_1:Vector2, pos_2:Vector2) -> DirectionsType:
	for dir:DirectionsType in get_all():
		if (pos_2 == dir.get_relative(pos_1)): return dir;
	return SELF;

static func get_cw(direction: DirectionsType, cardinal_only: bool = true) -> DirectionsType:
	if (direction.simple_direction == SIMPLE_DIRECTION.SELF): return SELF;
	var dirs: Array[DirectionsType] = get_cardinal() if (cardinal_only) else get_all();
	dirs.erase(SELF);
	return dirs[(dirs.find(direction) + 1) % dirs.size()];

static func get_ccw(direction: DirectionsType, cardinal_only: bool = true) -> DirectionsType:
	if (direction.simple_direction == SIMPLE_DIRECTION.SELF): return SELF;
	var dirs: Array[DirectionsType] = get_cardinal() if (cardinal_only) else get_all();
	dirs.erase(SELF);
	dirs.reverse();
	return dirs[(dirs.find(direction) + 1) % dirs.size()];

static func get_opposite(direction: DirectionsType) -> DirectionsType:
	match direction.simple_direction:
		SIMPLE_DIRECTION.SELF: return SELF;
		SIMPLE_DIRECTION.NORTH: return SOUTH;
		SIMPLE_DIRECTION.NORTH_EAST: return SOUTH_WEST;
		SIMPLE_DIRECTION.EAST: return WEST;
		SIMPLE_DIRECTION.SOUTH_EAST: return NORTH_WEST;
		SIMPLE_DIRECTION.SOUTH: return NORTH;
		SIMPLE_DIRECTION.SOUTH_WEST: return NORTH_EAST;
		SIMPLE_DIRECTION.WEST: return EAST;
		SIMPLE_DIRECTION.NORTH_WEST: return SOUTH_EAST;
	print("ERROR: Directions.get_opposite() : Not a valid direction given: ", direction.to_string());
	return null;

static func get_from_simple(simple_direction: int) -> DirectionsType:
	for dir in get_all():
		if (dir.simple_direction == simple_direction): return dir;
	return null;

static func get_from_name(type_name: String) -> DirectionsType:
	var upper:String = type_name.to_upper();
	for dir in get_all():
		if (dir.name.to_upper() == upper): return dir;
	return null;
