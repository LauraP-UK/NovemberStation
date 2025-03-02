@tool
extends Node3D

@export var size: Vector3 = Vector3(1, 1, 1) :
	set(value):
		size = value
		update_size()
		notify_property_list_changed() # Force refresh in Inspector

var initialized := false

func _process(delta):
	if not initialized:
		initialized = true
		detect_existing_size()

func detect_existing_size():
	var mesh = $Mesh
	if mesh and mesh is MeshInstance3D and mesh.mesh is BoxMesh:
		size = mesh.mesh.size # Preserve existing size

	update_size()
	notify_property_list_changed() # Ensure Inspector reflects the correct size

func update_size():
	var mesh = $Mesh
	var collision = $Collision

	if mesh and mesh is MeshInstance3D and mesh.mesh is BoxMesh:
		if not mesh.mesh.resource_local_to_scene:
			mesh.mesh = mesh.mesh.duplicate() # Ensure unique mesh resource
			mesh.mesh.resource_local_to_scene = true # Mark it as unique
		mesh.mesh.size = size

	if collision and collision is CollisionShape3D and collision.shape is BoxShape3D:
		if not collision.shape.resource_local_to_scene:
			collision.shape = collision.shape.duplicate() # Ensure unique collision resource
			collision.shape.resource_local_to_scene = true # Mark it as unique
		collision.shape.size = size
