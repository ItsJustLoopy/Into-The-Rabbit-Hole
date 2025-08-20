using Godot;
using IntoTheRabbitHole.TileObjects;

namespace IntoTheRabbitHole.Traits;

public class Trap : Trait
{
	private readonly Sprite2D ownerSprite;

	public Trap(TileObject o) : base(o)
	{
		ownerSprite = o.GetNode<Sprite2D>("Sprite2D");

		ownerSprite.Modulate = new Color(1, 0, 0, 0.5f); // Semi-transparent red
	}


	public override ushort ExecutionPriority => 5;

	public override void SteppedOn(TileObject tileObject, Vector2I fromDir)
	{
		tileObject.Kill();
	}

	public override void ModifyAppearance(double deltaTime)
	{
		float posOffset = (float) Mathf.Abs(Mathf.Sin(Time.GetTicksMsec() * 0.002) * 0.5f);

		ownerSprite.Modulate = new Color(1, 1 - posOffset, 0 - posOffset); // Adjust alpha based on position offset
	}
}