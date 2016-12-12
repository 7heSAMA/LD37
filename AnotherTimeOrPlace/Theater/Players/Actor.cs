using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AnotherTimeOrPlace.Util;

namespace AnotherTimeOrPlace.Theater
{
    public abstract class Actor : IKeeper, IUpdate, IDraw
    {
        public List<IComponent> Components { get; private set; }

        public Vector2 Position { get { return Body.Position; } }
        public Vector2 Velocity { get { return Body.Velocity; } }
        public Vector2 Acceleration { get { return Body.Acceleration; } }

        public int Foot { get { return Body.Foot; } }
        public float Mass { get { return Body.Mass; } }

        public Rectangle Bounds { get { return Body.Bounds; } }

        protected CBody Body;
        private bool Drawn;

        protected Actor()
        {
            // default size 4 seems reasonable for the average actor 
            Components = new List<IComponent>(4);
        }

        protected void AddComp(IComponent toAdd)
        {
            Components.Add(toAdd);
        }
        
        public void Update()
        {
            foreach (IComponent comp in Components)
            {
                IUpdate toUpdate = comp as IUpdate;
                if (toUpdate != null)
                    toUpdate.Update();
            }

            Drawn = false;
        }

        public void Impulse(Vector2 impulse)
        {
            Body.Impulse(impulse);
        }

        public void Draw(SpriteBatch batch)
        {
            if (Drawn)
                return;

            foreach (IComponent comp in Components)
            {
                IDraw toDraw = comp as IDraw;
                if (toDraw != null)
                    toDraw.Draw(batch);
            }

            Drawn = true;
        }
    }
}
