using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using AnotherTimeOrPlace.Theater;

namespace AnotherTimeOrPlace.Util
{
    public class STitle : GameState
    {
        public STitle()
            : base("Title")
        { }

        public override void Update()
        {
            if (Registry.JustPressed(Keys.Escape))
                Active = false;

            if (Registry.JustPressed(Keys.Space))
                Manager.Push(new SRoom());
        }
        
        public override void Draw(SpriteBatch batch)
        {
            batch.DrawString(Registry.Font, 
                "Used WASD to navigate and Space to interact. \n \n" +
                "You were fighting with your S.O. \n" +
                "and this place just doesn't feel like home. \n" +
                "Try and make yourself comfortable. \n \n" +
                "Created by Kris with a K for LD37.", 
                Vector2.Zero, Color.White);
        }
    }

    public class SRoom : GameState
    {
        public SRoom()
            : base("Room")
        {
            int width = 14;
            int height = 6;

            // TODO: make viewport size an actual variable
            // note: halved because of 2x zoom  
            Vector2 offset = new Vector2(
                (480 - (width * Tile.FOOT)) / 2,
                (270 - (height * Tile.FOOT)) / 2);
            
            Registry.SetStage(new Map());
        }

        public override void Update()
        {
            if (Registry.JustPressed(Keys.Escape))
                Manager.Push(new SPause());

            foreach (Actor actor in Registry.Actors)
                actor.Update();

            Registry.Stage.DivideTurf();
        }

        public override void Draw(SpriteBatch batch)
        {
            Registry.Stage.Draw(batch);
            /*foreach (Actor actor in Registry.Actors)
                actor.Draw(batch);*/
            Registry.DrawQuad(batch, Registry.MousePos, Color.DarkRed, MathHelper.Pi / 4.0f,
                new Vector2(4.0f), 0.0f, true);
        }
    }

    public class SPause : GameState
    {
        public SPause()
            : base("Pause")
        {
            DrawNext = true;
        }

        public override void Update()
        {
            if (Registry.JustPressed(Keys.Escape))
            {
                Active = false;
            }
        }
    }
    
}
