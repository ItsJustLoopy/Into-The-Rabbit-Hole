using Godot;
using IntoTheRabbitHole.TileObjects;

namespace IntoTheRabbitHole.Traits;

public class Collectible : Trait
{
	public Collectible(TileObject o) : base(o) { }

	public override ushort ExecutionPriority => 80;

	public override void SteppedOn(TileObject o, Vector2I fromDir)
	{
		var player = o as Player;
		if (player != null)
		{
			//this is the player
			player.Score++;
			GD.Print("player collected a collectible! Score: " + player.Score);
		}

		//everything picks up the collectible
		Owner.Kill();

		base.SteppedOn(o, fromDir);
	}
}