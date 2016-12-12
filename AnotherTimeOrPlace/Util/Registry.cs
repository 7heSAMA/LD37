using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AnotherTimeOrPlace.Theater;

namespace AnotherTimeOrPlace.Util
{
    [Flags]
    public enum InputStatus
    {
        Released = 0,
        JustReleased = 1,
        Pressed = 2,
        JustPressed = 4,
    }
    
    public enum Wheel
    {
        Pale,
        Tan,
        Dark,
        Green,
        Teal,
        Blue,
        Shadow,
        Red,
        Sam,
        Taylor,
        Meld,
    }

    public static class Registry
    {
        private static Main Game;
        private static StateManager Manager;
        public static Dictionary<string, Texture2D> Textures;
        public static Dictionary<Wheel, Color> Colors;

        private static readonly float TargetCycle = 1.0f / 60.0f;
        public static float CycleTime { get; private set; }

        // TODO: compartmentalize this sh!t
        public static float Zoom { get; private set; }
        public static Matrix Scale { get; private set; }

        public static List<Actor> Actors { get; private set; }
        public static Map Stage { get; private set; }

        private static KeyboardState LastBoard;
        private static KeyboardState CurrentBoard;
        private static MouseState LastMouse;
        private static MouseState CurrentMouse;

        public static SpriteFont Font { get; private set; }

        private static List<Keys> UsedKeys;
        private static bool UsedLeftClick;
        private static bool UsedRightClick;
        public static Vector2 MousePos;
        public static Point MousePoint;

        static Registry()
        {
            Manager = new StateManager();
            GameState.InitManager(Manager);

            Textures = new Dictionary<string, Texture2D>();
            Actors = new List<Actor>();
            UsedKeys = new List<Keys>();
            
            CurrentBoard = Keyboard.GetState();
            CurrentMouse = Mouse.GetState();
        }
        
        public static void LoadGameServices(
            Main game, GraphicsDevice graphics, ContentManager content)
        {
            Game = game;
            Zoom = 2.0f;
            Scale = Matrix.CreateScale(Zoom);

            Colors = new Dictionary<Wheel, Color>();
            Colors.Add(Wheel.Pale, new Color(234, 230, 212));
            Colors.Add(Wheel.Tan, new Color(128, 110, 85));
            Colors.Add(Wheel.Dark, new Color(30, 30, 30));
            Colors.Add(Wheel.Red, new Color(169, 58, 58));
            Colors.Add(Wheel.Green, new Color(66, 72, 66));
            Colors.Add(Wheel.Teal, new Color(38, 53, 59));
            Colors.Add(Wheel.Blue, new Color(8, 18, 32));
            Colors.Add(Wheel.Shadow, new Color(24, 8, 18));
            Colors.Add(Wheel.Sam, Avatar.Shade[Avatar.Faction.Sam]);
            Colors.Add(Wheel.Taylor, Avatar.Shade[Avatar.Faction.Taylor]);
            Colors.Add(Wheel.Meld, Avatar.Shade[(Avatar.Faction.Sam | Avatar.Faction.Taylor)]);

            Textures.Add("Sprites", content.Load<Texture2D>("spritesheet.png"));
            Textures.Add("Tilesheet", content.Load<Texture2D>("tilesheet.png"));
            Textures.Add("Props", content.Load<Texture2D>("props.png"));

            Font = content.Load<SpriteFont>("Body");

            Texture2D blank = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            Textures.Add("Blank", blank);

            Manager.Push(new STitle());
        }

        public static void AddActor(Actor toAdd)
        {
            Actors.Add(toAdd);
        }

        public static void SetStage(Map stage)
        {
            Stage = stage;
        }

        public static void Update(GameTime gameTime)
        {
            CycleTime = (float)gameTime.ElapsedGameTime.TotalSeconds / TargetCycle;

            UpdateInput();
            Manager.UpdateStates();

            if (Manager.Front == null)
                Game.Exit();
        }
        
        #region Input
        private static void UpdateInput()
        {
            UsedKeys.Clear();
            UsedLeftClick = false;
            UsedRightClick = false;

            LastBoard = CurrentBoard;
            CurrentBoard = Keyboard.GetState();

            LastMouse = CurrentMouse;
            CurrentMouse = Mouse.GetState();

            Point mouse = Mouse.GetState().Position;
            MousePos = new Vector2(mouse.X, mouse.Y) / Zoom;
            //MousePoint = new Point((int)Math.Floor(MousePos.X), (int)Math.Floor(MousePos.Y));
        }

        public static bool JustPressed(Keys key)
        {
            if (LastBoard.IsKeyUp(key) && CurrentBoard.IsKeyDown(key))
                return true;
            else
                return false;
        }

        public static bool Pressed(Keys key)
        {
            if (CurrentBoard.IsKeyDown(key))
                return true;
            else
                return false;
        }

        public static bool JustReleased(Keys key)
        {
            if (LastBoard.IsKeyDown(key) && CurrentBoard.IsKeyUp(key))
                return true;
            else
                return false;
        }

        public static bool Released(Keys key)
        {
            if (CurrentBoard.IsKeyDown(key))
                return true;
            else
                return false;
        }

        public static bool LeftClick()
        {
            if (CurrentMouse.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }
        #endregion

        public static void Draw(SpriteBatch batch)
        {
            Manager.DrawStates(batch);
        }
        
        #region Drawing
        public static void DrawQuad(SpriteBatch spriteBatch, 
            Vector2 position, Color color, float rotation,
            Vector2 scale, float depth, bool centered)
        {
            Vector2 origin;
            if (centered)
                origin = new Vector2(0.5f);
            else
                origin = Vector2.Zero;
            spriteBatch.Draw(Textures["Blank"], position, null, color, 
                rotation, origin, scale, SpriteEffects.None, depth);
        }

        public static void DrawQuad(SpriteBatch spriteBatch, 
            Vector2 position, Color color, float rotation,
            Vector2 scale, float depth, Vector2 origin)
        {
            spriteBatch.Draw(Textures["Blank"], position, null, color, 
                rotation, origin, scale, SpriteEffects.None, depth);
        }

        public static void CenterLine(SpriteBatch spriteBatch, 
            float width, Color color, Vector2 p1, Vector2 p2, float depth)
        {
            DrawLine(spriteBatch, width / 2, color, p1, p2, depth);
            DrawLine(spriteBatch, width / 2, color, p2, p1, depth);
        }

        public static void DrawLine(SpriteBatch spriteBatch, 
            float width, Color color, Vector2 p1, Vector2 p2, float depth)
        {
            float angle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            float length = Vector2.Distance(p1, p2);

            spriteBatch.Draw(Textures["Blank"], p1, null, color,
                angle, Vector2.Zero, new Vector2(length, width),
                SpriteEffects.None, depth);
        }

        public static void DrawCircle(SpriteBatch spriteBatch, 
            float r, float cutr, Vector2 position, float depth, Color color)
        {
            float width = r - cutr;
            float segments = (float)Math.Floor(r / MathHelper.Pi * 2.0f);
            for (float i = 0; i < segments; i++)
            {
                float sA = (float)(i / segments) * MathHelper.TwoPi;
                Vector2 start = new Vector2((float)Math.Cos(sA) * r, (float)Math.Sin(sA) * r);
                float eA = (float)((i + 1) / segments) * MathHelper.TwoPi;
                Vector2 end = new Vector2((float)Math.Cos(eA) * r, (float)Math.Sin(eA) * r);
                DrawLine(spriteBatch, width, color, start + position, end + position, depth);
            }
        }
        #endregion
    }
}
