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
    public abstract class PhysicalObject : GameObject
    {
        #region Fields
        public Sprite sprite;
        public Vector2 spriteDimension; // sprite dimensions
        public Vector2 dimension; // width and height of object
        public Vector2 scale;
        public Body body;
        public List<Fixture> fixtures;
        protected Vertices vertices;
        protected int[,] layout;
        protected Vector2 compensation; // coordinate transform for multiple blocks
        public Matrix TransformationMatrix;
        public Vector2 Scale;

        private World world;
        private List<Rectangle> rectangles;
        private Vector2 pivot;
        private int baseWidth;

        public float HitBoxTolerance;
        int id;

        private bool isMirrored;

        #endregion

        #region Properties
        public int ID { get { return id; } set { id = value; } }

        public int BaseWidth
        {
            get { return baseWidth; }
        }

        public bool IsMirrored
        {
            set { isMirrored = value; }
            get { return isMirrored; }
        }

        public World World
        {
            set { world = value; }
            get { return world; }
        }

        public List<Rectangle> Rectangles
        {
            set { rectangles = value; }
            get { return rectangles; }
        }

        public Vector2 Pivot
        {
            set { pivot = value; }
            get { return pivot; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create a new box at the origin. Activates physics and saves in single
        /// texture that is used to define box dimensions.
        /// </summary>
        /// <param name="sprite">Sprite applied to this box</param>
        /// <param name="w">World object this box is added to</param>
        public PhysicalObject(Sprite sprite, World w) :
            this(sprite, w, new Vector2(0, 0), 0) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite">Sprite applied to this box</param>
        /// <param name="w">World obect this box is added to</param>
        /// <param name="position">Position vector box will be initialized to</param>
        public PhysicalObject(Sprite sprite, World w, Vector2 position) :
            this(sprite, w, position, 0) { }

        /// <summary>
        /// Create a new box at the origin. Activates physics and saves in single
        /// texture that is used to define box dimensions.
        /// </summary>
        /// <param name="sprite">Sprite applied to this box</param>
        /// <param name="w">World object this box is added to</param>
        /// <param name="position">Position vector box will be initialized to</param>
        /// <param name="rotation">Initial rotation of box</param>
        public PhysicalObject(Sprite sprite, World w, Vector2 position, float rotation)
        {
            HitBoxTolerance = PhysicsConstants.OBJECT_EPSILON;
            this.world = w;
            this.pivot = new Vector2(0, 0);
            
            this.sprite = sprite;
            fixtures = new List<Fixture>();
            // old scaling stuff (unused)
            this.spriteDimension = new Vector2((float)sprite.Width, (float)sprite.Height);
            this.scale = new Vector2(spriteDimension.X / sprite.Width, spriteDimension.Y / sprite.Height);
            
            layout = new int[,] { {0, 0, 0}, {0, 1, 0}, {0, 0, 0} };
            this.vertices = new Vertices();
            
            int rows = layout.GetLength(0);
            //calculate vector transform for new shape
            int xx = 0;
            while (layout[rows - 2, xx] == 0)
            {
                xx++;
            }
            compensation = new Vector2(-xx, -(rows - 2));

            generateVertices();

            body = BodyFactory.CreateBody(w, this);
            // initialize physics stuff if body was successfully created
            if (body != null)
            {
                body.BodyType = BodyType.Static;
                body.Position = position;
                body.Rotation = rotation;
                CreateShape(1.0f);
            }
            TransformationMatrix = Matrix.Identity;
            Scale = Vector2.One;
        }

        /// <summary>
        /// Create one body but a bunch of shapes laid out according to the shape provided.
        /// </summary>
        /// <param name="sprite">Sprite to tile.</param>
        /// <param name="w">world object this gets added to.</param>
        /// <param name="position">Initial position of this object</param>
        /// <param name="rotation">Initial rotation of the box</param>
        /// <param name="layout">Talk to Kevin.</param>
        public PhysicalObject(Sprite sprite, World w, Vector2 position, float rotation, List<Rectangle> rects) :
            this(sprite,w,position,rotation,rects,new Vector2(0,0)){}

        /// <summary>
        /// Create one body but a bunch of shapes laid out according to the shape provided.
        /// </summary>
        /// <param name="sprite">Sprite to tile.</param>
        /// <param name="w">world object this gets added to.</param>
        /// <param name="position">Initial position of this object</param>
        /// <param name="rotation">Initial rotation of the box</param>
        /// <param name="layout">Talk to Kevin.</param>
        /// <param name="pivot">index of layout to place origin at (defaults to bottomleft most tile.</param>
        public PhysicalObject(Sprite sprite, World w, Vector2 position, float rotation, List<Rectangle> rects, Vector2 pivot)
        {
            this.world = w;
            this.rectangles = rects;
            this.pivot = pivot;
            HitBoxTolerance = PhysicsConstants.OBJECT_EPSILON;
            
            this.sprite = sprite;
            fixtures = new List<Fixture>();
            // old scaling stuff (unused)
            this.spriteDimension = new Vector2((float)sprite.Width, (float)sprite.Height);
            this.scale = new Vector2(spriteDimension.X / sprite.Width, spriteDimension.Y / sprite.Height);

            if (rects != null)
            {
                this.layout = generateLayoutFromRectangles(rects);
            }
            else
            {
                this.layout = new int[3, 3] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
                this.dimension = new Vector2(1, 1);
            }

            this.vertices = new Vertices();

            // compensation
            int rows = layout.GetLength(0);
            //calculate vector transform for new shape
            int xx = 0;
            while (layout[rows - 2, xx] == 0)
            {
                xx++;
            }
            compensation = new Vector2(-xx, -(rows - 2)) + pivot;
            

            generateVertices();

            body = BodyFactory.CreateBody(w, this);
            // initialize physics stuff if body was successfully created
            if (body != null)
            {
                body.BodyType = BodyType.Static;
                body.Position = position;
                body.Rotation = rotation;
                CreateComplexShape(1.0f,rects);
            }

            TransformationMatrix = Matrix.Identity;
            Scale = Vector2.One;
        }

        public virtual PhysicalObject CreateMirroredBoxObject()
        {
            return new BoxObject(sprite, world, body.Position, body.Rotation, Rectangles, Pivot, true);
        }

        /// <summary>
        /// Extra constructor only for use by the mirroring method.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="w"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="rects"></param>
        /// <param name="pivot"></param>
        /// <param name="isMirrored"></param>
        public PhysicalObject(Sprite sprite, World w, Vector2 position, float rotation, List<Rectangle> rects, Vector2 pivot, bool isMirrored)
        {
            this.sprite = sprite;
            this.isMirrored = isMirrored;
            this.rectangles = rects;
            this.pivot = pivot;
            HitBoxTolerance = PhysicsConstants.OBJECT_EPSILON;

            fixtures = new List<Fixture>();
            // old scaling stuff (unused)
            this.spriteDimension = new Vector2((float)sprite.Width, (float)sprite.Height);
            this.scale = new Vector2(spriteDimension.X / sprite.Width, spriteDimension.Y / sprite.Height);

            if (rects != null)
            {
                this.layout = generateLayoutFromRectangles(rects);
            }
            else
            {
                this.layout = new int[3, 3] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
                this.dimension = new Vector2(1, 1);
            }

            this.vertices = new Vertices();

            // compensation
            int rows = layout.GetLength(0);
            //calculate vector transform for new shape
            int xx = 0;
            while (layout[rows -2, xx] == 0)
            {
                xx++;
            }
            compensation = new Vector2(-xx, -(rows - 2)) + pivot;


            generateVertices();

            body = BodyFactory.CreateBody(w, this);
            // initialize physics stuff if body was successfully created
            if (body != null)
            {
                body.BodyType = BodyType.Static;
                body.Position = position;
                body.Rotation = rotation;
                CreateComplexShape(1.0f, rects);
            }

            TransformationMatrix = Matrix.Identity;
            Scale = Vector2.One;
        }

        public PhysicalObject(Sprite sprite, World w, Vector2 position, float rotation, List<Rectangle> rects, Vector2 pivot, bool isMirrored, float offset) :
            this(sprite,w,position,rotation,rects,pivot,isMirrored)
        {
            HitBoxTolerance = offset;
        }

        public PhysicalObject(Sprite sprite, World w, Vector2 position, float rotation, List<Rectangle> rects, Vector2 pivot, float offset) :
            this(sprite, w, position, rotation, rects, pivot)
        {
            HitBoxTolerance = offset;
        }

        /// <summary>
        /// Genertase an int array layout from a list of rectangles provided by xml file.
        /// </summary>
        /// <param name="rectangles"></param>
        /// <returns>Layout array.</returns>
        private int[,] generateLayoutFromRectangles(List<Rectangle> rectangles)
        {
            int[,] tempArray = new int[50, 50];
            int m = tempArray.GetLength(0);
            int n = tempArray.GetLength(1);

            int maxii = 0; int minii = m;
            int maxjj = 0; int minjj = n;

            foreach (Rectangle r in rectangles)
            {
                int ii = r.Y;
                int jj = r.X;
                // max sure the box will fit in the temp array
                if (ii > m || jj > n) { throw new IndexOutOfRangeException(); }

                tempArray[ii, jj] = 1;

                // keep track of used dimensions
                if (ii > maxii) { maxii = ii; }
                if (jj > maxjj) { maxjj = jj; }
                if (ii < minii) { minii = ii; }
                if (jj < minjj) { minjj = jj; }
            }

            // parse temp array into a layout array
            int[,] newArray = new int[maxii - minii + 3, maxjj - minjj + 3];
            for (int ii = minii; ii <= maxii; ii++)
            {
                for (int jj = minjj; jj <= maxjj; jj++)
                {
                    newArray[ii - minii + 1, jj - minjj + 1] = tempArray[ii, jj];
                }
            }

            // update new definition of dimensions
            dimension = new Vector2(newArray.GetLength(0) - 2,newArray.GetLength(1) - 2);

            // mirror if appropriate
            if (isMirrored)
            {
                tempArray = new int[newArray.GetLength(0), newArray.GetLength(1)]; 
                for (int ii = 0; ii < newArray.GetLength(0); ii++)
                {
                    for (int jj = 0; jj < newArray.GetLength(1); jj++)
                    {
                        tempArray[ii, jj] = newArray[newArray.GetLength(0) - ii - 1, jj];
                    }
                }
                newArray = tempArray;
            }

            return newArray;
        }

        /// <summary>
        /// Helper method to generate vertices from the provided layout
        /// </summary>
        private void generateVertices()
        {
            int rows = layout.GetLength(0);
            int cols = layout.GetLength(1);
            List<Vector2> vertexList = new List<Vector2>();

            // Shrink fixture slightly
            Vector2 topLeftEps = new Vector2(HitBoxTolerance, HitBoxTolerance);
            Vector2 topRightEps = new Vector2(-HitBoxTolerance, HitBoxTolerance);
            Vector2 botLeftEps = new Vector2(HitBoxTolerance, -HitBoxTolerance);
            Vector2 botRightEps = new Vector2(-HitBoxTolerance, -HitBoxTolerance);

            for (int ii = 0; ii < rows; ii++)
            {
                for (int jj = 0; jj < cols; jj++)
                {
                    if (layout[ii, jj] == 1)
                    {
                        // check if vertices should be added to list
                        if (layout[ii, jj - 1] + layout[ii - 1, jj - 1] + layout[ii - 1, jj] == 0 ||
                            layout[ii, jj - 1] + layout[ii - 1, jj - 1] + layout[ii - 1, jj] == 2)
                        {
                            Vector2 topLeft = new Vector2(jj - 0.5f, ii - 0.5f) + topLeftEps;
                            if (!vertexList.Contains(topLeft)) { vertexList.Add(topLeft); } //add top left
                        } 
                        if (layout[ii, jj - 1] + layout[ii + 1, jj - 1] + layout[ii + 1, jj] == 0 ||
                            layout[ii, jj - 1] + layout[ii + 1, jj - 1] + layout[ii + 1, jj] == 2) 
                        {
                            Vector2 botLeft = new Vector2(jj - 0.5f, ii + 0.5f) + botLeftEps;
                            if (!vertexList.Contains(botLeft)) { vertexList.Add(botLeft); } //add bottom left
                        }
                        if (layout[ii + 1, jj] + layout[ii + 1, jj + 1] + layout[ii, jj + 1] == 0 ||
                            layout[ii + 1, jj] + layout[ii + 1, jj + 1] + layout[ii, jj + 1] == 2) 
                        {
                            Vector2 botRight = new Vector2(jj + 0.5f, ii + 0.5f) + botRightEps;
                            if (!vertexList.Contains(botRight)) { vertexList.Add(botRight); } //add bottom right 
                        }
                        if (layout[ii, jj + 1] + layout[ii - 1, jj + 1] + layout[ii - 1, jj] == 0 ||
                            layout[ii, jj + 1] + layout[ii - 1, jj + 1] + layout[ii - 1, jj] == 2) 
                        {
                            Vector2 topRight = new Vector2(jj + 0.5f, ii - 0.5f) + topRightEps;
                            if (!vertexList.Contains(topRight)) { vertexList.Add(topRight); } //add top right
                        } 
                    }
                }
            }

            // add vertex in list to the vertices in a coherent order
            Vector2 cursor = vertexList[0];

            // find next closest vertex and add it next
            int count = vertexList.Count;
            
            for (int ii = 0; ii < count; ii++)
            {
                vertices.Add(cursor + compensation);
                vertexList.Remove(cursor);
                float dist = float.PositiveInfinity;
                Vector2 closest = new Vector2(0, 0);
                foreach (Vector2 target in vertexList)
                {
                    Vector2 temp = cursor - target;
                    if (temp.Length() < dist && (temp.X == 0 || temp.Y == 0)) { dist = temp.Length(); closest = target; } 
                }
                cursor = closest;
            }

            FindBaseWidth();

            #if DEBUG
                //Console.WriteLine(count);
                //Console.WriteLine(vertices);
            #endif
        }

        private void FindBaseWidth()
        {
            // find lowest vertices
            Vector2 lowest = new Vector2(0, 0);
            foreach (Vector2 vertex in vertices)
            {
                if (vertex.Y > lowest.Y) { lowest = vertex; }
            }
            // find the other lowest and find max and min X
            float minX = float.PositiveInfinity; float maxX = 0;
            foreach (Vector2 vertex in vertices)
            {
                if (vertex.Y == lowest.Y)
                {
                    if (vertex.X < minX) minX = vertex.X;
                    else if (vertex.X > maxX) maxX = vertex.X;
                }
            }
            baseWidth = (int) Math.Round(maxX - minX);
        }

        /// <summary>
        /// Updates the object's physics state (NOT GAME LOGIC).
        /// </summary>
        /// <remarks>Used to buffer changes to the fixture (which must be
        /// destroyed and recreated to change.</remarks>
        public virtual void Update(float dt)
        {
        }

        public virtual void Draw(Canvas c)
        {
            Draw(c, Color.White);
        }

        public virtual void Draw(Canvas c, Color tint)
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
                        int column = GraphicsHelper.Mod(jj - 1, sprite.Columns);
                        int row = GraphicsHelper.Mod(ii - 1, sprite.Rows);
                        sprite.CurrentFrame = sprite.Columns * row + column;
                        c.DrawSprite(sprite, tint, Vector2.Transform(MathUtils.Mul(ref xf, new Vector2(jj, ii) + compensation), TransformationMatrix), body.Rotation, Scale, isMirrored);

                    }
                }
            }

            #if DEBUG
                // Debug origin markers
                c.DrawRectangle(Color.LightGreen, 4, body.Position, new Vector2(sprite.Width / 2, sprite.Height / 2), body.Rotation, false);
                // Debug physics shapes
                List<Vector2> debugger = vertices.ToArray().ToList();
                for (int ii = 0; ii < debugger.Count; ii++) { debugger[ii] = MathUtils.Mul(ref xf, debugger[ii]); }
                c.DrawPolygon(Color.Blue, 4, debugger, false);
            #endif
        }

        public void DrawSelection(Canvas c)
        {
            c.DrawRectangle(Color.LightGreen, 4, body.Position, new Vector2(sprite.Width, sprite.Height), body.Rotation, false);
        }

        /// <summary>
        /// Create a new box shape.
        /// </summary>
        protected virtual void CreateShape(float density)
        {
            foreach (Fixture fixture in fixtures)
            {
            if (fixture != null)
            {
                body.DestroyFixture(fixture);
                    fixtures.Remove(fixture);
                }
            }


            Vertices rectangleVertices = PolygonTools.CreateRectangle(spriteDimension.X / 2 - 2*HitBoxTolerance, spriteDimension.Y / 2 - 2*HitBoxTolerance);


            PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
            fixtures.Add(body.CreateFixture(rectangleShape, this));
        }

        /// <summary>
        /// Create a box with a complicated geometry by tiling regular shapes.
        /// </summary>
        /// <param name="density"></param>
        protected virtual void CreateComplexShape(float density, List<Rectangle> rects)
        {
            for (int i = 0; i < fixtures.Count; i++)
            {
                if (fixtures[i] != null)
                {
                    body.DestroyFixture(fixtures[i]);
                        fixtures.Remove(fixtures[i]);
                }
            }
            fixtures.Clear();

            if (rects == null) return;
            foreach (Rectangle rect in rects)
            {
                Vertices rectangleVertices = PolygonTools.CreateRectangle(rect.Width / 2.0f - 2*HitBoxTolerance, rect.Height / 2.0f - 2*HitBoxTolerance, new Vector2(rect.Center.X + 1, rect.Center.Y + 1) + compensation, 0.0f);
                PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
                fixtures.Add(body.CreateFixture(rectangleShape, this));
            }
        }
        #endregion
    }
}
