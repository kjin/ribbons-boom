using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Engine;

namespace CodeLibrary.Graphics
{
    #region unused
    /*public interface Path2D
    {
        Vector2 GetPosition(float t);

        Vector2 GetNormal(float t);
    }

    public class ThreePointPath : Path2D
    {
        Vector2 start, middle, end;
        float midPercent;

        public ThreePointPath(Vector2 start, Vector2 middle, Vector2 end)
        {
            this.start = start;
            this.middle = middle;
            this.end = end;
            float stm = (middle - start).Length();
            float mte = (end - middle).Length();
            midPercent = stm / (stm + mte);
        }

        public Vector2 GetPosition(float t)
        {
            if (t < midPercent)
                return Vector2.Lerp(start, middle, t / midPercent);
            else
                return Vector2.Lerp(middle, end, (t - midPercent) / (1 - midPercent));
        }

        public Vector2 GetNormal(float t)
        {
            Vector2 ret;
            if (t < midPercent)
                ret = middle - start;
            else
                ret = end - middle;
            ret.Normalize();
            return new Vector2(ret.Y, -ret.X);
        }
    }

    public class RibbonCrossSection : Path2D
    {
        Vector2 dimensions;
        float curved;
        float straight;
        float all;

        public float CurveSectionLength { get { return curved; } }
        public float StraightSectionLength { get { return straight; } }
        public float TotalLength { get { return all; } }

        public RibbonCrossSection(Vector2 dimensions)
        {
            this.dimensions = dimensions;
            curved = dimensions.Y * MathHelper.Pi;
            straight = 2 * dimensions.X;
            all = curved + curved + straight + straight;
        }

        public Vector2 GetPosition(float t)
        {
            t *= all;
            if (t <= curved)
            {
                float angle = MathHelper.Lerp(-MathHelper.PiOver2, MathHelper.PiOver2, t / curved);
                return new Vector2(dimensions.X + dimensions.Y * (float)Math.Cos(angle), dimensions.Y * (float)Math.Sin(angle));
            }
            else if (t <= curved + straight)
                return new Vector2(MathHelper.Lerp(straight / 2, -straight / 2, (t - curved) / straight), dimensions.Y);
            else if (t <= 2 * curved + straight)
            {
                float angle = MathHelper.Lerp(MathHelper.PiOver2, 3 * MathHelper.PiOver2, (t - curved - straight) / curved);
                return new Vector2(dimensions.Y * (float)Math.Cos(angle) - dimensions.X, dimensions.Y * (float)Math.Sin(angle));
            }
            else
                return new Vector2(MathHelper.Lerp(-straight / 2, straight / 2, (t - 2 * curved - straight) / straight), -dimensions.Y);
        }

        public Vector2 GetNormal(float t)
        {
            t *= all;
            if (t <= curved)
            {
                float angle = MathHelper.Lerp(-MathHelper.PiOver2, MathHelper.PiOver2, t / curved);
                return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }
            else if (t <= curved + straight)
                return Vector2.UnitY;
            else if (t <= 2 * curved + straight)
            {
                float angle = MathHelper.Lerp(MathHelper.PiOver2, 3 * MathHelper.PiOver2, (t - curved - straight) / curved);
                return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }
            else
                return -Vector2.UnitY;
        }
    }*/
    #endregion

    /// <summary>
    /// A class that wraps the Path3D class, shrouding parameters common to all ribbons.
    /// </summary>
    public class Ribbon3D : Path3D
    {
        /// <summary>
        /// Constructs a new 3D ribbon mesh.
        /// </summary>
        /// <param name="corners">The corners that define the ribbon's path.</param>
        public Ribbon3D(List<Vector2> corners, bool loops)
            : base(corners, 0, 0.08f, 5, new RectangleCrossSection(new Vector2(0f, 0.1f)), false    )
        {
        }
    }
}
