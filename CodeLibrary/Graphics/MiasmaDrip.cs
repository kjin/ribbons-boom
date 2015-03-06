using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace CodeLibrary.Graphics
{
    public class MiasmaDrip : UserIndexedPrimitives<VertexPositionNormalTexture, short>
    {
        class MiasmaNode
        {
            Vector2 initialPosition;
            public Vector2 InitialPosition { get { return initialPosition; } }
            public Vector2 Value;
            Vector2 cachedValue;
            public MiasmaNode Left;
            public MiasmaNode Right;
            public MiasmaNode Up;
            public MiasmaNode Down;
            int index;
            public int Index { get { return index; } }
            int contributors;

            //osc settings
            bool oscEnabled;
            float amp;
            float period;
            float phase;

            public MiasmaNode(int index, Vector2 initialPosition)
            {
                this.initialPosition = initialPosition;
                this.index = index;
                contributors = -1;
                oscEnabled = false;// GraphicsHelper.Random.NextDouble() < 0.1;
                if (oscEnabled)
                {
                    amp = (float)GraphicsHelper.Random.NextDouble() / 2 + 0.5f;
                    period = GraphicsHelper.Random.Next(50, 70);
                    phase = (float)GraphicsHelper.Random.NextDouble();
                }
            }

            public void PreProcess()
            {
                //cachedValue = Value;
                if (contributors == -1)
                {
                    contributors = 0;
                    if (Left != null) contributors++;
                    if (Right != null) contributors++;
                    if (Up != null) contributors++;
                    if (Down != null) contributors++;
                }
            }

            public void Update(int time)
            {
                Vector2 acc = Vector2.Zero;
                if (Left != null)
                    acc += Left.cachedValue;
                if (Right != null)
                    acc += Right.cachedValue;
                if (Up != null)
                    acc += Up.cachedValue;
                if (Down != null)
                    acc += Down.cachedValue;
                Value = GraphicsConstants.MIASMA_DAMPING * (acc * (2f / contributors) - Value);
                if (oscEnabled)
                {
                    float angle = MathHelper.TwoPi * (time / period + phase);
                    Value.X = amp * ((float)Math.Cos(angle) + 1) / 2;
                    Value.Y = amp * ((float)Math.Sin(angle) + 1) / 2;
                }
            }

            public void PostProcess()
            {
                Vector2 temp = Value;
                Value = cachedValue;
                cachedValue = temp;
            }
        }

        List<MiasmaNode> nodes;
        VertexPositionNormalTexture[] vertices;
        short[] indices;
        int time = 0;

        int oscillator;

        private bool Valid(bool[,] fillMap, int nodesPerPoint, int x, int y)
        {
            return fillMap[x / nodesPerPoint, y / nodesPerPoint];
        }

        public MiasmaDrip(List<Rectangle> rectangles, int nodesPerPoint = 1)
        {
            Rectangle boundingBox;
            bool[,] fillMap = GraphicsHelper.GetFillMap(rectangles, out boundingBox, true);
            nodes = new List<MiasmaNode>();
            int previousRowIndex = 0;
            int previousRowYOffset = 0;
            int numSquares = 0;
            int xMax = (boundingBox.Width - 1) * nodesPerPoint;
            int yMax = (boundingBox.Height - 1) * nodesPerPoint;
            for (int i = 0; i <= xMax; i++)
            {
                float x = (float)i / nodesPerPoint;
                int rowYTop = yMax;
                int rowYBottom = 0;
                for (int j = 0; j <= yMax; j++)
                {
                    float y = (float)j / nodesPerPoint;
                    if (Valid(fillMap, nodesPerPoint, i, j))
                    {
                        if (x < boundingBox.Width && y < boundingBox.Height &&
                            Valid(fillMap, nodesPerPoint, i + 1, j) &&
                            Valid(fillMap, nodesPerPoint, i, j + 1) &&
                            Valid(fillMap, nodesPerPoint, i + 1, j + 1))
                            numSquares++;
                        if (j < rowYTop)
                            rowYTop = j;
                        if (j > rowYBottom)
                            rowYBottom = j;
                        MiasmaNode node = new MiasmaNode(nodes.Count, new Vector2(x + boundingBox.X, y + boundingBox.Y));
                        if (y > 0 && Valid(fillMap, nodesPerPoint, i, j - 1))
                        {
                            int prevIndex = nodes.Count - 1;
                            node.Left = nodes[prevIndex];
                            nodes[prevIndex].Right = node;
                        }
                        if (x > 0 && Valid(fillMap, nodesPerPoint, i - 1, j))
                        {
                            int prevIndex = previousRowIndex + j - previousRowYOffset;
                            node.Up = nodes[prevIndex];
                            nodes[prevIndex].Down = node;
                        }
                        nodes.Add(node);
                    }
                }
                previousRowIndex = nodes.Count + rowYTop - rowYBottom - 1;
                previousRowYOffset = rowYTop;
            }
            vertices = new VertexPositionNormalTexture[nodes.Count];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal = new Vector3(0, 0, -1);
                vertices[i].TextureCoordinate = new Vector2(0, 0);
            }
            UpdateVertexPositions();
            indices = new short[numSquares * 6];
            Dump("output.txt");
            int index = 0;
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i].Right != null && nodes[i].Down != null && nodes[i].Right.Down != null)
                {
                    indices[index++] = (short)nodes[i].Index;
                    indices[index++] = (short)nodes[i].Right.Down.Index;
                    indices[index++] = (short)nodes[i].Right.Index;
                    indices[index++] = (short)nodes[i].Down.Index;
                    indices[index++] = (short)nodes[i].Down.Right.Index;
                    indices[index++] = (short)nodes[i].Index;
                }

            oscillator = GraphicsHelper.Random.Next(0, nodes.Count);
        }

        protected void UpdateVertexPositions()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                //vertices[i].Position = new Vector3(nodes[i].InitialPosition.X + nodes[i].Value.X / 100, nodes[i].InitialPosition.Y + nodes[i].Value.Y / 100, 0);
                vertices[i].Position = new Vector3(nodes[i].InitialPosition.X, nodes[i].InitialPosition.Y, 0);
                vertices[i].TextureCoordinate = nodes[i].Value;
            }
        }

        public void Update()
        {
            int index = GraphicsHelper.Random.Next(0, 10 * nodes.Count);
            if (index < nodes.Count)
            {
                float angle = MathHelper.TwoPi * (float)GraphicsHelper.Random.NextDouble();
                nodes[index].Value.X = 5;// *(float)Math.Cos(angle);
                nodes[index].Value.Y = 5;// *(float)Math.Sin(angle);
            }
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].PreProcess();
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Update(time);
            UpdateVertexPositions();
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].PostProcess();
            time++;
        }

        public override VertexPositionNormalTexture[] Vertices
        {
            get { return vertices; }
        }

        public override short[] Indices
        {
            get { return indices; }
        }

        public override PrimitiveType PrimitiveType { get { return PrimitiveType.TriangleList; } }

        protected void Dump(string file)
        {
            using (TextWriter tw = new StreamWriter(file))
            {
                for (int i = 0; i < nodes.Count; i++)
                    tw.WriteLine("{0} {1}", nodes[i].InitialPosition.X, nodes[i].InitialPosition.Y);
            }
        }
    }
}