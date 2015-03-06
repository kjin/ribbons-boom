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
    class RotatingObject : PhysicalObject, RibbonFeature
    {
        protected SpriteCollection sprites;
        private World world;
        private Vector2 vector21;
        private float p1;
        private int[,] p2;
        private Vector2 vector22;
        private float speed;

        /// <summary>
        /// Creates a new rotating object with specified layout and pivot point.
        /// </summary>
        /// <param name="sprite">sprite to use</param>
        /// <param name="w">world object</param>
        /// <param name="position">position of the pivot point</param>
        /// <param name="rotation">iinitial rotation</param>
        /// <param name="layout">layout of the tiles in this object</param>
        /// <param name="pivot">index of the pivot point corresponding to the layout tiles</param>
        public RotatingObject(SpriteCollection sprites, World w, Vector2 position, float rotation, List<Rectangle> rects, Vector2 pivot, float rotationSpeed = 2.0f) :
            base(sprites[0],w,position,rotation,rects,pivot + new Vector2(-1,0))
        {
            body.BodyType = BodyType.Kinematic;
            body.FixedRotation = false;
            this.sprites = sprites;
            speed = rotationSpeed;
        }

        /// <summary>
        /// Rotates the block when called while the ribbon is being moved.
        /// </summary>
        /// <param name="distance"></param>
        public void Move(float distance, float time)
        {
            body.Rotation += speed*distance;
        }

        public override void Draw(Canvas c)
        {
            base.Draw(c);
            c.DrawRectangle(Color.PapayaWhip, 4, body.Position, new Vector2(sprite.Width / 3, sprite.Height / 3), body.Rotation, false);
        }
    }
}
