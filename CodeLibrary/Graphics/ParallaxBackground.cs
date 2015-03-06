using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeLibrary.Graphics
{
    //Represents a single scrolling background object.
    public class ParallaxBackground
    {
        Texture2D texture;
        Vector2 initialOffset;
        Vector2 velocity;
        Anchor anchor;
        float dist;
        float scale;
        bool repeatX;
        bool repeatY;

        public ParallaxBackground(Canvas canvas, Texture2D texture, Vector2 initialOffset, Vector2 velocity, float dist, float scale, Anchor anchor, Vector2 repeat)
        {
            this.texture = texture;
            this.dist = dist;
            this.initialOffset = initialOffset - (GraphicsHelper.ComputeAnchorOrigin(anchor, new Vector2(texture.Width, texture.Height)) - GraphicsHelper.ComputeAnchorOrigin(anchor, GraphicsConstants.DEFAULT_DIMENSIONS));
            this.anchor = anchor;
            this.velocity = velocity;
            this.scale = scale;
            this.repeatX = repeat.X != 0;
            this.repeatY = repeat.Y != 0;
        }

        /// <summary>
        /// Gets the texture associated with the object.
        /// </summary>
        public Texture2D Texture { get { return texture; } }
        /// <summary>
        /// Gets the distance from the camera, which is factored into how quickly the background scrolls.
        /// </summary>
        public float Distance { get { return dist; } }
        /// <summary>
        /// Gets the initial offset, which is where the background should be drawn when the camera is at the origin.
        /// </summary>
        public Vector2 InitialOffset { get { return initialOffset; } }
        /// <summary>
        /// Gets the velocity, which is how quickly the background should move.
        /// </summary>
        public Vector2 Velocity { get { return velocity; } }
        /// <summary>
        /// Gets the anchor position of the backgrounds.
        /// </summary>
        public Anchor Anchor { get { return anchor; } }
        /// <summary>
        /// Gets the scale factor to apply to the background when drawing.
        /// </summary>
        public float Scale { get { return scale; } }
        /// <summary>
        /// Gets whether the background repeats in the X direction.
        /// </summary>
        public bool RepeatX { get { return repeatX; } }
        /// <summary>
        /// Gets whether the background repeats in the Y direction.
        /// </summary>
        public bool RepeatY { get { return repeatY; } }
    }

    public class ParallaxBackgroundSet
    {
        ParallaxBackground[] backgrounds;
        BackgroundParticleSet particles;
        Camera camera;
        Color color;
        int particleLayer;
        Vector2 seamstressStart;

        public static ParallaxBackgroundSet Build(Canvas canvas, Vector2 seamstressStart, string assetName)
        {
            int layers = canvas.AssetDictionary.LookupInt32(assetName, "layers");
            ParallaxBackground[] backgrounds = new ParallaxBackground[layers];
            BackgroundParticleSet particles = new BackgroundParticleSet(canvas.Assets, canvas.Camera.Dimensions, Convert.ToInt32(assetName.Substring(3)));
            int particleLayer = -1;
            for (int i = 0; i < layers; i++)
            {
                Texture2D tex;
                if (canvas.AssetDictionary.CheckPropertyExists(assetName, "name" + i))
                    tex = canvas.Assets.GetTexture(canvas.AssetDictionary.LookupString(assetName, "name" + i));
                else
                    tex = canvas.Assets.GetTexture(assetName + "_layer" + i);
                float dist, scale;
                Vector2 offset, velocity, repeat;
                Anchor anchor;
                try { dist = canvas.AssetDictionary.LookupSingle(assetName, "dist" + i); }
                catch { dist = 1; }
                try { offset = canvas.AssetDictionary.LookupVector2(assetName, "offset" + i); }
                catch { offset = Vector2.Zero; }
                try { velocity = canvas.AssetDictionary.LookupVector2(assetName, "velocity" + i); }
                catch { velocity = Vector2.Zero; }
                try { scale = canvas.AssetDictionary.LookupSingle(assetName, "scale" + i); }
                catch { scale = 1; }
                try { repeat = canvas.AssetDictionary.LookupVector2(assetName, "repeat" + i); }
                catch { repeat = new Vector2(1, 0); }
                if (!canvas.AssetDictionary.CheckPropertyExists(assetName, "anchor" + i) ||
                    !Enum.TryParse<Anchor>(canvas.AssetDictionary.LookupString(assetName, "anchor" + i), out anchor))
                    anchor = Anchor.BottomLeft;
                backgrounds[i] = new ParallaxBackground(canvas, tex, offset, velocity, dist, scale, anchor, repeat);
                if (dist > particles.Distance)
                    particleLayer = i;
            }
            ParallaxBackgroundSet pbs = new ParallaxBackgroundSet();
            pbs.backgrounds = backgrounds;
            pbs.camera = canvas.Camera;
            pbs.color = canvas.AssetDictionary.LookupColor(assetName, "color");
            pbs.particleLayer = particleLayer;
            pbs.particles = particles;
            pbs.seamstressStart = seamstressStart;

            return pbs;
        }

        public void Update(GameTime gameTime)
        {
            particles.Update(camera.ActualPosition);
        }

        public void Draw(GameTime gameTime, Canvas canvas)
        {
            canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            if (particleLayer == -1)
                particles.Draw(canvas);
            Vector2 offset = canvas.Camera.ActualPosition - seamstressStart;
            offset.Y = 0;
            for (int i = 0; i < backgrounds.Length; i++)
            {
                Vector2 position = backgrounds[i].InitialOffset - offset * GraphicsConstants.PIXELS_PER_UNIT / backgrounds[i].Distance + backgrounds[i].Velocity * (float)gameTime.TotalGameTime.TotalSeconds;
                if (backgrounds[i].RepeatX)
                {
                    while (position.X < -backgrounds[i].Texture.Width)
                        position.X += backgrounds[i].Texture.Width;
                    while (position.X > 0)
                        position.X -= backgrounds[i].Texture.Width;
                }
                if (backgrounds[i].RepeatY)
                {
                    while (position.Y < -backgrounds[i].Texture.Height)
                        position.Y += backgrounds[i].Texture.Height;
                    while (backgrounds[i].RepeatY && position.Y > 0)
                        position.Y -= backgrounds[i].Texture.Height;
                }
                while (position.X <= canvas.Camera.Dimensions.X)
                {
                    float cachedY = position.Y;
                    while (position.Y <= canvas.Camera.Dimensions.Y)
                    {
                        canvas.DrawTexture(backgrounds[i].Texture, Color.White, position * GraphicsConstants.GRAPHICS_SCALE, Anchor.TopLeft);
                        if (backgrounds[i].RepeatY)
                            position.Y += backgrounds[i].Texture.Height;
                        else break;
                    }
                    position.Y = cachedY;
                    if (backgrounds[i].RepeatX)
                        position.X += backgrounds[i].Texture.Width;
                    else break;
                }
                if (particleLayer == i)
                    particles.Draw(canvas);
            }
            canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
        }

        public Color Color { get { return color; } }
    }

}
