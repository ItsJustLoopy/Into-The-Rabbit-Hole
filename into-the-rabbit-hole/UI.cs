using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole;

public partial class UI : ColorRect
{
	public static UI Instance;


	public ColorRect Fox;
	ShaderMaterial Shader;
	public override void _Ready()
	{
		Instance = this;
		Fox = GetNode<ColorRect>("fox"); 
		Shader = this.Material as ShaderMaterial;
		
	}


	public override void _Process(double delta)
	{
		//move fox to player(middle of screen) based on closeness to death
		var killTime = TileManager.Instance.TimeTillePlayerKill;
    
		// Get screen center position
		var screenCenter = GetViewportRect().Size / 2;
    
		// Calculate movement factor (0 = far from death, 1 = very close to death)
		var maxKillTime = 100.0f; // Adjust this value based on your game's max kill time
		var movementFactor = Mathf.Clamp(1.0f - (float)(killTime / maxKillTime), 0.0f, 1.0f);
		
		Shader.SetShaderParameter("intensity", movementFactor);
		GD.Print($"intensityr=: {movementFactor}");
		// Interpolate fox position towards screen center
		var targetPosition = screenCenter - (Fox.Size / 2); // Center the fox sprite
		var currentPosition = Fox.Position;
		var shouldBePosition = currentPosition.Lerp(targetPosition, movementFactor);
		Fox.Position = currentPosition.Lerp(shouldBePosition, movementFactor * (float)delta * 2.0f);
    
		base._Process(delta);
	}
}