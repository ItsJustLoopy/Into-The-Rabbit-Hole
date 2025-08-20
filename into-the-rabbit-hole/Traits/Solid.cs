using Godot;
using IntoTheRabbitHole.TileObjects;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class Solid : Trait
{
	public Solid(TileObject o) : base(o) { }

	public override ushort ExecutionPriority => 5;

	public override void SteppedOn(TileObject o, Vector2I fromDir)
	{
		World.Instance.Move(o, o.TilePostion + fromDir);
	}
}