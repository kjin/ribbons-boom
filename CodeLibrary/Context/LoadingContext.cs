/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CodeLibrary.Content;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Graphics;

namespace CodeLibrary.Context
{
    /// <summary>
    /// Intermediate context whose sole purpose is to load the next context.
    /// This prevents the other contexts from hanging when loading an intensive
    /// context such as a scene or level.
    /// </summary>
    public class LoadingContext : GameContext
    {
        bool loaded = false;
        string contextName;

        public LoadingContext(string contextName, GameContext other) : base(other)
        {
            this.contextName = contextName;
        }

        public override void Initialize()
        {
            loaded = false;
            FadeMultiplier = 1;
        }

        public override void Dispose() { }

        public override void Update(GameTime gameTime)
        {
            if (!loaded)
            {
                loaded = true;
                Text levelList = Canvas.Assets.GetText("levelprogression");

                if (contextName == null)
                {
                    if (levelList[0] == "level")
                    {
                        NextContext = new LevelContext(levelList[1], 0, this);
                        return;
                    }
                    if (levelList[0] == "cutscene")
                    {
                        NextContext = new CutsceneContext(levelList[1], this);
                        return;
                    }
                    if (levelList[0] == "splash")
                    {
                        NextContext = new SplashScreenMenuContext(this);
                        return;
                    }
                }

                for (int ii = 0; ii < levelList.Length; ii++)
                {
                    if (levelList[ii] == contextName)
                    {
                        if (levelList[ii + 1] == "level")
                        {
                            NextContext = new LevelContext(levelList[ii + 2], 0, this);
                            return;
                        }
                        if (levelList[ii + 1] == "cutscene")
                        {
                            NextContext = new CutsceneContext(levelList[ii + 2], this);
                            return;
                        }
                        if (levelList[ii + 1] == "splash")
                        {
                            NextContext = new SplashScreenMenuContext(this);
                            return;
                        }
                    }
                }
                NextContext = new SplashScreenMenuContext(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Canvas.DrawString("LOADING", Color.White, Canvas.Camera.Dimensions / 2, Anchor.Center);
        }

        public override void PlayAudio(GameTime gameTime) { }
    }
}
*/