using Godot;

namespace IntoTheRabbitHole;

public abstract class Trait
{
	private TileObject _owner;
	
	public Trait(TileObject o)
	{
		_owner = o;
	}
	
	public abstract ushort ExecutionPriority { get; }
	//bigger number triggers after smaller number
	
	public abstract void StepOn(TileObject o, Vector2I fromDir);
	public abstract void LeapOver(TileObject o, Vector2I fromDir);

}