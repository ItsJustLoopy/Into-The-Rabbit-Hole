using Godot;
using System;

public partial class Player : Node2D
{

	[Export] private TileMapLayer _tileMap;
	private Vector2I _playerTilePosition = new Vector2I(0, 0);


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

		Position = _tileMap.MapToLocal(_playerTilePosition);
		
		base._Process(delta);
	}
	
	public void Move(Vector2I direction)
	{
		if (direction == Vector2I.Zero)
			return;
		//only allow up left, down right movement of 1 unit
		if (Math.Abs(direction.X) + Math.Abs(direction.Y) > 1)
			return;
		var intermediatePosition = _playerTilePosition + direction;
		var newPosition = _playerTilePosition + direction*2;
		
		TileData ti = _tileMap.GetCellTileData(intermediatePosition);
		TileData tn = _tileMap.GetCellTileData(newPosition);
		
		if(ti.GetCustomData("ground").AsBool())
		{
			_playerTilePosition = intermediatePosition;
			if (tn.GetCustomData("ground").AsBool())
			{
				_playerTilePosition = newPosition;
			}
		}
		else
		{
			GD.Print("Cannot move to that tile!");
		}
	}
}
