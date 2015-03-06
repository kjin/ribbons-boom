using CodeLibrary.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Engine
{
    public class RibbonElementStore : PhysicalObject
    {
        int ribbonID;
        float ribbonPos;
        bool ribbonFlipped;
        string type;
        Vector2 position;

        public Vector2 Position { get { return position; } set { position = value; } }
        public string Type { get { return type; } set { type = value; } }

        public int RibbonID { get { return ribbonID; } set { ribbonID = value; } }
        public float RibbonPos { get { return ribbonPos; } set { ribbonPos = value; } }
        public bool RibbonFlipped { get { return ribbonFlipped; } set { ribbonFlipped = value; } }

        public RibbonElementStore(string type, int ribbonID, Sprite sprite, World w, Vector2 position, float ribbonPos, bool ribbonFlipped)
            :base(sprite, w, position)
        {
            this.type = type;
            this.body.Enabled = false;
            this.ribbonPos = ribbonPos;
            this.ribbonID = ribbonID;
            this.ribbonFlipped = ribbonFlipped;
            this.position = position;
        }

        public override void Draw(Canvas c) {
            c.DrawRectangle(Color.Red, 4, new Rectangle((int)this.body.Position.X, (int)this.body.Position.Y - 1, 1, 1), false);
        }
    }
}
