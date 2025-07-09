using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole.Traits;

public abstract class Trait
{
	protected readonly TileObjects.TileObject Owner;

	public Trait(TileObjects.TileObject o)
	{
		Owner = o;
	}


	public abstract ushort ExecutionPriority { get; }

	public virtual void ModifyAppearance(double deltaTime)
	{
		// Default implementation does nothing
	}
	//bigger number triggers after smaller number

	public virtual void SteppedOn(TileObjects.TileObject o, Vector2I fromDir)
	{
	}

	public virtual void FloatedOn(TileObjects.TileObject o, Vector2I fromDir)
	{
	}

	public virtual void TileEntered(Tile t)
	{
	}

	public virtual void Tick()
	{
	}


	public virtual void SteppedUnder(TileObjects.TileObject tileObject, Vector2I fromDir)
	{
	}
}