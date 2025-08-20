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


	private bool playeMoveCompleteFlag;
	private bool playerMoveing;

	private bool tickInProgress;
	private TileMapLayer tileMap;
	private Tile[,] tiles;

	public double TimeTillePlayerKill = 100f;
	private double timeTillNextTick = 1;
	private TileMapLayer wallTileMap;

	public override void _Ready()
	{
		Instance = this;
		tileMap = GetNode<TileMapLayer>("GroundLayer"); //do we need this?
		wallTileMap = GetNode<TileMapLayer>("WallLayer");
		levelGenerator = GetNode<LevelGenerator>("LevelGenerator");

		mapSize = Database.GetLevel(CurrentLevel).MapSize;
		levelGenerator.MapSize = mapSize;

		if (tileMap == null)
		{
			GD.PrintErr("TileMapLayer node not found in TileManager.");
			return;
		}


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
		if (!playerMoveing)
		{
			timeTillNextTick -= delta;
			TimeTillePlayerKill -= delta;
		}

		if (timeTillNextTick <= 0)
		{
			tickInProgress = true;
			Tick();
			tickInProgress = false;
			timeTillNextTick = 1f;
		}

		if (TimeTillePlayerKill <= 0) Player.Instance.Kill();


		base._Process(delta);
	}

	private void Tick()
	{
		foreach (var act in nextTickActions) act.Invoke();
		nextTickActions.Clear();
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


	public Vector2 MapToLocal(Vector2I playerTilePosition) => tileMap.MapToLocal(playerTilePosition);

	public void Move(TileObject o, Vector2I pos, bool tempFloat = false)
	{
		var targetTile = GetTile(pos);
		if (targetTile == null)
		{
			GD.PrintErr("Target tile not found for position: " + pos);
			return;
		}

		Move(o, targetTile, tempFloat);
	}

	private void Move(TileObject o, Tile target, bool tempFloat = false)
	{
		if (o.ParentTile != null) o.ParentTile.TileObjects.Remove(o);
		target.Place(o, tempFloat);

		//this is a bit of sphagetti, hopefully eventually it will be smoothed out
		if (playerMoveing)
		{
			GD.Print(o.TilePostion + " player move");
			playeMoveCompleteFlag = true;
		}
	}

	public void PlayerMove(TileObject o, Vector2I dir)
	{
		//sanity check
		if (o.GetType() != typeof(Player))
		{
			GD.PrintErr("PlayerMove called on non-player object: " + o.GetType());
			return;
		}

		if (playerMoveing) return;

		var player = (Player) o;
		//do this in a separate thread to avoid blocking the main thread
		Task.Run(() =>
		{
			playerMoveing = true; //pause the simulation

			// Animation parameters
			float animationDuration = 0.5f; // Half second animation
			float elapsedTime = 0f;
			var startPosition = MapToLocal(player.TilePostion);
			var jumpTilePos = MapToLocal(player.TilePostion + dir);
			var endPosition = MapToLocal(player.TilePostion + dir * 2);

			bool movedToJumpTile = false;

			// Animation loop
			while (elapsedTime < animationDuration)
			{
				float t = elapsedTime / animationDuration;

				// Pause at midpoint for evaluation
				if (t >= 0.5f && !movedToJumpTile)
				{
					// Use a flag to track when move is complete
					playeMoveCompleteFlag = false;

					CallDeferred(MethodName.Move, player, player.TilePostion + dir, true);

					//TODO STEP BY STEP TRAIT ACTIVIATION DISPLAY
					Thread.Sleep(150);

					while (!playeMoveCompleteFlag)
						// Wait until move is processed
						Thread.Sleep(16);
					movedToJumpTile = true;
				}

				// 3-point bezier curve: start -> jump tile -> move tile

				var p0 = startPosition;
				var p1 = jumpTilePos;
				var p2 = endPosition;

				// Quadratic bezier interpolation
				var currentPos = p0 * (1 - t) * (1 - t) + p1 * 2 * (1 - t) * t + p2 * t * t;

				// Scale effect - player gets bigger during jump
				float scaleMultiplier = 1.0f + Mathf.Sin(t * Mathf.Pi) * 0.3f;

				// Update position and scale
				player.CallDeferred(Node2D.MethodName.SetPosition, currentPos);
				player.CallDeferred(Node2D.MethodName.SetScale, Vector2.One * scaleMultiplier);

				elapsedTime += 0.016f; // ~60 FPS
				Thread.Sleep(16); // 16ms delay
			}

			playeMoveCompleteFlag = false;
			// Actually move the player in the tile system
			CallDeferred(MethodName.Move, player, player.TilePostion + dir, false);
			//TODO STEP BY STEP TRAIT ACTIVIATION DISPLAY
			while (!playeMoveCompleteFlag)
				// Wait until move is processed
				Thread.Sleep(16);

			playerMoveing = false;

			//print tilemap position
			var tileData = wallTileMap.GetCellTileData(player.TilePostion);
			if (tileData != null) GD.Print($"Player moved to: {player.TilePostion} with data: {tileData.GetCustomData("solid")}");
		});
	}


	public void UpdateCameraPosition(float camRotation)
	{
		camraRotation = camRotation;
		foreach (var obj in GetGlobalList()) obj.UpdateCameraPosition(camRotation);
	}
}