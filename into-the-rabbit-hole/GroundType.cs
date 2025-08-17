using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole;

public partial class GroundType : Node2D
{
	public Tile ParentTile;
	public bool IsWalkable = true;

	
	public GroundType(Tile parentTile, string gType)
	{
		ParentTile = parentTile;
		Position = ParentTile.TileManager.MapToLocal(ParentTile.TilePosition);

		var component = GD.Load<PackedScene>("res://Ground/" + gType + ".tscn").Instantiate();
		AddChild(component);
	}
}