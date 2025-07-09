using Godot;

public partial class UI : Control
{
	public static UI Instance;

	public override void _Ready()
	{
		Instance = this;
		// Initialize UI elements here
	}
}