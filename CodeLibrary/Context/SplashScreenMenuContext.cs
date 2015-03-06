using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Engine;
using CodeLibrary.Storage;

namespace CodeLibrary.Context
{
    /// <summary>
    /// The splash screen context of the game.
    /// </summary>
    public class SplashScreenMenuContext : MenuContext
    {
        Texture2D midground;
        Texture2D foreground;
        Texture2D ribbon;
        Texture2D title;
        List<Selector> selectors;
        int active;

        Color brightBackground = new Color(0f, 0.2f, 0.4f, 1f);
        Color darkBackground = new Color(0f, 0.1f, 0.3f, 1f);

        public SplashScreenMenuContext(GameContext other)
            : base(other)
        {
        }

        public override void Initialize()
        {
            FileManager.ActiveFile = FileManager.LoadPlayerFile(0);
            //get info from graphics.txt
            string startSelectorTheme = Canvas.AssetDictionary.LookupString("splashScreen", "startSelectorTheme");
            Vector2 startSelectorPosition = Canvas.AssetDictionary.LookupVector2("splashScreen", "startSelectorPosition");
            Anchor startSelectorAnchor = Anchor.Center;
            if (Canvas.AssetDictionary.CheckPropertyExists("splashScreen", "startSelectorAnchor"))
                Enum.TryParse<Anchor>(Canvas.AssetDictionary.LookupString("splashScreen", "startSelectorAnchor"), out startSelectorAnchor);

            //load textures
            midground = Canvas.Assets.GetTexture("splashMidground");
            foreground = Canvas.Assets.GetTexture("splashForeground");
            ribbon = Canvas.Assets.GetTexture("splashRibbon");
            title = Canvas.Assets.GetTexture("splashTitle");

            //add options
            List<Option> options = new List<Option>();
            //options.Add(new TextureOption(Canvas.Assets.GetTexture("text_newgame")));
            //options.Add(new TextureOption(Canvas.Assets.GetTexture("text_loadgame")));
            options.Add(new TextOption("Quick Play"));
            options.Add(new TextOption("File Select"));
            options.Add(new TextOption("Options"));
            options.Add(new TextOption("Exit"));
            //options.Add(new TextureOption(Canvas.Assets.GetTexture("text_options")));
            //options.Add(new TextureOption(Canvas.Assets.GetTexture("text_exit")));

            //initialize selectors
            active = 0;
            selectors = new List<Selector>();
            //selectors.Add(new SingularSelector(Canvas, startSelectorTheme, startSelectorPosition, startSelectorAnchor, new TextOption("PRESS START")));
            selectors.Add(new SingularSelector(Canvas, startSelectorTheme, startSelectorPosition, startSelectorAnchor, new TextureOption(Canvas.Assets.GetTexture("text_pressenter"))));
            selectors.Add(ContextHelper.BuildMultiOptionSelector(Canvas, "splashScreen", "start", options, new TextureCursor(false), 0));

            //SongObject songObject = new SongObject(audioPlayer, "act1");
            //audioPlayer.PlaySong(songObject);
            AudioPlayer.StopSong();
        }

        public override void Update(GameTime gameTime)
        {
            selectors[active].Update(InputController);
            if (InputController.MenuForward.JustPressed)
            {
                if (InputController.Debug.Pressed)
                {
                    NextContext = new LevelSelectContext(this);
                    FileManager.ActiveFile = FileManager.LoadPlayerFile(999);
                    return;
                }
                switch (active)
                {
                    case 0:
                        active = 1;
                        break;
                    case 1:
                        switch (selectors[active].IntValue)
                        {
                            case 0:
                                NextContext = new LevelSelectContext(this);
                                FileManager.ActiveFile = FileManager.LoadPlayerFile(999);
                                break;
                            case 1:
                                NextContext = new FileSelectContext(this);
                                break;
                            case 2:
                                NextContext = new OptionsContext(this);
                                break;
                            case 3:
                                Exit = true;
                                break;
                        }
                        break;
                }
            }
            BackgroundColor = Color.Lerp(brightBackground, darkBackground, (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds / 8) + 1) / 2);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            float correctScale = Math.Max(Canvas.Camera.Dimensions.X / midground.Width, Canvas.Camera.Dimensions.Y / midground.Height) / GraphicsConstants.GRAPHICS_SCALE;
            Vector2 center = Canvas.Camera.Dimensions / 2;
            Vector2 backgroundOffset = new Vector2(0, 10 * ((float)Math.Cos(gameTime.TotalGameTime.TotalSeconds / 2) + 1) / 2);
            Vector2 titleRibbonOffset = new Vector2(0, 5 * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds));
            Canvas.DrawTexture(midground, Color.White, 2 * backgroundOffset + new Vector2(0, -20), 0f, correctScale);
            Canvas.DrawTexture(foreground, Color.White, backgroundOffset, 0f, correctScale);
            Canvas.DrawTexture(ribbon, Color.White, center + new Vector2(0, -50) - titleRibbonOffset, Anchor.Center, 0f, correctScale);
            Canvas.DrawTexture(title, Color.White, center + new Vector2(-150, -50) + titleRibbonOffset, Anchor.Center, 0f, correctScale);
            Canvas.Offset = titleRibbonOffset;
            selectors[active].Draw(Canvas);
            Canvas.Offset = Vector2.Zero;
            //Canvas.DrawString(AudioPlayer.songSwitch - DateTime.Now.TimeOfDay, Color.Red, Vector2.Zero);
            base.Draw(gameTime);
        }

        public override void PlayAudio(GameTime gameTime)
        {
            for (int i = 0; i < selectors.Count; i++)
                selectors[i].PlayAudio(AudioPlayer);
            base.PlayAudio(gameTime);
        }
    }
}
