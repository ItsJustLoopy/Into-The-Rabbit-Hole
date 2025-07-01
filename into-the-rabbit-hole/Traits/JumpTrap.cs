using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class JumpTrap : Trait
{
	public JumpTrap(TileObject o) : base(o)
	{
	}

	public override ushort ExecutionPriority => 4;

	public override void FloatedOn(TileObject o, Vector2I fromDir)
	{
		o.Kill();
	}


}