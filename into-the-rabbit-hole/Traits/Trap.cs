using Godot;

namespace IntoTheRabbitHole;

public class Trap : Trait
{
	public Trap(TileObject o) : base(o)
	{
	}

	public override ushort ExecutionPriority => 10;

	public override void StepOn(TileObject o, Vector2I fromDir)
	{
		o.Kill();
	}

	public override void LeapOver(TileObject o, Vector2I fromDir)
	{
		
	}
}