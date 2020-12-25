using Godot;
using SimGame;
using SimGame.source;
using System;
using System.Collections.Generic;

public class Game : Node
{
	[Export]
	public int NumCharsToSpawn = 5000;
	[Export]
	public int NumVegetationToSpawn = 30000;
	private int currentId = 0;
	public Rect2 SpawnArea = new Rect2(0, 0, 30, 30);
	private RandomNumberGenerator random = new RandomNumberGenerator();
	[Export]
	public NodePath GroundTileMapPath;

	public static Game GetSingleton()
	{
		return self;
	}

	public override void _Ready()
	{
		self = this;
		var consts = Consts.GetSingleton();
		SpawnArea = new Rect2(0, 0, consts.MapSize, consts.MapSize);

		// Setting ground
		var tileMap = GetNode<TileMap>(GroundTileMapPath);
		for (int i = 0; i < consts.MapSize; i++)
			for (int j = 0; j < consts.MapSize; j++)
				tileMap.SetCell(i, j, 0);

		// Spawning characters
		for (int i = 0; i < NumCharsToSpawn; i++)
		{
			var creature = new Animal(currentId++, this);
			creature.ChessLocation = GetRandomPosition(SpawnArea);
			creatures.Add(creature);
		}

		// Spawning vegetation
		for (int i = 0; i < NumVegetationToSpawn; i++)
		{
			var creature = new Vegetation(currentId++, this);
			creature.ChessLocation = GetRandomPosition(SpawnArea);
			creatures.Add(creature);
		}
	}

	public override void _Process(float delta)
    {
		Creature[] creaturesCopy = new Creature[creatures.Count];
		creatures.CopyTo(creaturesCopy);
		foreach (var creature in creaturesCopy)
			if (!creature.MarkedForDelete)
				creature.Update(delta);
    }

	private Vector2 GetRandomPosition(Rect2 rect)
	{
		return new Vector2(random.RandiRange((int)rect.Position.x, (int)rect.Position.x + (int)rect.Size.x),
							random.RandiRange((int)rect.Position.y, (int)rect.Position.y + (int)rect.Size.y));
	}

	public Vector2 WorldClamp(Vector2 v)
    {
		var consts = Consts.GetSingleton();
		return new Vector2(Mathf.Clamp(v.x, 0.0f, consts.MapSize), Mathf.Clamp(v.y, 0.0f, consts.MapSize));
    }

	private static Game self = null;
	private List<Creature> creatures = new List<Creature>();
	public List<Creature> Creatures { get { return creatures; } }
}
