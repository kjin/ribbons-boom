using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Graphics
{
    /// <summary>
    /// A collection of sprites.
    /// </summary>
    public class SpriteCollection
    {
        protected Canvas canvas;
        protected List<Sprite> sprites;

        /// <summary>
        /// Constructs a new SpriteCollection instance.
        /// </summary>
        /// <param name="canvas">The canvas associated with the game.</param>
        public SpriteCollection(Canvas canvas)
        {
            this.canvas = canvas;
            sprites = new List<Sprite>();
        }

        /// <summary>
        /// Adds a sprite to this collection.
        /// </summary>
        /// <param name="sprite">The sprite object to add.</param>
        public void Add(Sprite sprite) { sprites.Add(sprite); }

        /// <summary>
        /// Adds a sprite to this collection based on its asset name.
        /// </summary>
        /// <param name="assetName">The asset name of the sprite object to add.</param>
        public void Add(string assetName) { sprites.Add(Sprite.Build(canvas, assetName)); }

        /// <summary>
        /// Gets the total number of sprites in this collection.
        /// </summary>
        public int Length { get { return sprites.Count; } }

        public Sprite this[int index] { get { return sprites[index]; } }
    }
}
