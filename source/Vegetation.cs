using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimGame.source
{
    class Vegetation : Creature
    {
        public Vegetation(int id, Node root)
            : base(id, root)
        {
            var animres = (SpriteFrames)ResourceLoader.Load("res://data/scenes/Bush.tres");
            character.Frames = animres;
        }
    }
}
