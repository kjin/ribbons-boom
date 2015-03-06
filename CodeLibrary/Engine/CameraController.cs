using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;
using CodeLibrary.Input;
using CodeLibrary.Audio;

namespace CodeLibrary.Engine
{
    /// <summary>
    /// Provides control over the camera, which decides which portion of the level is drawn.
    /// </summary>
    public class CameraController
    {
        Camera camera;
        InputController input;
        bool zoomedOut;
        bool followPlayer;

        List<CameraHotspot> hotspots;
        CameraHotspot activeCameraHotspot;

        SoundObjectCollection cameraNoises;

        /// <summary>
        /// Create a new camera controller.
        /// </summary>
        /// <param name="camera">The camera to control.</param>
        /// <param name="hotspots">A list of camera hotspots.</param>
        /// <param name="input">The input controller associated with the game.</param>
        /// <param name="audioPlayer">The audio player associated with the game.</param>
        public CameraController(Camera camera, List<CameraHotspot> hotspots, InputController input, AudioPlayer audioPlayer)
        {
            this.camera = camera;
            this.hotspots = hotspots;
            this.input = input;
            cameraNoises = new SoundObjectCollection(audioPlayer);
            cameraNoises.Add("camera_click");
        }

        /// <summary>
        /// Update the camera controller. The camera follows the seamstress unless it has been both zoomed out
        /// and moved manually.
        /// </summary>
        /// <param name="seamstressLocation">The seamstress's current location.</param>
        public void Update(Vector2 seamstressLocation)
        {
            if (activeCameraHotspot == null || !activeCameraHotspot.Contains(seamstressLocation))
            {
                activeCameraHotspot = null;
                foreach (CameraHotspot hotspot in hotspots)
                    if (hotspot.Contains(seamstressLocation + new Vector2(0, -2), 2))
                        activeCameraHotspot = hotspot;
            }
            if (activeCameraHotspot != null)
                activeCameraHotspot.Align(camera, seamstressLocation + new Vector2(0, -2));
            else
            {
                camera.Scale = 1;
                camera.Position = seamstressLocation + new Vector2(0, -2);
                if (input.Zoom.JustPressed)
                    zoomedOut = !zoomedOut;
                Vector2 cameraMovement = Vector2.Zero;
                if (zoomedOut)
                {
                    camera.Scale = GraphicsConstants.CAMERA_ALT_ZOOM;
                    if (input.CameraLeft.Pressed)
                        cameraMovement.X -= GraphicsConstants.CAMERA_SPEED * input.CameraLeft.Value;
                    if (input.CameraRight.Pressed)
                        cameraMovement.X += GraphicsConstants.CAMERA_SPEED * input.CameraRight.Value;
                    if (input.CameraUp.Pressed)
                        cameraMovement.Y -= GraphicsConstants.CAMERA_SPEED * input.CameraUp.Value;
                    if (input.CameraDown.Pressed)
                        cameraMovement.Y += GraphicsConstants.CAMERA_SPEED * input.CameraDown.Value;
                    camera.Position += cameraMovement;
                    if (cameraMovement.LengthSquared() != 0)
                        followPlayer = false;
                }
                else
                {
                    followPlayer = true;
                    if (camera.Scale == GraphicsConstants.CAMERA_ALT_ZOOM)
                        camera.Scale = 1;
                }
                if (followPlayer)
                    camera.Position = seamstressLocation;
            }
        }

        public void PlayAudio(AudioPlayer audioPlayer)
        {
            audioPlayer.PlayOnSetTrue(cameraNoises[0], input.Zoom.Pressed);
        }
    }
}
