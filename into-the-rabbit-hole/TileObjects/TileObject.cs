using Godot;
using System;
using System.Collections.Generic;
using IntoTheRabbitHole;
using IntoTheRabbitHole.Tiles;

public partial class TileObject : Node2D
{
	public Tile ParentTile = null;
	public bool Solid = false;
	private List<Trait> traits = new List<Trait>();
	public Vector2I TilePostion => ParentTile.TilePosition;
	Sprite2D sprite = null;//coca cola acxaxasxxcdxdxdxdxd

	public TileObject(Tile parentTile, string type)
	{
		ParentTile = parentTile;
		var objDef = Database.GetObjectType(type);
		
		sprite = new Sprite2D();
		sprite.Texture = objDef.Texture;
		sprite.Name = "Sprite2D";
		AddChild(sprite);
		
		
		
		foreach (var t in objDef.Traits)
		{
			traits.Add((Trait)Activator.CreateInstance(t, this));
		}
		//sort by execution order
		traits.Sort(((traitA, traitB) => traitA.ExecutionPriority.CompareTo(traitB.ExecutionPriority)));

		TileManager.Instance.AddChild(this);
		
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
		foreach (var t in traits)
		{
			t.ModifyAppearance(delta);
		}
	}

	public bool HasTrait<T>() where T : Trait
	{
		foreach (var trait in traits)
		{
			if (trait is T)
			{
				return true;
			}
		}
		return false;
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
	
	
	public void SteppedOn(TileObject o, Vector2I fromDir)
	{
		foreach (var trait in traits)
		{
			trait.SteppedOn(o, fromDir);
		}
	}

	public void TileEntered(Tile t)
	{
		foreach (var trait in traits)
		{
			trait.TileEntered(t);
		}
	}


	public void Tick()
	{
		foreach (var trait in traits)
		{
			trait.Tick();
		}
	}

	public void FloatedOn(TileObject tileObject, Vector2I fromDir)
	{
		foreach (var trait in traits)
		{
			trait.FloatedOn(tileObject,fromDir);
		}
	}

	public void SteppedUnder(TileObject tileObject, Vector2I fromDir)
	{
		foreach (var trait in traits)
		{
			trait.SteppedUnder(tileObject,fromDir);
		}
	}

	public void UpdateCameraPosition(float camRotation)
	{
		//roatate sprites to stay upright
		if (sprite != null)
		{
			sprite.Rotation = camRotation;
		}
		else
		{
			GD.PrintErr("TileObject has no sprite to update camera position.");
		}
	}
}
