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
    class FlipBarObject : RibbonElement
    {
        #region Fields
        protected int elementTouching;
        protected int flippedelementTouching;
        protected SeamstressObject seamstress;
        #endregion

        #region Properties
        #endregion

        public FlipBarObject(World w, RibbonObject r, float pos, Sprite sprite, float rotation) :
            base(w, r, new BoxObject(sprite, w, new Vector2(0,0), rotation, BasicRectangleList(), new Vector2(0,0)), pos, rotation)
        {
            foreach (Fixture f in element.fixtures)
            {
                f.IsSensor = true;
            }
            foreach (Fixture f in flippedelement.fixtures)
            {
                f.IsSensor = true;
            }

            elementTouching = 0;
            flippedelementTouching = 0;
        }

        private static List<Rectangle> BasicRectangleList()
        {
            List<Rectangle> rects = new List<Rectangle>();
            rects.Add(new Rectangle(0, 0, 1, 1));
            return rects;

        }

        public void SeamstressTouchStarted(SeamstressObject seamstress, Body b)
        {
            if (b == element.body)
            {
                elementTouching++;
                this.seamstress = seamstress;
            }
            else
            {
                flippedelementTouching++;
            }
        }

        public void SeamstressTouchEnded(Body b)
        {
            if (b == element.body)
            {
                elementTouching--;
            }
            else
            {
                flippedelementTouching--;
            }
        }

        protected override void ExecuteFlip()
        {
            yScale = 0;

            PhysicalObject boxtemp = element;
            element = flippedelement;
            flippedelement = boxtemp;

            List<Fixture> listtemp = sensorContactFixtures;
            sensorContactFixtures = contactFixtures;
            contactFixtures = listtemp;

            foreach (Fixture f in element.fixtures)
            {
                f.IsSensor = true;
            }
            foreach (Fixture f in flippedelement.fixtures)
            {
                f.IsSensor = true;
            }

            if (elementTouching > 0)
            {
                seamstress.body.Position = element.body.Position;
            }

            int inttemp = elementTouching;
            elementTouching = flippedelementTouching;
            flippedelementTouching = inttemp;
        }

        public override void Update(float dt)
        {
            glowTheta += 0.1f;
            if (glowTheta > 2 * Math.PI)
            {
                glowTheta -= (float)(2 * Math.PI);
            }
        }
    }
}
