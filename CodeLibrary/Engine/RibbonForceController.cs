using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using CodeLibrary.Input;
using CodeLibrary.Graphics;

namespace CodeLibrary.Engine
{
    public class RibbonForceController : Controller
    {

        SeamstressObject seamstress;
        InputController inputController;

        public RibbonForceController(SeamstressObject s, InputController i) :
            base(ControllerType.AbstractForceController)
        {
            seamstress = s;
            inputController = i;
        }

        public override void Update(float dt)
        {

            //Console.WriteLine("started update");
            RibbonObject ribbon = seamstress.Ribbon;

            if (ribbon == null)
            {
                return;
            }

            /*
            if (ribbon.Contact())
            {              
                if (!resolvingConflict)
                {
                    resolvingConflict = true;
                    ribbonSpeed = -1.0f*ribbonSpeed;
                }
                
                ribbon.Move(ribbonSpeed, dt);

                flippedPreviously = false;

                return;
            }*/

            //resolvingConflict = false;

            if (inputController.RibbonLeft.Pressed)
            {
                ribbon.ribbonTextureLabel = "ribbon_texture_3";
                ribbon.selectedSpool = "left";
                /*
                ribbonSpeed -= ribbon.RIBBON_SPEED * inputController.RibbonLeft.Value;
                ribbonSpeed -= ribbon.RIBBON_DYNAMIC_DRAG * ribbonSpeed;
                holdMovement += ribbonSpeed;
                 * */
                ribbon.ApplyLeftImpulse(inputController.RibbonLeft.Value);
            }
            else if (inputController.RibbonRight.Pressed)
            {
                ribbon.ribbonTextureLabel = "ribbon_texture_2";
                ribbon.selectedSpool = "right";
                /*
                ribbonSpeed += ribbon.RIBBON_SPEED * inputController.RibbonRight.Value;
                ribbonSpeed -= ribbon.RIBBON_DYNAMIC_DRAG * ribbonSpeed;
                holdMovement += ribbonSpeed;
                 * */
                ribbon.ApplyRightImpulse(inputController.RibbonRight.Value);
            }
            else
            {
                ribbon.ribbonTextureLabel = "ribbon_texture_4";
                ribbon.selectedSpool = "none";
                //only applies static drag if within THRESHOLD of integer location
                /*
                if (PhysicsConstants.RIBBON_DISCRETE_ENABLE)
                {
                    if (ribbon.IsDiscrete())
                    {
                        ribbonSpeed -= ribbon.RIBBON_STATIC_DRAG * ribbonSpeed;
                    }
                }
                else if (PhysicsConstants.RIBBON_PULL_ENABLE)
                {
                    if (ribbon.IsDiscrete())
                    {
                        ribbonSpeed -= ribbon.RIBBON_STATIC_DRAG * ribbonSpeed;
                        holdMovement = 0;
                    }
                    else
                    {
                        float offset = GraphicsHelper.Mod(ribbon.ribbonStart, 1);

                        if (holdMovement > 0 && holdMovement < 1)
                        {
                            ribbonSpeed += (offset) * PhysicsConstants.RIBBON_PULL_POWER;
                        }
                        else if (holdMovement < 0 && holdMovement > -1)
                        {
                            ribbonSpeed -= (1 - offset) * PhysicsConstants.RIBBON_PULL_POWER;
                        }
                        else
                        {
                            ribbonSpeed += (offset) * PhysicsConstants.RIBBON_PULL_POWER;
                            ribbonSpeed -= (1 - offset) * PhysicsConstants.RIBBON_PULL_POWER;
                        }
                    }
                }
                else
                {
                    ribbonSpeed -= ribbon.RIBBON_STATIC_DRAG * ribbonSpeed;
                }*/
            }

            ribbon.spoolSpin += ribbon.ribbonSpeed * 4;
            //ribbon.Move(ribbonSpeed, dt);

            if (inputController.RibbonFlip.JustPressed)
            {
                ribbon.Flip();

            }
            //Console.WriteLine(holdMovement);
            seamstress.ribbonFloatSpeed = ribbon.ribbonSpeed;
        }

    }
}
