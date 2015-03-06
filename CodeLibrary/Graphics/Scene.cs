using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;

namespace CodeLibrary.Engine
{
    class Scene
    {
        private Sprite sprite;
        private SpriteCollection sprites;
        private Vector2 position;
        private int layer;
        private Color tint;

        public int Layer
        {
            get { return layer; }
        }

        public Scene(Sprite sprite, Vector2 position, int layer, Color tint)
        {
            this.sprite = sprite;
            this.position = position;
            this.layer = layer;
            this.tint = tint;
            this.sprites = null;
        }

        public Scene(SpriteCollection sprites, Vector2 position, int layer, Color tint)
        {
            this.sprites = sprites;
            this.position = position;
            this.layer = layer;
            this.tint = tint;
            this.sprite = null;
        }

        public void Draw(Canvas c)
        {
            if (sprite != null)
            {
                c.DrawSprite(sprite, tint, position);
            }
            else
            {
                // Draw the animation of the sprite collection
            }
        }

    }
}