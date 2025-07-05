using System;
using System.Collections.Generic;
using Godot;
using IntoTheRabbitHole.Traits;

namespace IntoTheRabbitHole;

public static class Database
{
	static Dictionary<string, ObjectType> traits = new Dictionary<string, ObjectType>();

	public struct ObjectType
	{
		public Texture2D Texture;
		public List<Type> Traits;
	}

	

	static Database()
	{
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
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Hawk.png"),
			Traits = new List<Type> { typeof(Grabber), typeof(Float),typeof(Walk) }
		});

    
		traits.Add("Wall", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Wall.png"),
			Traits = new List<Type> {  }
		});
		
		traits.Add("Player", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Player.png"),
			Traits = new List<Type> { typeof(Camo)}
		});
		
		traits.Add("Fox", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Player.png"),
			Traits = new List<Type> { typeof(FollowPlayer), typeof(Attack) }
		});
		traits.Add("Carrot", new ObjectType { 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Carrot.png"),
			Traits = new List<Type> { typeof(Collectible) }
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
}