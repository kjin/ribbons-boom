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
    public class TelescopicBox : PhysicalObject
    {
        #region Fields
            public Boolean extended;
            public Boolean hasBeenExtended;
            protected Fixture telescope;
            protected Joint connection;
            public World world;
            protected SpriteCollection sprites;
            public int height;
        #endregion

        #region Methods
            public TelescopicBox(SpriteCollection s, World w, Vector2 pos, float rot, int height = 3, bool isMirrored = false) : 
                base(s[0],w,pos,rot,BasicRectangleList(), new Vector2(0,0), isMirrored)
            {
                this.sprites = s;
                this.height = height;

                telescope = null;
                if (body.Rotation >= MathHelper.Pi/2 || body.Rotation < -MathHelper.Pi/2)
                {
                    extended = true ;
                }
                else
                {
                    extended = false;
                }
                hasBeenExtended = false;
                world = w;
            }

            private static List<Rectangle> BasicRectangleList()
            {
                List<Rectangle> rects = new List<Rectangle>();
                rects.Add(new Rectangle(0, 0, 1, 1));
                return rects;
            
            }


            public override PhysicalObject CreateMirroredBoxObject()
            {
                return new TelescopicBox(sprites, world, body.Position, body.Rotation, height);
            }

            public override void Update(float dt)
            {
                float ep = .2f;

                while (body.Rotation > Math.PI)
                {
                    body.Rotation -= (float)Math.PI * 2;
                }
                while (body.Rotation < -Math.PI)
                {
                    body.Rotation += (float)Math.PI * 2;
                }

                if (body.Rotation < -MathHelper.Pi+ep || body.Rotation > MathHelper.Pi-ep  )
                {
                    extended = true;
                }
                else if (body.Rotation < ep && body.Rotation > -ep)
                {
                    extended = false;
                }

                if (extended && !hasBeenExtended)
                {
                    // create the bodies and fixtures on top of it
                    Extend();
                    hasBeenExtended = true;
                }
                else if (!extended && hasBeenExtended)
                {
                    // remove any bodies and fixtures that exist on top of it
                    Flatten();
                    hasBeenExtended = false;
                }
                #if DEBUG
               //Console.WriteLine(body.Rotation + " is the rotation");
                #endif
                base.Update(dt);
            }

            public void UpdateExtend(bool extended)
            {
                float ep = .2f;
                if (body.Rotation < -MathHelper.Pi + ep || body.Rotation > MathHelper.Pi - ep)
                {
                    extended = true;
                }
                else if (body.Rotation < ep && body.Rotation > -ep)
                {
                    extended = false;
                }
                else
                {
                    this.extended = extended;
                }

                if (extended && !hasBeenExtended)
                {
                    // create the bodies and fixtures on top of it
                    Extend();
                    hasBeenExtended = true;
                }
                else if (!extended && hasBeenExtended)
                {
                    // remove any bodies and fixtures that exist on top of it
                    Flatten();
                    hasBeenExtended = false;
                }
            }

            public override void Draw(Canvas c, Color tint)
            {
                base.Draw(c, tint);
                if (extended)
                {
                    Transform xf;
                    body.GetTransform(out xf);
                    for (int ii = 1; ii < height; ii++)
                    {
                        c.DrawSprite(sprites[1], tint, Vector2.Transform(MathUtils.Mul(ref xf, new Vector2(1, 1 - ii) + compensation), TransformationMatrix), body.Rotation);
                    }
                }
            }

            private void Extend()
            {
                Vector2 newPos = new Vector2(body.Position.X, body.Position.Y);
                //telescope = FixtureFactory.AttachRectangle(sprite.Width,height*sprite.Height,1,new Vector2(0,-sprite.Height),body);
                telescope = FixtureFactory.AttachRectangle(sprite.Width, height * sprite.Height, 1, new Vector2(0, -height / 2), body);
                telescope.UserData = this;
                telescope.CollidesWith = Category.All & ~PhysicsConstants.GROUND_CATEGORY;
                fixtures.Add(telescope);
                
                //connection = JointFactory.CreateWeldJoint(world, body, telescope, new Vector2(newPos.X+sprite.Width/2-.2f, newPos.Y), new Vector2(newPos.X+sprite.Width/2+.2f, newPos.Y));
                
                //Console.WriteLine("EXTENDED TELESCOPE BLOCK");
            }

            private void Flatten()
            {
                if (telescope != null)
                {
                    body.DestroyFixture(telescope);
                    fixtures.Remove(telescope);
                    telescope = null;
                }
                //connection = null;
                //Console.WriteLine("FLATTENED TELESCOPE BLOCK");
            }
        #endregion
    }
}
