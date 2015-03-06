using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using CodeLibrary.Content;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Context;

namespace CodeLibrary.Engine
{
    public class DecorationObject
    {

        private Sprite sprite;
        private Vector2 position;
        private float rotation;
        private Vector2 scale;

        public DecorationObject(Canvas c, string file, Vector2 position, Vector2 dimensions, float rotation = 0.0f)
        {
            sprite = Sprite.Build(c, file);
            this.position = position;

            scale.X = dimensions.X / sprite.Width;
            scale.Y = dimensions.Y / sprite.Height;

            this.rotation = rotation;

        }

        public void Draw(Canvas c)
        {
            c.DrawSprite(sprite, Color.White, position, rotation, scale);
        }
    }
}
