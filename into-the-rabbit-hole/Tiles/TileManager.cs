using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace IntoTheRabbitHole.Tiles;

public partial class TileManager : Node
{
	private TileMapLayer _tileMap;
	private Tile[,] _tiles;

	public static TileManager Instance;

	public override void _Ready()
	{
		Instance = this;
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
		
		

		for (int i = 0; i < 15; i+=3)
		{
			var trapTile = GetTile(new Vector2I(i, 8));
			if (trapTile != null)
			{
				new TileObject(trapTile, "Trap");

			}
		}
		
		for (int i = 0; i < 15; i+=3)
		{
			var trapTile = GetTile(new Vector2I(i, 12));
			if (trapTile != null)
			{
				new TileObject(trapTile, "JumpTrap");
			}
		}
		
		//make some collectibles
		
		for (int i = 0; i < 15; i+=3)
		{
			var collectTile = GetTile(new Vector2I(i, 4));
			if (collectTile != null)
			{
				new TileObject(collectTile, "Carrot");

			}
		}

		var randomTile = GetTile(new Vector2I(GD.RandRange(-5,5), GD.RandRange(-5,5)));
		new TileObject(randomTile,"Hawk");
		randomTile = GetTile(new Vector2I(GD.RandRange(-5,5), GD.RandRange(-5,5)));
		new TileObject(randomTile,"Hawk");
		randomTile = GetTile(new Vector2I(GD.RandRange(-5,5), GD.RandRange(-5,5)));
		new TileObject(randomTile,"Hawk");

	}


	private double timeTillNextTick = 1;
	private List<TileObject> GlobalList = new List<TileObject>();
	
	private List<TileObject> GetGlobalList()
	{
		GlobalList.Clear();
		//we collect all objects and update them, "updating tiles" will cause moving objects to get ticked more than once
		foreach (var t in _tiles)
		{
			GlobalList.AddRange(t.TileObjects);
		}
		return GlobalList;
	}
	
	
	List<Action> NextTickActions = new List<Action>();
	List<Action> PostTickActions = new List<Action>();

	public void DoNextTick(Action t)
	{
		NextTickActions.Add(t);
	}
	bool TickInProgress = false;
	bool PlayerMoveing = false;
	public void DoAfterThisTick(Action t)
	{
		if (!TickInProgress)
		{
			t.Invoke();//if we are not in a tick, we can just invoke the action
			return;
		}
		PostTickActions.Add(t);
	}
	public override void _Process(double delta)
	{
		if(!PlayerMoveing)//game is paused during player movement
			timeTillNextTick -= delta;
		if (timeTillNextTick <= 0)
		{
			TickInProgress = true;
			Tick();
			TickInProgress = false;
			timeTillNextTick = 1f;
		}
		base._Process(delta);
		
	}

	private void Tick()
	{
		
		foreach (var act in NextTickActions)
		{
			act.Invoke();
		}
		NextTickActions.Clear();
		foreach (var o in GetGlobalList())
		{
			o.Tick();
		}
		foreach (var act in PostTickActions)
		{
			act.Invoke();
		}
		PostTickActions.Clear();
		
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


	bool playeMoveCompleteFlag = false;
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
		if (o.ParentTile != null)
		{
			o.ParentTile.TileObjects.Remove(o);
		}
		target.Place(o,tempFloat);
		
		//this is a bit of sphagetti, hopefully eventually it will be smoothed out
		if (PlayerMoveing)
		{
			GD.Print(o.TilePostion + " player move");
			playeMoveCompleteFlag = true;
		}
	}

	public void PlayerMove(TileObject o, Vector2I dir)
{
    //sanity check
    if(o.GetType() != typeof(Player))
    {
        GD.PrintErr("PlayerMove called on non-player object: " + o.GetType());
        return;
    }

    var player = (Player)o;
    //do this in a separate thread to avoid blocking the main thread
    Task.Run(() =>
    {
        PlayerMoveing = true;//pause the simulation

        // Animation parameters
        float animationDuration = 0.5f; // Half second animation
        float elapsedTime = 0f;
        Vector2 startPosition = MapToLocal(player.TilePostion);
        Vector2 jumpTilePos = MapToLocal(player.TilePostion + dir);
        Vector2 endPosition = MapToLocal(player.TilePostion + dir * 2);

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
                { // Wait until move is processed
                    System.Threading.Thread.Sleep(16);
                }
                movedToJumpTile = true;
            }

            // 3-point bezier curve: start -> jump tile -> move tile
           
            Vector2 p0 = startPosition;
            Vector2 p1 = jumpTilePos;
            Vector2 p2 = endPosition;

            // Quadratic bezier interpolation
            Vector2 currentPos = p0 * (1 - t) * (1 - t) + p1 * 2 * (1 - t) * t + p2 * t * t;

            // Scale effect - player gets bigger during jump
            float scaleMultiplier = 1.0f + (Mathf.Sin(t * Mathf.Pi) * 0.3f);

            // Update position and scale
            player.CallDeferred(Node2D.MethodName.SetPosition, currentPos);
            player.CallDeferred(Node2D.MethodName.SetScale, Vector2.One * scaleMultiplier);

            elapsedTime += 0.016f; // ~60 FPS
            System.Threading.Thread.Sleep(16); // 16ms delay
        }

        playeMoveCompleteFlag = false;
        // Actually move the player in the tile system
        CallDeferred(MethodName.Move, player, player.TilePostion + dir, false);
        //TODO STEP BY STEP TRAIT ACTIVIATION DISPLAY
        while (!playeMoveCompleteFlag)
        { // Wait until move is processed
	        System.Threading.Thread.Sleep(16);
        }
    
        PlayerMoveing = false;
    });
}

	

	float CamraRotation = 0f;
	public void UpdateCameraPosition(float camRotation)
	{
		CamraRotation = camRotation;
		foreach (var obj in GetGlobalList())
		{
			obj.UpdateCameraPosition(camRotation);
		}
	}
}