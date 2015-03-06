using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeLibrary.Graphics
{
    /// <summary>
    /// A collection of content assets specific to each game act.
    /// </summary>
    public class ActAssets : SpriteCollection
    {
        string theme;
        Texture2D groundFill;
        Texture2D groundEdge;
        //Texture2D miasmaFill;
        //Texture2D miasmaEdge;

        /// <summary>
        /// Constructs a new ActAssets instance.
        /// </summary>
        /// <param name="canvas">The canvas associated with the game.</param>
        public ActAssets(Canvas canvas, int actNumber) : base(canvas)
        {
            theme = "act" + actNumber;
            AddElement(canvas, "box");        //0
            AddElement(canvas, "hookbody");   //1
            AddElement(canvas, "hooktop");    //2
            AddElement(canvas, "telebase");   //3
            AddElement(canvas, "teleextend"); //4
            AddElement(canvas, "cannon");     //5
            AddElement(canvas, "bullet");     //6
            AddElement(canvas, "platformTexture");     //7
            PlatformTexture.TicksPerFrame = 0;
            /*groundFill = SetTexture(canvas, "groundFill");
            groundEdge = SetTexture(canvas, "groundEdge");
            miasmaFill = SetTexture(canvas, "miasmaFill");
            miasmaEdge = SetTexture(canvas, "miasmaEdge");*/
        }

        private void AddElement(Canvas canvas, string elementName)
        {
            if (canvas.AssetDictionary.CheckPropertyExists(theme, elementName))
                Add(canvas.AssetDictionary.LookupString(theme, elementName));
            else
                Add(canvas.AssetDictionary.LookupString("default", elementName));
        }

        private Texture2D SetTexture(Canvas canvas, string elementName)
        {
            if (canvas.AssetDictionary.CheckPropertyExists(theme, elementName))
                return canvas.Assets.GetTexture(canvas.AssetDictionary.LookupString(theme, elementName));
            else
                return canvas.Assets.GetTexture(canvas.AssetDictionary.LookupString("default", elementName));
        }

        public Sprite Box { get { return this[0]; } }
        public Sprite HookBody { get { return this[1]; } }
        public Sprite HookTop { get { return this[2]; } }
        public Sprite TelescopingBase { get { return this[3]; } }
        public Sprite TelescopingExtension { get { return this[4]; } }
        public Sprite Cannon { get { return this[5]; } }
        public Sprite Bullet { get { return this[6]; } }
        public Sprite PlatformTexture { get { return this[7]; } }

        public string Theme { get { return theme; } }

        /*public Texture2D GroundFill { get { return groundFill; } }
        public Texture2D GroundEdge { get { return groundEdge; } }
        public Texture2D MiasmaFill { get { return miasmaFill; } }
        public Texture2D MiasmaEdge { get { return miasmaEdge; } }*/
    }
}
