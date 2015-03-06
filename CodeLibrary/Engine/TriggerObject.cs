using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;

namespace CodeLibrary.Engine
{
    class TriggerObject
    {
        private Body body;
        private Fixture fixture;
        private List<String> script;

        public TriggerObject(Rectangle rect, List<String> script, World w)
        {
            this.script = script;
            
            this.body = BodyFactory.CreateBody(w, this);
            // initialize physics stuff if body was successfully created
            if (body != null)
            {
                body.BodyType = BodyType.Static;
                body.Position = new Vector2(rect.X,rect.Y);
                body.Rotation = 0;
                Vertices rectangleVertices = PolygonTools.CreateRectangle(rect.Width / 2, rect.Height / 2);
                PolygonShape rectangleShape = new PolygonShape(rectangleVertices, 1.0f);
                fixture = body.CreateFixture(rectangleShape, this);
            }
            body.IsSensor = true;
        }

        public void Play(Canvas c, Camera cam)
        {

        }
    }
}
