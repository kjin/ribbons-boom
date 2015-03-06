using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace CodeLibrary.Engine
{
    public class CollisionController
    {

        World world;
        SeamstressObject seamstress;
        //RibbonObject ribbon;
        List<GameObject> environmentObjects;

        List<Fixture> landedFixtures;
        List<Fixture> landedSensors;

        public bool reset;

        public CollisionController(World w, SeamstressObject s, List<GameObject> m)
        {
            world = w;
            seamstress = s;
            //ribbon = r;
            environmentObjects = m;

            world.ContactManager.BeginContact += ContactStarted;
            world.ContactManager.EndContact += ContactEnded;

            landedFixtures = new List<Fixture>();
            landedSensors = new List<Fixture>();

            reset = false;

        }

        public void Dispose()
        {
            reset = true;
            world.ContactManager.BeginContact += EmptyStart;
            world.ContactManager.EndContact += EmptyEnd;
        }

        private bool EmptyStart(Contact contact) { return false; }

        private void EmptyEnd(Contact contact) { }

        private bool ContactStarted(Contact contact)
        {
            Fixture fixture1 = contact.FixtureA;
            Fixture fixture2 = contact.FixtureB;

            bool ab = ProcessOneWayContactsStarted(fixture1, fixture2);
            bool ba = ProcessOneWayContactsStarted(fixture2, fixture1);
            return ab || ba;
        }

        private void ContactEnded(Contact contact)
        {
            Fixture fixture1 = contact.FixtureA;
            Fixture fixture2 = contact.FixtureB;

            ProcessOneWayContactsEnded(fixture1, fixture2);
            ProcessOneWayContactsEnded(fixture2, fixture1);
        }

        private bool ProcessOneWayContactsStarted(Fixture fixture1, Fixture fixture2)
        {
            Body body1 = fixture1.Body;
            Body body2 = fixture2.Body;

            var ud1 = fixture1.UserData;
            var ud2 = fixture2.UserData;
            
            //check for DEATH
            if (body1.UserData is SeamstressObject)
            {
                if ((body2.UserData is BulletObject && !fixture2.IsSensor))
                    seamstress.Die(DeathMethods.Shooter);
                else if (body2.UserData is SpikeBox)
                    seamstress.Die(DeathMethods.Spikes);
                else if (body2.UserData is MiasmaObject)
                    seamstress.Die(DeathMethods.Miasma);
                else if (body2.UserData is int && ((int)body2.UserData & PhysicalObjectTypes.DEATH) != 0)
                    seamstress.Die(DeathMethods.Falling);
            }

            //check for WIN
            if (body1.UserData is SeamstressObject)
            {
                if (body2.UserData is SaveRockObject)
                {

                    if (((SaveRockObject)body2.UserData).endFlag && seamstress.alive)
                    {
                        seamstress.Win();
                    }
                    else
                    {
                        seamstress.spawnID = ((SaveRockObject)body2.UserData).id;
                    }
                    ((SaveRockObject)body2.UserData).CheckpointReached();
                }
            }

            // Check for collected!
            if (body1.UserData is CollectableObject && body2.UserData is SeamstressObject)
            {
                ((CollectableObject)body1.UserData).Collected = true;
            }

            //Check for enabling ribbon gem
            if ((body1.UserData is RibbonGemObject && body2.UserData is SeamstressObject))
            {
                ((RibbonGemObject)body1.UserData).Enabled = true;
            }


            // See if we have landed on the ground.
            if (seamstress.SensorName.Equals(ud2) && seamstress != body1.UserData && !(body1.UserData is FlipBarObject))
            {

                if (!fixture1.IsSensor)
                {
                    landedFixtures.Add(fixture1);
                    seamstress.IsGrounded = true;

                    if (body1.UserData is RibbonObject)
                    {
                        ((RibbonObject)body1.UserData).SeamstressContactStarted(seamstress);
                    }
                    if (body1.UserData is RibbonElement)
                    {
                        ((RibbonElement)body1.UserData).SeamstressContactStarted(seamstress);
                    }
                }
                else
                {
                    landedSensors.Add(fixture1);
                }
            }

            // Detects bullet collisions
            try
            {
                if (body1.UserData is BulletObject && (!fixture2.IsSensor) && !(body2.UserData is ShooterBox) && !(body2.UserData is BulletObject))
                {
                    if (!(fixture1.IsSensor && body2.UserData is SeamstressObject))
                    {
                        ((BulletObject)body1.UserData).Collision();
                    }
                }
            }
            catch (Exception e)
            {
                #if DEBUG
                    throw e;
                #endif
            }


            // check for ribbon element collisions:
            if (body1.UserData is RibbonElement)
            {
                if (!(body2.UserData is SeamstressObject) && !(body2.UserData is RibbonObject) && !(body2.UserData == body1.UserData) && (body2.IsStatic || body2.UserData is RibbonElement))
                {
                    if ((body2.UserData) != ((RibbonElement)body1.UserData).ribbon)
                    {
                        if (fixture1.IsSensor == true)
                        {
                            ((RibbonElement)body1.UserData).SensorContact(fixture2);
                        }
                        else
                        {
                            ((RibbonElement)body1.UserData).Contact(fixture2);
                        }

                    }

                }
            }

            //check for seamstress/flipbarobject collisions
            if (body1.UserData is FlipBarObject && body2.UserData is SeamstressObject && !fixture2.IsSensor)
            {
                ((FlipBarObject)body1.UserData).SeamstressTouchStarted(seamstress, body1);
                //return false;
            }

            return true;
        }

        private void ProcessOneWayContactsEnded(Fixture fixture1, Fixture fixture2)
        {
            Body body1 = fixture1.Body;
            Body body2 = fixture2.Body;

            var ud1 = fixture1.UserData;
            var ud2 = fixture2.UserData;

            // See if we are off the ground.
            if (seamstress.SensorName.Equals(ud2) && seamstress != body1.UserData && !(body1.UserData is FlipBarObject))
            {
                if (!fixture1.IsSensor)
                {
                    landedFixtures.Remove(fixture1);
                    if (landedFixtures.Count == 0)
                    {
                        seamstress.IsGrounded = false;
                    }

                    if (body1.UserData is RibbonObject)
                    {
                        ((RibbonObject)body1.UserData).SeamstressContactEnded();
                    }
                    if (body1.UserData is RibbonElement)
                    {
                        ((RibbonElement)body1.UserData).SeamstressContactEnded();
                    }
                }
                else
                {
                    landedSensors.Remove(fixture1);
                }
            }

            // ribbon element collision
            if (body1.UserData is RibbonElement)
            {
                if (!(body2.UserData is SeamstressObject) && !(body2.UserData is RibbonObject) && !(body2.UserData == body1.UserData) && (body2.IsStatic || body2.UserData is RibbonElement))
                {
                    if (body2.UserData != ((RibbonElement)body1.UserData).ribbon)
                    {

                        if (fixture1.IsSensor)
                        {
                            ((RibbonElement)body1.UserData).SensorContactEnded(fixture2);
                        }
                        else
                        {
                            ((RibbonElement)body1.UserData).ContactEnded(fixture2);
                        }
                    }
                }
            }


            //check for seamstress/flipbarobject collisions
            if (body1.UserData is FlipBarObject && body2.UserData is SeamstressObject && !fixture2.IsSensor)
            {
                ((FlipBarObject)body1.UserData).SeamstressTouchEnded(body1);
            }
        }


        public void Update()
        {
            List<Fixture> tempFixtures = new List<Fixture>();

            foreach (Fixture f in landedFixtures)
            {
                if (f.IsSensor)
                {
                    landedSensors.Add(f);

                    if (f.Body.UserData != null && f.Body.UserData is RibbonElement)
                    {
                        RibbonElement re = (RibbonElement)f.Body.UserData;
                        re.SeamstressContactEnded();
                    }
                }
                else
                {
                    tempFixtures.Add(f);
                }
            }
            landedFixtures = tempFixtures;
            tempFixtures = new List<Fixture>();

            foreach (Fixture f in landedSensors)
            {
                if (!f.IsSensor)
                {
                    landedFixtures.Add(f);

                    if (f.Body.UserData is RibbonElement)
                    {
                        RibbonElement re = (RibbonElement)f.Body.UserData;
                        re.SeamstressContactStarted(seamstress);
                    }

                }
                else
                {
                    tempFixtures.Add(f);
                }

            }
            landedSensors = tempFixtures;

            if (landedFixtures.Count == 0)
            {
                seamstress.IsGrounded = false;
            }
            else
            {
                seamstress.IsGrounded = true;
            }           

        }


    }
}
