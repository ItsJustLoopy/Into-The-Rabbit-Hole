using System.Collections.Generic;
using Godot;
using IntoTheRabbitHole.Traits;

namespace IntoTheRabbitHole.Tiles;

public class Tile
{
	public readonly TileManager TileManager;
	private GroundType? _groundType;

	public GroundType? GroundType
	{
		get => _groundType;
		set
		{
			if (_groundType != null)
			{
				_groundType.QueueFree(); // Remove from scene tree
			}

			_groundType = value;
			if (_groundType != null)
			{
				_groundType.Position = TileManager.MapToLocal(TilePosition);
				TileManager.AddChild(_groundType);
			}
		}
	}

	private readonly List<TileObjects.TileObject> _safeList = new();
	public List<TileObjects.TileObject> TileObjects = new();
	public Vector2I TilePosition;

	public Tile(int x, int y, TileManager tmanager)
	{
		TilePosition = new Vector2I(x, y);
		TileManager = tmanager;
	}

	//floating bool is a special case for the player jumping over
	public void Place(TileObjects.TileObject tileObject, bool floating = false)
	{
		var fromDir = tileObject.TilePostion - TilePosition;
		//normalise into simple directions
		if (fromDir.X != 0)
			fromDir.X = fromDir.X > 0 ? 1 : -1;
		if (fromDir.Y != 0)
			fromDir.Y = fromDir.Y > 0 ? 1 : -1;

		tileObject.ParentTile = this;
		tileObject.GlobalPosition = TileManager.MapToLocal(TilePosition);

		if (floating || tileObject.HasTrait<Float>())
			FloatedOn(tileObject, fromDir);
		else
			SteppedOn(tileObject, fromDir);

		tileObject.TileEntered(this);
		TileObjects.Add(tileObject);
	}

	private List<TileObjects.TileObject> GetSafeList()
	{
		_safeList.Clear();
		_safeList.AddRange(TileObjects);
		return _safeList;
	}

	private void FloatedOn(TileObjects.TileObject tileObject, Vector2I fromDir)
	{
		foreach (var to in GetSafeList())
		{
			to.FloatedOn(tileObject, fromDir);

			//if we move into another floating object we "step" on it
			if (to.HasTrait<Float>()) to.SteppedOn(tileObject, fromDir);
		}
	}


	public void SteppedOn(TileObjects.TileObject o, Vector2I fromDir)
	{
		foreach (var to in GetSafeList())
		{
			to.SteppedOn(o, fromDir);
			if (to.HasTrait<Float>()) to.SteppedUnder(o, fromDir);
		}
	}
}
