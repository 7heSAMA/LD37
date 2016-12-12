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
    public class Tile
    {
        #region STATICS
        public const int FOOT = 32;
        public static readonly Vector2 PRINT = new Vector2(FOOT);
        public static readonly Vector2 ORIGIN = new Vector2(FOOT / 2);
        public delegate void ApplyToTiles(object[] args);

        public enum Collision
        {
            Solid,
            Empty,
        }

        public enum Style
        {
            Lone,
            Floor,
            Wall,
        }

        public static int GetArray(float pos)
        {
            return (int)Math.Floor(pos / FOOT);
        }
        #endregion

        public Collision Shape { get; protected set; }
        public Style Design { get; protected set; }
        public Color Fill { get; protected set; }

        public Wheel[,] Spaces;

        protected Rectangle Source;
        protected SpriteEffects Fx;

        public Rectangle Bounds { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Vector2 Center { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }

        protected Vector2 SourceOrigin;

        public Tile(int x, int y, Color fill)
        {
            Fill = fill;
            InitDimen(x, y, Collision.Empty, Style.Lone);
        }

        public Tile(int x, int y, Collision shape, Style design, 
            Rectangle source, SpriteEffects fx)
        {
            Fx = fx;
            Bounds = new Rectangle(X * FOOT, Y * FOOT, FOOT, FOOT);
            Source = source;
            SourceOrigin = new Vector2(0, FOOT);

            InitDimen(x, y, shape, design);
        }

        private void InitDimen(int x, int y, Collision shape, Style design)
        {
            X = x;
            Y = y;
            Position = new Vector2(X, Y) * PRINT;
            Center = Position + ORIGIN;

            Shape = shape;
            Design = design;
            Spaces = new Wheel[4, 4];
        }

        public virtual void Draw(SpriteBatch batch)
        {
            // for some reason calling Avatar.Shade[Turf] here throws 
            // a key-not-found error, even tho it assigns fine in SetTurf

            if (X > 0 && X < 14 && Y > 0 && Y < 7)
            {
                Vector2 quarter = new Vector2(FOOT / 4);
                for (int y = 0; y < 4; y++)
                    for (int x = 0; x < 4; x++)
                    {
                        Vector2 pos = Position + new Vector2(x * FOOT / 4, y * FOOT / 4);
                        Registry.DrawQuad(batch, pos, Registry.Colors[Spaces[x, y]],
                            0.0f, PRINT / 4, 0.0f, false);
                    }
            }
            else
            {
                Registry.DrawQuad(batch, Position, Fill, 0.0f, PRINT, 0.0f, false);
            }

            if (Source != Rectangle.Empty)
            {
                batch.Draw(
                    Registry.Textures["Tilesheet"],
                    Position,
                    Source,
                    Color.White,
                    0.0f,
                    SourceOrigin,
                    1.0f,
                    Fx,
                    0.0f);
            }
        }
        
        public void CalcTurf(List<ITurf> contenders)
        {
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    float minimum = 1000.0f;
                    ITurf closest = null;

                    foreach (ITurf contender in contenders)
                    {
                        Vector2 pos = Position + new Vector2(
                            (x * 2 + 1) * (FOOT / 4), (y * 2 + 1) * (FOOT / 4));

                        float distance = Vector2.Distance(pos, contender.Position);

                        if (distance < minimum)
                        {
                            closest = contender;
                            minimum = distance;
                        }
                    }
                    
                    switch (closest.Tag)
                    {
                        case Avatar.Faction.Sam:
                            Spaces[x, y] = Wheel.Sam;
                            break;

                        case Avatar.Faction.Taylor:
                            Spaces[x, y] = Wheel.Taylor;
                            break;

                        case (Avatar.Faction.Sam | Avatar.Faction.Taylor):
                            Spaces[x, y] = Wheel.Meld;
                            break;
                    }
                }
        }
    }

    public class Turf : Tile, ITurf
    {
        public Avatar.Faction Tag { get; protected set; }

        public Turf(int x, int y)
            : base(x, y, Color.White)
        { }
        
        public void SetTurf(Avatar.Faction turf)
        {
            Tag = turf;
            // have to assign fill because of error outlined in draw (below) 
            Fill = Avatar.Shade[Tag];
        }
    }
}
