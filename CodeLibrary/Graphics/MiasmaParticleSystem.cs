using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeLibrary.Graphics
{
    public class MiasmaParticleSystem : UserIndexedPrimitives<VertexPositionNormalTexture, short>
    {
        VertexPositionNormalTexture[] vertices;
        short[] indices;

        List<MiasmaParticle> particles;
        int numParticles;

        static short[] numbers = new short[] { 4, 0, 6, 2, 6, 0, 0, 1, 2, 3, 2, 1, 0, 4, 1, 5, 1, 4, 1, 5, 3, 7, 3, 5, 3, 7, 2, 6, 2, 7, 5, 4, 7, 6, 7, 4 };

        public MiasmaParticleSystem(int numParticles)
        {
            particles = new List<MiasmaParticle>();
            this.numParticles = numParticles;
            for (int i = 0; i < numParticles; i++)
            {
                MiasmaParticle p = new MiasmaParticle(new Vector3(GraphicsHelper.Random.Next(-10, 11), GraphicsHelper.Random.Next(-5, 6), 0), (i / (numParticles - 1f)) * Vector2.One);
                particles.Add(p);
                for (int j = 0; j < 1; j++)
                {
                    p = new MiasmaParticle(p);
                    particles.Add(p);
                }
            }
            vertices = new VertexPositionNormalTexture[8 * particles.Count];
            indices = new short[numbers.Length * particles.Count];
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].SetVertices(vertices, i);
                for (int j = 0; j < numbers.Length; j++)
                    indices[numbers.Length * i + j] = (short)(8 * i + numbers[j]);
            }
        }

        public void Update()
        {
            for (int i = 0; i < particles.Count; i++)
                particles[i].Update();
        }

        public int NumParticles { get { return numParticles; } }

        public override VertexPositionNormalTexture[] Vertices
        {
            get { return vertices; }
        }

        public override short[] Indices
        {
            get { return indices; }
        }

        public override PrimitiveType PrimitiveType
        {
            get { return PrimitiveType.TriangleList; }
        }
    }

    public class MiasmaParticle
    {
        Vector3 position;
        Vector3 velocity;
        float size;
        Vector2 texCoords;
        MiasmaParticle parent;

        VertexPositionNormalTexture[] vertices;
        int index;

        static float childSize = 0.75f;

        public MiasmaParticle(Vector3 position, Vector2 texCoords)
        {
            this.position = position;
            size = 0.0625f;
            this.texCoords = texCoords;
        }

        public MiasmaParticle(MiasmaParticle parent)
        {
            position = parent.position;
            size = parent.size * childSize;
            this.texCoords = parent.texCoords;
            this.parent = parent;
        }

        public void SetVertices(VertexPositionNormalTexture[] vertices, int id)
        {
            this.vertices = vertices;
            this.index = 8 * id;
            for (int i = 0; i < 8; i++)
            {
                vertices[index + i].TextureCoordinate = this.texCoords;
            }
            float root3Over2 = (float)Math.Sqrt(3.0) / 2f;
            vertices[index    ].Normal = new Vector3(-root3Over2, -root3Over2, -root3Over2);
            vertices[index + 1].Normal = new Vector3(root3Over2, -root3Over2, -root3Over2);
            vertices[index + 2].Normal = new Vector3(-root3Over2, root3Over2, -root3Over2);
            vertices[index + 3].Normal = new Vector3(root3Over2, root3Over2, -root3Over2);
            vertices[index + 4].Normal = new Vector3(-root3Over2, -root3Over2, root3Over2);
            vertices[index + 5].Normal = new Vector3(root3Over2, -root3Over2, root3Over2);
            vertices[index + 6].Normal = new Vector3(-root3Over2, root3Over2, root3Over2);
            vertices[index + 7].Normal = new Vector3(root3Over2, root3Over2, root3Over2);
            UpdateVertices();
        }

        public void Update()
        {
            if (parent != null)
            {
                Vector3 thisToParent = parent.position - position;
                if (thisToParent.LengthSquared() == 0)
                    velocity = Vector3.Zero;
                else
                {
                    thisToParent.Normalize();
                    velocity *= Vector3.Dot(velocity, thisToParent);
                }
            }
            position += velocity;
            UpdateVertices();
        }

        private void UpdateVertices()
        {
            vertices[index    ].Position = position + new Vector3(-size, -size, -size);
            vertices[index + 1].Position = position + new Vector3(size, -size, -size);
            vertices[index + 2].Position = position + new Vector3(-size, size, -size);
            vertices[index + 3].Position = position + new Vector3(size, size, -size);
            vertices[index + 4].Position = position + new Vector3(-size, -size, size);
            vertices[index + 5].Position = position + new Vector3(size, -size, size);
            vertices[index + 6].Position = position + new Vector3(-size, size, size);
            vertices[index + 7].Position = position + new Vector3(size, size, size);
        }

        public Vector3 Position { get { return position; } }
        public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    }
}
