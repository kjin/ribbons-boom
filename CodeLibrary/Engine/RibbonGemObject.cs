using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Audio;

namespace CodeLibrary.Engine
{
    public class RibbonGemObject : PhysicalObject
    {
        private int colorID;
        Sprite gemBase;
        Sprite gemRibbonDecoration;
        Effect shimmerEffect;
        Vector2 position;
        private int timeSinceActivation;

        private SoundObject activationSound;

        static float INITIAL_FADE = 0.6f;
        static float FADE_RATE = 0.01f;

        public RibbonGemObject(Canvas c, World w, Vector2 position, int id, bool enabled = false) :
            base(Sprite.Build(c,"RibbonGem/ribbongem_middle"), w, position + .5f*Vector2.One, 0) 
        {
            activationSound = new SoundObject(c.Assets.GetSFX("ribbongem_powerup"), false);
            gemBase = Sprite.Build(c, "RibbonGem/ribbongem_bottom");
            gemRibbonDecoration = Sprite.Build(c, "RibbonGem/ribbongem_top");
            shimmerEffect = c.Assets.GetEffect("Shimmer");
            foreach (Fixture fixture in fixtures)
            {
                fixture.IsSensor = true;
            }
            this.position = position;
            colorID = id;
            timeSinceActivation = -1;
        }

        public int ColorID
        {
            get { return colorID; }
            set { colorID = value; }
        }

        public bool Enabled
        {
            get { return timeSinceActivation >= 0; }
            set
            {
                if (timeSinceActivation >= 0 && value)
                    return;
                else if (value)
                    timeSinceActivation = 0;
                else
                    timeSinceActivation = -1;
            }
        }

        public void PlaySound(AudioPlayer audioPlayer)
        {
            audioPlayer.PlayOnSetTrue(activationSound, Enabled);
        }

        public int EnabledTime
        {
            get
            {
                return timeSinceActivation;
            }
        }

        public override void Update(float dt)
        {
            if (timeSinceActivation >= 0)
                timeSinceActivation++;
            base.Update(dt);
        }

        public override void Draw(Canvas c)
        {
            if (timeSinceActivation == -1) //unactivated
            {
                Color faded = Color.Lerp(RibbonColors.GetColor(colorID), Color.White, INITIAL_FADE);
                c.DrawSprite(gemBase, Color.White, position);
                c.DrawSprite(sprite, faded, position);
            }
            else
            {
                c.DrawSprite(gemBase, Color.White, position);
                float fade = Math.Max(INITIAL_FADE - timeSinceActivation * FADE_RATE, 0);
                shimmerEffect.Parameters["Time"].SetValue(timeSinceActivation);
                c.EndDraw();
                c.BeginDraw(0, BlendState.NonPremultiplied, null, null, null, shimmerEffect);
                Color transition = Color.Lerp(RibbonColors.GetColor(colorID), Color.White, fade);
                c.DrawSprite(sprite, transition, position);
                c.EndDraw();
                c.BeginDraw(0, BlendState.NonPremultiplied);
                c.DrawSprite(gemRibbonDecoration, transition, position);
            }
        }
    }
}
