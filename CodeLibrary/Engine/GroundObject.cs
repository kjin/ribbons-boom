using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

namespace CodeLibrary.Engine
{
    /// <summary>
    /// Represents a group of "ground" pieces, each defined by a rectangle.
    /// The pieces may intersect, or even be disjoint.
    /// </summary>
    public class GroundObject : GameObject, I3D
    {
        public Body body;
        List<Fixture> outsideFixtures;
        List<Fixture> insideFixtures;
        public List<Rectangle> rectangles;
        GroundMap groundMap;

        /// <summary>
        /// Constructs a new ground object.
        /// </summary>
        /// <param name="canvas">The canvas associated with the game.</param>
        /// <param name="theme">The fill/edge texture theme.</param>
        /// <param name="rectangles">The rectangles that make up this object.</param>
        /// <param name="world">The physics world.</param>
        public GroundObject(Canvas canvas, ActAssets actAssets, List<Rectangle> rectangles, World world)
        {
            body = BodyFactory.CreateBody(world, this);
            body.BodyType = BodyType.Static;
            body.Position = new Vector2(0, 0);
            body.Rotation = 0;
            this.rectangles = rectangles;

            outsideFixtures = new List<Fixture>();
            insideFixtures = new List<Fixture>();
            float density = 1.0f;
            foreach (Rectangle rect in rectangles)
            {
                Vector2 rectCenter = new Vector2(rect.X + rect.Width/2f, rect.Y + rect.Height/2f);
                Vertices rectangleVertices =
                    PolygonTools.CreateRectangle(rect.Width / 2f - PhysicsConstants.GROUND_EPSILON_SMALL, rect.Height / 2f - PhysicsConstants.GROUND_EPSILON_SMALL, rectCenter, 0);
                PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
                Fixture fixture = body.CreateFixture(rectangleShape, this);
                fixture.CollisionCategories = PhysicsConstants.GROUND_CATEGORY;

                outsideFixtures.Add(fixture);

                rectangleVertices =
                    PolygonTools.CreateRectangle(rect.Width / 2f - PhysicsConstants.GROUND_EPSILON_LARGE, rect.Height / 2f - PhysicsConstants.GROUND_EPSILON_LARGE, rectCenter, 0);
                rectangleShape = new PolygonShape(rectangleVertices, density);
                fixture = body.CreateFixture(rectangleShape, this);
                fixture.CollisionCategories = PhysicsConstants.COLLISIONGROUND_CATEGORY;

                insideFixtures.Add(fixture);
            }
            groundMap = new GroundMap(canvas, actAssets.Theme, rectangles);
        }

        public void Update(float ft)
        {

        }

        /// <summary>
        /// Draws this ground object.
        /// </summary>
        /// <param name="c">The canvas associated with this game.</param>
        public void Draw(Canvas c)
        {
            c.DrawGroundMap2DPortion(groundMap);
        }

        public void Draw3D(Canvas c, Projections pass)
        {
            c.DrawGroundMap3DPortion(groundMap, pass);
        }
    }
}
