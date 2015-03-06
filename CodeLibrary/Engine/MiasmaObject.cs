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
using System.Threading;

namespace CodeLibrary.Engine
{
    /// <summary>
    /// Represents a group of "ground" pieces, each defined by a rectangle.
    /// The pieces may intersect, or even be disjoint.
    /// </summary>
    public class MiasmaObject : GameObject, I3D
    {
        public Body body;
        List<Fixture> fixtures;
        GroundMap groundMap;
        public List<Rectangle> rectangles;
        LevelGridStore[,] gridModel;

        public LevelGridStore[,] GridModel { get { return gridModel; } set { gridModel = value; } }

        /// <summary>
        /// Constructs a new ground object.
        /// </summary>
        /// <param name="canvas">The canvas associated with the game.</param>
        /// <param name="theme">The fill/edge texture theme.</param>
        /// <param name="rectangles">The rectangles that make up this object.</param>
        /// <param name="world">The physics world.</param>
        public MiasmaObject(Canvas canvas, ActAssets actAssets, List<Rectangle> rectangles, World world)
        {
            body = BodyFactory.CreateBody(world, this);
            body.BodyType = BodyType.Static;
            body.Position = new Vector2(0, 0);
            body.Rotation = 0;

            this.rectangles = rectangles;

            fixtures = new List<Fixture>();
            float density = 1.0f;
            foreach (Rectangle rect in rectangles)
            {
                Vector2 rectCenter = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
                Vertices rectangleVertices =
                    PolygonTools.CreateRectangle(rect.Width / 2f - PhysicsConstants.MIASMA_EPSILON, rect.Height / 2f - PhysicsConstants.MIASMA_EPSILON, rectCenter, 0);
                PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
                Fixture fixture = body.CreateFixture(rectangleShape, this);
                fixture.IsSensor = true;

                fixtures.Add(fixture);
            }
            groundMap = new GroundMap(canvas, "miasma", rectangles, -0.03f);
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
            if (groundMap != null)
                c.DrawMiasmaAnimation(groundMap, gridModel);
        }

        public void Draw3D(Canvas c, Projections pass)
        {
            //c.DrawGroundMap3DPortion(groundMap, pass);
        }
    }
}
