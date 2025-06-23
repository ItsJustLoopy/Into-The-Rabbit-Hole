using System.Collections.Generic;
using Godot;

namespace IntoTheRabbitHole;

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
		GroundType = new GroundType();
		TileManager = tmanager;
	}



	public void Place(TileObject tileObject)
	{
		tileObject.ParentTile = this;
		TileObjects.Add(tileObject);
		tileObject.GlobalPosition = TileManager.MapToLocal(TilePosition);
	}
}