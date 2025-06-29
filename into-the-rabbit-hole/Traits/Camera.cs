using Godot;

namespace IntoTheRabbitHole.Traits;

public class Camera : Trait
{
	public Camera(TileObject o) : base(o)
	{
		o.AddChild(new Camera2D());
	}

	public override ushort ExecutionPriority => 0;
	
}