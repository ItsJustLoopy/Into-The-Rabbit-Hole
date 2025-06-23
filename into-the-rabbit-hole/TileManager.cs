using Godot;
using System;
using System.Collections.Generic;
using IntoTheRabbitHole;

public partial class TileManager : Node
{
	private TileMapLayer _tileMap;
	private Tile[,] _tiles;

	public override void _Ready()
	{
		_tileMap = GetNode<TileMapLayer>("TileMapLayer");
		if (_tileMap == null)
		{
			GD.PrintErr("TileMapLayer node not found in TileManager.");
			return;
		}

		// Initialize or load tiles here if needed
		_tiles = new Tile[100, 100];
		
		// Populate the tile array with Tile objects
		for (int x = 0; x < _tiles.GetLength(0); x++)
		{
			for (int y = 0; y < _tiles.GetLength(1); y++)
			{
				_tiles[x, y] = new Tile(x - 50, y - 50, this); // Adjusting for center origin
			}
		}

		var tile = GetTile(0, 0);
		Player p = new Player(tile);
		Move(p,tile);
		AddChild(p);

	}



	private Tile GetTile(int x, int y)
	{
		return GetTile(new Vector2I(x, y));
	}

	public Tile GetTile(Vector2I position)
	{
		//convert from tilemap coordinates to array coordinates
		int x = position.X + 50; // Assuming the tilemap is centered at (0,0)
		int y = position.Y + 50; // Adjust based on your tilemap's origin

		if (x < 0 || x >= _tiles.GetLength(0) || y < 0 || y >= _tiles.GetLength(1))
		{
			GD.PrintErr("Tile position out of bounds: " + position);
			return null;
		}

		return _tiles[x, y];
	}


	public Vector2 MapToLocal(Vector2I playerTilePosition)
	{
		return _tileMap.MapToLocal(playerTilePosition);
	}

	public void Move(TileObject o, Vector2I pos)
	{
		Move(o,GetTile(pos));
	}
	private void Move(TileObject o, Tile target)
	{
		if (o.ParentTile != null)
		{
			o.ParentTile.TileObjects.Remove(o);
		}
		target.Place(o);
	}
}
