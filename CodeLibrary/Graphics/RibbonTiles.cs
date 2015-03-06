using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CodeLibrary.Graphics
{
    public class RibbonTiles
    {
        Tileset tileset;
        List<TilesetComponent> components;

        public RibbonTiles(Tileset tileset, List<Vector2> vertices, bool clockwise = true)
        {
            this.tileset = tileset;
            components = new List<TilesetComponent>();
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 prev = vertices[(i + vertices.Count - 1) % vertices.Count];
                Vector2 next = vertices[(i + 1) % vertices.Count];
                components.Add(GraphicsHelper.GetTilesetComponent(prev, vertices[i], next, clockwise));
                if (i == 0)
                    components.Add(GraphicsHelper.GetTilesetComponent(prev, vertices[i], 2 * vertices[i] - prev, clockwise));
                else if (i == vertices.Count - 1)
                    components.Add(GraphicsHelper.GetTilesetComponent(2 * vertices[i] - next, vertices[i], next, clockwise));
                if (i < vertices.Count - 1)
                {
                    int points = (int)Math.Abs(next.Y - vertices[i].Y + next.X - vertices[i].X) + 1;
                    Vector2 lPrev = vertices[i];
                    Vector2 lCurr = Vector2.Lerp(vertices[i], next, 1f / (points - 1f));
                    for (int j = 1; j < points - 1; j++)
                    {
                        Vector2 lNext = Vector2.Lerp(vertices[i], next, (j + 1) / (points - 1f));
                        components.Add(GraphicsHelper.GetTilesetComponent(lPrev, lCurr, lNext, clockwise));
                        lPrev = lCurr;
                        lCurr = lNext;
                    }
                }
            }
        }

        public Tileset Tileset { get { return tileset; } }
        public int Length { get { return components.Count; } }
        public TilesetComponent this[int i] { get { return components[i]; } }
    }
}
