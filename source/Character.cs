using Godot;
using SimGame;

public class Character : AnimatedSprite
{
	public Vector2 ChessLocation 
	{
		get 
		{
			return Location.Floor();
		}
		set 
		{
			Location = value + new Vector2(0.5f, 0.5f);
		}
	}

	public int Id { get; set; }
	public Vector2 Location { get; set; }
	public float MaxSpeed { get; set; } = 0.95f;

	private Vector2 walkToNeighbour = new Vector2();
	private Vector2 walkToLocation = new Vector2();
	private RandomNumberGenerator random = new RandomNumberGenerator();
	public float Satiety { get; set; } = 100.0f;


	public void WalkToward(Vector2 dir)
	{
		DebugTools.Assert(dir.Length() <= 1.0f, "Too far");
		walkToNeighbour = ChessLocation + new Vector2(0.5f, 0.5f) + dir;
	}

	public void WalkTo(Vector2 position)
	{
		walkToLocation = position.Floor() + new Vector2(0.5f, 0.5f);
	}

	public override void _Ready()
	{
		random.Seed = (ulong)Id;
		walkToNeighbour = Location;
		walkToLocation = Location;
	}

	public override void _Process(float delta)
	{
		float stepLen = MaxSpeed * delta;
		if (Location.Floor() != walkToLocation.Floor())
		{
			var travelVec = walkToLocation - Location;
			if (Mathf.Abs(travelVec.x) > stepLen)
			{
				WalkToward(new Vector2(Mathf.Sign(travelVec.x), 0));
			}
			else
			{
				WalkToward(new Vector2(0, Mathf.Sign(travelVec.y)));
			}
		}
		else
		{
			walkToLocation = Location;
		}

		if (Location.DistanceTo(walkToNeighbour) > stepLen)
		{
			var step = Location.DirectionTo(walkToNeighbour) * stepLen;
			Location = Location + step;
		}
		else
		{
			Location = walkToNeighbour;
			//Vector2[] dirs = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
			//WalkToward(dirs[random.RandiRange(0, 3)]);
		}

		const float Hunger = 0.1f;
		Satiety -= Hunger * delta;

		if (Input.IsKeyPressed((int)KeyList.T))
		{
			WalkTo(new Vector2(7, 1));
		}

		Position = Location * 64.0f;
	}
}
