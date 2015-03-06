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
    class BulletObject
    {
        public float vel;
        public float dir;
        public Body body;
        public ShooterBox shotFrom;
        public Sprite sprite;
        public Boolean removed;
        public World w;
        public Fixture sensorFixture;

        public BulletObject(Sprite s, float velocity, float direction, World w, ShooterBox fired)
        {
            dir = direction;
            vel = velocity*4;
            body = BodyFactory.CreateCircle(w, .05f, 1f, new Vector2(fired.body.Position.X, fired.body.Position.Y - .08f));
            body.FixtureList[0].CollisionGroup = -1;
            body.IgnoreCollisionWith(fired.body);
            body.IgnoreGravity = true;
            body.BodyType = BodyType.Dynamic;
            body.Mass = .1f;
            body.IsBullet = true;
            Vector2 force = new Vector2(-vel * (float)Math.Cos(dir), -vel * (float)Math.Sin(dir));
            body.ApplyForce(force,body.Position);
            //body.BodyType = BodyType.Kinematic;
            //body.LinearVelocity = new Vector2(-vel * (float)Math.Cos(dir), vel * (float)Math.Sin(dir));
            shotFrom = fired;
            sprite = s;
            body.UserData = this;
            this.w = w;

            // collision extrasensory perception
            Vertices rectangleVertices = PolygonTools.CreateRectangle(0.3f, 0.3f);
            PolygonShape rectangleShape = new PolygonShape(rectangleVertices, 1.0f);
            sensorFixture = body.CreateFixture(rectangleShape, this);
            sensorFixture.IsSensor = true;
        }

        public bool Removed
        {

            get { return removed; }
        }

        public void Collision()
        {
            if (!removed)
            {
                w.RemoveBody(body);
                shotFrom.listOfBullets.Remove(this);
                removed = true;
            }
        }

        public void Draw(Canvas c)
        {
            c.DrawSprite(sprite, Color.White, new Vector2(body.Position.X,body.Position.Y + .05f));
           /* List<Vector2> l = new List<Vector2>();
            Vertices cs = ((PolygonShape)rect.Shape).Vertices;
            Transform xf;
            body.GetTransform(out xf);
            foreach (Vector2 v in cs)
            {
                Vector2 v1 = MathUtils.Mul(ref xf, v);
                l.Add(v1);
            }

            c.DrawPolygon(Color.DarkSalmon, 10, l, false);
            * */
        }
    }
}
