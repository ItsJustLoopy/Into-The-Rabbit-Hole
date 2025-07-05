using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class Walk : Trait
{

	public Walk(TileObject o) : base(o)
	{
	}


	public override ushort ExecutionPriority => 5;

	public override void Tick()
	{
		int dirInt = GD.RandRange(0, 3);
		Vector2I dir = Vector2I.Zero;
		switch (dirInt)
		{
			case 0:
				dir = Vector2I.Up;
				break;
			case 1:
				dir = Vector2I.Down;
				break;
			case 2:
				dir = Vector2I.Left;
				break;
			case 3:
				dir = Vector2I.Right;
				break;
		}
		TileManager.Instance.Move(_owner,_owner.TilePostion + dir);
	}
}