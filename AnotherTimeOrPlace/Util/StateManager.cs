using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using AnotherTimeOrPlace.Theater;

namespace AnotherTimeOrPlace.Util
{
    public abstract class GameState : IUpdate, IDraw
    {
        public GameState Next { get; protected set; }

        public bool UpdateNext { get; protected set; } = false;
        public bool DrawNext { get; protected set; } = false;
        public bool Active { get; protected set; }
        public string Name { get; protected set; }

        // wanted to make this a readonly but there was no way to pass the variable 
        // to the static constructor 
        // keeping the instance of StateManager private to Registry and GameStates 
        // to ensure that only those two are modifying states 
        protected static StateManager Manager { get; private set; }

        public GameState(string name)
        {
            Name = name;
            Active = true;
        }

        public static void InitManager(StateManager manager)
        {
            Manager = manager;
        }

        public virtual void Entered(GameState next)
        {
            string nextName = "[null]";
            if (next != null)
            {
                nextName = next.Name;
                //next.Obscuring();
                Next = next;
            }

            Console.WriteLine("Entered " + Name + ", obscuring: " + nextName);
        }

        public virtual void Leaving()
        {
            Console.WriteLine("Leaving " + Name);
            while (Next != null && !Next.Active)
            {
                Next.Leaving();
                Next = Next.Next;
                Next.Entered(Next.Next);
            }
        }

        //public virtual void Obscuring() { Console.WriteLine("Obscuring " + Name); }
        //public virtual void Revealed() { Console.WriteLine("Revealed " + Name); }

        public virtual void Update()
        {
            if (Next != null && UpdateNext)
                Next.Update();
        }

        public virtual void Draw(SpriteBatch batch)
        {
            if (Next != null && DrawNext)
                Next.Draw(batch);
        }
    }

    public class StateManager
    {
        public GameState Front { get; private set; }
        
        public StateManager()
        {
        }
        
        public void UpdateStates()
        {
            Front.Update();

            if (Front.Active == false)
            {
                Front.Leaving();
                Front = Front.Next;
            }
        }
        
        public void Push(GameState state)
        {
            state.Entered(Front);
            Front = state;
        }
       
        public void DrawStates(SpriteBatch batch)
        {
            if (Front != null)
                Front.Draw(batch);
        }
    }
}
