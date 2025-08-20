using Godot;
using IntoTheRabbitHole.TileObjects;

namespace IntoTheRabbitHole.Traits;

public class Camo : Trait
{
	private readonly Sprite2D ownerSprite;

	public Camo(TileObject o) : base(o) => ownerSprite = o.GetNode<Sprite2D>("Sprite2D");


	public override ushort ExecutionPriority => 5;

	public override void ModifyAppearance(double deltaTime)
	{
		base.ModifyAppearance(deltaTime);
		float min = 0.1f;
		float max = 0.5f;

		float offset = (float) Mathf.Abs(Mathf.Sin(Time.GetTicksMsec() * 0.001) * 0.9f);

		ownerSprite.Modulate = new Color(1, 1, 1, Mathf.Pow(offset, 4)); // Adjust alpha based on position offset
	}
}