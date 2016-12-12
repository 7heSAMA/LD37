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
    public abstract class Prop : Actor, ITurf
    {
        public Avatar.Faction Tag { get; protected set; }

        public Prop(Vector2 pos, int foot, Avatar.Faction tag)
        {
            Body = new CRoundBody(this, pos, foot, 150.0f, 0.0f);

            Tag = tag;
        }

        public abstract void Interact(Avatar avatar);
    }

    public class Photo : Prop
    {
        private bool FaceUp;
        private CSprite Table;
        private CSprite Picture;
        private Avatar.Faction StoredTag;

        private static Rectangle UpSource = new Rectangle(0, 0, 12, 12);
        private static Rectangle DownSource = new Rectangle(12, 0, 12, 12);

        public Photo(Vector2 pos, Avatar.Faction tag, bool up)
            : base(pos, Tile.FOOT / 2, tag)
        {
            Body = new CRoundBody(this, pos, Tile.FOOT / 2, 150.0f, 0.0f);
            Table = new CSprite(this, "Props", new Rectangle(
                0, 12, Tile.FOOT, Tile.FOOT * 2 - 12), new Vector2(0.5f, (52.0f / 64.0f)));
            Picture = new CSprite(this, "Props", UpSource, new Vector2(0.4f, 2.9f));

            FaceUp = up;
            StoredTag = tag;

            if (up)
                Tag = StoredTag;
            else
            {
                Tag = Avatar.Faction.None;
                Picture.Animate(DownSource, SpriteEffects.None);
            }

            AddComp(Body);
            AddComp(Table);
            AddComp(Picture);
        }

        public override void Interact(Avatar avatar)
        {
            FaceUp = !FaceUp;
            if (FaceUp)
            {
                Tag = StoredTag;
                Picture.Animate(UpSource, SpriteEffects.None);
            }
            else
            {
                Tag = Avatar.Faction.None;
                Picture.Animate(DownSource, SpriteEffects.None);
            }
        }
    }

    public class Rack : Prop
    {
        private CSprite Stand;

        private Rectangle JacketedSource = new Rectangle(80, 0, 24, 48);
        private Rectangle EmptySource = new Rectangle(32 + 24, 0, 24, 48);

        private bool HasJacket = true;

        public Rack(Vector2 pos)
            : base(pos, 10, Avatar.Faction.Taylor)
        {
            Body = new CRoundBody(this, pos, 10, 150.0f, 0.0f);
            Stand = new CSprite(this, "Props", JacketedSource,
                new Vector2(0.5f, 38.0f / 48.0f));
            AddComp(Stand);
        }

        public override void Interact(Avatar avatar)
        {
            if (HasJacket)
            {
                HasJacket = false;
                Stand.Animate(EmptySource, SpriteEffects.None);
                Tag = Avatar.Faction.None;
                avatar.PutOnJacket();
            }
        }
    }
}
