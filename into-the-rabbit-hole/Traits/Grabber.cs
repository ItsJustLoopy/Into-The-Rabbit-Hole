using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class Grabber : Trait
{
	
	Sprite2D ownerSprite;
	public Grabber(TileObject o) : base(o)
	{
		ownerSprite = o.GetNode<Sprite2D>("Sprite2D");

		ownerSprite.Modulate = new Color(1, 0, 0, 0.5f); // Semi-transparent red
		
	}

	public override ushort ExecutionPriority => 4;

	public override void FloatedOn(TileObject tileObject, Vector2I fromDir)
	{
		tileObject.Kill();
	}

	public override void SteppedUnder(TileObject tileObject, Vector2I fromDir)
	{
		tileObject.Kill();
	}

	
	public override void ModifyAppearance(double deltaTime)
	{
		float posOffset = (float)Mathf.Abs(Mathf.Sin(Time.GetTicksMsec()*0.001)*0.5f);
		
			ownerSprite.Modulate = new Color(1, 1- posOffset, 0-posOffset, 1); // Adjust alpha based on position offset

	}
}