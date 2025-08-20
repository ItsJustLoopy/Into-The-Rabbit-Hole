using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole;

public partial class GroundType : Node2D
{
	public bool IsWalkable = true;
	public Tile ParentTile;


	public GroundType(Tile parentTile, string gType)
	{
		ParentTile = parentTile;
		Position = ParentTile.World.MapToLocal(ParentTile.TilePosition);

		var component = GD.Load<PackedScene>("res://Ground/" + gType + ".tscn").Instantiate();
		AddChild(component);
	}
}