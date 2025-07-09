using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole;

public abstract class Trait
{
	protected readonly TileObject Owner;

	public Trait(TileObject o)
	{
		Owner = o;
	}


	public abstract ushort ExecutionPriority { get; }

	public virtual void ModifyAppearance(double deltaTime)
	{
		// Default implementation does nothing
	}
	//bigger number triggers after smaller number

	public virtual void SteppedOn(TileObject o, Vector2I fromDir)
	{
	}

	public virtual void FloatedOn(TileObject o, Vector2I fromDir)
	{
	}

	public virtual void TileEntered(Tile t)
	{
	}

	public virtual void Tick()
	{
	}


	public virtual void SteppedUnder(TileObject tileObject, Vector2I fromDir)
	{
	}
}