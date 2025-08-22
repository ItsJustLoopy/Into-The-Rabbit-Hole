using System.Collections.Generic;
using Godot;
using IntoTheRabbitHole.TileObjects;
using IntoTheRabbitHole.Traits;

namespace IntoTheRabbitHole.Tiles;

public class Tile
{
	private readonly List<TileObject> safeList = new();
	public readonly World World;
	private GroundType? groundType;
	public List<TileObject> TileObjects = new();
	public Vector2I TilePosition;

	public Tile(int x, int y, World tmanager)
	{
		TilePosition = new Vector2I(x, y);
		World = tmanager;
	}

	public GroundType? GroundType
	{
		get => groundType;
		set
		{
			if (groundType != null) groundType.QueueFree(); // Remove from scene tree

			groundType = value;
			if (groundType != null)
			{
				groundType.Position = World.MapToLocal(TilePosition);
				World.AddChild(groundType);
			}
		}
	}

	//floating bool is a special case for the player jumping over
	public void Place(TileObject tileObject, bool floating = false)
	{
		var fromDir = tileObject.TilePostion - TilePosition;
		//normalise into simple directions
		if (fromDir.X != 0)
			fromDir.X = fromDir.X > 0 ? 1 : -1;
		if (fromDir.Y != 0)
			fromDir.Y = fromDir.Y > 0 ? 1 : -1;

		tileObject.ParentTile = this;
		if (floating || tileObject.HasTrait<Float>())
			FloatedOn(tileObject, fromDir);
		else
			SteppedOn(tileObject, fromDir);

		tileObject.TileEntered(this);
		TileObjects.Add(tileObject);
	}

	private List<TileObject> GetSafeList()
	{
		safeList.Clear();
		safeList.AddRange(TileObjects);
		return safeList;
	}

	private void FloatedOn(TileObject tileObject, Vector2I fromDir)
	{
		foreach (var to in GetSafeList())
		{
			to.FloatedOn(tileObject, fromDir);

			//if we move into another floating object we "step" on it
			if (to.HasTrait<Float>()) to.SteppedOn(tileObject, fromDir);
		}
	}


	public void SteppedOn(TileObject o, Vector2I fromDir)
	{
		foreach (var to in GetSafeList())
		{
			to.SteppedOn(o, fromDir);
			if (to.HasTrait<Float>()) to.SteppedUnder(o, fromDir);
		}
	}
}