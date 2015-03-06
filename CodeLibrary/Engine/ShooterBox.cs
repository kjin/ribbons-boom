using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;

using CodeLibrary.Graphics;
using System.Diagnostics;

namespace CodeLibrary.Engine
{
    class ShooterBox : PhysicalObject
    {
        World w;
        public int shootCooldown;
        public int COOLDOWN_TIME;
        public LinkedList<BulletObject> listOfBullets;
        public Sprite bulletSprite;
        public float bulletspeed;
        public bool isMirrored;

        public ShooterBox(Sprite shooter, Sprite bullet, World w, Vector2 pos, float rot, int cool)
            : base(shooter, w, pos, rot)
        {
            this.w = w;
            COOLDOWN_TIME = cool;
            listOfBullets = new LinkedList<BulletObject>();
            bulletSprite = bullet;
            bulletspeed = 30f;
            this.isMirrored = false;
            //body.UserData = this;
        }

        private static List<Rectangle> BasicRectangleList()
        {
            List<Rectangle> rects = new List<Rectangle>();
            rects.Add(new Rectangle(0, 0, 1, 1));
            return rects;
            
        }

        private ShooterBox(Sprite shooter, Sprite bullet, World w, Vector2 pos, float rot, int cool, bool isMirrored)
            : base(shooter, w, pos, rot, BasicRectangleList(), new Vector2(0,0), isMirrored)
        {
            this.w = w;
            COOLDOWN_TIME = cool;
            listOfBullets = new LinkedList<BulletObject>();
            bulletSprite = bullet;
            bulletspeed = 30f;
            this.isMirrored = isMirrored;
            //body.UserData = this;
        }

        public override PhysicalObject CreateMirroredBoxObject()
        {
             return new ShooterBox(sprite, bulletSprite, World, body.Position, body.Rotation, COOLDOWN_TIME, true);
        }

        public void Shoot(float dt)
        {
            BulletObject b;
            if (isMirrored)
            {
                b = new BulletObject(bulletSprite, bulletspeed, (float)(body.Rotation + Math.PI), w, this);
            }
            else
            {
                b = new BulletObject(bulletSprite, bulletspeed, (float)(body.Rotation), w, this);
            }
            b.body.IgnoreCollisionWith(body);
            listOfBullets.AddLast(b);
        }

        public override void Draw(Canvas c, Color tint)
        {
            foreach (BulletObject b in listOfBullets)
            {
                b.Draw(c);
            }
            base.Draw(c, tint);
        }

        public override void Update(float dt)
        {
            if (shootCooldown > COOLDOWN_TIME)
            {
                Shoot(dt);
                shootCooldown = 0;
            }
            shootCooldown++;
            if (listOfBullets.Count > 120)
            {
                w.RemoveBody(listOfBullets.First.Value.body);
                listOfBullets.Remove(listOfBullets.First);
            }
        }
    }
}
