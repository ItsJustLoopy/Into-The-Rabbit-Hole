using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace IntoTheRabbitHole.Tiles;

public partial class TileManager : Node
{
	private TileMapLayer _tileMap;
	
	private Tile[,] _tiles;
	public Tile[,] Tiles => _tiles;
	public int CurrentLevel { get; private set; } = 1;

	
	private static TileManager _instance;
	public static TileManager Instance
	{
		get => _instance;
		private set => _instance = value;
	}
	
	[Export] LevelGenerator _levelGenerator;

	public void InitializeTiles(int width, int height)
	{
		_tiles = new Tile[width, height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				
				_tiles[x, y] = new Tile(x - 50, y - 50, this);
			}
		}
	}

	



	
	public override void _Ready()
	{
		//Null check maxxing
		if (Instance != null)
		{
			GD.PrintErr("Infinitwe pringles detected!");
			QueueFree();
			return;
		}
		Instance = this;
		
		_tileMap = GetNode<TileMapLayer>("TileMapLayer");
		if (_tileMap == null)
		{
			GD.PrintErr("TileMapLayer node not found in TileManager.");
			return;
		}
		
		if (_levelGenerator == null)
		{
			GD.PrintErr("LevelGenerator not assigned in TileManager  - godot exported property.");
			return;
		}

		
		InitializeTiles(100,100);
		
		//Initialize level generator
		_levelGenerator.Initialize();
		_levelGenerator.GenerateLevel();

		// Create baba
		var origin = GetTile(0, 0);
		if (origin != null)
		{
			Player p = new Player(origin);
			Move(p, origin);
		}
		else
		{
			GD.PrintErr("Could not get origin for player placement");
		}

		



	}


	private double timeTillNextTick = 1;
	private List<TileObject> GlobalList = new List<TileObject>();
	
	private List<TileObject> GetGlobalList()
	{
		GlobalList.Clear();
		//we collect all objects and update them, "updating tiles" will cause moving objects to get ticked more than once
		foreach (var t in _tiles)
		{
			if (t != null)
			{
				GlobalList.AddRange(t.TileObjects ?? Enumerable.Empty<TileObject>());
			}

		}
		return GlobalList;
	}
	public override void _Process(double delta)
	{
		timeTillNextTick -= delta;
		if (timeTillNextTick <= 0)
		{
			
			
			
			foreach (var o in GetGlobalList())
			{
				o.Tick();
			}

			timeTillNextTick = 1;
		}
		base._Process(delta);
		
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
	
	public void SetTile(int x, int y, Tile tile)
	{
		if (x < 0 || y < 0 || x >= _tiles.GetLength(0) || y >= _tiles.GetLength(1))
		{
			GD.PrintErr($"Attempted to set tile outside bounds at {x},{y}");
			return;
		}
		_tiles[x, y] = tile;
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

	public void Jump(TileObject o, Vector2I pos)
	{
		Jump(o,GetTile(pos));
	}
	
	private void Jump(TileObject o, Tile target)
	{
		if (o.ParentTile != null)
		{
			o.ParentTile.TileObjects.Remove(o);
		}
		target.Place(o,true);
	}

	public void UpdateCameraPosition(float camRotation)
	{
		foreach (var obj in GetGlobalList())
		{
			obj.UpdateCameraPosition(camRotation);
		}
	}
	

	
	

}
