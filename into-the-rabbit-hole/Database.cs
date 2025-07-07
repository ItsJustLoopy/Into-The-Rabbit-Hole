using System;
using System.Collections.Generic;
using Godot;
using IntoTheRabbitHole.Traits;

namespace IntoTheRabbitHole;

public static class Database
{
	static Dictionary<string, ObjectType> traits = new Dictionary<string, ObjectType>();
	static Dictionary<int, Level> levels = new Dictionary<int, Level>();

	public struct ObjectType
	{
		public Texture2D Texture;
		public List<Type> Traits;
	}

	public struct Level
	{
		public List<string> GroundTypes;
		public Dictionary<string,int> TileObjects;
	}

	

	static Database()
	{
		//Traits
		traits.Add("Trap", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Trap.png"),
			Traits = new List<Type> { typeof(Trap) }
		});
		
		traits.Add("JumpTrap", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Trap.png"),
			Traits = new List<Type> { typeof(Grabber) }
		});
		
		traits.Add("FloatingTrap", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Trap.png"),
			Traits = new List<Type> { typeof(Float),typeof(Trap) }
		});
		

		//tuah 
		traits.Add("Hawk", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Trap.png"),
			Traits = new List<Type> { typeof(Grabber), typeof(Float) }
		});

    
		traits.Add("Wall", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Wall.png"),
			Traits = new List<Type> {  }
		});
		
		traits.Add("Player", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Player.png"),
			Traits = new List<Type> {typeof(Float) }
		});
		
		traits.Add("Fox", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Player.png"),
			Traits = new List<Type> { typeof(FollowPlayer), typeof(Attack) }
		});
		traits.Add("Carrot", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Carrot.png"),
			Traits = new List<Type> { typeof(Collectible) }
		});
		
		//Levels
		levels.Add(1, new Level
		{
			GroundTypes = ["Grass", "Water"],
			TileObjects = new Dictionary<string, int>
			{
				{"Trap", 10},
				{"Carrot", 15}
			}
		});

	}

	
	
	public static ObjectType GetObjectType(string type)
	{
		if (!traits.ContainsKey(type))
		{
			GD.PrintErr("Traits for type '" + type + "' not found in database.");
			return new ObjectType();
		}
		return traits[type];
	}

	public static Level GetLevel(int num)
	{
		return levels[num];
	}
}