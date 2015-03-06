using CodeLibrary.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Engine
{
    class GridStoreObject : PhysicalObject
    {
        string type;
        int id;
        int width;
        int height;

        public int Height { get { return height; } set { height = value; } }
        public int Width { get { return width; } set { width = value; } }
        public string Type { get { return type; } set { type = value; } }
        public int ID { get { return id; } set { id = value; } }
        public GridStoreObject(Canvas c, World w, Vector2 position, string type, int id)
            :base(Sprite.Build(c,"1x1plat"), w, position)
        {
            this.type = type;
            this.id = id;
        }
    }
}
