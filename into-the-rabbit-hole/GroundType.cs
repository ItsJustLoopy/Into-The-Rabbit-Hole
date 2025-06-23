using Godot;

namespace IntoTheRabbitHole;

public partial class GroundType : Node2D
{
	public Tile ParentTile = null;
	public bool canStand = true;

	public GroundType(Tile parentTile, string gType)
	{
		ParentTile = parentTile;
		Position = ParentTile.TileManager.MapToLocal(ParentTile.TilePosition);
		var component = GD.Load<PackedScene>("res://Ground/"+gType+".tscn").Instantiate();
		AddChild(component);
	}
	
}