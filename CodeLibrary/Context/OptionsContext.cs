using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeLibrary.Graphics;
using CodeLibrary.Input;
using CodeLibrary.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeLibrary.Context
{
    public class OptionsContext : GameContext
    {
        List<Vector2> resolutions;
        List<MultiOptionSelector> options;
        int activeOption;
        Texture2D ribbonsLogo;
        Vector2 offset;
        Vector2 offsetIncrement;

        public OptionsContext(GameContext other)
            : base(other)
        {
        }

        public override void Initialize()
        {
            resolutions = new List<Vector2>();
            resolutions.Add(new Vector2(320, 240));
            resolutions.Add(new Vector2(640, 480));
            resolutions.Add(new Vector2(800, 600));
            resolutions.Add(new Vector2(1024, 768));
            resolutions.Add(new Vector2(1280, 720));
            resolutions.Add(new Vector2(1280, 768));
            resolutions.Add(new Vector2(1280, 800));
            resolutions.Add(new Vector2(1280, 1024));
            resolutions.Add(new Vector2(1366, 768));
            resolutions.Add(new Vector2(1440, 900));
            resolutions.Add(new Vector2(1600, 900));
            resolutions.Add(new Vector2(1920, 1080));
            resolutions.Add(new Vector2(1920, 1200));

            List<Option> resolutionOptionsList = new List<Option>();
            for (int i = 0; i < resolutions.Count; i++)
            {
                TextOption option = new TextOption(resolutions[i].ToString());
                resolutionOptionsList.Add(option);
            }
            options = new List<MultiOptionSelector>();
            options.Add(ContextHelper.BuildMultiOptionSelector(Canvas, "options", "options", resolutionOptionsList, new DoubleTextureCursor()));

            List<Option> fullScreenOptions = new List<Option>();
            fullScreenOptions.Add(new TextOption("Windowed"));
            fullScreenOptions.Add(new TextOption("Fullscreen"));
            options.Add(ContextHelper.BuildMultiOptionSelector(Canvas, "options", "options", fullScreenOptions, new DoubleTextureCursor()));

            ribbonsLogo = Canvas.Assets.GetTexture("ribbonsLogo");
            BackgroundColor = Color.Green;
            offsetIncrement = new Vector2(ribbonsLogo.Width, ribbonsLogo.Height);
            offsetIncrement.Normalize();
        }

        public override void Dispose() { }

        public void ApplyOptions()
        {
            GraphicsConstants.VIEWPORT_WIDTH = (int)resolutions[options[0].IntValue].X;
            GraphicsConstants.VIEWPORT_HEIGHT = (int)resolutions[options[0].IntValue].Y;
            GraphicsConstants.FULL_SCREEN = options[1].IntValue == 1;
            Canvas.SetViewParameters();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputController.MenuForward.JustPressed)
            {
                if (activeOption == 1)
                {
                    ApplyOptions();
                    NextContext = new SplashScreenMenuContext(this);
                }
                else
                    activeOption++;
            }
            if (InputController.MenuBackward.JustPressed)
            {
                if (activeOption == 0)
                    NextContext = new SplashScreenMenuContext(this);
                else
                    activeOption--;
            }
            options[activeOption].Update(InputController);
            offset += 2 * offsetIncrement;
            if (offset.X > ribbonsLogo.Width)
            {
                offset.X -= ribbonsLogo.Width;
                offset.Y -= ribbonsLogo.Height;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            int x = -2 * ribbonsLogo.Width;
            while (x < Canvas.Camera.Width)
            {
                int y = -2 * ribbonsLogo.Height;
                while (y < Canvas.Camera.Height)
                {
                    Canvas.DrawTexture(ribbonsLogo, BackgroundColor, GraphicsConstants.GRAPHICS_SCALE * (new Vector2(x, y) + offset), 0f, 1f);
                    Canvas.DrawTexture(ribbonsLogo, BackgroundColor, GraphicsConstants.GRAPHICS_SCALE * (new Vector2(x + ribbonsLogo.Width, y + ribbonsLogo.Height) + offset), 0f, 1f);
                    y += ribbonsLogo.Height * 2;
                }
                x += ribbonsLogo.Width * 2;
            }
            options[activeOption].Draw(Canvas);
        }

        public override void PlayAudio(GameTime gameTime)
        {
            options[activeOption].PlayAudio(AudioPlayer);
        }
    }
}

