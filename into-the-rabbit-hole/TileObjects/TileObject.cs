using Godot;
using System;
using System.Collections.Generic;
using IntoTheRabbitHole;
using IntoTheRabbitHole.TileManager;

public partial class TileObject : Node2D
{
	public Tile ParentTile = null;
	public bool Solid = false;
	private List<Trait> traits = new List<Trait>();

	public Vector2I TilePostion => ParentTile.TilePosition;
	public TileManager TileManager => ParentTile.TileManager;
	public TileObject(Tile parentTile, string type)
	{
		ParentTile = parentTile;
		
		var objDef = Database.GetObjectType(type);
		foreach (var t in objDef.Traits)
		{
			traits.Add((Trait)Activator.CreateInstance(t, this));
		}
		//sort by execution order
		traits.Sort(((traitA, traitB) => traitA.ExecutionPriority.CompareTo(traitB.ExecutionPriority)));
		Sprite2D sprite = new Sprite2D();
		sprite.Texture = objDef.Texture;
		AddChild(sprite);
		
		
	}


	public void Kill()
	{
		if (ParentTile != null)
		{
			ParentTile.TileObjects.Remove(this);
			ParentTile = null;
		}
		else
		{
			GD.PrintErr("TileObject has no parent tile to remove from.");
		}
		QueueFree();
	}
	
	
	public void StepOn(TileObject o, Vector2I fromDir)
	{
		foreach (var trait in traits)
		{
			trait.StepOn(o, fromDir);
		}
	}
	
	
	
	
	
}
