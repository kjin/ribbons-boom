using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeLibrary.Graphics
{
    /// <summary>
    /// A class that holds data related to ground. A ground map is defined by a set of axis-aligned rectangles.
    /// </summary>
    public class GroundMap
    {
        Effect effect;
        List<Texture2D> fillTextures;
        Texture2D edgeTexture;
        List<Vector2> fillMotions;
        List<Color> fillColors;
        List<Rectangle> rectangles;
        List<Path3D> paths;
        Rectangle groundExtent;

        float time;

        /// <summary>
        /// Constructs a new ground map.
        /// </summary>
        /// <param name="canvas">The canvas associated with the game.</param>
        /// <param name="groundType">The type of ground which determines which textures are drawn. These are found in the "graphics.txt" text file.</param>
        /// <param name="rectangles">A list of rectangles defining the covered area of the ground map.</param>
        public GroundMap(Canvas canvas, string theme, List<Rectangle> rectangles, float offset = 0)
        {
            this.rectangles = rectangles;
            groundExtent = new Rectangle();
            if (rectangles.Count > 0) groundExtent = rectangles[0];
            for (int i = 1; i < rectangles.Count; i++)
                groundExtent = Rectangle.Union(groundExtent, rectangles[i]);
            List<List<Vector2>> controlPoints = GraphicsHelper.GetBorders(rectangles);
            paths = new List<Path3D>();
            Vector2 edgeDimensions = new Vector2(0f, 0.05f);
            if (canvas.AssetDictionary.CheckPropertyExists(theme, "groundEdgeDimensions"))
                edgeDimensions = canvas.AssetDictionary.LookupVector2(theme, "groundEdgeDimensions");
            for (int i = 0; i < controlPoints.Count; i++)
                paths.Add(new Path3D(controlPoints[i], offset, 0.08f, 5, new RectangleCrossSection(edgeDimensions), true));
            //set up drawing
            if (canvas.AssetDictionary.CheckPropertyExists(theme, "groundEffect"))
                effect = canvas.Assets.GetEffect(canvas.AssetDictionary.LookupString(theme, "groundEffect"));
            else
                effect = canvas.Assets.GetEffect("GroundEffect");
            fillTextures = new List<Texture2D>();
            fillMotions = new List<Vector2>();
            fillColors = new List<Color>();
            int numFillTextures = 1;
            if (canvas.AssetDictionary.CheckPropertyExists(theme, "groundFillLayers"))
                numFillTextures = canvas.AssetDictionary.LookupInt32(theme, "groundFillLayers");
            for (int i = 0; i < numFillTextures; i++)
            {
                string fillTextureName = canvas.AssetDictionary.LookupString(theme, "groundFillTexture" + i);
                Texture2D texture;
                texture = canvas.Assets.GetTexture(fillTextureName);
                fillTextures.Add(texture);

                Vector2 motion = Vector2.Zero;
                Color color = Color.White;
                if (canvas.AssetDictionary.CheckPropertyExists(theme, "groundFillMotion" + i))
                    motion = canvas.AssetDictionary.LookupVector2(theme, "groundFillMotion" + i);
                if (canvas.AssetDictionary.CheckPropertyExists(theme, "groundFillColor" + i))
                    color = canvas.AssetDictionary.LookupColor(theme, "groundFillColor" + i);

                fillMotions.Add(motion);
                fillColors.Add(color);
            }
            edgeTexture = canvas.Assets.GetTexture(canvas.AssetDictionary.LookupString(theme, "groundEdgeTexture"));
            Color edgeColor = Color.White;
            if (canvas.AssetDictionary.CheckPropertyExists(theme, "groundEdgeColor"))
                edgeColor = canvas.AssetDictionary.LookupColor(theme, "groundEdgeColor");
            //effect.Parameters["Texture"].SetValue(edgeTexture);
            effect.Parameters["Color"].SetValue(edgeColor.ToVector4());
            effect.Parameters["TextureHeight"].SetValue(edgeTexture.Height / GraphicsConstants.PIXELS_PER_UNIT * (GraphicsConstants.SPRITE_SCALE * GraphicsConstants.GRAPHICS_SCALE));
        }

        /// <summary>
        /// Gets the edge texture of this ground map.
        /// </summary>
        public Texture2D EdgeTexture
        {
            get
            {
                return edgeTexture;
            }
        }

        /// <summary>
        /// Gets the list of rectangles that make up this ground map.
        /// </summary>
        public List<Rectangle> Rectangles
        {
            get
            {
                return rectangles;
            }
        }

        /// <summary>
        /// Gets the effect with which the ground is drawn.
        /// </summary>
        public Effect Effect
        {
            get
            {
                return effect;
            }
        }

        /// <summary>
        /// Gets a list of all the 3D edges of the ground.
        /// </summary>
        public List<Path3D> Paths
        {
            get
            {
                return paths;
            }
        }

        /// <summary>
        /// Gets a bounding rectangle of the ground.
        /// </summary>
        public Rectangle GroundExtent
        {
            get
            {
                return groundExtent;
            }
        }

        /// <summary>
        /// Gets the texture that fills the interior portion of the ground.
        /// </summary>
        public List<Texture2D> FillTextures
        {
            get
            {
                return fillTextures;
            }
        }

        /// <summary>
        /// Gets the motions of the fill textures.
        /// </summary>
        public List<Vector2> FillMotions
        {
            get
            {
                return fillMotions;
            }
        }

        /// <summary>
        /// Gets the colors of the fill textures.
        /// </summary>
        public List<Color> FillColors
        {
            get
            {
                return fillColors;
            }
        }

        public float Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }
    }
}
