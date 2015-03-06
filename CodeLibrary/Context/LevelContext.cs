using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;
using CodeLibrary.Input;
using CodeLibrary.Audio;
using CodeLibrary.Engine;
using XMLContent;

using CodeLibrary.Storage;

namespace CodeLibrary.Context
{
    public class LevelContext : GameContext
    {
        bool paused;

        MultiOptionSelector selector;
        public int SelectedOption { get { return selector.IntValue; } }
        Texture2D pauseMenu;
        Texture2D pauseText;

        Texture2D titleCardBottom;
        string titleText = "";
        Vector2 titleCardLocation;
        SpriteFont titleFont;
        float titleStringLength;

        LevelEngine levelEngine;
        string levelName = "";
        bool firstPass;

        int lossDelay;

        public Level level;
        LevelInfo levelInfo;

        int winTime = -1;
        int loseTime = -1;
        Texture2D winTexture;
        Texture2D[] loseTextures;
        Texture2D[] actBackgrounds;
        Rectangle source;

        bool songPlaying = false;

        SpriteFont spriteFont;

        public LevelContext(Level level, LevelInfo levelInfo, GameContext other)
            : this(levelInfo, other)
        {
            this.level = level;
        }

        public LevelContext(LevelInfo levelInfo, GameContext other)
            : base(other)
        {
            this.levelInfo = levelInfo;
        }

        public override void Initialize()
        {
            //title stuff
            titleCardBottom = Canvas.Assets.GetTexture("LevelTitle/titleCardBottom");
            titleFont = Canvas.Assets.GetFont("Conformity32");
            titleCardLocation.Y = Canvas.Camera.Dimensions.Y - 50;
            titleText = String.Format("Level {0}-{1}\n{2}", levelInfo.ActID, levelInfo.LevelID, FileManager.GetLevelTitle(levelInfo.ActID, levelInfo.LevelID));
            titleStringLength = titleFont.MeasureString(titleText).X + 50;

            //end stuff
            winTexture = Canvas.Assets.GetTexture("LevelEndCards/end_victory");
            loseTextures = new Texture2D[4];
            loseTextures[(int)DeathMethods.Falling] = Canvas.Assets.GetTexture("LevelEndCards/end_falldeath");
            loseTextures[(int)DeathMethods.Miasma] = Canvas.Assets.GetTexture("LevelEndCards/end_miasmadeath");
            loseTextures[(int)DeathMethods.Shooter] = Canvas.Assets.GetTexture("LevelEndCards/end_shooterdeath");
            loseTextures[(int)DeathMethods.Spikes] = Canvas.Assets.GetTexture("LevelEndCards/end_spikedeath");
            actBackgrounds = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                actBackgrounds[i] = Canvas.Assets.GetTexture("SketchBackgrounds/sketch_act" + i);
            source = new Rectangle(actBackgrounds[0].Width / 2 - loseTextures[0].Width, actBackgrounds[0].Height / 2 - loseTextures[0].Height, 2 * loseTextures[0].Width, 2 * loseTextures[0].Height);

            lossDelay = 100;
            string selectorTheme = Canvas.AssetDictionary.LookupString("pauseMenu", "selectorTheme");
            Vector2 selectorPosition = Canvas.AssetDictionary.LookupVector2("pauseMenu", "selectorPosition");
            Anchor selectorAnchor = Anchor.Center;
            if (Canvas.AssetDictionary.CheckPropertyExists("pauseMenu", "selectorAnchor"))
                Enum.TryParse<Anchor>(Canvas.AssetDictionary.LookupString("pauseMenu", "selectorAnchor"), out selectorAnchor);

            List<Option> options = new List<Option>();
            /*options.Add(new TextureOption(Canvas.Assets.GetTexture("text_unpause")));
            options.Add(new TextureOption(Canvas.Assets.GetTexture("text_restartfromcheckpoint")));
            options.Add(new TextureOption(Canvas.Assets.GetTexture("text_restartfrombeginning")));
            options.Add(new TextureOption(Canvas.Assets.GetTexture("text_mainmenu")));*/
            options.Add(new TextOption("Unpause"));
            options.Add(new TextOption("Restart from Checkpoint"));
            options.Add(new TextOption("Restart from Beginning"));
            options.Add(new TextOption("Back to Level Select"));

            selector = new MultiOptionSelector(Canvas, "pauseSelector", selectorPosition, selectorAnchor, options, MultiOptionArrangement.ListY, new TextureCursor(false), 0);
            if (level != null)
                levelEngine = new LevelEngine(level, levelInfo, InputController, Canvas, AudioPlayer);
            else
                levelEngine = new LevelEngine(levelInfo, InputController, Canvas, AudioPlayer);
            paused = false;
            firstPass = true;
            this.BackgroundColor = levelEngine.BackgroundColor;

            pauseMenu = Canvas.Assets.GetTexture("menu_background");
            pauseText = Canvas.Assets.GetTexture("text_pause");
            spriteFont = Canvas.Assets.GetFont("Conformity24");
        }

        public override void Dispose()
        {
            levelEngine.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            float secondsElapsed = (float)(gameTime.TotalGameTime - TimeLoaded).TotalSeconds;
            if (secondsElapsed > 1 && secondsElapsed < 1.5f)
                titleCardLocation.X = GraphicsHelper.FastStep((secondsElapsed - 1) * 2) * titleStringLength;
            else if (secondsElapsed > 4.5f)
                titleCardLocation.X = GraphicsHelper.FastStep((5 - secondsElapsed) * 2) * titleStringLength;
            if (paused)
            {
                selector.Update(InputController);
                //if (InputController.Pause.JustPressed && levelName != "none")
                if (InputController.Pause.JustPressed || InputController.MenuForward.JustPressed)
                {
                    if (selector.IntValue == 1)
                        NextContext = new LevelContext(levelInfo, this);
                    else if (selector.IntValue == 2)
                        NextContext = new LevelContext(FileManager.ActiveFile.GetLevelInfo(levelInfo.ActID, levelInfo.LevelID), this);
                    else if (selector.IntValue == 3)
                        NextContext = new LevelSelectContext(this);
                    paused = false;
                }
            }
            else
            {
                if (InputController.Pause.JustPressed && !levelEngine.Win && !levelEngine.Lose)
                    paused = true;
                levelEngine.Update(gameTime);
            }

            if (levelEngine.Win)
                winTime++;
            if (levelEngine.Lose)
                loseTime++;
            if (levelEngine.Win && NextContext == null && InputController.AllKeys.JustPressed && levelName != "none")
            {
                //save
                FileManager.ActiveFile.SetLevelInfo(levelInfo);
                FileManager.ActiveFile.Save();
                //decide next level
                NextContext = FileManager.LevelProgression.GetNextContext(this, "Level" + levelInfo.ActID + "_" + levelInfo.LevelID);
            }

            if (levelEngine.Lose && NextContext == null && InputController.AllKeys.JustPressed && levelName != "none")
            {
                // Resets to last checkpoint after some time.
                //if (lossDelay < 0)
                //{
                    NextContext = new LevelContext(levelInfo, this);
                //}
                //if (!paused)
                //    lossDelay--;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            if (firstPass)
            {
                Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
                Canvas.Camera.Scale = 1f;
                firstPass = false;
            }

            if (paused)
            {
                Canvas.Frozen = true;
                levelEngine.Draw(gameTime);
                Canvas.Frozen = false;
                Canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
                //Canvas.DrawString("PAUSED", Color.Red, Canvas.Camera.Dimensions / 2, Anchor.Center);
                Canvas.DrawTexture(pauseMenu, Color.White, Canvas.Camera.Dimensions / 2, Anchor.Center, 0, 0.5f);
                Canvas.DrawTexture(pauseText, Color.White, Canvas.Camera.Dimensions / 2 + new Vector2(-40, -220), Anchor.Center, 0, 1f);
                selector.Draw(Canvas);
            }
            else
            {
                Canvas.Camera.Update(gameTime);
                levelEngine.Draw(gameTime);
            }
            //Rectangle r = Canvas.Camera.Bounds;
            //r.Inflate(Canvas.Camera.VisiblePhysical.Width / 2, Canvas.Camera.VisiblePhysical.Height / 2);
            //Canvas.DrawRectangle(Color.Red, 10, r, true);
            Canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            Canvas.DrawTexture(titleCardBottom, Color.White, titleCardLocation, Anchor.CenterRight, 0, 0.75f);
            Canvas.DrawString(titleText, titleFont, Color.Black, titleCardLocation + new Vector2(-30, 10), Anchor.CenterRight);
            if (levelEngine.Win)
            {
                float alpha = MathHelper.Clamp(winTime / 15f - 0.1f, 0, 1);
                Canvas.DrawTexture(actBackgrounds[this.levelInfo.ActID], source, new Color(1, 1, 1, alpha), Canvas.Camera.Dimensions / 2, Anchor.Center, 0f, 0.375f);
                Canvas.DrawTexture(winTexture, new Color(1, 1, 1, alpha), Canvas.Camera.Dimensions / 2, Anchor.Center, 0f, 0.75f);
                Canvas.DrawRectangle(new Color(0, 0, 0, alpha), 3, Canvas.Camera.Dimensions / 2 - 0.375f * new Vector2(winTexture.Width, winTexture.Height), 0.75f * new Vector2(winTexture.Width, winTexture.Height), 0, false);
            }
            else if (levelEngine.Lose)
            {
                float alpha = MathHelper.Clamp(loseTime / 15f - 0.1f, 0, 1);
                Canvas.DrawTexture(actBackgrounds[this.levelInfo.ActID], source, new Color(1, 1, 1, alpha), Canvas.Camera.Dimensions / 2, Anchor.Center, 0f, 0.375f);
                Canvas.DrawTexture(loseTextures[(int)levelEngine.DeathMethod], new Color(1, 1, 1, alpha), Canvas.Camera.Dimensions / 2, Anchor.Center, 0f, 0.75f);
                Canvas.DrawRectangle(new Color(0, 0, 0, alpha), 3, Canvas.Camera.Dimensions / 2 - 0.375f * new Vector2(winTexture.Width, winTexture.Height), 0.75f * new Vector2(winTexture.Width, winTexture.Height), 0, false);
            }
            if (levelEngine.Lose || levelEngine.Win)
                Canvas.DrawString("PRESS ANY BUTTON TO CONTINUE", spriteFont, new Color(0, 0, 0, ((float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2) + 1) / 2), new Vector2(Canvas.Camera.Dimensions.X / 2, Canvas.Camera.Dimensions.Y - 50), Anchor.BottomCenter);
        }

        public override void PlayAudio(GameTime gameTime)
        {
            if (!songPlaying && (gameTime.TotalGameTime - TimeLoaded).TotalSeconds > 1)
            {
                AudioPlayer.PlaySong("act" + levelEngine.ActNumber);
                songPlaying = true;
            }
            selector.PlayAudio(AudioPlayer);
            levelEngine.PlayAudio(gameTime);
        }
    }
}
