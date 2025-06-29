using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class Trap : Trait
{
	public Trap(TileObject o) : base(o)
	{
	}

	public override ushort ExecutionPriority => 5;

	public override void SteppedOn(TileObject o, Vector2I fromDir)
	{
		o.Kill();
	}


}