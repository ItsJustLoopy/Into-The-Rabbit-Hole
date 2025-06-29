using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole;

public abstract class Trait
{
	protected readonly TileObject _owner;
	
	public Trait(TileObject o)
	{
		_owner = o;
	}
	
	public abstract ushort ExecutionPriority { get; }
	//bigger number triggers after smaller number

	public virtual void SteppedOn(TileObject o, Vector2I fromDir)
	{
		
	}

	public virtual void TileEntered(Tile t)
	{
		
	}

	public virtual void Tick()
	{
		
	}
}