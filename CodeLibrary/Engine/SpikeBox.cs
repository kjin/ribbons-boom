using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeLibrary.Graphics;

using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;


namespace CodeLibrary.Engine
{
    public class SpikeBox : PhysicalObject, RibbonFeature
    {
        Vector2 centerPosition;

        Vector2 amplitude;
        float frequency;

        float theta;

        public SpikeBox(Sprite sprite, World w, Vector2 position, float rotation, List<Rectangle> rect, float amplitude, float frequency)
            : base(sprite, w, position, rotation, rect)
        {
            this.amplitude = new Vector2(0, amplitude);
            this.frequency = frequency;

            foreach (Fixture fixture in fixtures)
            {
                fixture.IsSensor = true;
            }

            centerPosition = position;

            theta = 0;
        }

        public void Move(float dx, float dt)
        {
            theta += frequency*dx;

            body.Position = centerPosition + ((float)Math.Sin(theta)) * amplitude;

        }
    }
}
