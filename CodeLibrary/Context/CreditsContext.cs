using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CodeLibrary.Content;
using CodeLibrary.Graphics;

namespace CodeLibrary.Context
{
    public class CreditsContext : GameContext
    {
        string text;

        public CreditsContext(GameContext other)
            : base(other)
        {
            
        }

        public override void Initialize()
        {
            Text t = Canvas.Assets.GetText("creditstext");
            text = "";
            for (int i = 0; i < t.Length; i++)
                text += t[i] + '\n';
        }

        public override void Dispose() { }

        public override void Update(GameTime gameTime)
        {
            if (InputController.AllKeys.JustPressed)
                NextContext = new SplashScreenMenuContext(this);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            Canvas.DrawString(text, Color.White, Vector2.Zero);
        }

        public override void PlayAudio(GameTime gameTime) { }
    }
}
