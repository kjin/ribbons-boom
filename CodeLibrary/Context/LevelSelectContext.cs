using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeLibrary.Graphics;
using CodeLibrary.Input;
using CodeLibrary.Audio;
using CodeLibrary.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeLibrary.Context
{
    public class LevelSelectContext : GameContext
    {
        List<List<string>> levels;
        List<MultiOptionSelector> options;
        List<Vector2> optionOffsets;

        int activeWorld;
        int activeLevel;
        LevelInfo activeLevelInfo;
        string activeLevelName;

        Texture2D[] backgrounds;
        Texture2D prevArrow;
        Texture2D nextArrow;
        bool songPlaying;

        SpriteFont titleFont;
        SpriteFont subtitleFont;
        SpriteFont infoFont;

        Texture2D collectable;
        Texture2D bowStone;
        Texture2D ribbon;

        public LevelSelectContext(GameContext other)
            : base(other)
        {
        }

        public override void Initialize()
        {
            List<List<Option>> optionsList = new List<List<Option>>();
            levels = new List<List<string>>();
            for (int i = 0; i < 4; i++)
            {
                optionsList.Add(new List<Option>());
                levels.Add(new List<string>());
            }
            List<string> levelNames = Canvas.Assets.GetLevelList();
            int[] maxLevel = new int[4];
            for (int i = 0; i < levelNames.Count; i++)
                maxLevel[StorageHelper.GetActNumber(levelNames[i])] = Math.Max(maxLevel[StorageHelper.GetActNumber(levelNames[i])], StorageHelper.GetLevelNumber(levelNames[i]));
            for (int i = 0; i < levelNames.Count; i++)
            {
                int act = StorageHelper.GetActNumber(levelNames[i]);
                //Option option = new LevelOption(Canvas, act, level, true);
                levels[act].Add(levelNames[i]);
            }

            for (int i = 0; i < levels.Count; i++)
            {
                //sort levels
                levels[i].Sort(new LevelSorter());
                for (int j = 0; j < levels[i].Count; j++)
                {
                    int act = StorageHelper.GetActNumber(levels[i][j]);
                    int level = StorageHelper.GetLevelNumber(levels[i][j]);
                    //TODO: Change this back
                    Option option = new LevelOption(Canvas, act, level, FileManager.ActiveFile.GetLevelInfo(act, level).Complete || (act == 0 && level == 1) || FileManager.ActiveFile.ID == 999);
                    optionsList[act].Add(option);
                }
            }

            options = new List<MultiOptionSelector>();
            optionOffsets = new List<Vector2>();
            backgrounds = new Texture2D[optionsList.Count];
            for (int i = 0; i < optionsList.Count; i++)
            {
                Cursor cursor = new TextureCursor();
                options.Add(ContextHelper.BuildMultiOptionSelector(Canvas, "levelSelect", "level", optionsList[i], new MultiOptionArrangement(1, optionsList[i].Count, true), cursor));
                options[i].EnableCursorBounds(new RectangleF(0, 100, 0, GraphicsConstants.VIEWPORT_HEIGHT * 490 / 720f));
                backgrounds[i] = Canvas.Assets.GetTexture("SketchBackgrounds/sketch_act" + i);
                optionOffsets.Add(GraphicsConstants.VIEWPORT_WIDTH * Vector2.UnitX);
            }
            BackgroundColor = Color.DeepSkyBlue;
            activeWorld = 0;

            titleFont = Canvas.Assets.GetFont("Conformity96");
            subtitleFont = Canvas.Assets.GetFont("Conformity48");
            infoFont = Canvas.Assets.GetFont("SplashFont");
            collectable = Canvas.Assets.GetTexture("Collectables/collect0");
            bowStone = Canvas.Assets.GetTexture("collect_stone");
            ribbon = Canvas.Assets.GetTexture("verticalribbon");
            prevArrow = Canvas.Assets.GetTexture("prevArrow");
            nextArrow = Canvas.Assets.GetTexture("nextArrow");

            activeLevelInfo = FileManager.ActiveFile.GetLevelInfo(activeWorld, activeLevel);
            activeLevelName = FileManager.GetLevelTitle(activeWorld, activeLevel);
        }

        private void ProcessOptionSpread()
        {
            for (int i = 0; i < 4; i++)
                optionOffsets[i] = Vector2.Lerp(optionOffsets[i], (i - activeWorld) * GraphicsConstants.VIEWPORT_WIDTH * Vector2.UnitX, 0.5f);
        }

        public override void Dispose() { }

        public override void Update(GameTime gameTime)
        {
            //cache previous values.
            int previousAct = activeWorld;
            int previousLevel = activeLevel;
            if (InputController.MenuLeft.JustPressed)
                activeWorld = Math.Max(activeWorld - 1, 0);
            if (InputController.MenuRight.JustPressed)
                activeWorld = Math.Min(activeWorld + 1, options.Count - 1);
            activeLevel = StorageHelper.GetLevelNumber(levels[activeWorld][options[activeWorld].IntValue]);
            //switch level info
            if (previousAct != activeWorld || previousLevel != activeLevel)
            {
                activeLevelInfo = FileManager.ActiveFile.GetLevelInfo(activeWorld, activeLevel);
                activeLevelName = StorageHelper.Intellisplit(FileManager.GetLevelTitle(activeWorld, activeLevel), 60);
            }
            if (InputController.MenuForward.JustPressed && (FileManager.ActiveFile.GetLevelInfo(activeWorld, activeLevel).Complete || (activeWorld == 0 && activeLevel == 1) || FileManager.ActiveFile.ID == 999))
                NextContext = new LevelContext(FileManager.ActiveFile.GetLevelInfo(activeWorld, activeLevel), this);
            if (InputController.MenuBackward.JustPressed)
                NextContext = new SplashScreenMenuContext(this);
            options[activeWorld].Update(InputController);
            ProcessOptionSpread();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 offset1 = new Vector2(0, 10 * ((float)Math.Cos(gameTime.TotalGameTime.TotalSeconds / 2) + 1) / 2);
            Vector2 offset2 = new Vector2(0, 5 * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds));

            Canvas.DrawTexture(backgrounds[activeWorld], Color.Gray, Canvas.Camera.Dimensions / 2, Anchor.Center, 0, 720f / 1080f);
            Canvas.EndDraw();
            Canvas.BeginDraw(0, BlendState.NonPremultiplied);
            for (int i = 0; i < options.Count; i++)
            {
                Canvas.Offset = optionOffsets[i] + offset1;
                Canvas.DrawTexture(ribbon, Color.White, new Vector2(100, -50), 0, 0.8f, false);
                options[i].Draw(Canvas);
            }
            Canvas.Offset = Vector2.Zero;
            Canvas.DrawString("Level Select", titleFont, Color.White, new Vector2(Canvas.Camera.Dimensions.X - 50, 50) + offset1, Anchor.TopRight);
            Canvas.DrawString(String.Format("Level {0}-{1}", activeWorld, activeLevel), subtitleFont, Color.White, new Vector2(440, 640) + offset1, Anchor.BottomLeft);
            Canvas.DrawString(activeLevelName, infoFont, Color.White, new Vector2(440, 640) + offset2, Anchor.TopLeft);
            //draw collectables
            if (activeWorld > 0)
            {
                float collectedIntensity = 0.8f + 0.1f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2);
                Color collected = new Color(collectedIntensity, collectedIntensity, collectedIntensity, 1);
                Canvas.DrawTexture(bowStone, Color.White, new Vector2(920, 300) + offset2, Anchor.Center, 0, 0.5f);
                Canvas.DrawTexture(bowStone, Color.White, new Vector2(1040, 300) + offset2, Anchor.Center, 0, 0.5f);
                Canvas.DrawTexture(bowStone, Color.White, new Vector2(1160, 300) + offset2, Anchor.Center, 0, 0.5f);
                Canvas.DrawTexture(collectable, (activeLevelInfo.Collectables & 0x1) != 0 ? collected : Color.Gray, new Vector2(920, 300) + offset2, Anchor.Center, 0, 0.5f);
                Canvas.DrawTexture(collectable, (activeLevelInfo.Collectables & 0x2) != 0 ? collected : Color.Gray, new Vector2(1040, 300) + offset2, Anchor.Center, 0, 0.5f);
                Canvas.DrawTexture(collectable, (activeLevelInfo.Collectables & 0x4) != 0 ? collected : Color.Gray, new Vector2(1160, 300) + offset2, Anchor.Center, 0, 0.5f);
                Canvas.DrawString("COLLECTABLES", infoFont, Color.White, new Vector2(840, 300) + offset2, Anchor.TopRight);
            }
            int mins = (int)((activeLevelInfo.CompletionTime / 1000f) / 60f);
            int secs = (int)(activeLevelInfo.CompletionTime / 1000f) - mins * 60;
            int millisecs = activeLevelInfo.CompletionTime - mins * 1000 * 60 - secs * 1000;
            Canvas.DrawString(String.Format("{0:D2}:{1:D2}:{2:D3}", mins, secs, millisecs), subtitleFont, Color.White, new Vector2(860, 400) + offset2, Anchor.TopLeft);
            Canvas.DrawString("BEST TIME", infoFont, Color.White, new Vector2(840, 400) + offset2, Anchor.TopRight);
            if (activeWorld > 0)
            {
                Canvas.DrawTexture(prevArrow, Color.White, new Vector2(0, 720) + offset2, Anchor.BottomLeft, 0, 0.4f);
            }
            if (activeWorld < options.Count - 1)
            {
                Canvas.DrawTexture(nextArrow, Color.White, new Vector2(1280, 720) + offset2, Anchor.BottomRight, 0, 0.4f);
            }
        }

        public override void PlayAudio(GameTime gameTime)
        {
            if (!songPlaying && (gameTime.TotalGameTime - TimeLoaded).TotalSeconds > 0.5f)
            {
                AudioPlayer.PlaySong("menu");
                songPlaying = true;
            }
            options[activeWorld].PlayAudio(AudioPlayer);
        }
    }

    public class LevelSorter : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int levelX = StorageHelper.GetLevelNumber(x);
            int levelY = StorageHelper.GetLevelNumber(y);
            return levelX - levelY;
        }
    }
}

