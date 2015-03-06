using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;
using CodeLibrary.Storage;
using CodeLibrary.Audio;

namespace CodeLibrary.Engine
{
    public class CollectableObject : PhysicalObject
    {
        private int id;
        private bool previouslyCollected;
        private int collectionTime;
        private int frames;
        private SoundObject collectSound;

        public CollectableObject(Canvas c, World w, Vector2 position, int id, bool collected, bool previouslyCollected) :
            base(Sprite.Build(c,"Collectables/collect"+id), w, position + .5f*Vector2.One, 0) 
        {
            collectSound = new SoundObject(c.Assets.GetSFX("collectable_collect"), false);
            frames = 0;
            foreach (Fixture fixture in fixtures)
            {
                fixture.IsSensor = true;
            }

            this.id = id;
            this.collectionTime = collected ? 0 : -1;
            this.previouslyCollected = previouslyCollected;
            frames = 0;
        }

        public override void Draw(Canvas c)
        {
            Color tint = Color.White;
            if (previouslyCollected)
                tint = new Color(1, 1, 1, 0.5f);
            if (collectionTime == -1)
            {
                this.TransformationMatrix = Matrix.CreateTranslation(0, .1f * (float)Math.Sin(frames / 10.0), 0);
                base.Draw(c, tint);
            }
            else if (frames - collectionTime < 50)
            {
                this.TransformationMatrix = Matrix.CreateTranslation(0, (collectionTime - frames) / 20f, 0);
                tint.A = (byte)((float)tint.A / Math.Max(1, frames - collectionTime));
                base.Draw(c, tint);
           }
        }

        public int CollectID
        {
            get { return id; }
            set { id = value; }
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            frames++;
        }

        public void PlaySound(AudioPlayer audioPlayer)
        {
            audioPlayer.PlayOnSetTrue(collectSound, Collected);
        }

        public bool Collected
        {
            get { return collectionTime >= 0; }
            set
            {
                if (value && collectionTime == -1)
                    collectionTime = frames;
                else if (!value)
                    collectionTime = -1;
            }
        }
    }
}
