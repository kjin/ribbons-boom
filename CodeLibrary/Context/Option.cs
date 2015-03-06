﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;
using CodeLibrary.Input;

namespace CodeLibrary.Context
{
    public abstract class Option
    {
        protected bool isActive;
        protected bool wasActive;
        protected Vector2 dimensions;

        protected Vector2 position;

        public virtual void Initialize(Canvas canvas, string themeName) { }

        public virtual void Update(bool active)
        {
            isActive = active;
        }

        public virtual void Draw(Canvas canvas, Vector2 selectorPosition) { }

        public Vector2 Dimensions { get { return dimensions; } }
        public Vector2 Position { get { return position; } set { position = value; } }
    }

    public class TextOption : Option
    {
        string text;

        SpriteFont fontFace;
        Color fontColor;

        public TextOption(string text)
        {
            this.text = text;
        }

        public override void Initialize(Canvas canvas, string themeName)
        {
            fontFace = canvas.Assets.GetFont(canvas.AssetDictionary.LookupString(themeName, "fontFace"));
            try { fontColor = new Color(canvas.AssetDictionary.LookupVector4(themeName, "fontColor")); }
            catch { fontColor = new Color(canvas.AssetDictionary.LookupVector3(themeName, "fontColor")); }
            dimensions = fontFace.MeasureString(text) + new Vector2(0, -5);
        }

        public override void Draw(Canvas canvas, Vector2 selectorPosition)
        {
            canvas.DrawString(text, fontFace, fontColor, position + selectorPosition);
        }
    }

    public class TextureOption : Option
    {
        protected Texture2D texture;
        protected Color textureColor;

        public TextureOption(Texture2D texture, Anchor textAnchor = Anchor.Center)
        {
            this.texture = texture;
            dimensions = new Vector2(texture.Width, texture.Height) / 2;
        }

        public override void Initialize(Canvas canvas, string themeName)
        {
            try { textureColor = new Color(canvas.AssetDictionary.LookupVector4(themeName, "textureColor")); }
            catch
            {
                try { textureColor = new Color(canvas.AssetDictionary.LookupVector3(themeName, "textureColor")); }
                catch { textureColor = Color.White; }
            }
        }

        public override void Draw(Canvas canvas, Vector2 selectorPosition)
        {
            canvas.DrawTexture(texture, textureColor, position + selectorPosition, 0f, 0.5f);
        }
    }
}
