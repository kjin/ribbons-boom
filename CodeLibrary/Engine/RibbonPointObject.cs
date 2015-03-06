using CodeLibrary.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Engine
{
    public class RibbonPointObject : PhysicalObject
    {
        private Vector2 point;
        private String pointType;

        public Vector2 Point { get { return point; } set { point = value; } }

        public String PointType { get { return pointType; } set { pointType = value; } }

        public RibbonPointObject(String pointType, Sprite sprite, World w, Vector2 position) :
            base(sprite, w, position)
        {
            point = new Vector2(position.X, position.Y);
            this.pointType = pointType;
        }
    }
}
