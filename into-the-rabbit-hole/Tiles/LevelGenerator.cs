using System;
using Godot;

namespace IntoTheRabbitHole.Tiles;


public partial class LevelGenerator : Node
{
	[Export] public TileManager TileManager { get; private set; }
	[Export] private NoiseTexture2D _noiseTexture;
	[Export] private int _seed;

	private Random random = new Random(Guid.NewGuid().GetHashCode());

	private int _mapSize = 100; // move this to database later
	
	private FastNoiseLite _noise;
	private Database.Level _level;
	

	public void Initialize()
	{
		_noise = _noiseTexture.Noise as FastNoiseLite;
		UpdateNoiseSeed(_seed*=TileManager.Instance.CurrentLevel+3);
		
		//init (bruv) tile array
		TileManager.InitializeTiles(_mapSize, _mapSize);
		
	}

	public void GenerateLevel()
	{
		_level = Database.GetLevel(TileManager.CurrentLevel);
		var objectcounter = 0;
		
		for (int x = 0; x < _mapSize; x++)
		{
			for (int y = 0; y < _mapSize; y++)
			{
				var val = _noise.GetNoise2D(x, y);

				if (_level.GroundTypes.Contains("Water") && _level.GroundTypes.Contains("Grass")) // change to use the level num later to decide what ground types there are
				{
				  GenerateTerrain(x,y,0.3,"Grass","Water");
				}

				if (x == 0 || y == 0 || x == _mapSize  || y == _mapSize ) // randomness to be added later - thicker walls and scattered the more you go inwards
				{
					GenerateWalls(x,y);
				}
				
				GenerateObjects(x,y,objectcounter++);
				


			}
		}

	}

	private void GenerateTerrain(int X, int Y, double GroundThreshhold, string WalkableGroundType, string NonWalkableGroundType)
{
	var t = TileManager.GetTile(new Vector2I(X,Y));
	if (t == null)
	{
		GD.PrintErr($"Cannot generate terrain at {X},{Y}: Tile is null");
		return;
	}

	if (_noise.GetNoise2D(X, Y) > GroundThreshhold)
	{
		t.GroundType = new GroundType(t, WalkableGroundType);
	}
	else
	{
		t.GroundType = new GroundType(t, NonWalkableGroundType);
	}
}

	private void GenerateWalls(int X, int Y)
	{
		var t = TileManager.GetTile(new Vector2I(X,Y));
		var wall = new TileObject(t, "Wall");
		t.Place(wall);
	}

	private void GenerateObjects(int X, int Y, int count)
	{
		var t = TileManager.GetTile(new Vector2I(X,Y));
		foreach (var tileObject in _level.TileObjects)
		{
			while (tileObject.Value > count)
			{
				if (random.Next(0, _mapSize) > _mapSize - 2)
				{
					t.Place(new TileObject(t, tileObject.Key));
				}
			}
		}
	}

	public void UpdateNoiseSeed(int newSeed)
	{
		_seed = newSeed;
		if (_noise != null)
		{
			_noise.Seed = _seed;
		}
	}

	public int Seed
	{
		get => _seed;
		set => UpdateNoiseSeed(value);
	}

}
