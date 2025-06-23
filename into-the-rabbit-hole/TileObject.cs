using Godot;
using System;
using IntoTheRabbitHole;

public partial class TileObject : Node2D
{
	public Tile ParentTile = null;
	public bool Solid = false;
	public bool WillKill = false;

	public Vector2I TilePostion => ParentTile.TilePosition;
	public TileManager TileManager => ParentTile.TileManager;
	public TileObject(Tile parentTile)
	{
		this.ParentTile = parentTile;
	}
	
	
}
