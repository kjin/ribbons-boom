using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeLibrary.Graphics
{
    public class TilesetComponent
    {
        public TilesetComponents Tile;
        public Vector2 Position;
        public RightAngleRotations Rotation;
    }

    /// <summary>
    /// Superseded by GroundMap class.
    /// </summary>
    public class Tileset
    {
        protected Texture2D edgeTexture;
        protected Texture2D fillTexture;

        public Tileset(Texture2D edgeTexture, Texture2D fillTexture)
        {
            this.edgeTexture = edgeTexture;
            this.fillTexture = fillTexture;
        }

        public Texture2D EdgeTexture { get { return edgeTexture; } }

        public Texture2D FillTexture { get { return fillTexture; } }

        public Rectangle GetEdgeTile(TilesetComponents tile)
        {
            return new Rectangle((int)tile * edgeTexture.Width / 3, 0, edgeTexture.Width / 3, edgeTexture.Height);
        }

        public static Tileset Build(Canvas canvas, string assetName)
        {
            Texture2D edge = null;
            Texture2D fill = null;
            bool fillExists = true;
            try { edge = canvas.Assets.GetTexture(assetName + "_fill"); }
            catch { fillExists = false; }
            if (fillExists)
            {
                edge = canvas.Assets.GetTexture(assetName + "_edge");
            }
            else
                edge = canvas.Assets.GetTexture(assetName);
            return new Tileset(edge, fill);
        }
    }
}
