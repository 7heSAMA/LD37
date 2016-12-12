using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnotherTimeOrPlace.Theater
{
    public interface IKeeper : ISpatial
    {
        List<IComponent> Components { get; }
    }

    public interface IComponent
    {
        IKeeper Owner { get; }
    }

    public interface IUpdate
    {
        void Update();
    }

    public interface IDraw
    {
        void Draw(SpriteBatch batch);
    }

    public interface ITurf
    {
        Vector2 Position { get; }
        Avatar.Faction Tag { get; }
    }

    public interface ISpatial
    {
        Vector2 Position { get; }
        Vector2 Velocity { get; }
        Vector2 Acceleration { get; }

        float Mass { get; }
        int Foot { get; }

        void Impulse(Vector2 impulse);

        // TODO: utilize dot product to see greater momentum along direction for collisions 

        //void Collide(ISpatial other);
        //Vector2 ClosestBoundingPoint(Vector2 pos);
    }
}
