using CodeLibrary.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Engine
{
    public class RibbonElementPoint : PhysicalObject
    {
        Vector2 position;
        bool flipStatus;
        float ribbonPos;
        int ribbonID;

        public Vector2 Position { get { return position; } set { position = value; } }
        public bool FlipStatus { get { return flipStatus; } set { flipStatus = value; } }
        public float RibbonPos { get { return ribbonPos; } set { ribbonPos = value; } }
        public int RibbonID { get { return ribbonID; } set { ribbonID = value; } }

        public RibbonElementPoint(Sprite sprite, World w, Vector2 position, bool flipStatus, float ribbonPos, int ribbonID)
            :base(sprite, w, position)
        {
            this.flipStatus = flipStatus;
            this.position = new Vector2(position.X - 0.5f, position.Y - 0.5f);
            this.body.Enabled = false;
            this.ribbonPos = ribbonPos;
            this.ribbonID = ribbonID;
        }

        public override void Draw(Canvas c)
        {
            c.DrawRectangle(Color.IndianRed, Color.IndianRed, 1, new Rectangle((int)position.X, (int)position.Y, 1,1), false);
        }
    }
}
