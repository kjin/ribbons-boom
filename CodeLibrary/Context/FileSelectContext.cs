using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;
using CodeLibrary.Storage;

namespace CodeLibrary.Context
{
    /// <summary>
    /// The context in which a player's saved game can be chosen.
    /// A new game can be started by either selecting an empty file or a "no save" option.
    /// When a game is selected, options displayed are: Story Mode/Level Select/Erase.
    /// </summary>
    public class FileSelectContext : GameContext
    {
        static int NUM_FILES = 10;

        List<PlayerFile> filePointers;

        MultiOptionSelector files;
        MultiOptionSelector fileActions;

        List<MultiOptionSelector> optionSets;
        int activeSet;

        Texture2D actionBackdrop;
        bool songPlaying;

        SpriteFont titleFont;

        ScrollingLogoBackground bg;

        public FileSelectContext(GameContext other)
            : base(other) { }

        public override void Initialize()
        {
            List<Option> fileList = new List<Option>();
            filePointers = new List<PlayerFile>();
            for (int i = 0; i < NUM_FILES; i++)
            {
                PlayerFile pf = this.FileManager.LoadPlayerFile(i);
                filePointers.Add(pf);
                FileOption option = new FileOption(pf);
                fileList.Add(option);
            }
            Cursor cursor = new TextureCursor();
            files = ContextHelper.BuildMultiOptionSelector(Canvas, "fileSelect", "file", fileList, new MultiOptionArrangement(NUM_FILES, 1, true), cursor, 0);
            files.EnableCursorBounds(new RectangleF(0, (GraphicsConstants.VIEWPORT_WIDTH) * 870/1280f, 0, 100));

            List<Option> actionsList = new List<Option>();
            actionsList.Add(new TextOption("Story Mode"));
            actionsList.Add(new TextOption("Level Select"));
            actionsList.Add(new TextOption("Erase"));
            fileActions = ContextHelper.BuildMultiOptionSelector(Canvas, "fileSelect", "action", actionsList, new MultiOptionArrangement(1, 3, true), new DoubleTextureCursor());
            actionBackdrop = Canvas.Assets.GetTexture("file_actionback");

            optionSets = new List<MultiOptionSelector>();
            optionSets.Add(files);
            optionSets.Add(fileActions);

            titleFont = Canvas.Assets.GetFont("Conformity96");

            BackgroundColor = Color.SeaGreen;
            bg = new ScrollingLogoBackground(Canvas.Assets.GetTexture("ribbonsLogo"), BackgroundColor);
        }

        public override void Dispose() { }

        public override void Update(GameTime gameTime)
        {
            optionSets[activeSet].Update(InputController);
            if (InputController.MenuForward.JustPressed)
            {
                if (optionSets[activeSet].Equals(files))
                {
                    FileManager.ActiveFile = filePointers[files.IntValue];
                    if (FileManager.ActiveFile.Progress == 0)
                        NextContext = FileManager.LevelProgression.GetNextContext(this, String.Format("Level{0}_{1}", filePointers[files.IntValue].FarthestAct, filePointers[files.IntValue].FarthestLevel));
                    else
                        activeSet++;
                }
                else if (optionSets[activeSet].Equals(fileActions))
                {
                    switch (fileActions.IntValue)
                    {
                        case 0:
                            NextContext = FileManager.LevelProgression.GetNextContext(this, String.Format("Level{0}_{1}", filePointers[files.IntValue].FarthestAct, filePointers[files.IntValue].FarthestLevel));
                            break;
                        case 1:
                            NextContext = new LevelSelectContext(this);
                            break;
                        case 2:
                            FileManager.ActiveFile.Clear();
                            FileManager.SavePlayerFile(files.IntValue);
                            activeSet--;
                            break;
                    }
                }
            }
            else if (InputController.MenuBackward.JustPressed)
            {
                if (optionSets[activeSet].Equals(files))
                    NextContext = new SplashScreenMenuContext(this);
                else if (optionSets[activeSet].Equals(fileActions))
                    activeSet--;
            }
            bg.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            bg.Draw(Canvas);
            Canvas.EndDraw();
            Canvas.BeginDraw(0, BlendState.NonPremultiplied);
            Canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            //Canvas.Offset = -new Vector2(Math.Min(files.CursorPosition.X, 700), files.CursorPosition.Y);
            files.Draw(Canvas);
            Canvas.Offset = Vector2.Zero;
            if (activeSet > 0)
            {
                Canvas.DrawTexture(actionBackdrop, Color.Blue, Canvas.Camera.Dimensions / 2, Anchor.Center);
                fileActions.Draw(Canvas);
            }
            Canvas.DrawString("File Select", titleFont, Color.White, new Vector2(50, 50));
        }

        public override void PlayAudio(GameTime gameTime)
        {
            if (!songPlaying && (gameTime.TotalGameTime - TimeLoaded).TotalSeconds > 0.5f)
            {
                AudioPlayer.PlaySong("menu");
                songPlaying = true;
            }
            files.PlayAudio(AudioPlayer);
        }
    }
}
