using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using AnotherTimeOrPlace.Util;

namespace AnotherTimeOrPlace.Theater
{
    public class Avatar : Actor, ITurf
    {
        #region STATICS
        [Flags]
        public enum Faction
        {
            None = 0,
            Sam = 1,
            Taylor = 2,
        }

        public static Dictionary<Faction, Color> Shade { get; private set; }
        #endregion

        public Faction Tag { get; private set; }
        private CController Input;
        private CBrain Brain;
        private CSprite Sprite; // TODO: separate into legs + body 

        static Avatar()
        {
            Shade = new Dictionary<Faction, Color>();
            Shade.Add(Faction.Sam, Color.Lerp(Color.LightGray, Color.MonoGameOrange, 0.1f));
            Shade.Add(Faction.Taylor, Color.IndianRed);
            Shade.Add((Faction.Taylor | Faction.Sam), 
                Color.Lerp(Shade[Faction.Sam], Shade[Faction.Taylor], 0.5f));
            
            Console.WriteLine(string.Join("; ", Shade));
        }

        public Avatar(CController.Mode input, Vector2 pos, Faction name)
            : base()
        {
            Input = CController.MakeController(this, input);
            Body = new CRoundBody(this, pos, 8, 100.0f, 0.75f);
            Sprite = new CSprite(this, "Sprites", 
                new Rectangle(0, 0, 16, 40), CSprite.BotCenter);
            Brain = new CBrain(this, Sprite);

            AddComp(Input);
            AddComp(Brain);
            AddComp(Body);
            AddComp(Sprite);

            Tag = name;
        }

        public void PutOnJacket()
        {
            Tag = (Faction.Sam | Faction.Taylor);
            Brain.PutOnJacket();
        }
    }

    public class CBrain : CompBasic, IUpdate
    {
        private float AniTime;
        private int AniNum = 1;
        private CSprite Sprite;
        private SpriteEffects Fx;
        private int Height;

        public CBrain(IKeeper owner, CSprite sprite)
            : base(owner)
        {
            Sprite = sprite;
            Fx = SpriteEffects.FlipHorizontally;
        }

        public void Update()
        {
            if (Owner.Velocity.Length() > 0.5f)
            {
                if (Owner.Velocity.X < -0.5f)
                    Fx = SpriteEffects.None;
                else
                    Fx = SpriteEffects.FlipHorizontally;

                AniTime += Registry.CycleTime;

                if (AniTime > 6.0f)
                {
                    AniNum++;
                    if (AniNum > 4)
                        AniNum = 1;
                    Sprite.Animate(new Rectangle(16 * AniNum, Height, 16, 40), Fx);
                    AniTime = 0.0f;
                }
            }
            else
            {
                Sprite.Animate(new Rectangle(0, Height, 16, 40), Fx);
            }

            if (Registry.JustPressed(Keys.Space))
                CheckInteract();
        }
        
        private void CheckInteract()
        {
            float closest = 32.0f;
            Prop interactWith = null;

            foreach (Actor actor in Registry.Actors)
            {
                Prop prop = actor as Prop;
                if (prop != null)
                {
                    float distance = Vector2.Distance(Owner.Position, prop.Position);
                    if (interactWith == null && distance < closest)
                    {
                        closest = distance;
                        interactWith = prop;
                    }
                }
            }

            if (interactWith != null)
                interactWith.Interact(Owner as Avatar);
        }

        public void PutOnJacket()
        {
            Height = 40;
        }
    }
    
    public abstract class CController : CompBasic, IUpdate
    {
        protected float MaxMpS2 { get; set; } = 1.0f;

        public enum Mode
        {
            Keyboard,
            Mouse,
        }

        public static CController MakeController(IKeeper owner, Mode mode)
        {
            if (mode == Mode.Keyboard)
                return new CKeyController(owner);
            else
                return new CMouseController(owner);
        }

        protected CController(IKeeper owner)
            : base(owner)
        { }

        public abstract void Update();
    }

    public class CMouseController : CController
    {
        public CMouseController(IKeeper owner)
            : base(owner)
        { }

        public override void Update()
        {
            float distanceToMouse = Vector2.Distance(Owner.Position, Registry.MousePos);

            if (Registry.LeftClick() && distanceToMouse > Owner.Foot * 2)
            {
                Owner.Impulse(Gizmo.AngleAsVector(Gizmo.FindAngle(
                    Owner.Position, Registry.MousePos)) * 
                    MathHelper.Clamp(distanceToMouse / 15.0f, 0.0f, MaxMpS2));
            }
        }
    }

    public class CKeyController : CController
    {
        private Keys
           Up = Keys.W,
           Down = Keys.S,
           Left = Keys.A,
           Right = Keys.D;
        
        public CKeyController(IKeeper owner)
            : base(owner)
        { }

        public override void Update()
        {
            Vector2 accel = Vector2.Zero;

            if (Registry.Pressed(Up))
                accel.Y -= 1.0f;
            if (Registry.Pressed(Down))
                accel.Y += 1.0f;
            if (Registry.Pressed(Left))
                accel.X -= 1.0f;
            if (Registry.Pressed(Right))
                accel.X += 1.0f;

            if (accel != Vector2.Zero)
            {
                accel.Normalize();
                Owner.Impulse(accel * MaxMpS2);
            }
        }
    }
}
