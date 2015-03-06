using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

using CodeLibrary.Graphics;
using System.Diagnostics;

namespace CodeLibrary.Engine
{
    public class BoxObject : PhysicalObject
    {
        public BoxObject(Sprite sprite, World w, Vector2 position, float rotation, List<Rectangle> rects, Vector2 pivot) :
            base(sprite, w, position, rotation, rects, pivot) { }

        public BoxObject(Sprite sprite, World w, Vector2 position, float rotation, List<Rectangle> rects, Vector2 pivot, bool isMirrored) :
            base(sprite, w, position, rotation, rects, pivot, isMirrored) { }

        public override void Draw(Canvas c)
        {
            base.Draw(c);
            //c.DrawLine(Color.Gray, 2, this.body.FixtureList[0].Shape., false);
        }
    }
}
