using System;
using Godot;
using IntoTheRabbitHole.TileObjects;

namespace IntoTheRabbitHole.Tiles;

public partial class LevelGenerator : Node
{
	[Export] private float groundThreshold = 0.3f;

	// Level stuff
	private Database.Level level;

	public int MapSize;

	private FastNoiseLite noise;

	// Procedural generation stuff
	[Export] private NoiseTexture2D noiseTexture;
	private Random random;
	[Export] private int seed;
	private Database.TerrainConfig terrainConfig;

	// Terrain stuff
	private bool[,] terrainMap;
	[Export] public int WallSource, TerrainSetId;

	public int Seed
	{
		get => seed;
		set => UpdateNoiseSeed(value);
	}


	public void Initialize()
	{
		seed = new Random().Next(); // TODO allow input for seeded runs
		noise = noiseTexture.Noise as FastNoiseLite;
		random = new Random(seed);
		terrainMap = new bool[MapSize, MapSize];

		noise.Seed = seed;

		ConfigureNoise();
		GD.Print($"Generating level with seed: {seed}");


		level = Database.GetLevel(World.Instance.CurrentLevel);
		GD.Print($"Starting level generation for level {World.Instance.CurrentLevel}");
		GD.Print($"Level config: {level.TileObjects.Count} object types to place");


		terrainConfig = Database.GetTerrainConfigForLevel(World.Instance.CurrentLevel);
		GD.Print($"Terrain config: {terrainConfig.MainGround} main ground, {terrainConfig.Threshold} threshold");
	}

	private void ConfigureNoise()
	{
		if (noise == null) return;

		noise.FractalOctaves = 5; // Noise layers
		noise.Frequency = 0.3f; // How large the variation is (best to keep at low value)
		noise.FractalLacunarity = 2f; // Frequency increase in each consecutive octave
		noise.FractalGain = 0.5f; // How much each octave matters to the final result (each is half the last at 0.5)
	}

	public void GenerateLevel(ref TileMapLayer wallTileMap)
	{
		GenerateBaseTerrain(ref wallTileMap);
		SpawnPlayer();
		PopulateObjects();
	}

	private void GenerateBaseTerrain(ref TileMapLayer wallTileMap)
	{
		// Terrain determinant
		for (int x = 0; x < MapSize; x++)
		for (int y = 0; y < MapSize; y++)
		{
			float noiseValue = noise.GetNoise2D(x, y);
			terrainMap[x, y] = noiseValue > terrainConfig.Threshold;
		}

		// Apply cellular automata thingamabob
		for (int i = 0; i < 5; i++) terrainMap = ApplyCellularAutomata(terrainMap);

		// Ground and walls
		for (int x = 0; x < MapSize; x++)
		for (int y = 0; y < MapSize; y++)
		{
			var tile = GetTileManagerTileAt(x, y);

			tile.GroundType = new GroundType(tile, terrainConfig.MainGround);

			// Place walls where theres a false
			if (!terrainMap[x, y])
				wallTileMap.SetCell(
					tile.TilePosition,
					WallSource,
					Vector2I.Zero
				);
		}

		wallTileMap.SetCellsTerrainConnect(wallTileMap.GetUsedCells(), TerrainSetId, 1);
	}

	private void PopulateObjects()
	{
		if (level.TileObjects == null)
		{
			GD.PrintErr("No tile objects defined for this level !");
			return;
		}

		foreach (var objectType in level.TileObjects)
		{
			//GD.Print($"Attempting to place {objectType.Value} objects of type {objectType.Key}"); shhh
			int remainingObjects = objectType.Value;
			int attempts = 0;
			int maxAttempts = remainingObjects * 3;
			int placed = 0;

			while (remainingObjects > 0 && attempts < maxAttempts)
			{
				attempts++;
				int x = random.Next(MapSize);
				int y = random.Next(MapSize);

				var tile = GetTileManagerTileAt(x, y);

				if (tile != null && CanPlaceObjectAt(tile))
				{
					PlaceObjectAt(tile, objectType.Key);
					placed++;
					remainingObjects--;
				}
			}

			GD.Print($"Placed {placed} objects of type {objectType.Key} after {attempts} attempts");
		}
	}

	private void PlaceObjectAt(Tile tile, string objectType)
	{
		if (tile != null)
			new TileObject(tile, objectType);
		else
			GD.PrintErr($"Cannot place object at {tile.TilePosition} - tile is null");
	}


	private bool[,] ApplyCellularAutomata(bool[,] map)
	{
		bool[,] newMap = new bool[MapSize, MapSize];

		for (int x = 0; x < MapSize; x++)
		for (int y = 0; y < MapSize; y++)
		{
			int wallCount = CountWallNeighbors(map, x, y);

			// Thicker walls near edges
			int distanceFromEdge = Math.Min(
				Math.Min(x, MapSize - 1 - x),
				Math.Min(y, MapSize - 1 - y)
			);

			// Wall threshold is based on distance from edge
			int wallThreshold = distanceFromEdge < 5 ? 3 : 4;

			// If a cell has more than wallThreshold neighbors, it becomes a wall
			newMap[x, y] = wallCount < wallThreshold;

			// Force walls at the edges 
			if (x < MapSize / 5 || y < MapSize / 5 || x >= MapSize - MapSize / 5 || y >= MapSize - MapSize / 5) newMap[x, y] = false; // False = wall
		}

		return newMap;
	}

	private int CountWallNeighbors(bool[,] map, int x, int y)
	{
		int count = 0;
		for (int i = -1; i <= 1; i++)
		for (int j = -1; j <= 1; j++)
		{
			int nx = x + i;
			int ny = y + j;

			// Count outof bounds cells as walls
			if (nx < 0 || ny < 0 || nx >= MapSize || ny >= MapSize)
			{
				count++;
				continue;
			}

			if (!map[nx, ny]) // false = wall
				count++;
		}

		return count;
	}

	/* Do we need this?
	private void ClearWallVisuals()
	{
		WallTileMap.Clear();
	}

	public void RefreshWallVisuals()
	{
		ClearWallVisuals();

		for (int x = 0; x < MapSize; x++)
		{
			for (int y = 0; y < MapSize; y++)
			{
				var tile = GetTileManagerTileAt(x, y);
				if (tile.TileObjects.Exists(obj => obj.Type == "Wall"))
				{
					WallTileMap.SetCell(
						tile.TilePosition,
						WallSource,
						Vector2I.Zero
					);
				}
			}
		}
		WallTileMap.SetCellsTerrainConnect(WallTileMap.GetUsedCells(), TerrainSetId, 1);
	}
*/

	private Tile GetTileManagerTileAt(int x, int y)
	{
		var tile = World.Instance.GetTile(new Vector2I(x - MapSize / 2, y - MapSize / 2));
		return tile;
	}


	private bool CanPlaceObjectAt(Tile tile)
	{

		if (tile.TileObjects.Count > 0)
		{
			GD.Print($"Cannot place object: Tile already has objects at {tile.TilePosition}");
			return false;
		}
		
		var tileRealPos = tile.TilePosition + new Vector2I(MapSize / 2, MapSize / 2);
		if (terrainMap[tileRealPos.X, tileRealPos.Y] == false)
		{
			return false;
		}

		return true;
	}

	private void SpawnPlayer()
	{
		var playerSpawn = new Vector2I(random.Next(MapSize / 4, MapSize / 2), random.Next(MapSize / 4, MapSize / 2));
		var playerSpawnTile = GetTileManagerTileAt(playerSpawn.X, playerSpawn.Y);

		if (CanPlaceObjectAt(playerSpawnTile))
		{
			Player.Instance.SetPosition(playerSpawnTile.TilePosition); 
			Player.Instance.sprite.ZIndex = 1;
			GD.Print($"Player placed at tile: {playerSpawnTile.TilePosition}");
		}
		else
		{
			GD.PrintErr($"Could not place player at {playerSpawnTile.TilePosition}");
			SpawnPlayer();
		}
	}

	private void UpdateNoiseSeed(int newSeed)
	{
		seed = newSeed;
		if (noise != null) noise.Seed = seed;
	}

	private bool IsOdd(int num) => num % 2 == 1;
}