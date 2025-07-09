using Godot;

namespace IntoTheRabbitHole.Traits;

public class Grabber : Trait
{
	private readonly Sprite2D _ownerSprite;

	public Grabber(TileObject o) : base(o)
	{
		_ownerSprite = o.GetNode<Sprite2D>("Sprite2D");

		_ownerSprite.Modulate = new Color(1, 0, 0, 0.5f); // Semi-transparent red
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
		float posOffset = (float) Mathf.Abs(Mathf.Sin(Time.GetTicksMsec() * 0.001) * 0.5f);

		_ownerSprite.Modulate = new Color(1, 1 - posOffset, 0 - posOffset); // Adjust alpha based on position offset
	}
}