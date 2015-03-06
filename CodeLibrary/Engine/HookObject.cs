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
    public class HookObject : PhysicalObject
    {
        #region Fields
        SpriteCollection sprites;
        private const float HOOK_WIDTH_RATIO = 0.1f;
        private const float HOOK_CENTER_OFFSET = -0.038f;
        private const float TRIANGLE_HEIGHT = 0.2f;
        private const float TRIANGLE_WIDTH = 2.5f;
        private List<Vertices> vertexLists;
        #endregion

        public HookObject(SpriteCollection sprites, World w, Vector2 position, float rotation) :
            this(sprites, w, position, rotation, null) { }

        public HookObject(SpriteCollection sprites, World w, Vector2 position, float rotation, List<Rectangle> rects)
            : base(sprites[0], w, position, rotation, rects)
        {
            this.sprites = sprites;
            //CreateComplexShape(1.0f, new List<Rectangle>());
        }

        private HookObject(SpriteCollection sprites, World w, Vector2 position, float rotation, List<Rectangle> rects, bool isMirrored)
            : base(sprites[0], w, position, rotation, rects, new Vector2(0,0), isMirrored)
        {
            this.sprites = sprites;
            //CreateComplexShape(1.0f, new List<Rectangle>());
        }

        public override PhysicalObject CreateMirroredBoxObject()
        {
            return new HookObject(sprites, World, body.Position, body.Rotation, Rectangles, true);
        }

        protected override void CreateComplexShape(float density, List<Rectangle> rects)
        {
            base.CreateComplexShape(density, rects);

            foreach (Fixture fixture in fixtures)
            {
                fixture.CollidesWith = PhysicsConstants.COLLISIONGROUND_CATEGORY;
            }

            vertexLists = new List<Vertices>();
            
            // find lowest vertices
            Vector2 lowest = new Vector2(0,0);
            foreach (Vector2 vertex in vertices)
            {
                if (vertex.Y > lowest.Y) { lowest = vertex; }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 next = vertices.NextVertex(i);
                if (vertices[i].Y == next.Y && lowest.Y > next.Y)
                {
                    //this is a horizontal surface

                    Vector2 up = new Vector2(0, -TRIANGLE_HEIGHT);
                    Vector2 left = vertices[i] - next;
                    left.Normalize();
                    left *= TRIANGLE_WIDTH / 2;

                    //create a line fixture
                    Vertices lineVertices = new Vertices();
                    lineVertices.Add(vertices[i]);
                    lineVertices.Add(next);
                    vertexLists.Add(lineVertices);
                    Shape lineShape = new EdgeShape(vertices[i], next);
                    fixtures.Add(body.CreateFixture(lineShape, this));

                    // fucking stuff
                        //Vertices rectVertices = new Vertices();
                        //rectVertices.Add(vertices[i] - left);
                        //rectVertices.Add(next + left);
                        //rectVertices.Add(next + up + left);
                        //rectVertices.Add(vertices[i] + up - left);
                        //vertexLists.Add(rectVertices); // I don't remember why I was saving these. Maybe debug graphics?
                        //Shape bulletShape = new PolygonShape(rectVertices, density);
                        //Fixture temp = body.CreateFixture(bulletShape, this);
                        //temp.CollidesWith &= ~PhysicsConstants.GROUND_CATEGORY & ~PhysicsConstants.COLLISIONGROUND_CATEGORY;
                        //temp.IsSensor = true;
                        //fixtures.Add(temp);

                    //create triangular shape
                    //Vector2 left = vertices[i] - next;
                    //left.Normalize();
                    //left *= TRIANGLE_WIDTH / 2;
                    //Vector2 down = new Vector2(0, TRIANGLE_HEIGHT);

                    //Vector2 fixer = new Vector2(0, HOOK_CENTER_OFFSET);

                    //Vertices lineVertices = new Vertices();
                    //lineVertices.Add(vertices[i] - left + fixer);
                    //lineVertices.Add(next + left + fixer);
                    //lineVertices.Add(next - left + down / 2 + fixer);
                    //lineVertices.Add(next + left + down + fixer);
                    //lineVertices.Add(vertices[i] - left + down + fixer);
                    //lineVertices.Add(vertices[i] + left + down / 2 + fixer);

                    //vertexLists.Add(lineVertices);
                    //Shape lineShape = new PolygonShape(lineVertices, density);
                    
                    //fixtures.Add(body.CreateFixture(lineShape, this));
                }
            }
        }

        public override void Draw(Canvas c, Color tint)
        {
            int rows = layout.GetLength(0);
            int cols = layout.GetLength(1);

            Transform xf;
            body.GetTransform(out xf);

            for (int ii = 1; ii < rows - 1; ii++)
            {
                for (int jj = 1; jj < cols - 1; jj++)
                {
                    if (layout[ii, jj] == 1)
                    {
                        if (layout[ii - 1, jj] == 0)
                        {
                            c.DrawSprite(sprites[1], tint, Vector2.Transform(MathUtils.Mul(ref xf, new Vector2(jj, ii + HOOK_CENTER_OFFSET) + compensation), TransformationMatrix), body.Rotation, Scale);
                        }
                        else
                        {
                            c.DrawSprite(sprite, tint, Vector2.Transform(MathUtils.Mul(ref xf, new Vector2(jj, ii) + compensation), TransformationMatrix), body.Rotation, Scale);
                        }
                    }
                }
            }
            #if DEBUG
                // Debug origin markers
                c.DrawRectangle(Color.Red, 4, body.Position, new Vector2(sprite.Width / 2, sprite.Height / 2), body.Rotation, false);
            #endif
        }

    }
}