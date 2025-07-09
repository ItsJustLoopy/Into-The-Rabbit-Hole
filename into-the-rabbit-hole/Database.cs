using System;
using System.Collections.Generic;
using Godot;
using IntoTheRabbitHole.Traits;

namespace IntoTheRabbitHole;

public static class Database
{
	private static readonly Dictionary<string, ObjectType> Traits = new();


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
			Texture = GD.Load<Texture2D>("res://Assets/Textures/TileObjects/Wall.png"),
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

	public struct ObjectType
	{
		public Texture2D Texture;
		public List<Type> Traits;
	}
}