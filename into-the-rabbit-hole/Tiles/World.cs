using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using IntoTheRabbitHole.TileObjects;

namespace IntoTheRabbitHole.Tiles;

public partial class World : Node
{
	public static World Instance;

	private readonly List<TileObject> globalList = new();
	private readonly List<Action> nextTickActions = new();

	private readonly List<Action> postTickActions = new();

	private float camraRotation;
	public int CurrentLevel = 1;
	private LevelGenerator levelGenerator;
	private int mapSize;
	private TileMapLayer occlusionLayer;
	

	private bool tickInProgress;
	private TileMapLayer tileMap;
	private Tile[,] tiles;

	public double TimeTillePlayerKill = 100f;
	private double timeTillNextTick = 1;
	private TileMapLayer wallTileMap;

	private const float TimePerTick = 0.2f;

	public override void _Ready()
	{
		Instance = this;
		wallTileMap = GetNode<TileMapLayer>("WallLayer");
		levelGenerator = GetNode<LevelGenerator>("LevelGenerator");

		mapSize = Database.GetLevel(CurrentLevel).MapSize;
		levelGenerator.MapSize = mapSize;
		

		// Initialize or load tiles here if needed
		tiles = new Tile[mapSize, mapSize];

		// Populate the tile array with Tile objects
		for (int x = 0; x < tiles.GetLength(0); x++)
		for (int y = 0; y < tiles.GetLength(1); y++)
			tiles[x, y] = new Tile(x - mapSize / 2, y - mapSize / 2, this); // Adjusting for center origin


		var p = new Player(GetTile(0, 0));
		Move(p, GetTile(0, 0));
		levelGenerator.Initialize();
		levelGenerator.GenerateLevel(ref wallTileMap);
	}

	private List<TileObject> GetGlobalList()
	{
		globalList.Clear();
		//we collect all objects and update them, "updating tiles" will cause moving objects to get ticked more than once
		foreach (var t in tiles) globalList.AddRange(t.TileObjects);
		return globalList;
	}

	public void DoNextTick(Action t)
	{
		nextTickActions.Add(t);
	}

	public void DoAfterThisTick(Action t)
	{
		if (!tickInProgress)
		{
			t.Invoke(); //if we are not in a tick, we can just invoke the action
			return;
		}

		postTickActions.Add(t);
	}

	public override void _Process(double delta)
	{
		//if (!playerMoveing)
		//{
			timeTillNextTick -= delta;
			TimeTillePlayerKill -= delta;
		//}

		if (timeTillNextTick <= 0)
		{
			tickInProgress = true;
			Tick();
			tickInProgress = false;
			timeTillNextTick = TimePerTick;
		}

		if (TimeTillePlayerKill <= 0) Player.Instance.Kill();


		base._Process(delta);
	}

	List<Action> thisTickActions = new();
	private void Tick()
	{
		thisTickActions.AddRange(nextTickActions);//this weird stuf is cause of concurrecny issues, we need to copy the list to avoid modifying it while iterating
		foreach (var act in thisTickActions)
		{
			nextTickActions.Remove(act);
			act.Invoke();
		}
		thisTickActions.Clear();
		foreach (var o in GetGlobalList()) o.Tick();
		foreach (var act in postTickActions) act.Invoke();
		postTickActions.Clear();
	}


	private Tile GetTile(int x, int y) => GetTile(new Vector2I(x, y));

	public Tile GetTile(Vector2I position)
	{
		//convert from tilemap coordinates to array coordinates
		int x = position.X + mapSize / 2; // Assuming the tilemap is centered at (0,0)
		int y = position.Y + mapSize / 2; // Adjust based on your tilemap's origin

		if (x < 0 || x >= tiles.GetLength(0) || y < 0 || y >= tiles.GetLength(1))
		{
			GD.PrintErr("Tile position out of bounds: " + position);
			return null;
		}

		return tiles[x, y];
	}


	public Vector2 MapToLocal(Vector2I playerTilePosition) => wallTileMap.MapToLocal(playerTilePosition);
	
	public void StartJump(TileObject o, Vector2I dir, int distance = 2)
	{
		if (o.Moving)
		{
			return;
		}
		GD.Print("StartJump called with direction: " + dir + " and distance: " + distance);
		o.Moving = true;
		DoNextTick(delegate { JumpToTile(o, dir, distance); });
	}

	public void JumpToTile(TileObject o, Vector2I dir, int distance)
	{
		GD.Print("JumpToTile called with direction: " + dir + " and distance: " + distance);
		//make sure dir its a normalized direction
		if (dir.X != 0)
			dir.X = dir.X > 0 ? 1 : -1;
		if (dir.Y != 0)
			dir.Y = dir.Y > 0 ? 1 : -1;
		//no diagonal jumps
		if (Math.Abs(dir.X) + Math.Abs(dir.Y) != 1)
		{
			GD.PrintErr("JumpToTile called with invalid direction: " + dir);
			return;
		}
		

		//move 1 distance in the given direction
		bool tempFloat = distance > 1; //no float on last jump
		bool res = Move(o, o.TilePostion + dir, tempFloat);
		GD.Print("Moved to: " + (o.TilePostion + dir) + " with result: " + res);
		if (res)
		{
			if (distance > 1)
			{
				GD.Print("Quued Jumped to tile: " + o.TilePostion + " in direction: " + dir + " with distance: " + distance);
				//we moved successfully, so we can jump to the next tile
				DoNextTick(delegate { JumpToTile(o, dir, distance - 1); });

			}
			else
			{
				GD.Print("done moving");
				o.Moving = false; //stop moving if we reached the last tile
			}
		}
		else
		{
			GD.Print("hit wall");
			//stop floating if we can't move
			Move(o, o.TilePostion, false);
			o.Moving = false;
		}



	}

	public bool Move(TileObject o, Vector2I pos, bool tempFloat = false)
	{
		var targetTile = GetTile(pos);
		if (targetTile == null)
		{
			GD.PrintErr("Target tile not found for position: " + pos);
			return false;
		}

		return Move(o, targetTile, tempFloat);
	}

	public bool Move(TileObject o, Tile target, bool tempFloat = false)
	{
		if (o.ParentTile != null) o.ParentTile.TileObjects.Remove(o);
		
		var tileData = wallTileMap.GetCellTileData(target.TilePosition);
		if (tileData != null)
		{
			if ((bool) tileData.GetCustomData("solid"))//can't move into solids
			{
				return false;
			}
		}
		
		target.Place(o, tempFloat);
		o.TempFloating = tempFloat;

		return true;
	}



	public void UpdateCameraPosition(float camRotation)
	{
		camraRotation = camRotation;
		foreach (var obj in GetGlobalList()) obj.UpdateCameraPosition(camRotation);
	}
}