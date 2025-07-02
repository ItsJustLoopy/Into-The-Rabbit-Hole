using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class Float : Trait
{



	public Float(TileObject o) : base(o)
	{
	}

	public override ushort ExecutionPriority => 5;

	public override void TileEntered(Tile t)
	{
		base.TileEntered(t);
	}
	
	public override void ModifyAppearance(double delta)
	{


		float posOffset = (float) Mathf.Sin(Time.GetTicksMsec()*0.005)*0.1f;

		// Apply the offset to the object's position
		_owner.Position = _owner.Position with {Y = _owner.Position.Y - posOffset};
	}

}