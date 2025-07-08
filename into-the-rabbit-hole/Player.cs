using System;
using Godot;
using IntoTheRabbitHole.Tiles;

namespace IntoTheRabbitHole;

public partial class Player : TileObject
{

	public static Player Instance;
	private Camera2D cam;
	private float currentRotation = 0f; // Track player's current rotation in radians
	private float camLerpSpeed = 9f; // Speed of camera rotation lerp
	
	public Player(Tile parentTil): base(parentTil,"Player")
	{
		Instance = this;
		cam = new Camera2D();
		cam.IgnoreRotation = false;
		this.AddChild(cam);
	}

	public int Score { get; set; }

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Left"))
		{
			RotatePlayer(-Mathf.Pi / 2); // Rotate 90 degrees counter-clockwise
		}
		else if (Input.IsActionJustPressed("Right"))
		{
			RotatePlayer(Mathf.Pi / 2); // Rotate 90 degrees clockwise
		}
		else if (Input.IsActionJustPressed("Up"))
		{
			MoveForward(); // Move forward relative to current rotation
		}
		else if (Input.IsActionJustPressed("Down"))
		{
			MoveBackward(); // Move backward relative to current rotation
		}

		UpdateCameraRotation(delta);
		base._Process(delta);
	}


	private void RotatePlayer(float rotationDelta)
	{
		currentRotation += rotationDelta;
		// Normalize rotation to keep it between 0 and 2π
		currentRotation = Mathf.Wrap(currentRotation, 0, Mathf.Pi * 2);

	}
	
	private void MoveForward()
	{
		Vector2I direction = GetDirectionFromRotation();
		Move(direction);
	}
	
	private void MoveBackward()
	{
		Vector2I direction = GetDirectionFromRotation();
		Move(-direction); // Move in opposite direction
	}
	
	private Vector2I GetDirectionFromRotation()
	{
		// Convert rotation to grid direction (up, down, left, right)
		// Normalize rotation to nearest 90-degree increment
		float normalizedRotation = Mathf.Round(currentRotation / (Mathf.Pi / 2)) * (Mathf.Pi / 2);
		
		// Calculate direction vector based on rotation
		Vector2 directionFloat = Vector2.Up.Rotated(normalizedRotation);
		
		// Convert to integer grid directions
		return new Vector2I(Mathf.RoundToInt(directionFloat.X), Mathf.RoundToInt(directionFloat.Y));
	}
	
	private void UpdateCameraRotation(double delta)
	{
		float target = currentRotation;
		float lerped = (float) Mathf.LerpAngle(cam.Rotation, target, delta * camLerpSpeed);
		cam.Rotation = lerped;
		TileManager.Instance.UpdateCameraPosition(cam.Rotation);
	}

	public void Move(Vector2I direction)
	{
		if (direction == Vector2I.Zero)
			return;
		//only allow up left, down right movement of 1 unit
		if (Math.Abs(direction.X) + Math.Abs(direction.Y) > 1)
			return;

		TileManager.Instance.PlayerMove(this,direction);



	}
}