shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_nearest;
uniform float intensity : hint_range(0.0, 1.0) = 0.0;
uniform float max_zoom : hint_range(1.0, 20.0) = 20.0;
uniform float max_shake : hint_range(0.0, 0.1) = 0.2;
uniform float time_scale : hint_range(0.1, 10.0) = 0.1;

void fragment() {
    // Scale all effects based on intensity
    float current_zoom = mix(1.0, max_zoom, intensity);
    float current_shake = max_shake * intensity;
    float current_red = intensity;

    // Calculate shake offset
    vec2 shake_offset = vec2(
        sin(TIME * time_scale) * current_shake,
        cos(TIME * time_scale * 1.2) * current_shake
    );

    // Calculate zoom and center the effect
    vec2 centered_uv = (SCREEN_UV - 0.5) / current_zoom + 0.5;

    // Apply shake to the UV coordinates
    vec2 final_uv = centered_uv + shake_offset;

    // Sample the screen texture
    vec4 bg = texture(SCREEN_TEXTURE, final_uv);

    // Apply red overlay based on intensity
    vec3 red_overlay = mix(bg.rgb, vec3(1.0, 0.0, 0.0), current_red/2.0);

    COLOR = vec4(red_overlay, bg.a);
}