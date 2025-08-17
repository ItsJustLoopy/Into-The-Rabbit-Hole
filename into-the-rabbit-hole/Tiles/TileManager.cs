using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace IntoTheRabbitHole.Tiles;

public partial class TileManager : Node
{
	public static TileManager Instance;
	[Export] private LevelGenerator _levelGenerator;
	private TileMapLayer _tileMap;
	private TileMapLayer _occlusionLayer;
	private Tile[,] _tiles;
	public int CurrentLevel = 1;
	private int MapSize;

	private float _camraRotation;
	
	private readonly List<TileObjects.TileObject> _globalList = new();

	private readonly List<Action> _postTickActions = new();
	private readonly List<Action> _nextTickActions = new();


	private bool _playeMoveCompleteFlag;
	private bool _playerMoveing;

	private bool _tickInProgress;
	private double _timeTillNextTick = 1;

	public double TimeTillePlayerKill = 100f;

	public override void _Ready()
	{
		Instance = this;
		_tileMap = GetNode<TileMapLayer>("GroundLayer");
		MapSize = Database.GetLevel(CurrentLevel).MapSize;
		_levelGenerator.MapSize = MapSize;
		
		if (_tileMap == null)
		{
			GD.PrintErr("TileMapLayer node not found in TileManager.");
			return;
		}

		// Initialize or load tiles here if needed
		_tiles = new Tile[MapSize, MapSize];

		// Populate the tile array with Tile objects
		for (int x = 0; x < _tiles.GetLength(0); x++)
		for (int y = 0; y < _tiles.GetLength(1); y++)
			_tiles[x, y] = new Tile(x - (MapSize/2), y - (MapSize/2), this); // Adjusting for center origin
		
		_levelGenerator.Initialize();
		_levelGenerator.GenerateLevel();

	}

	private List<TileObjects.TileObject> GetGlobalList()
	{
		_globalList.Clear();
		//we collect all objects and update them, "updating tiles" will cause moving objects to get ticked more than once
		foreach (var t in _tiles) _globalList.AddRange(t.TileObjects);
		return _globalList;
	}

	public void DoNextTick(Action t)
	{
		_nextTickActions.Add(t);
	}

	public void DoAfterThisTick(Action t)
	{
		if (!_tickInProgress)
		{
			t.Invoke(); //if we are not in a tick, we can just invoke the action
			return;
		}

		_postTickActions.Add(t);
	}

	public override void _Process(double delta)
	{
		if (!_playerMoveing)
		{
			_timeTillNextTick -= delta;
			TimeTillePlayerKill -= delta;
		}
			
		if (_timeTillNextTick <= 0)
		{
			_tickInProgress = true;
			Tick();
			_tickInProgress = false;
			_timeTillNextTick = 1f;
		}

		if (TimeTillePlayerKill <= 0)
		{
			Player.Instance.Kill();
		}
		
		
		base._Process(delta);
	}

	private void Tick()
	{
		foreach (var act in _nextTickActions) act.Invoke();
		_nextTickActions.Clear();
		foreach (var o in GetGlobalList()) o.Tick();
		foreach (var act in _postTickActions) act.Invoke();
		_postTickActions.Clear();
	}


	private Tile GetTile(int x, int y)
	{
		return GetTile(new Vector2I(x, y));
	}

	public Tile GetTile(Vector2I position)
	{
		//convert from tilemap coordinates to array coordinates
		int x = position.X + (MapSize/2); // Assuming the tilemap is centered at (0,0)
		int y = position.Y + (MapSize/2); // Adjust based on your tilemap's origin

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

	public void Move(TileObjects.TileObject o, Vector2I pos, bool tempFloat = false)
	{
		var targetTile = GetTile(pos);
		if (targetTile == null)
		{
			GD.PrintErr("Target tile not found for position: " + pos);
			return;
		}

		Move(o, targetTile, tempFloat);
	}

	private void Move(TileObjects.TileObject o, Tile target, bool tempFloat = false)
	{
		if (o.ParentTile != null) o.ParentTile.TileObjects.Remove(o);
		target.Place(o, tempFloat);

		//this is a bit of sphagetti, hopefully eventually it will be smoothed out
		if (_playerMoveing)
		{
			GD.Print(o.TilePostion + " player move");
			_playeMoveCompleteFlag = true;
		}
	}

	public void PlayerMove(TileObjects.TileObject o, Vector2I dir)
	{
		//sanity check
		if (o.GetType() != typeof(Player))
		{
			GD.PrintErr("PlayerMove called on non-player object: " + o.GetType());
			return;
		}
		if(_playerMoveing) return;

		var player = (Player) o;
		//do this in a separate thread to avoid blocking the main thread
		Task.Run(() =>
		{
			_playerMoveing = true; //pause the simulation

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
					_playeMoveCompleteFlag = false;

					CallDeferred(MethodName.Move, player, player.TilePostion + dir, true);

					//TODO STEP BY STEP TRAIT ACTIVIATION DISPLAY
					Thread.Sleep(150);

					while (!_playeMoveCompleteFlag)
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

			_playeMoveCompleteFlag = false;
			// Actually move the player in the tile system
			CallDeferred(MethodName.Move, player, player.TilePostion + dir, false);
			//TODO STEP BY STEP TRAIT ACTIVIATION DISPLAY
			while (!_playeMoveCompleteFlag)
				// Wait until move is processed
				Thread.Sleep(16);

			_playerMoveing = false;
		});
	}


	public void UpdateCameraPosition(float camRotation)
	{
		_camraRotation = camRotation;
		foreach (var obj in GetGlobalList()) obj.UpdateCameraPosition(camRotation);
	}
}
