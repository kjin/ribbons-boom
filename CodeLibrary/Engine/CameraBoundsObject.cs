using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

namespace CodeLibrary.Engine
{
    /// <summary>
    /// Constructs left and right boxes to prevent the seamstress from leaving the camera's view,
    /// and a KILLING FLOOR MUHAHAHA that kills the seamstress when she falls out from the bottom.
    /// </summary>
    public class CameraBoundsObject : GameObject
    {
        Body walls;
        Fixture leftFixture;
        Fixture rightFixture;
        Body floor;
        Fixture bottomFixture;
        RectangleF rect;

        public CameraBoundsObject(Camera camera, World world)
        {
            float density = 1.0f;
            rect = camera.Bounds;
            //rect.Inflate(camera.VisiblePhysical.Width / 2, camera.VisiblePhysical.Height / 2);
            Vector2 center = new Vector2((rect.Left + rect.Right) / 2f, (rect.Bottom + rect.Top) / 2f);

            //build walls
            walls = BodyFactory.CreateBody(world, PhysicalObjectTypes.SOLID);
            Vertices rectangleVertices = PolygonTools.CreateRectangle(1, rect.Height / 2f + 1, new Vector2(rect.Left - 1, center.Y), 0);
            PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
            leftFixture = walls.CreateFixture(rectangleShape, this);
            rectangleVertices = PolygonTools.CreateRectangle(1, rect.Height / 2f + 1, new Vector2(rect.Right + 1, center.Y), 0);
            rectangleShape = new PolygonShape(rectangleVertices, density);
            rightFixture = walls.CreateFixture(rectangleShape, this);

            //build floor
            floor = BodyFactory.CreateBody(world, PhysicalObjectTypes.SOLID | PhysicalObjectTypes.DEATH);
            rectangleVertices = PolygonTools.CreateRectangle(rect.Width / 2f + 1, 1, new Vector2(center.X, rect.Bottom + 1), 0);
            rectangleShape = new PolygonShape(rectangleVertices, density);
            bottomFixture = floor.CreateFixture(rectangleShape, this);
        }

        public void Update(float dt) { }

        public void Draw(Canvas c)
        {
            //c.DrawRectangle(Color.Red, 10, rect);
        }
    }
}
