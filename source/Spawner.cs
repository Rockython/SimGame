using Godot;
using SimGame;
using System;

public class Spawner : Node
{
	[Export]
	public int NumCharsToSpawn = 5000;
	[Export]
	public string CharsScene = "res://data/scenes/Character.tscn";
	private int currentId = 0;
	public Rect2 SpawnArea = new Rect2(0, 0, 30, 30);
	private RandomNumberGenerator random = new RandomNumberGenerator();
	[Export]
	public NodePath ObjectsTileMapPath;
	[Export]
	public NodePath GroundTileMapPath;

	public override void _Ready()
	{
		var consts = Consts.GetSingleton();
		SpawnArea = new Rect2(0, 0, consts.MapSize, consts.MapSize);

		// Setting ground
		var tileMap = GetNode<TileMap>(GroundTileMapPath);
		for (int i = 0; i < consts.MapSize; i++)
			for (int j = 0; j < consts.MapSize; j++)
				tileMap.SetCell(i, j, 0);


		// Spawning characters
		var scene = (PackedScene)ResourceLoader.Load(CharsScene);
		for (int i = 0; i < NumCharsToSpawn; i++)
		{
			var instance = scene.Instance();
			var character = (Character)instance;
			DebugTools.Assert(character != null, "Invalid type");
			character.Id = currentId++;
			character.ChessLocation = GetRandomPosition(SpawnArea);
			AddChild(instance);
		}

		// Spawning vegetation
		tileMap = GetNode<TileMap>(ObjectsTileMapPath);
		DebugTools.Assert(tileMap != null, "No tilemap");
		for (int i = 0; i < 100000; i++)
		{
			var pos = GetRandomPosition(SpawnArea);
			tileMap.SetCell((int)pos.x, (int)pos.y, 0);
		}
		tileMap.UpdateBitmaskRegion();
	}

	private Vector2 GetRandomPosition(Rect2 rect)
	{
		return new Vector2(random.RandiRange((int)rect.Position.x, (int)rect.Position.x + (int)rect.Size.x),
							random.RandiRange((int)rect.Position.y, (int)rect.Position.y + (int)rect.Size.y));
	}
}
