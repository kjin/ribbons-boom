using CodeLibrary.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Engine
{
    public class LevelGridStore
    {
        DebugColors dc;
        Vector2 position;
        string type;

        public Vector2 Position { get { return position; } set { position = value; } }
        public string Type { get { return type; } set { type = value; } }
        
        public LevelGridStore(Vector2 position, string type)
        {
            dc = new DebugColors();
            this.position = position;
            this.type = type;
        }

        public void DrawDebug(Canvas canvas)
        {
            canvas.DrawRectangle(dc.DebugColor(type), 5, new Rectangle((int)position.X, (int)position.Y, 1, 1));
        }
    }

    public class DebugColors
    {
        Color miasma = Color.Purple;
        Color ground = Color.Green;

        public Color DebugColor(string type)
        {
            switch (type)
            {
                case "miasma":
                    return miasma;
                case "ground":
                    return ground;
                default:
                    return Color.Transparent;
            }
        }
    }
}
