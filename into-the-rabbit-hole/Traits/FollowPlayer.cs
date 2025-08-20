using System;
using Godot;
using IntoTheRabbitHole.TileObjects;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class FollowPlayer : Trait
{
	public FollowPlayer(TileObject o) : base(o) { }

	public override ushort ExecutionPriority => 1;

	public override void Tick()
	{
		//find player
		var p = Player.Instance;

		//move towards
		if (p == null || p.TilePostion == Owner.TilePostion)
			return;
		var direction = p.TilePostion - Owner.TilePostion;
		//convert into best fitting flat direction
		if (Math.Abs(direction.X) > Math.Abs(direction.Y))
			direction = new Vector2I(Math.Sign(direction.X), 0);
		else
			direction = new Vector2I(0, Math.Sign(direction.Y));


		GD.Print(direction.ToString());

		var postToMove = Owner.TilePostion + direction;
		World.Instance.Move(Owner, postToMove);
	}
}