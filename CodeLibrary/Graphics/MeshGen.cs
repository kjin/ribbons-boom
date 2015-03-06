﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeLibrary.Graphics
{
    public class TriangleMeshBuilder
    {
        public VertexPositionNormalTexture[] Vertices;
        public int[] Indices;
        int vertex;
        int index;

        public TriangleMeshBuilder(int numTriangles, int numRectangles)
        {
            Vertices = new VertexPositionNormalTexture[3 * numTriangles + 4 * numRectangles];
            Indices = new int[3 * numTriangles + 6 * numRectangles];
            vertex = 0;
            index = 0;
        }

        public void BuildTriangle(Vector3 a, Vector3 b, Vector3 c, Vector2 texCoords, Vector3 normal, bool recalculateNormal = true)
        {
            if (recalculateNormal)
            {
                Vector3 n2 = Vector3.Cross(b - a, c - a);
                n2.Normalize();
                if (Vector3.Dot(normal, n2) < 0)
                    n2 = -n2;
                normal = n2;
            }
            Indices[index++] = (int)vertex;
            Indices[index++] = (int)(vertex + 1);
            Indices[index++] = (int)(vertex + 2);
            Vertices[vertex++] = new VertexPositionNormalTexture(a, normal, texCoords);
            Vertices[vertex++] = new VertexPositionNormalTexture(b, normal, texCoords);
            Vertices[vertex++] = new VertexPositionNormalTexture(c, normal, texCoords);
        }

        public void BuildTriangle(Vector3 a, Vector3 b, Vector3 c, Vector2 texCoords)
        {
            Vector3 normal = Vector3.Cross(b - a, c - a);
            normal.Normalize();
            BuildTriangle(a, b, c, texCoords, normal, false);
        }

        public void BuildAxisAlignedRectangle(Vector3 a, Vector3 b, Vector3 normal, Vector2 texCoords)
        {
            Vector3 c = new Vector3();
            Vector3 d = new Vector3();
            if (a.X == b.X)
            {
                if (normal.X > 0)
                {
                    c = new Vector3(a.X, a.Y, b.Z);
                    d = new Vector3(a.X, b.Y, a.Z);
                }
                else
                {
                    c = new Vector3(a.X, b.Y, a.Z);
                    d = new Vector3(a.X, a.Y, b.Z);
                }
            }
            else if (a.Y == b.Y)
            {
                if (normal.Y > 0)
                {
                    c = new Vector3(b.X, a.Y, a.Z);
                    d = new Vector3(a.X, a.Y, b.Z);
                }
                else
                {
                    c = new Vector3(a.X, a.Y, b.Z);
                    d = new Vector3(b.X, a.Y, a.Z);
                }
            }
            else if (a.Z == b.Z)
            {
                if (normal.Z > 0)
                {
                    c = new Vector3(a.X, b.Y, a.Z);
                    d = new Vector3(b.X, a.Y, a.Z);
                }
                else
                {
                    c = new Vector3(b.X, a.Y, a.Z);
                    d = new Vector3(a.X, b.Y, a.Z);
                }
            }
            else
                return;
            Indices[index++] = (int)vertex;
            Indices[index++] = (int)(vertex + 1);
            Indices[index++] = (int)(vertex + 2);
            Indices[index++] = (int)(vertex + 2);
            Indices[index++] = (int)(vertex + 3);
            Indices[index++] = (int)vertex;
            Vertices[vertex++] = new VertexPositionNormalTexture(a, normal, texCoords);
            Vertices[vertex++] = new VertexPositionNormalTexture(c, normal, texCoords);
            Vertices[vertex++] = new VertexPositionNormalTexture(b, normal, texCoords);
            Vertices[vertex++] = new VertexPositionNormalTexture(d, normal, texCoords);
        }

        public void BuildBox(Vector3 a, Vector3 b, Vector2 texCoords)
        {
            BuildAxisAlignedRectangle(new Vector3(a.X, a.Y, a.Z), new Vector3(a.X, b.Y, b.Z), -Vector3.UnitX, texCoords);
            BuildAxisAlignedRectangle(new Vector3(b.X, a.Y, a.Z), new Vector3(b.X, b.Y, b.Z), Vector3.UnitX, texCoords);
            BuildAxisAlignedRectangle(new Vector3(a.X, a.Y, a.Z), new Vector3(b.X, a.Y, b.Z), -Vector3.UnitY, texCoords);
            BuildAxisAlignedRectangle(new Vector3(a.X, b.Y, a.Z), new Vector3(b.X, b.Y, b.Z), Vector3.UnitY, texCoords);
            BuildAxisAlignedRectangle(new Vector3(a.X, a.Y, a.Z), new Vector3(b.X, b.Y, a.Z), -Vector3.UnitZ, texCoords);
            BuildAxisAlignedRectangle(new Vector3(a.X, a.Y, b.Z), new Vector3(b.X, b.Y, b.Z), Vector3.UnitZ, texCoords);
        }
    }
}
