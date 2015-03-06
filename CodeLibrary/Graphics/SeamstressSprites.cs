using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CodeLibrary.Graphics
{
    /// <summary>
    /// A collection of Seamstress sprites.
    /// </summary>
    public class SeamstressSprites : SpriteCollection
    {
        /// <summary>
        /// Constructs a new SeamstressSprites instance.
        /// </summary>
        /// <param name="canvas">The canvas associated with the game.</param>
        public SeamstressSprites(Canvas canvas) : base(canvas)
        {
            Add("seamstress_stand"); //     0
            Add("seamstress_run");   //     1
            Add("seamstress_jump");  //     2
            Add("seamstress_fall");  //     3
            Add("seamstress_land");  //     4
            Add("seamstress_idle");  //     5
            Add("seamstress_stun");  //     6
        }

        public Sprite Stand { get { return this[0]; } }
        public Sprite Run { get { return this[1]; } }
        public Sprite Jump { get { return this[2]; } }
        public Sprite Fall { get { return this[3]; } }
        public Sprite Land { get { return this[4]; } }
        public Sprite Idle { get { return this[5]; } }
        public Sprite Stun { get { return this[6]; } }
    }
}
