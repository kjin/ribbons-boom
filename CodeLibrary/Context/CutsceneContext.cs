using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CodeLibrary.Content;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Context;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using CodeLibrary.Engine;
using XMLContent;

namespace CodeLibrary.Context
{
    /// <summary>
    /// Context for cutscene "levels"
    /// </summary>
    class CutsceneContext : GameContext
    {
        private SpriteFont spriteFont;
        
        private List<String> actor;
        private List<String> speech;
        private List<String> portraitId;
        private List<String> galleryId;
        
        private Dictionary<String,Texture2D> portrait;
        
        private String sceneName;
        private int ActNumber;

        private int lineNumber;

        private const int TEXT_X_OFFSET = 30;
        private const int TEXT_Y_OFFSET = 40;
        private const int CONTINUE_TEXT_OFFSET = 400;

        private ParallaxBackgroundSet backgrounds;

        public CutsceneContext(String sceneName, GameContext other)
            : base(other)
        {
            this.sceneName = sceneName;
        }

        public override void Initialize()
        {
            lineNumber = 0;
            actor = new List<String>();
            speech = new List<String>();
            portraitId = new List<String>();
            galleryId = new List<String>();
            portrait = new Dictionary<String, Texture2D>();

            // load script from file
            ProcessScript(Canvas.Assets.GetText(sceneName));

            // load portraits
            CollectPortraits(Canvas);

            // load font
            spriteFont = Canvas.Assets.GetFont("CutsceneFont");

            // load background
            ActAssets actAssets = new ActAssets(Canvas, ActNumber);
            backgrounds = ParallaxBackgroundSet.Build(Canvas, Vector2.Zero, actAssets.Theme);
            this.BackgroundColor = Canvas.AssetDictionary.LookupColor("act" + ActNumber, "color");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputController.Pause.JustPressed)
            {
                lineNumber++;

                if (lineNumber >= actor.Count)
                {
                    lineNumber = actor.Count - 1;
                    // end the scene
                    NextContext = FileManager.LevelProgression.GetNextContext(this, sceneName);
                }
            }
            if (InputController.AllKeys.Pressed && !InputController.Pause.Pressed)
            {
                // end the scene
                NextContext = FileManager.LevelProgression.GetNextContext(this, sceneName);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // draw backgrounds and stuff
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            backgrounds.Draw(gameTime, Canvas);

            Canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            // define text geometry
            Rectangle main = new Rectangle(Canvas.Camera.Width / 10, Canvas.Camera.Height * 7 / 10, Canvas.Camera.Width * 8 / 10, Canvas.Camera.Height * 2 / 10);
            Rectangle name = new Rectangle(Canvas.Camera.Width / 10, Canvas.Camera.Height * 65 / 100, Canvas.Camera.Width * 2 / 10, Canvas.Camera.Height * 1 / 20);
            // draw text box
            Canvas.DrawRectangle(Color.Blue, Color.BlanchedAlmond, 4, main, false);
            Canvas.DrawRectangle(Color.Blue, Color.BlanchedAlmond, 4, name, false);
            // draw text
            Canvas.DrawString(actor[lineNumber], spriteFont, Color.Black, new Vector2(name.Left + TEXT_X_OFFSET, name.Top + 4), 0, 1);
            Canvas.DrawString(speech[lineNumber], spriteFont, Color.Black, new Vector2(main.Left + TEXT_X_OFFSET,main.Top + TEXT_X_OFFSET), 0, 1);
            Canvas.DrawString("[ Hit Enter to Continue... ]", Color.Black, new Vector2(main.Right, main.Bottom), Anchor.BottomRight, 0, 1);
            Canvas.DrawString("[ Hit Any Key to Skip... ]", Color.Black, new Vector2(main.Right, main.Bottom + TEXT_Y_OFFSET), Anchor.BottomRight, 0, 1);
            // draw speaker portrait
            if (portraitId[lineNumber] != "none")
            {
                Canvas.DrawTexture(portrait[portraitId[lineNumber]], Color.White, new Vector2(name.Left, name.Top), Anchor.BottomLeft, 0, 1, true);
            }

            // draw background portraits
            int position = 0;
            foreach (String Id in portrait.Keys)
            {
                if (Id != portraitId[lineNumber] && galleryId.Contains(Id) && Id != "none")
                {
                    // Draw half size on right, in a row
                    Canvas.DrawTexture(portrait[Id], Color.White, new Vector2(main.Right - position * portrait[Id].Width / 2, main.Top), Anchor.BottomRight, 0, 0.5f, false);
                    position++;
                }
            }
        }

        /// <summary>
        /// Parses script file into the cutscene.
        /// </summary>
        /// <param name="raw">Raw Text object with lines of the file.
        /// MUST begin with an act number.
        /// Each time a character starts to talk their name must appear on a line.
        /// Blank lines between character's speech blocks to switch characters.</param>
        /// <returns>the fully parsed script.</returns>
        private void ProcessScript(Text raw)
        {
            List<String> temp = new List<String>();
            int ii = 0;
            while(ii < raw.Length)
            {
                // Collect act number
                if (raw[ii] == "\\act")
                {
                    ii++;
                    ActNumber = Convert.ToInt32(raw[ii]);
                    ii++;
                }
                // new line means speaker is changing
                else if (raw[ii] == "\\new")
                {
                    ii++;
                    actor.Add(raw[ii]);
                    ii++;
                    portraitId.Add(raw[ii]);
                    ii++;
                    if (raw[ii] == "\\show")
                    {
                        galleryId.Add(portraitId.Last());
                        ii++;
                    }
                    // now add the first line
                    speech.Add(raw[ii]);
                    ii++;
                }
                else
                {
                    // else we must be at a line of speech for current actor
                    actor.Add(actor.Last());
                    portraitId.Add(portraitId.Last());
                    speech.Add(raw[ii]);
                    ii++;
                }
            }
        }

        private void CollectPortraits(Canvas canvas)
        {
            foreach (String Id in portraitId)
            {
                if (!portrait.ContainsKey(Id) && Id != "none")
                {
                    portrait.Add(Id, canvas.Assets.GetTexture(Id));
                }
            }
        }

        public override void Dispose()
        {
            // nothing to do, I think
        }

        public override void PlayAudio(GameTime gameTime)
        {
            // VO someday...
        }
    }
}
