using CodeLibrary.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Engine
{
    public class RibbonLabel : RibbonPointObject
    {
        int ribbonID;

        public int RibbonID { get { return ribbonID; } }
        public RibbonLabel(int id, Sprite sprite, World w, Vector2 position) :
            base("label",sprite, w, position)
        {
                this.ribbonID = id - 1;
        }
    }
}
