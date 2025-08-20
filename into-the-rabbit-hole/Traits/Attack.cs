using IntoTheRabbitHole.TileObjects;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public class Attack : Trait
{
	public Attack(TileObject o) : base(o) { }

	public override ushort ExecutionPriority => 10;


	public override void TileEntered(Tile t)
	{
		foreach (var o in t.TileObjects)
		{
			//	o.Kill();
		}
	}
}