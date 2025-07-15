shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_nearest;
uniform float intensity : hint_range(0.0, 1.0) = 0.0;
uniform float max_zoom : hint_range(1.0, 20.0) = 20.0;
uniform float max_shake : hint_range(0.0, 0.1) = 0.15;
uniform float time_scale : hint_range(0.1, 10.0) = 0.1;
uniform float offset_mod : hint_range(0, 1) = 0.000001;
float random (vec2 uv) {
    return fract(sin(dot(uv.xy,
        vec2(12.9898,78.233))) * 43758.5453123);
}
vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
// HSV to RGB conversion
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

uniform float eye_aspect : hint_range(0.1, 5.0) = 2.5; // Controls eye width/height ratio
uniform float eye_sharpness : hint_range(0.1, 5.0) = 2.0; // Controls eye point sharpness

// Eye-shaped sine function
float eyeSin(float x, float y) {
    float adjusted_x = x * eye_aspect;
    float adjusted_y = y;
    float dist = sqrt(adjusted_x * adjusted_x + adjusted_y * adjusted_y);
    float angle = atan(adjusted_y, adjusted_x);
    float eye_modulation = pow(abs(cos(angle * 0.5)), eye_sharpness);
    return sin(dist * eye_modulation * 6.28318) * eye_modulation;
}

void fragment() {
//    float intensity = 0.1;
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

    float offset1 = random(vec2((UV.x/TIME),(UV.y/TIME)));
    float a = dot(cos(UV.x)*eyeSin(TIME*0.1,UV.x)*10.0,cos(UV.y)*eyeSin(TIME*0.0001,UV.x)*20.0);
    float b = dot(eyeSin(UV.x,UV.y)*cos(TIME*0.01)*50.0,eyeSin(UV.y,UV.x)*cos(TIME*0.001)*50.0);
    float c = dot(cos(UV.x)*eyeSin(TIME*0.001,UV.x)*20.0,eyeSin(UV.y,UV.x)*eyeSin(TIME*0.01,UV.x)*60.0);
    float d = dot(eyeSin(UV.x,UV.y)*cos(TIME*0.0001)*30.0,cos(UV.y)*cos(TIME*0.1)*30.0);
    float offset2 = a*4.0+b*3.0+c*2.0+d;

   float a2 = cos(UV.x * TIME) * cos(UV.y * TIME * 4.0);
    float b2 = sin(UV.x * TIME) * sin(UV.y * TIME * 3.0);
    float c2 = cos(UV.x * TIME) * sin(UV.y * TIME * 2.0);
    float d2 = sin(UV.x * TIME) * cos(UV.y * TIME);
    // Balanced combination that won't create directional bias
    float offset3 = (a2+b2+c2+d2)* 0.25;


    // Apply shake to the UV coordinates
    vec2 final_uv = centered_uv + shake_offset+offset3*0.0006;
    //final_uv = final_uv + offset2intensityoffset_mod;
    // Sample the screen texture
    vec4 bg = texture(SCREEN_TEXTURE, final_uv);
    vec3 bghsv = rgb2hsv(bg.rgb);
    bghsv.x += sin(offset2)*intensity;
    //bghsv.y += offset2intensity;
    //bghsv.z += offset2intensity;
    vec3 crakcerRGB = hsv2rgb(bghsv);
    vec3 crackervission = mix(bg.rgb, crakcerRGB, intensity/0.1);
    vec3 red_overlay = mix(crackervission, vec3(1.0,0.5,0.5), intensity/4.0);
    COLOR = vec4(red_overlay, bg.a);
}
