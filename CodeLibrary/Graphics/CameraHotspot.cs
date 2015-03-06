using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CodeLibrary.Graphics
{
    /// <summary>
    /// A camera "hotspot", which causes the camera to enter a certain
    /// zoom mode when the seamstress enters the hotspot area.
    /// In the case where two hotspots overlap, only the latter one to be initialized
    /// is activated.
    /// </summary>
    public class CameraHotspot
    {
        Rectangle area;
        float scale;
        bool followPlayer;
        Vector2 center;

        /// <summary>
        /// Constructs a new camera hotspot.
        /// </summary>
        /// <param name="area">The area in which the seamstress must be to activate the hotspot.</param>
        /// <param name="followPlayer">Whether the camera follows the player.</param>
        /// <param name="scale">The scale of the camera when the hotspot is active. This scale is unused if the camera doesn't follow the player.</param>
        public CameraHotspot(Rectangle area, bool followPlayer, float scale)
        {
            this.area = area;
            if (area.Left > area.Right)
                this.area = new Rectangle(area.Right, area.Top, -area.Width, area.Height);
            if (area.Top > area.Bottom)
                this.area = new Rectangle(area.Left, area.Bottom, area.Width, -area.Height);
            this.followPlayer = followPlayer;
            this.scale = scale;
            center = new Vector2(area.X + area.Width / 2f, area.Y + area.Height / 2f);
        }

        public void Align(Camera camera, Vector2 position)
        {
            if (followPlayer)
            {
                camera.Scale = scale;
                camera.Position = position;
            }
            else
            {
                camera.Scale = 1 / (Math.Min(area.Width / camera.Dimensions.X, area.Height / camera.Dimensions.Y) * GraphicsConstants.PIXELS_PER_UNIT);
                camera.Position = center;
            }
        }

        public bool Contains(Vector2 position, float buffer = 0)
        {
            return position.X > area.Left + buffer && position.Y > area.Top + buffer && position.X < area.Right - buffer && position.Y < area.Bottom - buffer;
        }
    }
}
