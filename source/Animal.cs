﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace SimGame.source
{
    class Animal : Creature
    {
        private Vector2 walkToNeighbour = new Vector2();
        private Vector2 walkToLocation = new Vector2();
        public float Satiety { get; set; } = 100.0f;
        public float Speed { get; set; } = 0.95f;

        public Animal(int id, Node root)
            : base(id, root)
        {
            Satiety = random.RandfRange(30.0f, 100.0f);
            satietyNode = character.GetNode<Polygon2D>("Satiety");
            DebugTools.Assert(satietyNode != null, "No satiety visual");
            satietyNode.Visible = true;
        }

        public void WalkToward(Vector2 dir)
        {
            DebugTools.Assert(dir.Length() <= 1.0f, "Too far");
            walkToNeighbour = ChessLocation + new Vector2(0.5f, 0.5f) + dir;
        }

        public void WalkTo(Vector2 position)
        {
            walkToLocation = position.Floor() + new Vector2(0.5f, 0.5f);
        }

        public override void Update(float dt)
        {
            const float Hunger = 0.5f;
            Satiety -= Hunger * dt;
            satietyNode.Scale = new Vector2(Satiety / 100.0f, 1.0f);
            satietyNode.Color = Color.Color8(0, 255, 0);

            if (Satiety <= 0.0f)
            {
                Destroy();
                return;
            }

            if (Satiety < Consts.GetSingleton().Starvation)
            {
                Creature closestFood = null;
                satietyNode.Color = Color.Color8(255, 0, 0);
                foreach (var creature in Game.GetSingleton().Creatures)
                {
                    var vege = creature as Vegetation;
                    if (vege != null)
                    {
                        float dist = vege.ChessLocation.DistanceSquaredTo(ChessLocation);
                        if (dist == 0.0f)
                        {
                            Satiety += vege.Calories;
                            closestFood = null;
                            vege.Destroy();
                            break;
                        }

                        if (closestFood == null)
                            closestFood = vege;
                        else if (dist < closestFood.ChessLocation.DistanceSquaredTo(ChessLocation))
                            closestFood = vege;
                    }
                }
                if (closestFood != null)
                    WalkTo(closestFood.ChessLocation);
            }
            else
            {
                if (!IsOnMove())
                {
                    Vector2[] dirs = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
                    var randLocation = ChessLocation + dirs[random.RandiRange(0, 3)];
                    randLocation = Game.GetSingleton().WorldClamp(randLocation);
                    WalkTo(randLocation);
                }
            }

            Travel(dt);
            base.Update(dt);
        }

        void Travel(float dt)
        {
            float stepLen = Speed * dt;
            if (Location.Floor() != walkToLocation.Floor())
            {
                var travelVec = walkToLocation - Location;
                if (Mathf.Abs(travelVec.x) > stepLen)
                    WalkToward(new Vector2(Mathf.Sign(travelVec.x), 0));
                else
                    WalkToward(new Vector2(0, Mathf.Sign(travelVec.y)));
            }
            else
                walkToLocation = Location;

            if (Location.DistanceTo(walkToNeighbour) > stepLen)
            {
                var step = Location.DirectionTo(walkToNeighbour) * stepLen;
                Location = Location + step;
            }
            else
                Location = walkToNeighbour;
        }

        bool IsOnMove()
        {
            return Location != walkToNeighbour;
        }

        protected override void OnChessLocationSet()
        {
            walkToNeighbour = Location;
            walkToLocation = Location;
        }

        private Polygon2D satietyNode = null;
    }
}