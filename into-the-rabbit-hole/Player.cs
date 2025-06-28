using System;
using Godot;
using IntoTheRabbitHole.TileManager;

namespace IntoTheRabbitHole;

public partial class Player : TileObject
{
	public Player(Tile parentTil): base(parentTil,"Player")
	{
	}

	public override void _Process(double delta)
	{

		if (Input.IsActionJustPressed("Up"))
		{
			Move(new Vector2I(0, -1));
		}
		else if (Input.IsActionJustPressed("Down"))
		{
			Move(new Vector2I(0, 1));
		}
		else if (Input.IsActionJustPressed("Left"))
		{
			Move(new Vector2I(-1, 0));
		}
		else if (Input.IsActionJustPressed("Right"))
		{
			Move(new Vector2I(1, 0));
		}

	
		base._Process(delta);
	}
	
	public void Move(Vector2I direction)
	{
		if (direction == Vector2I.Zero)
			return;
		//only allow up left, down right movement of 1 unit
		if (Math.Abs(direction.X) + Math.Abs(direction.Y) > 1)
			return;
		var intermediatePosition = TilePostion + direction;
		var newPosition = TilePostion + direction*2;

		Tile ti = TileManager.GetTile(intermediatePosition);
		Tile tn = TileManager.GetTile(newPosition);


		
		if(ti.GroundType.canStand)
		{
			TileManager.Move(this,intermediatePosition);
			if (tn.GroundType.canStand)
			{
				TileManager.Move(this,newPosition);
			}
		}
		else
		{
			GD.Print("Cannot move to that tile!");
		}
	}
}