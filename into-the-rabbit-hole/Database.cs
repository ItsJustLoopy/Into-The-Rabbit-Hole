using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using IntoTheRabbitHole.Traits;

namespace IntoTheRabbitHole;

public static class Database
{
	private static readonly Dictionary<string, ObjectType> Traits = new();
	private static readonly Dictionary<int, Level> Levels = new();
	private static readonly Dictionary<string, TerrainConfig> TerrainConfigs = new();

	static Database()
	{
		Traits.Add("Trap", new ObjectType
		{
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Trap.png"),
			Traits = new List<Type> {typeof(Trap)}
		});

		Traits.Add("JumpTrap", new ObjectType
		{
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Trap.png"),
			Traits = new List<Type> {typeof(Grabber)}
		});

		Traits.Add("FloatingTrap", new ObjectType
		{
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Trap.png"),
			Traits = new List<Type> {typeof(Float), typeof(Trap)}
		});


		//tuah
		Traits.Add("Hawk", new ObjectType
		{
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Hawk.png"),
			Traits = new List<Type> {typeof(Grabber), typeof(Float), typeof(Walk)}
		});


		Traits.Add("Wall", new ObjectType
		{
			Traits = new List<Type>()
		});

		Traits.Add("Player", new ObjectType
		{
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Player.png"),
			Traits = new List<Type>()
		});

		Traits.Add("Fox", new ObjectType
		{
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Player.png"),
			Traits = new List<Type> {typeof(FollowPlayer), typeof(Attack)}
		});
		Traits.Add("Carrot", new ObjectType
		{
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Carrot.png"),
			Traits = new List<Type> {typeof(Collectible)}
		});

		//Levels
		Levels.Add(1, new Level
		{
			MapSize = 60,
			TileObjects = new Dictionary<string, int>
			{
				{"Trap", 10},
				{"Carrot", 25},
				{"Hawk", 15}
			}
		});


		//Terrains
		TerrainConfigs.Add("Forrest", new TerrainConfig
			{
				Levels = [1, 2, 3],
				MainGround = "Grass",
				SecondaryGround = "Grass",
				Threshold = -0.2f
			}
		);


		GD.Print("Forrest terrain config loaded");
	}


	public static ObjectType GetObjectType(string type)
	{
		if (!Traits.ContainsKey(type))
		{
			GD.PrintErr("Traits for type '" + type + "' not found in database.");
			return new ObjectType();
		}

		return Traits[type];
	}

	public static Level GetLevel(int num)
	{
		foreach (var level in Levels)
			if (level.Key == num)
				return level.Value;
		return new Level();
	}


	public static TerrainConfig GetTerrainConfigForLevel(int lvl)
	{
		foreach (var config in TerrainConfigs)
			if (config.Value.Levels.Contains(lvl))
			{
				GD.Print($"Terrain config found for level {lvl}: {config.Key}");
				return config.Value;
			}

		GD.PrintErr($"Level {lvl} not found in any terrain config - returning forrest");
		return TerrainConfigs["Forrest"]; // Return default config instead of null
	}

	public struct ObjectType
	{
		public Texture2D? Texture;
		public List<Type> Traits;
	}

	public struct Level
	{
		public int MapSize;
		public Dictionary<string, int> TileObjects;
	}

	public struct TerrainConfig
	{
		public int[] Levels { get; set; }
		public string MainGround { get; set; }
		public string SecondaryGround { get; set; }
		public float Threshold { get; set; }
	}
}