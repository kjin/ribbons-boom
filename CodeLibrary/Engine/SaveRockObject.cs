using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeLibrary.Storage;

namespace CodeLibrary.Engine
{
    public class SaveRockObject : PhysicalObject
    {
        public int id;
        public bool endFlag;
        private LevelInfo levelInfo;
        List<CollectableObject> collectables;
        List<RibbonGemObject> gems;
        public bool activated;

        private Sprite bow;

        private float fade;

        public SaveRockObject(Sprite sprite, World w) :
            base(sprite, w, new Vector2(0, 0), 0) { }

        public SaveRockObject(Sprite rock, Sprite bow, World w, Vector2 position, int id, bool endFlag, LevelInfo levelInfo) :
            base(rock, w, position, 0) 
        {
            fade = .9f;
            foreach (Fixture fixture in fixtures)
            {
                fixture.IsSensor = true;
            }

            this.bow = bow;
            this.id = id;
            this.endFlag = endFlag;
            this.levelInfo = levelInfo;
            activated = false;
        }

        public void SetCollectablesAndGems(List<CollectableObject> collectables, List<RibbonGemObject> gems)
        {
            this.collectables = collectables;
            this.gems = gems;
        }
        
        public void CheckpointReached()
        {
            if (activated) return;
            activated = true;
            levelInfo.SpawnID = id;
            levelInfo.SaveTime = levelInfo.CurrentTime;
            for (int i = 0; i < collectables.Count; i++)
                levelInfo.CollectablesThisRound |= collectables[i].Collected ? (1 << collectables[i].CollectID) : 0;
            for (int i = 0; i < gems.Count; i++)
                levelInfo.RibbonGems |= gems[i].Enabled ? (1 << gems[i].ColorID) : 0;
            //TODO: Gems
            if(endFlag)
            {
                levelInfo.Complete = true;
                levelInfo.Collectables |= levelInfo.CollectablesThisRound;

                levelInfo.CompletionTime = Math.Min(levelInfo.CompletionTime, levelInfo.CurrentTime);
                // Add completion time
            }
#if DEBUG
            Console.WriteLine("CheckpointReached() called.");
#endif
        }

        public override void Draw(Canvas c)
        {
            base.Draw(c);
            if (endFlag)
            {
                Color red = Color.Lerp(Color.DarkRed, Color.White, .3f);
                c.DrawSprite(bow, Color.White, body.Position + new Vector2(0, -0.5f));
            }
            else
            {
                Color transition = Color.Lerp(Color.CornflowerBlue, Color.White, fade);
                if (activated)
                {
                    c.DrawSprite(bow, transition, body.Position);
                    if (fade > 0)
                    {
                        fade = fade - .015f;
                    }
                }
                else
                {
                    c.DrawSprite(bow, transition, body.Position);
                }
            }
        }

    }
}
