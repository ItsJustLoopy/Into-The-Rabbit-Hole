using System;
using System.Collections.Generic;
using Godot;

namespace IntoTheRabbitHole.Tiles;

public partial class Tile
{
	public List<TileObject> TileObjects = new List<TileObject>();
	public GroundType GroundType;
	public bool CanStandOn => GroundType?.canStand ?? true && TileObjects.TrueForAll(obj => !obj.Solid);
	public Vector2I TilePosition;
	public readonly TileManager TileManager;

	public Tile(int x, int y,TileManager tmanager)
	{
		TilePosition = new Vector2I(x, y);
		TileManager = tmanager;
		GroundType = new GroundType(this,"Grass");
		tmanager.AddChild(GroundType);
	}
	
	public void Place(TileObject tileObject)
	{
		Vector2I fromDir = tileObject.TilePostion - TilePosition;
		//normalise into simple directions
		if (fromDir.X != 0)
			fromDir.X = fromDir.X > 0 ? 1 : -1;
		if (fromDir.Y != 0)
			fromDir.Y = fromDir.Y > 0 ? 1 : -1;
		
		tileObject.ParentTile = this;
		tileObject.GlobalPosition = TileManager.MapToLocal(TilePosition);
		
		SteppedOn(tileObject,fromDir);
		tileObject.TileEntered(this);
		TileObjects.Add(tileObject);
	}
	
	
	public void SteppedOn(TileObject o, Vector2I fromDir)
	{
		foreach (var to in TileObjects)
		{
			to.SteppedOn(o,fromDir);
		}
	}

}