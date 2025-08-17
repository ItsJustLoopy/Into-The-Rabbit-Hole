using System;
using System.Collections.Generic;
using Godot;
using IntoTheRabbitHole.Tiles;
using IntoTheRabbitHole.Traits;

namespace IntoTheRabbitHole.TileObjects;

public partial class TileObject : Node2D
{
	public Tile ParentTile;
	public bool Solid = false;
	private Sprite2D _sprite = new();
	private readonly List<Trait> _traits = new();
	public string Type;

	public TileObject(Tile parentTile, string type)
	{
		ParentTile = parentTile;
		Type = type;

		var objDef = Database.GetObjectType(type);

		if (objDef.Texture != null)
		{
			_sprite.Texture = objDef.Texture;
			_sprite.Name = "Sprite2D";
			AddChild(_sprite);
		}
		
		foreach (var t in objDef.Traits) _traits.Add((Trait) Activator.CreateInstance(t, this));
		//sort by execution order
		_traits.Sort((traitA, traitB) => traitA.ExecutionPriority.CompareTo(traitB.ExecutionPriority));
		
		
		TileManager.Instance.AddChild(this);
		ParentTile.Place(this);
	}

	public Vector2I TilePostion => ParentTile.TilePosition; // mfw I try to access "TilePosition" and it doesn't exist because of a spelling error


	public override void _Process(double delta)
	{
		base._Process(delta);
		foreach (var t in _traits) t.ModifyAppearance(delta);
	}

	public bool HasTrait<T>() where T : Trait
	{
		foreach (var trait in _traits)
			if (trait is T)
				return true;

		return false;
	}


	public void Kill()
	{
		TileManager.Instance.DoAfterThisTick(() =>
		{
			ParentTile.TileObjects.Remove(this);
			QueueFree();
		});
	}


	public void SteppedOn(TileObject o, Vector2I fromDir)
	{
		foreach (var trait in _traits) trait.SteppedOn(o, fromDir);
	}

	public void TileEntered(Tile t)
	{
		foreach (var trait in _traits) trait.TileEntered(t);
	}


	public void Tick()
	{
		foreach (var trait in _traits) trait.Tick();
	}

	public void FloatedOn(TileObject tileObject, Vector2I fromDir)
	{
		foreach (var trait in _traits) trait.FloatedOn(tileObject, fromDir);
	}

	public void SteppedUnder(TileObject tileObject, Vector2I fromDir)
	{
		foreach (var trait in _traits) trait.SteppedUnder(tileObject, fromDir);
	}

	public void UpdateCameraPosition(float camRotation)
	{
		//roatate sprites to stay upright
		if (_sprite != null)
			_sprite.Rotation = camRotation; //OBJECT IS KILL WHY STILL TRACKED? - calm down bro
		else
			GD.PrintErr("TileObject has no sprite to update camera position.");
	}
	
	public Sprite2D GetSprite()
	{
		return _sprite;
	}

	
	
}
