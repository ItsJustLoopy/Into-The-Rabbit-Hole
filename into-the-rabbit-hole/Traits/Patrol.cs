using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class Patrol : Trait
{
	private readonly int _dirInt = GD.RandRange(0, 3);

	public Patrol(TileObjects.TileObject o) : base(o)
	{
	}


	public override ushort ExecutionPriority => 5;

	public override void Tick()
	{
		var dir = Vector2I.Zero;
		switch (_dirInt)
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

		TileManager.Instance.Move(Owner, Owner.TilePostion + dir);
		//TODO CHEC IF WE BOUNCE OF WALL, CHANGE DIRECTION THEN
	}
}