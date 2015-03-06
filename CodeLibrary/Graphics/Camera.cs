using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CodeLibrary.Graphics
{
    /// <summary>
    /// A class containing camera information for the graphics engine.
    /// </summary>
    public class Camera
    {
        Vector2 position;
        float rotation;
        float scale;
        float smoothness;
        int width;
        int height;
        Matrix world;
        Matrix projectionBehind;
        Matrix projectionFront;
        Matrix projectionFull;
        Matrix view;
        float tilt3D;

        //Bounds
        RectangleF edgeBounds;
        RectangleF centerBounds;
        bool applyBounds;

        //Temp variables for smoothness
        Vector2 positionActual;
        float rotationActual;
        float scaleActual;

        //For shaking
        Random rando;
        int shakeTimeRemaining;
        float shakeIntensity;

        /// <summary>
        /// Creates a new Camera object.
        /// </summary>
        /// <param name="width">The width of the camera, in pixels.</param>
        /// <param name="height">The height of the camera, in pixels.</param>
        /// <param name="position">The position of the camera, in physical units.</param>
        /// <param name="scale">The scale of the camera.</param>
        /// <param name="smoothness">The smoothness at which the camera moves, clamped to range [0,1). A smoothness of one indicates no smoothing.</param>
        public Camera(int width, int height, Vector2 position, float smoothness = GraphicsConstants.DEFAULT_CAMERA_SMOOTHNESS, float rotation = 0f, float scale = 1f)
        {
            this.width = width;
            this.height = height;
            this.smoothness = smoothness;
            if (this.smoothness < 0f)
                this.smoothness = 0f;
            else if (this.smoothness >= 1f)
                this.smoothness = 1f - GraphicsConstants.EPSILON;
            this.position = positionActual = position;
            this.rotation = rotationActual = rotation;
            this.scale = scaleActual = scale;
            UpdateTransformation();
            // set up shaking
            rando = new Random();
            shakeTimeRemaining = 0;
            shakeIntensity = 0;
            projectionBehind = Matrix.CreateOrthographic(this.width / GraphicsConstants.PIXELS_PER_UNIT, this.height / GraphicsConstants.PIXELS_PER_UNIT, -100, 0f);
            projectionFront = Matrix.CreateOrthographic(this.width / GraphicsConstants.PIXELS_PER_UNIT, this.height / GraphicsConstants.PIXELS_PER_UNIT, -0f, 100);
            projectionFull = Matrix.CreateOrthographic(this.width / GraphicsConstants.PIXELS_PER_UNIT, this.height / GraphicsConstants.PIXELS_PER_UNIT, -100, 100);
            view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, -Vector3.UnitY);
            tilt3D = (float)Math.Sin(GraphicsConstants.VIEWER_TILT);
        }

        public void SetCameraDimensions(int width, int height)
        {
            this.width = width;
            this.height = height;
            projectionBehind = Matrix.CreateOrthographic(this.width / GraphicsConstants.PIXELS_PER_UNIT, this.height / GraphicsConstants.PIXELS_PER_UNIT, -100, 0f);
            projectionFront = Matrix.CreateOrthographic(this.width / GraphicsConstants.PIXELS_PER_UNIT, this.height / GraphicsConstants.PIXELS_PER_UNIT, -0f, 100);
            projectionFull = Matrix.CreateOrthographic(this.width / GraphicsConstants.PIXELS_PER_UNIT, this.height / GraphicsConstants.PIXELS_PER_UNIT, -100, 100);
        }

        /// <summary>
        /// Enables camera bounds, so that the camera never leaves these bounds.
        /// </summary>
        /// <param name="bounds">The bounds for the camera.</param>
        public void EnableBounds(RectangleF bounds)
        {
            edgeBounds = bounds;
            centerBounds = bounds.Inflate(-Dimensions.X / GraphicsConstants.PIXELS_PER_UNIT / 2, -Dimensions.Y / GraphicsConstants.PIXELS_PER_UNIT / 2);
            applyBounds = true;
        }

        /// <summary>
        /// Sets the camera's position to its target position.
        /// </summary>
        public void JumpToTarget()
        {
            positionActual = position;
            rotationActual = rotation;
            scaleActual = scale;
        }

        /// <summary>
        /// Disables camera bounds, so that the camera moves freely.
        /// </summary>
        public void DisableBounds()
        {
            applyBounds = false;
        }

        /// <summary>
        /// Gets or sets the camera's position.
        /// Note: if smoothness > 0, the returned position is not the actual position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (applyBounds)
                {
                    position.X = Math.Min(Math.Max(position.X, centerBounds.Left), centerBounds.Right);
                    position.Y = Math.Min(Math.Max(position.Y, centerBounds.Top), centerBounds.Bottom);
                }
            }
        }
        /// <summary>
        /// Gets or sets the camera's rotation.
        /// Note: if smoothness > 0, the returned rotation is not the actual rotation of the camera.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        /// <summary>
        /// Gets or sets the camera's scale.
        /// Note: if smoothness > 0, the returned scale is not the actual scale of the camera.
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets the camera's smoothness factor.
        /// Increasing this value will cause the camera to lag slightly behind its intended position and orientation,
        /// to avoid choppy or sharp movements.
        /// </summary>
        public float Smoothness
        {
            get { return smoothness; }
            set { smoothness = value; }
        }

        /// <summary>
        /// Gets the camera's actual position, for drawing purposes.
        /// </summary>
        public Vector2 ActualPosition { get { return positionActual; } }
        /// <summary>
        /// Gets the camera's actual rotation, for drawing purposes.
        /// </summary>
        public float ActualRotation { get { return rotationActual; } }
        /// <summary>
        /// Gets the camera's actual scale, for drawing purposes.
        /// </summary>
        public float ActualScale { get { return scaleActual; } }

        private void UpdateTransformation()
        {
            world = Matrix.CreateScale(1 / scaleActual) * Matrix.CreateRotationZ(rotationActual) * Matrix.CreateTranslation(new Vector3(positionActual, 0));
            world = new Matrix(1,0,0,0,0,1,0,0,0,tilt3D,1,0,0,0,0,1) * Matrix.Invert(world);
        }

        /// <summary>
        /// Transforms a point from physical to screen coordinates.
        /// </summary>
        /// <param name="v">The point to transform.</param>
        /// <returns>The transformed point.</returns>
        public Vector2 Transform(Vector2 v)
        {
            return GraphicsConstants.PIXELS_PER_UNIT * GraphicsHelper.ApplyMatrix(world, v) + new Vector2(width / 2, height / 2);
        }

        /// <summary>
        /// Updates the camera. This should be called in Canvas.Update(), and nowhere else.
        /// </summary>
        /// <param name="gameTime">The current elapsed time of the game.</param>
        public void Update(GameTime gameTime)
        {
            positionActual = Vector2.Lerp(positionActual, position, smoothness);
            rotationActual = MathHelper.Lerp(rotationActual, rotation, smoothness);
            scaleActual = MathHelper.Lerp(scaleActual, scale, smoothness);
            if (shakeTimeRemaining > 0)
            {
                positionActual += new Vector2((2 * (float)rando.NextDouble() - 1) * shakeIntensity, (2 * (float)rando.NextDouble() - 1) * shakeIntensity);
                //rotationActual += (2 * (float)rando.NextDouble() - 1) * shakeIntensity / MathHelper.Pi;
                shakeTimeRemaining--;
            }
            UpdateTransformation();
        }

        /// <summary>
        /// Gets the width of this camera (and the gameplay window).
        /// </summary>
        public int Width { get { return width; } }
        /// <summary>
        /// Gets the height of this camera (and the gameplay window).
        /// </summary>
        public int Height { get { return height; } }
        /// <summary>
        /// Gets the visible set of pixels for this camera.
        /// </summary>
        public Rectangle VisibleScreen
        {
            get
            {
                return new Rectangle((int)(ActualPosition.X - Dimensions.X), (int)(ActualPosition.Y - Dimensions.Y), (int)Dimensions.X, (int)Dimensions.Y);
            }
        }
        /// <summary>
        /// Gets the visible set of blocks for this camera.
        /// TODO: This doesn't take rotation into account, for now.
        /// </summary>
        public Rectangle VisiblePhysical
        {
            get
            {
                int left = (int)Math.Ceiling(width / GraphicsConstants.PIXELS_PER_UNIT / 2);
                int right = left + 1;
                int top = (int)Math.Ceiling(height / GraphicsConstants.PIXELS_PER_UNIT / 2);
                int bottom = top + 1;
                return new Rectangle((int)Math.Floor(ActualPosition.X) - left, (int)Math.Floor(ActualPosition.Y) - top, right + left, bottom + top);
            }
        }
        /// <summary>
        /// Gets the bounding rectangle for this camera's movement.
        /// </summary>
        public RectangleF Bounds { get { return edgeBounds; } }
        /// <summary>
        /// Gets the dimensions of this camera (and the gameplay window).
        /// </summary>
        public Vector2 Dimensions { get { return new Vector2(width, height); } }
        /// <summary>
        /// Gets the view transformation matrix of this camera.
        /// </summary>
        public Matrix View { get { return view; } }
        /// <summary>
        /// Gets the projection transformation matrix of this camera.
        /// </summary>
        public Matrix ProjectionBehind { get { return projectionBehind; } }
        /// <summary>
        /// Gets the projection transformation matrix of this camera.
        /// </summary>
        public Matrix ProjectionFront { get { return projectionFront; } }
        /// <summary>
        /// Gets the projection transformation matrix of this camera.
        /// </summary>
        public Matrix Projection { get { return projectionFull; } }
        /// <summary>
        /// Gets the world transformation matrix of this camera.
        /// </summary>
        public Matrix World { get { return world; } }

        public Vector2 GetAnchorPosition(Anchor anchor)
        {
            return GraphicsHelper.ComputeAnchorOrigin(anchor, Dimensions);
        }

        /// <summary>
        /// Causes the camera to suddenly pan by a certain amount and then recover.
        /// </summary>
        /// <param name="amount"></param>
        public void OffsetAndRecover(Vector2 amount)
        {
            positionActual += amount;
        }

        /// <summary>
        /// Shakes the camera.
        /// </summary>
        /// <param name="frames">The number of frames to shake.</param>
        public void Shake(int frames, float intensity)
        {
            shakeTimeRemaining = frames;
            shakeIntensity = intensity / GraphicsConstants.PIXELS_PER_UNIT;
        }
    }
}
