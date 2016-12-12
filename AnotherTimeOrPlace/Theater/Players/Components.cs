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
    public abstract class CompBasic : IComponent
    {
        public IKeeper Owner { get; private set; }

        public CompBasic(IKeeper owner)
        {
            Owner = owner;
        }
    }

    public abstract class CBody : CompBasic, IUpdate, ISpatial //, IDraw
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Vector2 Acceleration { get; protected set; }
        protected Vector2 LastPos { get; private set; }

        public int Foot { get; protected set; }
        public float Mass { get; protected set; }
        public float StaticFriction { get; protected set; }
        public float KineticFriction { get; protected set; }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                  (int)Math.Floor(Position.X) - Foot,
                  (int)Math.Floor(Position.Y) - Foot,
                  Foot * 2, Foot * 2);
            }
        }
        
        public CBody(IKeeper owner, Vector2 pos, int foot, float mass, float friction)
            : base(owner)
        {
            Foot = foot;
            Mass = mass;
            Position = pos;
            StaticFriction = friction;
            KineticFriction = friction;
        }

        public void Update()
        {
            LastPos = Position;
            Collide();
            //SynthesizeAccel();

            Velocity += Acceleration * Registry.CycleTime;

            if (Registry.Stage != null)
            {
                RunAxis(Registry.CycleTime, "X");
                RunAxis(Registry.CycleTime, "Y");
            }
            else
                Position += Velocity * Registry.CycleTime;

            Velocity *= KineticFriction;
            Acceleration = Vector2.Zero;
        }

        #region tile collision
        protected abstract void Collide();

        public void Impulse(Vector2 impulse)
        {
            Acceleration += impulse;
        }
        
        private void RunAxis(float cycleTime, string xy)
        {
            int size = Foot * 2;

            var axis = typeof(Vector2).GetField(xy);
            object pos = Position;

            axis.SetValue(pos,
                (float)axis.GetValue(Position) + (float)axis.GetValue(Velocity) * cycleTime);
            Position = (Vector2)pos;

            int left = Tile.GetArray(Bounds.Left);
            int right = Tile.GetArray(Bounds.Right);
            int top = Tile.GetArray(Bounds.Top);
            int bot = Tile.GetArray(Bounds.Bottom);

            for (int y = top; y <= bot; y++)
                for (int x = left; x <= right; x++)
                    if (Registry.Stage.GetCollision(x, y) == Tile.Collision.Solid)
                    {
                        Rectangle tile = Registry.Stage.GetBounds(x, y);
                        if (Bounds.Intersects(tile))
                        {
                            float tileMin = MinOnAxis(tile, xy);
                            float tileMax = MaxOnAxis(tile, xy);

                            if ((float)axis.GetValue(LastPos) < (tileMin + tileMax) / 2)
                                axis.SetValue(pos, tileMin - Foot);
                            else
                                axis.SetValue(pos, tileMax + Foot);
                            Position = (Vector2)pos;

                            object vel = Velocity;
                            axis.SetValue(vel, (float)axis.GetValue(vel) * -0.1f);
                            Velocity = (Vector2)vel;
                        }
                    }
        }

        private int MinOnAxis(Rectangle box, string xy)
        {
            if (xy == "X")
                return box.Left;
            else
                return box.Top;
        }

        private int MaxOnAxis(Rectangle box, string xy)
        {
            if (xy == "X")
                return box.Right;
            else
                return box.Bottom;
        }
        #endregion

        /*public void Draw(SpriteBatch batch)
        {
            Registry.CenterLine(batch, 4.0f, Color.DarkRed, new Vector2(Bounds.Left, Bounds.Top),
                new Vector2(Bounds.Right, Bounds.Bottom), 0.0f);
            Registry.CenterLine(batch, 4.0f, Color.DarkRed, new Vector2(Bounds.Right, Bounds.Top),
                new Vector2(Bounds.Left, Bounds.Bottom), 0.0f);
        }*/
    }
    
    public class CRoundBody : CBody
    {
        public CRoundBody(IKeeper owner, Vector2 pos, int foot, float mass, float friction)
            : base(owner, pos, foot, mass, friction)
        { }

        protected override void Collide()
        {
            foreach (Actor actor in Registry.Actors)
            {
                if (actor == Owner)
                    continue;

                else if (Gizmo.circlesColliding(Position, Foot,
                    actor.Position, actor.Foot))
                {
                    float distance = Vector2.Distance(Position, actor.Position);
                    float radii = (Foot + actor.Foot);
                    float collisionDepth = radii - distance;

                    Vector2 toActor = new Vector2(actor.Position.X - Position.X,
                        actor.Position.Y - Position.Y);
                    toActor.Normalize();

                    actor.Impulse(toActor * collisionDepth * 0.5f);
                    Impulse(-toActor * collisionDepth * 0.5f);
                }
            }
        }
    }
    
    public class CSquareBody : CBody
    {
        public CSquareBody(IKeeper owner, Vector2 pos, int foot, float mass, float friction)
            : base(owner, pos, foot, mass, friction)
        { }

        protected override void Collide()
        {

        }
    }

    public class CSprite : CompBasic, IDraw
    {
        private string TexName;
        private Rectangle Source;
        private SpriteEffects Fx;
        private Vector2 Origin;

        public static readonly Vector2 BotCenter = new Vector2(0.5f, 1.0f);
        public static readonly Vector2 TopLeft = Vector2.Zero;

        public CSprite(IKeeper owner, string texName, Rectangle source, Vector2 originScale)
            : base(owner)
        {
            Source = source;
            TexName = texName;
            Origin = originScale;
            Origin.X *= Source.Width;
            Origin.Y *= Source.Height;
        }
        
        public void Animate(Rectangle? source, SpriteEffects fx)
        {
            Fx = fx;
            if (source.HasValue)
                Source = source.Value;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(
                Registry.Textures[TexName],
                Owner.Position,
                Source,
                Color.White,
                0.0f,
                Origin,
                1.0f,
                Fx,
                0.0f);
        }
    }
}
