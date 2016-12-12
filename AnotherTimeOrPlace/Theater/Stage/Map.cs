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
    public class Map
    {
        protected Tile[,] Tiles;

        #region Bounds and Collision
        public Tile.Collision GetCollision(Vector2 pos)
        {
            return GetCollision(
                Tile.GetArray(pos.X),
                Tile.GetArray(pos.Y));
        }

        public Tile.Style GetStyle(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return Tile.Style.Lone;
            else
                return Tiles[x, y].Design;
        }

        public Tile.Collision GetCollision(int x, int y)
        {
            if (x < 0 || x > Width - 1 || y > Height - 1 || y < 0)
                return Tile.Collision.Empty;
            else
                return Tiles[x, y].Shape;
        }

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.FOOT, y * Tile.FOOT, Tile.FOOT, Tile.FOOT);
        }
        
        /// <summary>
        /// Width of the level, in Tiles.
        /// </summary>
        public int Width
        {
            get { return Tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level, in Tiles.
        /// </summary>
        public int Height
        {
            get { return Tiles.GetLength(1); }
        }

        public Vector2 Center
        {
            get { return new Vector2(Width, Height) * Tile.ORIGIN; }
        }
        #endregion

        public Map()
        {
            LoadTiles("Content/trailer.txt");

            /*Tiles = new Tile[width, height];

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                        Tiles[x, y] = new Tile(x, y, 
                            Tile.Collision.Solid, Tile.Style.Wall, new Color(24, 8, 18));
                    else
                        Tiles[x, y] = new Tile(x, y, Color.White);
                }

            Tiles[8, Height - 1] = new Tile(8, Height - 1, Color.White);*/
        }

        #region loading
        private void LoadTiles(string mapName)
        {
            int width;
            List<string> Lines = new List<string>();

            using (System.IO.StreamReader reader = new System.IO.StreamReader(mapName))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    Lines.Add(line);
                    if (line.Length != width)
                        throw new NotSupportedException(string.Format(
                            "The length of line {0} is different from all preceding lines.", 
                            Lines.Count));
                    line = reader.ReadLine();
                }
            }

            Tiles = new Tile[width, Lines.Count];

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    char TileType = Lines[y][x];
                    Tiles[x, y] = LoadTile(TileType, x, y);
                }
            }
            
        }

        private Tile LoadTile(char icon, int x, int y)
        {
            Vector2 topLeft = new Vector2(x * Tile.FOOT, y * Tile.FOOT);
            //Color BoneWhite = new Color(180, 170, 160);
            //BoneWhite = Color.Lerp(Color.LightGray, Color.MonoGameOrange, 0.1f);
            //Color shadow = new Color(24, 8, 18);

            switch (icon)
            {
                case '.':
                    return new Turf(x, y);
                case '-':
                    return new Tile(x, y, Registry.Colors[Wheel.Green]);

                case '=':
                    return new Tile(x, y, Registry.Colors[Wheel.Dark]);

                case '|':
                    return new Tile(x, y, Tile.Collision.Solid, Tile.Style.Wall,
                        new Rectangle(Tile.FOOT, 0, Tile.FOOT, Tile.FOOT * 2), SpriteEffects.None);
                case '#':
                    return new Tile(x, y, Tile.Collision.Solid, Tile.Style.Wall,
                        new Rectangle(0, 0, Tile.FOOT, Tile.FOOT * 2), SpriteEffects.None);

                case '1':
                    Registry.Actors.Add(new Photo(topLeft + Tile.ORIGIN, 
                        Avatar.Faction.Taylor, true));
                    return new Turf(x, y);
                case '2':
                    Registry.Actors.Add(new Photo(topLeft + Tile.ORIGIN, 
                        Avatar.Faction.Sam, true));
                    return new Turf(x, y);
                case '3':
                    Registry.Actors.Add(new Photo(topLeft + Tile.ORIGIN, 
                        (Avatar.Faction.Sam | Avatar.Faction.Taylor), false));
                    return new Turf(x, y);
                case 'J':
                    Registry.Actors.Add(new Rack(topLeft + Tile.ORIGIN));
                    return new Turf(x, y);

                case 'b':
                    return new Tile(x, y, Tile.Collision.Solid, Tile.Style.Lone,
                        new Rectangle(Tile.FOOT * 2, 0, Tile.FOOT, Tile.FOOT * 2), 
                        SpriteEffects.None);
                case 'e':
                    return new Tile(x, y, Tile.Collision.Solid, Tile.Style.Lone,
                        new Rectangle(Tile.FOOT * 3, 0, Tile.FOOT, Tile.FOOT * 2),
                        SpriteEffects.None);
                case 'd':
                    return new Tile(x, y, Tile.Collision.Solid, Tile.Style.Lone,
                        new Rectangle(Tile.FOOT * 2, 0, Tile.FOOT, Tile.FOOT * 2),
                        SpriteEffects.FlipHorizontally);

                case 'S':
                    Avatar sam = new Avatar(CController.Mode.Keyboard, topLeft + Tile.ORIGIN,
                        Avatar.Faction.Sam);
                    
                    Registry.AddActor(sam);
                    return new Turf(x, y);

                case 'T':
                    Avatar taylor = new Avatar(CController.Mode.Mouse, topLeft + Tile.ORIGIN,
                        Avatar.Faction.Taylor);

                    Registry.AddActor(taylor);
                    return new Turf(x, y);

                default:
                    throw new NotSupportedException(string.Format(
                        "Unsupported character type {0} at {1}, {2}, depth of {3}.",
                        icon, x, y));
            }
        }
        #endregion

        public void DivideTurf()
        {
            List<ITurf> onTurf = new List<ITurf>();
            foreach(Actor actor in Registry.Actors)
            {
                ITurf contender = actor as ITurf;
                if (contender != null && contender.Tag != Avatar.Faction.None)
                    onTurf.Add(contender);
            }

            for (int y = 1; y < Height - 2; y++)
                for (int x = 1; x < Width - 1; x++)
                {
                    Tiles[x, y].CalcTurf(onTurf);
                    /*Turf patch = Tiles[x, y] as Turf;
                    if (patch == null)
                        continue;
                    
                    float minimum = 0.0f;
                    ITurf closest = null;

                    foreach (ITurf contender in onTurf)
                    {
                        float distance = Vector2.Distance(Tiles[x, y].Center, contender.Position);
                        if (closest == null || distance < minimum)
                        {
                            closest = contender;
                            minimum = distance;
                        }
                    }
                    
                    patch.SetTurf(closest.Tag);*/
                }
        }
        
        public void Draw(SpriteBatch batch)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                    Tiles[x, y].Draw(batch);

                foreach (Actor actor in Registry.Actors)
                    if (actor.Bounds.Bottom - 1 < (y + 1) * Tile.FOOT)
                        actor.Draw(batch);
            }
        }
    }
}
