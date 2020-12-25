using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace SimGame.source
{
    public abstract class Creature
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
                OnChessLocationSet();
            }
        }

        public Vector2 Location { get; set; }
        public int Id { get; set; }
        public float Calories { get; set; } = 60.0f;
        public bool MarkedForDelete { get; set; } = false;

        protected RandomNumberGenerator random = new RandomNumberGenerator();
        protected Character character = null;

        public Creature(int id, Node rootNode)
        {
            Id = id;
            random.Seed = (ulong)Id;
            var characterScene = (PackedScene)ResourceLoader.Load("res://data/scenes/Character.tscn");
            character = (Character)characterScene.Instance();
            DebugTools.Assert(character != null, "Invalid type");
            rootNode.AddChild(character);
        }

        public void Destroy()
        {
            if (MarkedForDelete)
                return;

            character.QueueFree();
            character = null;
            Game.GetSingleton().Creatures.Remove(this);
            MarkedForDelete = true;
        }

        public virtual void Update(float dt)
        {
            character.Position = Location * Consts.GetSingleton().CellSize;
        }

        protected virtual void OnChessLocationSet()
        {

        }
    }
}
