using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using CodeLibrary.Graphics;
using CodeLibrary.Audio;

namespace CodeLibrary.Engine
{
    public class RibbonElement
    {
        protected World world;

        public RibbonObject ribbon;

        public PhysicalObject element;
        public PhysicalObject flippedelement;
        protected Vector2 perpendicular;
        //public Fixture ribbonSensor;

        protected Body hinge1;
        protected Joint j1;
        protected Joint flipj1;
        public float pos1;

        protected Body hinge2;
        protected Joint j2;
        protected Joint flipj2;
        protected float pos2;

        protected float glowTheta;

        protected float lastDx;

        protected float seamstressOnRibbon;

        protected int flipSoundCooldown;

        protected bool playFlipSound;

        protected List<Fixture> contactFixtures;
        protected List<Fixture> sensorContactFixtures;
        protected List<Fixture> contactSensors;
        protected List<Fixture> sensorContactSensors;

        protected int contactCooldown;

        protected float originalRotation;

        protected const int CONTACT_COOLDOWN_DURATION = 10;

        protected float yScale;
        bool flipped;

        RibbonSounds ribbonSounds;

        #region Properties
        public bool Flipped { get { return flipped; } set { flipped = value; } }
        public Vector2 Perpendicular
        {
            get { return perpendicular; }
        }

        public int ContactCooldown
        {
            get { return contactCooldown; }
        }

        public bool PlayFlipSound
        {
            get { return playFlipSound; }
        }

        public Body Body
        {
            get { return element.body; }
        }
        #endregion

        public RibbonElement(World w, RibbonObject r, PhysicalObject b, float pos, float rotation = 0.0f, bool flipped = false)
        {
            world = w;
            ribbon = r;

            playFlipSound = false;
            flipSoundCooldown = 2000;

            pos1 = pos;
            pos2 = pos + 1;
            //need to go back later and generalize for multiple widths:
            //pos2 = pos + b.baseWidth

            element = b;
            flippedelement = element.CreateMirroredBoxObject();

            hinge1 = BodyFactory.CreateBody(world);
            hinge1.BodyType = BodyType.Kinematic;

            hinge2 = BodyFactory.CreateBody(world);
            hinge2.BodyType = BodyType.Kinematic;

            hinge1.Position = r.PositionToPoint(pos1);
            hinge2.Position = r.PositionToPoint(pos2);

            Vector2 v = hinge2.Position - hinge1.Position;
            v.Normalize();
            perpendicular = new Vector2(v.Y, -v.X);

            //need to use correct height
            element.body.Position = (hinge1.Position + hinge2.Position + perpendicular) / 2;
            element.body.BodyType = BodyType.Dynamic;

            flippedelement.body.Position = (hinge1.Position + hinge2.Position - b.spriteDimension.Y * perpendicular) / 2;
            flippedelement.body.BodyType = BodyType.Dynamic;

            foreach (Fixture f in flippedelement.fixtures)
            {
                f.IsSensor = true;
            }

            v = hinge1.Position - hinge2.Position;

            Vector2 v1 = hinge2.Position - hinge1.Position;
            element.body.Rotation = (float)Math.Atan2(v1.Y, v1.X) + rotation;
            flippedelement.body.Rotation = (float)Math.Atan2(v1.Y, v1.X) + rotation + (float)Math.PI;

            /*
            if (flipped)
            {
                j2 = JointFactory.CreateRevoluteJoint(world, element.body, hinge2, new Vector2(0, 0));
                flipj2 = JointFactory.CreateRevoluteJoint(world, flippedelement.body, hinge2, new Vector2(0, 0));

                j1 = JointFactory.CreateRevoluteJoint(world, element.body, hinge1, new Vector2(0, 0));
                flipj1 = JointFactory.CreateRevoluteJoint(world, flippedelement.body, hinge1, new Vector2(0, 0));
            }
            else
            {
                j1 = JointFactory.CreateRevoluteJoint(world, element.body, hinge1, new Vector2(0, 0));
                flipj1 = JointFactory.CreateRevoluteJoint(world, flippedelement.body, hinge1, new Vector2(0, 0));

                j2 = JointFactory.CreateRevoluteJoint(world, element.body, hinge2, new Vector2(0, 0));
                flipj2 = JointFactory.CreateRevoluteJoint(world, flippedelement.body, hinge2, new Vector2(0, 0));
            }*/

            j1 = JointFactory.CreateRevoluteJoint(world, element.body, hinge1, new Vector2(0, 0));
            flipj1 = JointFactory.CreateRevoluteJoint(world, flippedelement.body, hinge1, new Vector2(0, 0));

            j2 = JointFactory.CreateRevoluteJoint(world, element.body, hinge2, new Vector2(0, 0));
            flipj2 = JointFactory.CreateRevoluteJoint(world, flippedelement.body, hinge2, new Vector2(0, 0));

            element.body.UserData = this;
            flippedelement.body.UserData = this;

            
            foreach (Fixture f in element.fixtures)
            {
                f.CollidesWith &= ~PhysicsConstants.GROUND_CATEGORY;
            }
            foreach (Fixture f in flippedelement.fixtures)
            {
                f.CollidesWith &= ~PhysicsConstants.GROUND_CATEGORY;
            }

            this.flipped = flipped;

            if (flipped)
            {
                ExecuteFlip();
            }
            
            contactFixtures = new List<Fixture>();
            sensorContactFixtures = new List<Fixture>();
            contactSensors = new List<Fixture>();
            sensorContactSensors = new List<Fixture>();

            yScale = 1;

            contactCooldown = 0;
            originalRotation = rotation;
        }

        public void SetRibbonSounds(RibbonSounds ribbonSounds)
        {
            this.ribbonSounds = ribbonSounds;
        }

        public void Move(float dx, float dt)
        {
            Vector2 v;

            pos1 += dx;
            pos2 += dx;

            Vector2 v5 = (ribbon.PositionToPoint(pos1) - hinge1.Position) / dt;
            hinge1.LinearVelocity = v5;

            Vector2 v6 = (ribbon.PositionToPoint(pos2) - hinge2.Position) / dt;
            hinge2.LinearVelocity = v6;

            v = hinge2.Position - hinge1.Position;
            Vector2 v3 = v;
            Vector2 v4 = -v;

            
            float a1 = (float)Math.Atan2(v3.Y, v3.X);
            float a2 = (float)Math.Atan2(v4.Y, v4.X);
            float a3 = element.body.Rotation;

            float d1 = (float)Math.Min(Math.Min(Math.Abs(a1 - a3), Math.Abs(a1 - a3 + 2.0 * Math.PI)), Math.Abs(a1 - a3 - 2.0 * Math.PI));
            float d2 = (float)Math.Min(Math.Min(Math.Abs(a2 - a3), Math.Abs(a2 - a3 + 2.0 * Math.PI)), Math.Abs(a2 - a3 - 2.0 * Math.PI));

            if (d1 < d2)
            {
                element.body.Rotation = a1;
                flippedelement.body.Rotation = a1 + (float)Math.PI;
            }
            else
            {
                element.body.Rotation = a2;
                flippedelement.body.Rotation = a2 + (float)Math.PI;
            }

            if (seamstressOnRibbon > 0)
            {
                ribbon.seamstress.RibbonSpeed = (v5 + v6) / 2;
            }

            lastDx = dx;


        }

        public void Flip()
        {
            if (Vector2.Distance(ribbon.seamstress.body.Position, hinge1.Position) <= ribbon.FLIP_DISTANCE
                || Vector2.Distance(ribbon.seamstress.body.Position, hinge2.Position) <= ribbon.FLIP_DISTANCE)
            {
                if (sensorContactFixtures.Count <= 0)
                {
                    ExecuteFlip();
                }
            }
            flipped = !flipped;
        }

        protected virtual void ExecuteFlip()
        {
            playFlipSound = !playFlipSound;
            Vector2 center = (hinge1.Position + hinge2.Position) / 2;
            perpendicular = element.body.Position - center;

            yScale = -1;

            PhysicalObject boxtemp = element;
            element = flippedelement;
            flippedelement = boxtemp;

            List<Fixture> listtemp = sensorContactFixtures;
            sensorContactFixtures = contactFixtures;
            contactFixtures = listtemp;

            foreach (Fixture f in element.fixtures)
            {
                f.IsSensor = false;
            }
            foreach (Fixture f in flippedelement.fixtures)
            {
                f.IsSensor = true;
            }

        }

        public void Contact(Fixture f)
        {
            UpdateContacts();
            if (f.IsSensor)
            {
                contactSensors.Add(f);
            }
            else
            {
                contactFixtures.Add(f);
            }
        }
  
        public void ContactEnded(Fixture f)
        {
            UpdateContacts();
            if (f.IsSensor)
            {
                contactSensors.Remove(f);
            }
            else
            {
                contactFixtures.Remove(f);
            }
        }
 
        public void SensorContact(Fixture f)
        {
            UpdateContacts();
            if (f.IsSensor)
            {
                sensorContactSensors.Add(f);
            }
            else
            {
                sensorContactFixtures.Add(f);
            }
        }

        public void SensorContactEnded(Fixture f)
        {
            UpdateContacts();
            if (f.IsSensor)
            {
                sensorContactSensors.Remove(f);
            }
            else
            {
                sensorContactFixtures.Remove(f);
            }
        }

        private void UpdateContacts()
        {
            List<Fixture> templist = new List<Fixture>();

            foreach (Fixture f in contactFixtures)
            {
                if (f.IsSensor)
                {
                    contactSensors.Add(f);
                }
                else
                {
                    templist.Add(f);
                }
            }
            contactFixtures = templist;

            templist = new List<Fixture>();
            foreach (Fixture f in contactSensors)
            {
                if (f.IsSensor)
                {
                    templist.Add(f);
                }
                else
                {
                    contactFixtures.Add(f);
                }
            }
            contactSensors = templist;

            templist = new List<Fixture>();
            foreach (Fixture f in sensorContactFixtures)
            {
                if (f.IsSensor)
                {
                    sensorContactSensors.Add(f);
                }
                else
                {
                    templist.Add(f);
                }
            }
            sensorContactFixtures = templist;

            templist = new List<Fixture>();
            foreach (Fixture f in sensorContactSensors)
            {
                if (f.IsSensor)
                {
                    templist.Add(f);
                }
                else
                {
                    sensorContactFixtures.Add(f);
                }
            }
            sensorContactSensors = templist;

        }

        public bool GetContact()
        {
            /*if (contact - sensorContact > 0)
            {
                Console.WriteLine("jigglypuff!");
            }*/
            return contactFixtures.Count > 0;
        }

        public void SeamstressContactStarted(SeamstressObject seam)
        {
            ribbon.seamstress = seam;
            if (seam.Ribbon != null && seam.Ribbon != ribbon)
            {
                if (ribbon.RibbonGem == null || ribbon.RibbonGem.Enabled)
                {
                    ribbon.SeamstressRibbonPosition = ribbon.PointToPosition(this.Body.Position);
                    ribbon.SeamstressRecentContact = true;
                    seam.Ribbon = ribbon;
                }
            }
            if (seam.Ribbon == null)
            {
                if (ribbon.RibbonGem == null || ribbon.RibbonGem.Enabled)
                {
                    seam.Ribbon = ribbon;
                    ribbon.SeamstressRibbonPosition = ribbon.PointToPosition(this.Body.Position);
                    ribbon.SeamstressRecentContact = true;
                }
            }
            seamstressOnRibbon++;
        }

        public void SeamstressContactEnded()
        {
            seamstressOnRibbon--;
            ribbon.seamstress.prevRibbonSpeed = new Vector2(0, 0);
        }

        public virtual void Update(float dt)
        {
            UpdateContacts();
            element.Update(dt);

            if (flippedelement is TelescopicBox)
            {
                ((TelescopicBox)flippedelement).UpdateExtend(((TelescopicBox)element).extended);
                //flippedelement.Update(dt);
            }

            foreach (Fixture f in element.fixtures)
            {
                f.IsSensor = false;
            }
            foreach (Fixture f in flippedelement.fixtures)
            {
                f.IsSensor = true;
            }

            yScale = MathHelper.Lerp(yScale, 0, 0.3f);

            glowTheta += 0.1f;
            if (glowTheta > 2 * Math.PI)
            {
                glowTheta -= (float)(2 * Math.PI);
            }

            if (contactCooldown > 0) contactCooldown = contactCooldown - 1;
        }

        public void Draw(Canvas c)
        {
            element.TransformationMatrix = Matrix.CreateTranslation(new Vector3(-element.body.Position, 0)) * Matrix.CreateTranslation(-yScale * new Vector3(Vector2.Normalize(perpendicular), 0)) * Matrix.CreateTranslation(new Vector3(element.body.Position, 0));
            flippedelement.TransformationMatrix = Matrix.CreateTranslation(new Vector3(-flippedelement.body.Position, 0)) * Matrix.CreateTranslation(-yScale * new Vector3(Vector2.Normalize(perpendicular), 0)) * Matrix.CreateTranslation(new Vector3(flippedelement.body.Position, 0));
            element.Scale = new Vector2(1, Math.Sign(2 * yScale + 1));
            element.Draw(c);

            if (Vector2.Distance(ribbon.seamstress.body.Position, hinge1.Position) <= ribbon.FLIP_DISTANCE
                || Vector2.Distance(ribbon.seamstress.body.Position, hinge2.Position) <= ribbon.FLIP_DISTANCE)
            {
                if (sensorContactFixtures.Count <= 0 && ribbon.seamstress.Ribbon == ribbon)
                {
                    // The great graveyard of ribbon-flip indicators
                    // c.DrawRectangle(Color.LimeGreen, 10, flippedelement.body.Position, new Vector2(flippedelement.spriteDimension.X, flippedelement.spriteDimension.Y), flippedelement.body.Rotation, false);
                    flippedelement.Draw(c, new Color(0.5f, 0.5f, 0.5f, (float)(0.05 * Math.Sin(glowTheta) + 0.05)));

                    //c.DrawSprite(Sprite.Build(c, "aura_backsprite", 0), new Color(0.5f, 0.5f, 0.5f, (float)(0.05 * Math.Sin(glowTheta) + 0.05)), element.body.Position, element.body.Rotation, 1, false);
                    //c.DrawSprite(Sprite.Build(c, "aura_frontsprite", 0), Color.White, element.body.Position, element.body.Rotation, 1, false);
                }
            }

            if (contactFixtures.Count - sensorContactFixtures.Count > 0) contactCooldown = CONTACT_COOLDOWN_DURATION;

            //c.CoordinateMode = CoordinateMode.ScreenCoordinates;
            //c.DrawString(element.TransformationMatrix, Color.Red, Vector2.Zero);
            //c.CoordinateMode = CoordinateMode.PhysicalCoordinates;

            #if DEBUG
                c.DrawLine(Color.BurlyWood, 5, j1.WorldAnchorA, j1.WorldAnchorB, false);
                c.DrawLine(Color.BurlyWood, 5, j2.WorldAnchorA, j2.WorldAnchorB, false);

                c.DrawLine(Color.BurlyWood, 5, flipj1.WorldAnchorA, flipj1.WorldAnchorB, false);
                c.DrawLine(Color.BurlyWood, 5, flipj2.WorldAnchorA, flipj2.WorldAnchorB, false);

                if (sensorContactFixtures.Count <= 0 && ribbon.seamstress.Ribbon == ribbon)
                {
                    flippedelement.Draw(c, new Color(0.5f, 0.5f, 1.0f));
                }
                else
                {
                    flippedelement.Draw(c, new Color(1.0f, 0.5f, 0.5f));
                }
                //c.DrawRectangle(Color.AliceBlue, 10, flippedelement.body.Position, new Vector2(flippedelement.sprite.Width, flippedelement.sprite.Height), flippedelement.body.Rotation, false);

                /*Vector2 v1 = ((EdgeShape)ribbonSensor.Shape).Vertex1;
                Vector2 v2 = ((EdgeShape)ribbonSensor.Shape).Vertex2;
                c.DrawLine(Color.DarkSalmon, 5, v1, v2, false);*/

            //Vector2 v = ((EdgeShape)ribbonSensor.Shape).Vertex1;
           
            #endif
        }

        public void DrawCollisionNotification(Canvas c)
        {
            if (contactCooldown > 0)
            {
                c.CoordinateMode = CoordinateMode.PhysicalCoordinates;
                c.DrawSprite(Sprite.Build(c, "exclamation", 0), Color.White, FindScreenIntersect(c, element.body.Position, ribbon.seamstress.body.Position), 0f, 2f, false);
                c.Camera.Shake(1, 1);
                c.Camera.OffsetAndRecover(new Vector2(-0.01f, 0));
            }
        }

        public void PlaySound(AudioPlayer audioPlayer)
        {
            audioPlayer.PlayOnSetTrue(ribbonSounds.Collision, (ContactCooldown) > 0);
            audioPlayer.PlayOnSetTrue(ribbonSounds.Flip1, PlayFlipSound);
            audioPlayer.PlayOnSetTrue(ribbonSounds.Flip2, !PlayFlipSound);
        }

        /// <summary>
        /// Helper method to find where the screen intersects, or rather where to draw the bang for collisions.
        /// </summary>
        /// <param name="bounds">bounding rectangle</param>
        /// <param name="dir">direction vector to find intersection with</param>
        /// <param name="seam">location of seamstress</param>
        /// <returns>point of intersection</returns>
        private Vector2 FindScreenIntersect(Canvas c, Vector2 dir, Vector2 seam)
        {
            float[] boxXs = { (c.Camera.Position.X - c.Camera.Dimensions.X / GraphicsConstants.PIXELS_PER_UNIT / 2), (c.Camera.Position.X - c.Camera.Dimensions.X / GraphicsConstants.PIXELS_PER_UNIT / 2),
                                (c.Camera.Position.X + c.Camera.Dimensions.X / GraphicsConstants.PIXELS_PER_UNIT / 2), (c.Camera.Position.X + c.Camera.Dimensions.X / GraphicsConstants.PIXELS_PER_UNIT / 2),
                                (c.Camera.Position.X - c.Camera.Dimensions.X / GraphicsConstants.PIXELS_PER_UNIT / 2) };
            float[] boxYs = { (c.Camera.Position.Y - c.Camera.Dimensions.Y / GraphicsConstants.PIXELS_PER_UNIT / 2), (c.Camera.Position.Y + c.Camera.Dimensions.Y / GraphicsConstants.PIXELS_PER_UNIT / 2),
                                (c.Camera.Position.Y + c.Camera.Dimensions.Y / GraphicsConstants.PIXELS_PER_UNIT / 2), (c.Camera.Position.Y - c.Camera.Dimensions.Y / GraphicsConstants.PIXELS_PER_UNIT / 2),
                                (c.Camera.Position.Y - c.Camera.Dimensions.Y / GraphicsConstants.PIXELS_PER_UNIT / 2) };

            Vector2 comp = dir - seam;
            comp.Normalize();

            for (int ii = 0; ii < boxXs.Length - 1; ii++)
            {
                float x1 = dir.X; float y1 = dir.Y;
                float x2 = seam.X; float y2 = seam.Y;
                float x3 = boxXs[ii]; float y3 = boxYs[ii];
                float x4 = boxXs[ii + 1]; float y4 = boxYs[ii + 1];

                float Ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
                float Ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

                if (Ua >= 0 && Ua <= 1 && Ub >= 0 && Ub <= 1)
                {
                    return new Vector2(x1 + Ua * (x2 - x1), y1 + Ua * (y2 - y1)) - comp;
                }
            }
            return dir;
        }

    }
}
