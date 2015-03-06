using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;

namespace CodeLibrary.Context
{
    public class IntroContext : GameContext
    {
        Texture2D wetfloor;
        string topText;
        string bottomText;
        SpriteFont spriteFont;

        public IntroContext(GameContext other)
            : base(other)
        {
            BackgroundColor = Color.Black;
        }

        public override void Initialize()
        {
            wetfloor = Canvas.Assets.GetTexture("wetfloorsign");
            spriteFont = Canvas.Assets.GetFont("ImpactFont");
            topText = "PRESENTED BY";
            bottomText = "\"You'll fall on your butts!\"";
        }

        public override void Dispose() { }

        public override void Update(GameTime gameTime)
        {
            if (InputController.AllKeys.JustPressed || (gameTime.TotalGameTime - TimeLoaded).TotalSeconds > 3)
                NextContext = new SplashScreenMenuContext(this);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Canvas.DrawString(topText, spriteFont, Color.White, new Vector2(Canvas.Camera.Dimensions.X / 2, 100), Anchor.Center);
            Canvas.DrawTexture(wetfloor, Color.White, Canvas.Camera.Dimensions / 2, Anchor.Center, 0, 0.5f);
            Canvas.DrawString(bottomText, spriteFont, Color.White, new Vector2(Canvas.Camera.Dimensions.X / 2, Canvas.Camera.Dimensions.Y - 100), Anchor.Center);
        }

        public override void PlayAudio(GameTime gameTime) { }
    }
}
