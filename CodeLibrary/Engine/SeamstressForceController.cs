using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using CodeLibrary.Input;

namespace CodeLibrary.Engine
{
    class SeamstressForceController : Controller
    {
        #region Fields
        private SeamstressObject seamstress;
        private InputController inputController;
        bool wasRibboned;
        Vector2[] prevSeamVel;
        float[] prevRibFloatSpeed;
        int seamstressPushCounter;

        private const int DEBUG_SPEED = 10;
        #endregion

        #region Properties (READ-WRITE)
        /// <summary>
        /// The currently active avatar
        /// </summary>
        /// <remarks>
        /// The controller can only affect one avatar at a time.
        /// </remarks>
        public SeamstressObject Seamstress
        {
            get { return seamstress; }
            set { seamstress = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create a new controller for the given avatar
        /// </summary>
        /// <param name="s">The avatar</param>
        public SeamstressForceController(SeamstressObject s, InputController i)
            : base(ControllerType.AbstractForceController)
        {
            seamstress = s;
            inputController = i;
            wasRibboned = false;
            Vector2 empty = new Vector2(0, 0);
            prevSeamVel = new Vector2[10]{empty,empty,empty,empty,empty,empty,empty,empty,empty,empty};
            prevRibFloatSpeed = new float[10]{0,0,0,0,0,0,0,0,0,0};
            seamstressPushCounter = 0;
        }

        /// <summary>
        /// Apply appropriate forces while collisions are processed
        /// </summary>
        /// <param name="dt">Timing values from parent loop</param>
        public override void Update(float dt)
        {
            if (!seamstress.alive || seamstress.win) return;
            //Vector2 moveForce = new Vector2(seamstress.Movement, 0.0f);
            //Vector2 velocity = seamstress.body.LinearVelocity;

            // Ribbon Movement forces
            //seamstress.body.ApplyForce(RibbonSpeedToForce(seamstress.RibbonSpeed), seamstress.body.Position);

            Vector2 velocity = seamstress.body.LinearVelocity;
            velocity -= seamstress.prevRibbonSpeed;

            if (inputController.SeamstressJump.Pressed)
            {
                //Console.WriteLine(Seamstress.body.Position);
                if (seamstress.jumpCooldown == 0 && seamstress.IsGrounded)
                    velocity += new Vector2(0, SeamstressObject.PRIMARY_JUMPFORCE);
                else if (seamstress.jumpCooldown == seamstress.MAX_JUMP_COOLDOWN)
                    velocity += new Vector2(0, SeamstressObject.SECONDARY_JUMPFORCE);
                seamstress.jumpCooldown++;
            }
            else if (seamstress.IsGrounded)
            {
                seamstress.jumpCooldown = 0;
            }
            if (inputController.SeamstressJump.JustReleased)
            {
                seamstress.jumpCooldown = seamstress.MAX_JUMP_COOLDOWN + 1;
            }

            if (seamstress.IsGrounded)
            {
                seamstress.IsWalking = true;
                //for the sake of symmetry, don't do anything if both controls are pressed.
                if (inputController.SeamstressLeft.Pressed && inputController.SeamstressRight.Pressed) { }
                else if (inputController.SeamstressLeft.Pressed)
                {
                    velocity += new Vector2(-seamstress.SEAMSTRESS_GROUNDSPEED * inputController.SeamstressLeft.Value, 0);
                    seamstress.FacingLeft = true;
                }
                else if (inputController.SeamstressRight.Pressed)
                {
                    velocity += new Vector2(seamstress.SEAMSTRESS_GROUNDSPEED * inputController.SeamstressRight.Value, 0);
                    seamstress.FacingLeft = false;
                }
                else
                {
                    seamstress.IsWalking = false;
                }
                velocity -= velocity * seamstress.GroundDamping;
            }
            else
            {
                //for the sake of symmetry, don't do anything if both controls are pressed.
                if (inputController.SeamstressLeft.Pressed && inputController.SeamstressRight.Pressed) { }
                else if (inputController.SeamstressLeft.Pressed)
                {
                    velocity += new Vector2(-seamstress.SEAMSTRESS_AIRSPEED, 0);
                }
                else if (inputController.SeamstressRight.Pressed)
                {
                    velocity += new Vector2(seamstress.SEAMSTRESS_AIRSPEED, 0);
                }

                if (PhysicsConstants.SEAMSTRESS_BIDIRECTIONALDRAG)
                {
                    velocity.X -= PhysicsConstants.SEAMSTRESS_HORIZONTAL_AIRDRAG * velocity.X;
                    velocity.Y -= PhysicsConstants.SEAMSTRESS_VERTICAL_AIRDRAG * velocity.Y;
                }
                else if (seamstress.APPLY_AERIALDAMPING_VERTICALLY)
                {
                    velocity -= seamstress.AerialDamping * velocity;
                }
                else
                {
                    velocity -= new Vector2(seamstress.AerialDamping * velocity.X, 0);
                }
            }

            //new debug
            if (inputController.Debug.Pressed && inputController.MenuUp.Pressed) { velocity = new Vector2(0, -DEBUG_SPEED); }
            else if (inputController.Debug.Pressed && inputController.MenuDown.Pressed) { velocity = new Vector2(0, DEBUG_SPEED); }
            else if (inputController.Debug.Pressed && inputController.MenuLeft.Pressed) { velocity = new Vector2(-DEBUG_SPEED, 0); }
            else if (inputController.Debug.Pressed && inputController.MenuRight.Pressed) { velocity = new Vector2(DEBUG_SPEED, 0); }
            else if (inputController.Debug.Pressed) { velocity = new Vector2(0, 0); }
            if (inputController.DebugAlt.Pressed) { velocity *= 1.05f; }

            velocity += seamstress.RibbonSpeed;

            if (seamstress.Ribbon != null)
            {
                if (seamstressPushCounter == 0 && seamstress.Ribbon.elements != null && seamstress.Ribbon.elements.Count > 0)
                {
                    Vector2 ribbonVel = seamstress.Ribbon.elements.ElementAt(0).Body.LinearVelocity;
                    bool ribbonMoving = (ribbonVel.X > 1 || ribbonVel.X < -1) || (ribbonVel.Y > 1 || ribbonVel.Y < -1);
                    bool ribbonDirChange = seamstress.ribbonFloatSpeed > 0.02 && prevRibFloatSpeed[4] < -0.02 || seamstress.ribbonFloatSpeed < -0.02 && prevRibFloatSpeed[4] > 0.02;
                    if (ribbonDirChange)
                    Console.WriteLine(ribbonDirChange);
                    //Console.WriteLine("prevRibbOnSpeed: " + prevRibFloatSpeed[0]);

                    AddPrevRibbonSpeed(seamstress.ribbonFloatSpeed);

                    Vector2 seamstressVel = seamstress.body.LinearVelocity;
                    bool seamstressMoving = (int)seamstressVel.X != 0 || (int)seamstressVel.Y != 0;
                    bool prevSeamMoving = (int)prevSeamVel[4].X != 0 && (int)prevSeamVel[4].Y != 0;
                    AddPrevSeamVel(seamstressVel);

                    //Console.WriteLine("ribbon Vel: " + ribbonVel);
                    //Console.WriteLine("ribbonMoving: " + ribbonMoving);

                    //Console.WriteLine("seamstressMoving: " + seamstressMoving);
                    //Console.WriteLine("prevSeamMoving: " + prevSeamMoving);
                    //Console.WriteLine("currRibbonSpeed: " + seamstress.ribbonFloatSpeed);


                   /* if (seamstress.isRibboned && !seamstressMoving && ribbonMoving && !ribbonDirChange)
                    {
                        velocity += new Vector2(-20,0);
                        Console.WriteLine("bounce");
                    }*/
                    seamstressPushCounter = 0;
                }
                else
                {
                    seamstressPushCounter++;
                }
            }

            seamstress.prevRibbonSpeed = seamstress.RibbonSpeed;
            seamstress.body.LinearVelocity = velocity;

            wasRibboned = seamstress.isRibboned;

            // check if idle counter should reset to zero
            if (inputController.SeamstressLeft.Pressed ||
                inputController.SeamstressRight.Pressed ||
                inputController.SeamstressJump.Pressed)
                seamstress.FramesIdle = 0;
        }

        private void AddPrevRibbonSpeed(float ribbonSpeed)
        {
            prevRibFloatSpeed[0] = prevRibFloatSpeed[1];
            prevRibFloatSpeed[1] = prevRibFloatSpeed[2];
            prevRibFloatSpeed[2] = prevRibFloatSpeed[3];
            prevRibFloatSpeed[3] = prevRibFloatSpeed[4];
            prevRibFloatSpeed[4] = prevRibFloatSpeed[5];
            prevRibFloatSpeed[5] = prevRibFloatSpeed[6];
            prevRibFloatSpeed[6] = prevRibFloatSpeed[7];
            prevRibFloatSpeed[7] = prevRibFloatSpeed[8];
            prevRibFloatSpeed[8] = prevRibFloatSpeed[9];
            prevRibFloatSpeed[9] = ribbonSpeed;
        }

        private void AddPrevSeamVel(Vector2 seamVel)
        {
            prevSeamVel[0] = prevSeamVel[1];
            prevSeamVel[1] = prevSeamVel[2];
            prevSeamVel[2] = prevSeamVel[3];
            prevSeamVel[3] = prevSeamVel[4];
            prevSeamVel[4] = prevSeamVel[5];
            prevSeamVel[5] = prevSeamVel[6];
            prevSeamVel[6] = prevSeamVel[7];
            prevSeamVel[7] = prevSeamVel[8];
            prevSeamVel[8] = prevSeamVel[9];
            prevSeamVel[9] = seamVel;
            
        }
        #endregion
    }
}
