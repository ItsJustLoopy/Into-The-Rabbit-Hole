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
		public string Name;
		public Texture2D Texture;
		public List<Type> Traits;
	}

	

	static Database()
	{
		traits.Add("Trap", new ObjectType { 
			Name = "Trap", 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Trap.png"),
			Traits = new List<Type> { typeof(Trap) }
		});
    
	//	traits.Add("Wall", new ObjectType { 
	//		Name = "Wall", 
	//		Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Wall.png"),
	//		Traits = new List<Type> { typeof(Solid) }
	//	});
		
		traits.Add("Player", new ObjectType { 
			Name = "Player", 
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Player.png"),
			Traits = new List<Type> { typeof(Camera) }
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