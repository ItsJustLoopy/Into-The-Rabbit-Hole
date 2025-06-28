using Godot;
using 
	IntoTheRabbitHole.TileManager;
namespace IntoTheRabbitHole.Traits;

public class Camera : Trait
{
	public Camera(TileObject o) : base(o)
	{
		o.AddChild(new Camera2D());
	}

	public override ushort ExecutionPriority => 0;

	public override void StepOn(TileObject o, Vector2I fromDir)
	{
	}

	public override void LeapOver(TileObject o, Vector2I fromDir)
	{
		
	}
}