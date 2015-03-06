using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CodeLibrary.Content;
using CodeLibrary.Engine;
using System.Windows.Forms;

namespace CodeLibrary.Graphics
{
    /// <summary>
    /// The main graphics engine for the game.
    /// </summary>
    public class Canvas
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        Camera camera;

        CoordinateMode coordinateMode;
        bool debug;
        int animFrame;

        //Temporary SpriteFont
        SpriteFont spriteFont;

        //1x1 texture used in drawing lines and boxes
        Texture2D square1x1;

        AssetManager assets;
        TextDictionary assetDictionary;

        float layerDepth;
        bool frozen;
        Vector2 offset;

        public float Time;

        //2D transformation matrix.
        Matrix transformationMatrix;

        static float LAYER_DEPTH_DEFAULT = 0f;
        static int LAST_MIASMA_ANIMATION_FRAME = 119;

        /// <summary>
        /// Create a new Canvas object in screen coordinate mode.
        /// </summary>
        /// <param name="graphics">The GraphicsDeviceManager used in the game.</param>
        /// <param name="graphicsDevice">The game's GraphicsDevice object.</param>
        public Canvas(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
        {
            Time = 0;
            //set camera
            camera = new Camera(GraphicsConstants.VIEWPORT_WIDTH, GraphicsConstants.VIEWPORT_HEIGHT, Vector2.Zero);
            //change size of window
            this.graphics = graphics;
            graphics.PreferredBackBufferWidth = this.camera.Width;
            graphics.PreferredBackBufferHeight = this.camera.Height;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            //graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();
            //set other vars
            coordinateMode = CoordinateMode.ScreenCoordinates;
            debug = false;

            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);

            // initialize our tiny square.
            square1x1 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] data = new Color[1];
            data[0] = Color.White;
            square1x1.SetData<Color>(data);

            frozen = false;
            layerDepth = LAYER_DEPTH_DEFAULT;

            transformationMatrix = Matrix.Identity;
            animFrame = 0;
        }

        public void SetViewParameters()
        {
            camera.SetCameraDimensions(GraphicsConstants.VIEWPORT_WIDTH, GraphicsConstants.VIEWPORT_HEIGHT);
            graphics.PreferredBackBufferWidth = this.camera.Width;
            graphics.PreferredBackBufferHeight = this.camera.Height;
            graphics.IsFullScreen = GraphicsConstants.FULL_SCREEN;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Loads content for the canvas.
        /// </summary>
        /// <param name="assets">The AssetManager used to load content.</param>
        public void LoadContent(AssetManager assets)
        {
            this.assets = assets;
            assetDictionary = new TextDictionary(assets.GetText("graphics"));
            spriteFont = assets.GetFont(assetDictionary.LookupString("canvas", "internalFont"));
        }

        #region Begin/end draw calls
        /// <summary>
        /// Starts a spriteBatch pass.
        /// </summary>
        public void BeginDraw()
        {
            spriteBatch.Begin(0, null, null, null, null, null, transformationMatrix);
        }

        /// <summary>
        /// Starts a spriteBatch pass.
        /// </summary>
        public void BeginDraw(SpriteSortMode spriteSortMode = 0, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null)
        {
            spriteBatch.Begin(spriteSortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformationMatrix);
        }

        /// <summary>
        /// Ends a spriteBatch pass.
        /// </summary>
        public void EndDraw() { spriteBatch.End(); }

        public void Begin3D()
        {
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            /*DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            dss.DepthBufferWriteEnable = true;
            graphicsDevice.DepthStencilState = dss;*/
        }

        public void End3D()
        {
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
        #endregion

        #region 3D drawing methods
        /// <summary>
        /// Draws a mesh defined by a list of vertices and vertex indices.
        /// </summary>
        /// <typeparam name="T">The vertex declaration type of the vertices.</typeparam>
        /// <param name="effect">The effect to be used. This effect must contain the following parameters: World, View, and Projection (float4x4), as well as Time (float).</param>
        /// <param name="mesh">The mesh to be drawn.</param>
        /// <param name="projection">Which portion of the mesh to draw. To draw the full mesh, specify Projections.Full. Otherwise, specify a different value to draw either the portion
        /// of the mesh in front of the Z=0 plane, or the portion behind it.</param>
        public void DrawUserIndexedPrimitives<T>(Effect effect, UserIndexedPrimitives<T, short> mesh, Projections projection = Projections.Full) where T : struct, IVertexType
        {
            effect.Parameters["World"].SetValue(camera.World);
            effect.Parameters["View"].SetValue(Matrix.CreateTranslation(offset.X, offset.Y, 0) * camera.View);
            if (projection == Projections.Behind)
                effect.Parameters["Projection"].SetValue(camera.ProjectionBehind);
            else if (projection == Projections.Front)
                effect.Parameters["Projection"].SetValue(camera.ProjectionFront);
            else
                effect.Parameters["Projection"].SetValue(camera.Projection);
            if (!frozen)
                mesh.Tick();
            Time = mesh.Ticks;
            foreach (EffectPass e in effect.CurrentTechnique.Passes)
                e.Apply();
            int div = 3;
            if (mesh.PrimitiveType == PrimitiveType.LineList || mesh.PrimitiveType == PrimitiveType.LineStrip)
                div = 2;
            graphicsDevice.DrawUserIndexedPrimitives<T>(mesh.PrimitiveType, mesh.Vertices, 0, mesh.Vertices.Length, mesh.Indices, 0, mesh.Indices.Length / div);
        }
        #endregion

        #region Texture drawing methods
        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Color color, Vector2 position, float rotation = 0, float scale = 1, bool flip = false)
        {
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                DrawTexture(texture, color, position, Anchor.TopLeft, rotation, scale, flip);
            else
                DrawTexture(texture, color, position, Anchor.Center, rotation, scale, flip);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="sourceRectangle">The portion of the texture to draw.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Rectangle sourceRectangle, Color color, Vector2 position, float rotation = 0, float scale = 1, bool flip = false)
        {
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                DrawTexture(texture, sourceRectangle, color, position, Anchor.TopLeft, rotation, scale, flip);
            else
                DrawTexture(texture, sourceRectangle, color, position, Anchor.Center, rotation, scale, flip);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1, bool flip = false)
        {
            if (texture == null) return;
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = new Vector2(texture.Width, texture.Height);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            SpriteEffects effect = SpriteEffects.None;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                spriteBatch.Draw(texture, position + offset, null, color, rotation, origin, scale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
            else
                spriteBatch.Draw(texture, camera.Transform(position) + offset, null, color, rotation - camera.ActualRotation, origin, scale * camera.ActualScale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Color color, Vector2 position, Anchor anchor, float rotation, Vector2 scale, bool flip = false)
        {
            if (texture == null) return;
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = new Vector2(texture.Width, texture.Height);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            SpriteEffects effect = SpriteEffects.None;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                spriteBatch.Draw(texture, position + offset, null, color, rotation, origin, scale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
            else
                spriteBatch.Draw(texture, camera.Transform(position) + offset, null, color, rotation - camera.ActualRotation, origin, scale * camera.ActualScale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="sourceRectangle">The portion of the texture to draw.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Rectangle sourceRectangle, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1, bool flip = false)
        {
            if (texture == null) return;
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = new Vector2(sourceRectangle.Width, sourceRectangle.Height);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            SpriteEffects effect = SpriteEffects.None;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                spriteBatch.Draw(texture, position + offset, sourceRectangle, color, rotation, origin, scale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
            else
                spriteBatch.Draw(texture, camera.Transform(position) + offset, sourceRectangle, color, rotation - camera.ActualRotation, origin, scale * camera.ActualScale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
        }

        /// <summary>
        /// Draws a texture to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="texture">The texture to be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        /// <param name="flip">Whether the texture is horizontally flipped.</param>
        public void DrawTexture(Texture2D texture, Rectangle sourceRectangle, Color color, Vector2 position, Anchor anchor, float rotation, Vector2 scale, bool flip = false)
        {
            if (texture == null) return;
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = new Vector2(sourceRectangle.Width, sourceRectangle.Height);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            SpriteEffects effect = SpriteEffects.None;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
            else
                spriteBatch.Draw(texture, camera.Transform(position), sourceRectangle, color, rotation - camera.ActualRotation, origin, scale * camera.ActualScale * GraphicsConstants.GRAPHICS_SCALE, effect, layerDepth);
        }
        #endregion

        #region Sprite drawing methods
        /// <summary>
        /// Draws a sprite to the screen based on the canvas's coordinate mode. Generally, this should be the method used
        /// for drawing physical objects.
        /// </summary>
        /// <param name="sprite">The sprite to draw. If the sprite is animated, only the first frame will be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        public void DrawSprite(Sprite sprite, Color color, Vector2 position, float rotation = 0, float scale = 1, bool flip = false)
        {
            DrawSprite(sprite, color, position, rotation, scale * Vector2.One, flip);
        }

        /// <summary>
        /// Draws a sprite to the screen based on the canvas's coordinate mode. This method should only be called for physical
        /// objects that for some reason aren't positioned at their centers.
        /// </summary>
        /// <param name="sprite">The sprite to draw. If the sprite is animated, only the first frame will be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        public void DrawSprite(Sprite sprite, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1, bool flip = false)
        {
            DrawSprite(sprite, color, position, anchor, rotation, scale * Vector2.One, flip);
        }

        /// <summary>
        /// Draws a sprite to the screen based on the canvas's coordinate mode. Generally, this should be the method used
        /// for drawing physical objects.
        /// </summary>
        /// <param name="sprite">The sprite to draw. If the sprite is animated, only the first frame will be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        public void DrawSprite(Sprite sprite, Color color, Vector2 position, float rotation, Vector2 scale, bool flip = false)
        {
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                DrawSprite(sprite, color, position, Anchor.TopLeft, rotation, scale, flip);
            else
                DrawSprite(sprite, color, position, Anchor.Center, rotation, scale, flip);
        }

        /// <summary>
        /// Draws a sprite to the screen based on the canvas's coordinate mode. This method should only be called for physical
        /// objects that for some reason aren't positioned at their centers.
        /// </summary>
        /// <param name="sprite">The sprite to draw. If the sprite is animated, only the first frame will be drawn.</param>
        /// <param name="color">The color of the texture. To draw the texture unmodified, input Color.White.</param>
        /// <param name="position">The position at which the texture should be drawn.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        /// <param name="rotation">How much the texture is rotated.</param>
        /// <param name="scale">The scale at which the texture is drawn.</param>
        public void DrawSprite(Sprite sprite, Color color, Vector2 position, Anchor anchor, float rotation, Vector2 scale, bool flip = false)
        {
            if (!frozen)
                sprite.Tick();
            DrawTexture(sprite.Texture, sprite.GetFrame(sprite.CurrentFrame), color, position, anchor,
                                                                                    rotation, GraphicsConstants.SPRITE_SCALE * scale, flip);
        }
        #endregion

        /// <summary>
        /// Draws the background animation for miasma
        /// </summary>
        /// <param name="ground">The ground map to draw.</param>
        public void DrawMiasmaAnimation(GroundMap ground, LevelGridStore[,] gridModel)
        {
            EndDraw();
            BeginDraw(0, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            List<Rectangle> rectangles = ground.Rectangles;

            for (int i = 0; i < rectangles.Count; i++)
            {
                string textureKey = GraphicsHelper.GetRectEdge(rectangles[i], gridModel);
                DrawTexture(assets.GetMiasmaAnimation(textureKey).ElementAt(animFrame),
                    Color.White, new Vector2(rectangles[i].X, rectangles[i].Y), Anchor.TopLeft, 0, 1f);
            }

            if (animFrame == LAST_MIASMA_ANIMATION_FRAME)
                animFrame = 0;
            else
                animFrame++;

            EndDraw();
            BeginDraw();
        }

        public void DrawRibbonPath(List<Vector2> robustPath, Color ribbonColor, bool pathInactive)
        {
            BeginDraw();
            for (int i = 0; i < robustPath.Count; i++)
            {
                string directions;
                if (i == robustPath.Count - 1)
                    directions = FindPathDirection(robustPath[i - 1], robustPath[i]);
                else
                    directions = FindPathDirection(robustPath[i], robustPath[i + 1]);
                Vector2 position = new Vector2(-10000, 10000);
                float rotation = 0;

                switch (directions)
                {
                    case "none":
                        break;
                    case "up":
                        position = new Vector2(robustPath[i].X, robustPath[i].Y - 0.5f);
                        rotation = 90;
                        break;
                    case "down":
                        position = new Vector2(robustPath[i].X, robustPath[i].Y + 0.5f);
                        rotation = 90;
                        break;
                    case "left":
                        position = new Vector2(robustPath[i].X + 0.5f, robustPath[i].Y);
                        rotation = 0;
                        break;
                    case "right":
                        position = new Vector2(robustPath[i].X - 0.5f, robustPath[i].Y);
                        break;
                    default:
                        break;
                }

                rotation = (float)Math.PI / 180 * rotation;
                Color finalColor = ribbonColor;

                if (pathInactive)
                {
                    System.Drawing.Color storeColor = System.Drawing.Color.FromArgb(ribbonColor.A, ribbonColor.R, ribbonColor.G, ribbonColor.B);
                    storeColor = ControlPaint.Dark(storeColor);
                    finalColor.R = storeColor.R;
                    finalColor.G = storeColor.G;
                    finalColor.B = storeColor.B;
                    finalColor.A = storeColor.A;
                }

                if (position != new Vector2(-10000, 10000))
                {
                    string texNum = GraphicsHelper.GetAnimationLabelNumber(animFrame);
                    DrawSprite(Sprite.BuildAnimation(this, "ribbonPathAnim_" + texNum), finalColor, position, rotation);
                }
            }  
            EndDraw();
        }

        private string FindPathDirection(Vector2 curr, Vector2 next)
        {
            string output = "none";
            if (curr.X == next.X)
            {
                if (curr.Y < next.Y)
                    output = "down";
                if (curr.Y > next.Y)
                    output = "up";
            }
            if (curr.Y == next.Y)
            {
                if (curr.X < next.X)
                    output = "left";
                if (curr.X > next.X)
                    output = "right";
            }
            return output;
        }

        /// <summary>
        /// Draws a ground map.
        /// </summary>
        /// <param name="ground">The ground map to draw.</param>
        /// <param name="color">The color to tint the fill texture.</param>
        public void DrawGroundMap2DPortion(GroundMap ground)
        {
            EndDraw();
            if (!Frozen)
                ground.Time += 1;
            BeginDraw(0, null, SamplerState.LinearWrap, null, null);
            List<Rectangle> rectangles = ground.Rectangles;
            float scale = GraphicsConstants.PIXELS_PER_UNIT / GraphicsConstants.GRAPHICS_SCALE;


            for (int i = 0; i < rectangles.Count; i++)
                for (int j = 0; j < ground.FillTextures.Count; j++)
                {
                    Rectangle r = GraphicsHelper.Scale(rectangles[i], scale);
                    r.Offset((int)(ground.FillMotions[j].X * ground.Time), (int)(ground.FillMotions[j].Y * ground.Time));
                    DrawTexture(ground.FillTextures[j], r,
                        ground.FillColors[j], new Vector2(rectangles[i].X, rectangles[i].Y), Anchor.TopLeft);          
                }

            EndDraw();
            BeginDraw();
        }

        /// <summary>
        /// Draws a ground map.
        /// </summary>
        /// <param name="ground">The ground map to draw.</param>
        /// <param name="color">The color to tint the fill texture.</param>
        public void DrawGroundMap3DPortion(GroundMap ground, Projections pass)
        {
            BeginDraw(0, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            foreach (Path3D p in ground.Paths)
            {
                ground.Effect.Parameters["Texture"].SetValue(ground.EdgeTexture);
                ground.Effect.Parameters["TotalLength"].SetValue(p.TotalLength);
                this.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(ground.Effect, p, pass);
            }
            /*foreach (Path3D p in ground.Paths)
            {
                ground.Effect.Parameters["TotalLength"].SetValue(p.TotalLength);
                this.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(ground.Effect, p, Projections.Front);
            }*/
            //BeginDraw();
            EndDraw();
        }

        #region Text drawing methods
        /// <summary>
        /// Draws a string of text to the screen based on the canvas's coordinate mode, using the canvas's internal font.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position at which the text should be written.</param>
        /// <param name="rotation">How much the text is rotated.</param>
        /// <param name="scale">The scale at which the text is written.</param>
        public void DrawString(object text, Color color, Vector2 position, float rotation = 0, float scale = 1)
        {
            DrawString(text, spriteFont, color, position, rotation, scale);
        }

        /// <summary>
        /// Draws a string of text to the screen based on the canvas's coordinate mode, using the canvas's internal font.
        /// Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position at which the text should be written.</param>
        /// <param name="rotation">How much the text is rotated.</param>
        /// <param name="scale">The scale at which the text is written.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        public void DrawString(object text, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1)
        {
            DrawString(text, spriteFont, color, position, anchor, rotation, scale);
        }

        /// <summary>
        /// Draws a string of text to the screen based on the canvas's coordinate mode.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position at which the text should be written.</param>
        /// <param name="rotation">How much the text is rotated.</param>
        /// <param name="scale">The scale at which the text is written.</param>
        public void DrawString(object text, SpriteFont font, Color color, Vector2 position, float rotation = 0, float scale = 1)
        {
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                DrawString(text, font, color, position, Anchor.TopLeft, rotation, scale);
            else
                DrawString(text, font, color, position, Anchor.Center, rotation, scale);
        }

        /// <summary>
        /// Draws a string of text to the screen based on the canvas's coordinate mode. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position at which the text should be written.</param>
        /// <param name="rotation">How much the text is rotated.</param>
        /// <param name="scale">The scale at which the text is written.</param>
        /// <param name="anchor">The point on the texture where the origin is calculated.</param>
        public void DrawString(object text, SpriteFont font, Color color, Vector2 position, Anchor anchor, float rotation = 0, float scale = 1)
        {
            String objectText = text.ToString();
            Vector2 origin = Vector2.Zero;
            Vector2 dimensions = font.MeasureString(objectText);
            if (anchor != Anchor.TopLeft)
                origin = GraphicsHelper.ComputeAnchorOrigin(anchor, dimensions);
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                spriteBatch.DrawString(font, objectText, position + offset, color, rotation, origin, scale, SpriteEffects.None, 0);
            else
                spriteBatch.DrawString(font, objectText, camera.Transform(position) + offset, color, rotation - camera.ActualRotation, origin, scale * camera.ActualScale, SpriteEffects.None, 0);
        }
        #endregion

        #region Debug drawing methods
        /// <summary>
        /// Draws a line on the screen.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="thickness">The thickness of the line, in pixels.</param>
        /// <param name="p1">The starting point of the line, in the canvas's current coordinate mode.</param>
        /// <param name="p2">The ending point of the line, in the canvas's current coordinate mode.</param>
        /// <param name="debug">Whether the line should only be drawn in debug mode.</param>
        public void DrawLine(Color color, float thickness, Vector2 p1, Vector2 p2, bool debug = true)
        {
            if (!DebugMode && debug) return;
            Vector2 diff = p2 - p1;
            float angle = (float)Math.Atan2(diff.Y, diff.X);
            float length = diff.Length();
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                spriteBatch.Draw(square1x1, p1 + offset, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
            else
            {
                length *= GraphicsConstants.PIXELS_PER_UNIT * camera.ActualScale;
                spriteBatch.Draw(square1x1, camera.Transform(p1) + offset, null, color, angle - camera.ActualRotation, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
            }
        }


        /// <summary>
        /// Draws the outline of a polygon on the screen.
        /// </summary>
        /// <param name="color">The color of the outline.</param>
        /// <param name="thickness">The thickness of the outline, in pixels.</param>
        /// <param name="points">The points that make up this polygon.</param>
        /// <param name="debug">Whether the polygon should only be drawn in debug mode.</param>
        public void DrawPolygon(Color color, float thickness, List<Vector2> points, bool debug = true)
        {
            for (int i = 0; i < points.Count; i++)
                DrawLine(color, thickness, points[i], points[(i + 1) % points.Count], debug);
        }

        /// <summary>
        /// Draws an axis-aligned rectangle on the screen.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="rect">The rectangle's dimensions.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, int borderThickness, Rectangle rect, bool debug = true)
        {
            DrawRectangle(border, Color.Transparent, borderThickness, new Vector2(rect.X, rect.Y), Anchor.TopLeft, new Vector2(rect.Width, rect.Height), 0, debug);
        }

        /// <summary>
        /// Draws an axis-aligned rectangle on the screen.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="fill">The color of the rectangle's fill. Use Color.Transparent to prevent the rectangle from being filled.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="rect">The rectangle's dimensions.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, Color fill, int borderThickness, Rectangle rect, bool debug = true)
        {
            DrawRectangle(border, fill, borderThickness, new Vector2(rect.X, rect.Y), Anchor.TopLeft, new Vector2(rect.Width, rect.Height), 0, debug);
        }

        /// <summary>
        /// Draws a rectangle with no fill on the screen.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="position">The position of the rectangle.</param>
        /// <param name="size">The size of the rectangle, in the canvas's current coordinate mode.</param>
        /// <param name="rotation">The rotation the rectangle.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, int borderThickness, Vector2 position, Vector2 size, float rotation = 0, bool debug = true)
        {
            DrawRectangle(border, Color.Transparent, borderThickness, position, size, rotation, debug);
        }

        /// <summary>
        /// Draws a rectangle on the screen.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="fill">The color of the rectangle's fill. Use Color.Transparent to prevent the rectangle from being filled.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="position">The position of the rectangle.</param>
        /// <param name="size">The size of the rectangle, in the canvas's current coordinate mode.</param>
        /// <param name="rotation">The rotation the rectangle.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, Color fill, int borderThickness, Vector2 position, Vector2 size, float rotation = 0, bool debug = true)
        {
            if (coordinateMode == CoordinateMode.ScreenCoordinates)
                DrawRectangle(border, fill, borderThickness, position, Anchor.TopLeft, size, rotation, debug);
            else
                DrawRectangle(border, fill, borderThickness, position, Anchor.Center, size, rotation, debug);
        }

        /// <summary>
        /// Draws a rectangle on the screen. Origin info from coordinate mode will be ignored.
        /// Generally, this should only be called for non-physical components such as the HUD or background.
        /// </summary>
        /// <param name="border">The color of the rectangle's border.</param>
        /// <param name="fill">The color of the rectangle's fill. Use Color.Transparent to prevent the rectangle from being filled.</param>
        /// <param name="borderThickness">The thickness of the rectangle's border.</param>
        /// <param name="position">The position of the rectangle.</param>
        /// <param name="size">The size of the rectangle, in the canvas's current coordinate mode.</param>
        /// <param name="rotation">The rotation the rectangle.</param>
        /// <param name="debug">Whether the rectangle should only be drawn in debug mode.</param>
        public void DrawRectangle(Color border, Color fill, int borderThickness, Vector2 position, Anchor anchor, Vector2 size, float rotation = 0, bool debug = true)
        {
            if (!DebugMode && debug) return;
            Vector2 dimensions = GraphicsHelper.ComputeAnchorOrigin(anchor, size);
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);
            Vector2 p1 = -dimensions;
            p1 = new Vector2(position.X + cos * p1.X - sin * p1.Y, position.Y + sin * p1.X + cos * p1.Y);
            Vector2 p2 = new Vector2(size.X, 0) - dimensions;
            p2 = new Vector2(position.X + cos * p2.X - sin * p2.Y, position.Y + sin * p2.X + cos * p2.Y);
            Vector2 p3 = size - dimensions;
            p3 = new Vector2(position.X + cos * p3.X - sin * p3.Y, position.Y + sin * p3.X + cos * p3.Y);
            Vector2 p4 = new Vector2(0, size.Y) - dimensions;
            p4 = new Vector2(position.X + cos * p4.X - sin * p4.Y, position.Y + sin * p4.X + cos * p4.Y);
            if (fill.A != 0)
            {
                if (coordinateMode == CoordinateMode.ScreenCoordinates)
                    spriteBatch.Draw(square1x1, p1 + offset, null, fill, rotation, Vector2.Zero, size, SpriteEffects.None, 0);
                else
                {
                    Vector2 recScale = size * camera.ActualScale * GraphicsConstants.PIXELS_PER_UNIT;
                    spriteBatch.Draw(square1x1, camera.Transform(p1) + offset, null, fill, rotation - camera.ActualRotation, Vector2.Zero, recScale, SpriteEffects.None, 0);
                }
            }

            DrawLine(border, borderThickness, p1, p2, debug);
            DrawLine(border, borderThickness, p2, p3, debug);
            DrawLine(border, borderThickness, p3, p4, debug);
            DrawLine(border, borderThickness, p4, p1, debug);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the global transformation to apply to the canvas.
        /// </summary>
        public Matrix TransformationMatrix
        {
            get
            {
                return transformationMatrix;
            }
            set
            {
                transformationMatrix = value;
            }
        }
        
        /// <summary>
        /// Whether the canvas should stop all animations from advancing.
        /// </summary>
        public bool Frozen { get { return frozen; } set { frozen = value; } }

        /// <summary>
        /// The translational offset with which everything is drawn.
        /// </summary>
        public Vector2 Offset { get { return offset; } set { offset = value; } }

        /// <summary>
        /// Gets or sets whether the canvas draws debug mode objects.
        /// </summary>
        public bool DebugMode { get { return debug; } set { debug = value; } }

        /// <summary>
        /// Gets or sets the coordinate mode of the canvas.
        /// </summary>
        public CoordinateMode CoordinateMode { get { return coordinateMode; } set { coordinateMode = value; } }

        /// <summary>
        /// Gets or sets the active camera.
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
            set
            {
                if (value == null)
                    throw new ArgumentException("Camera cannot be null.");
                camera = value;
            }
        }

        public AssetManager Assets { get { return assets; } }

        public TextDictionary AssetDictionary { get { return assetDictionary; } }
        #endregion
    }
}