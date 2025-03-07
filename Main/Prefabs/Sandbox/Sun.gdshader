shader_type spatial;
render_mode unshaded, depth_draw_never, cull_disabled;

void fragment() {
    ALBEDO = vec3(1.0, 0.9, 0.5); // Sun color (warm yellow)
}