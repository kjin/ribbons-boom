using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;

namespace CodeLibrary.Context
{
    public class LevelOption : Option
    {
        Texture2D texture;
        Texture2D border;
        Rectangle sourceRectangle;
        int level;
        bool beaten;
        static Vector2 size = new Vector2(225, 150);

        public LevelOption(Canvas canvas, int act, int level, bool beaten)
            : base()
        {
            border = canvas.Assets.GetTexture("cardshell");
            texture = canvas.Assets.GetTexture("FullBackgrounds/act" + act + "_fullimage");
            //sourceRectangle = new Rectangle((int)((texture.Width - (int)size.X) * levelPortion), (int)((texture.Height - (int)size.Y) *  (1 - levelPortion)), (int)size.X, (int)size.Y);
            sourceRectangle = new Rectangle(0, 0, texture.Width, (int)(texture.Width * 150f/225f));
            this.level = level;
            this.beaten = beaten || (act == 0 && level == 1);
            dimensions = size;
        }

        public override void Initialize(Canvas canvas, string themeName)
        {
            base.Initialize(canvas, themeName);
        }

        public override void Draw(Canvas canvas, Vector2 selectorPosition)
        {
            canvas.DrawTexture(texture, sourceRectangle, beaten ? Color.White : Color.Gray, position + selectorPosition, 0f, 225f / texture.Width);
            canvas.DrawTexture(border, beaten ? Color.White : Color.Gray, position + selectorPosition, 0f, 0.5f);
            base.Draw(canvas, selectorPosition);
            //canvas.DrawRectangle(Color.Blue, Color.White, 4, position + selectorPosition, Anchor.TopLeft, );
        }

        public Texture2D Texture { get { return texture; } }
        public Rectangle SourceRectangle { get { return sourceRectangle; } }
    }

    public static class RainbowColors
    {
        static Color[] colors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Indigo, Color.Purple };

        public static Color GetColor(int i) { return colors[i % colors.Length]; }
    }
}
