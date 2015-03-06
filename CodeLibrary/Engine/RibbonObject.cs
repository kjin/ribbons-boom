using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

using Microsoft.Xna.Framework.Graphics;

using CodeLibrary.Audio;
using CodeLibrary.Graphics;
using System.Windows.Forms;

namespace CodeLibrary.Engine
{
    public class RibbonObject : GameObject, IAudible, I3D
    {
        public float RIBBON_SPEED = 0.008f;
        public float RIBBON_DYNAMIC_DRAG = 0.1f;
        public float RIBBON_STATIC_DRAG = 0.5f;

        public float FLIP_DISTANCE = 3.0f;

        World world;

        List<Vector2> path;
        List<Vector2> robustPath;
        float[] intervals;
        List<Vector2> orientations;

        public List<RibbonElement> elements;

        public List<RibbonFeature> features;

        List<Vector2> points;

        public float ribbonStart;
        public float ribbonEnd;
        float prevRibbonStart;
        float prevRibbonEnd;
        float ribbonLength;
        public bool loop;

        float shrinking;
        float contactTime;

        public Body body;
        public Fixture fixture;
        public Queue<Fixture> fixtures;

        public SeamstressObject seamstress;
        float seamstressOnRibbon;
        float seamstressRibbonPosition;
        public float SeamstressRibbonPosition { set { seamstressRibbonPosition = value; } }
        Vector2 seamstressOrientation;

        //for ribbon movement
        public float ribbonSpeed = 0.0f;
        float rightImpulse = 0.0f;
        float leftImpulse = 0.0f;
        bool resolvingConflict = false;
        float holdMovement = 0.0f;

        int ribbonColor;
        Ribbon3D ribbon3D;
        Effect ribbonEffect;
        bool seamstressRecentContact;
        public bool SeamstressRecentContact { set { seamstressRecentContact = value; } }

        RibbonGemObject ribbonGem;

        public float spoolSpin;
        public string selectedSpool;

        int ribbonContacts;
        public string ribbonTextureLabel;

        float time;
        bool pulseReset;
        bool drawSpools;

        public RibbonObject(Canvas canvas, World world, SeamstressObject s, int color, List<Vector2> path, float ribbonStart, float ribbonEnd, bool loop = true)
        {
            drawSpools = true;
            this.world = world;

            ribbonTextureLabel = "ribbon_texture_4";
            contactTime = 0;
            time = 0;
            ribbonGem = null;

            selectedSpool = "none";
            this.ribbonColor = color;
            this.path = path;

            this.loop = loop;

            pulseReset = true;

            seamstress = s;
            spoolSpin = 0;

            intervals = new float[path.Count];
            orientations = new List<Vector2>();
            ribbonLength = 0;
            for (int i = 0; i < path.Count; i++)
            {
                Vector2 v = path[(i + 1) % path.Count] - path[i];
                intervals[i] = v.Length();
                if ((i + 1 != path.Count) || (loop))
                {
                    ribbonLength += intervals[i];
                }
                else
                {
                    Console.WriteLine("charmander!");
                }

                v.Normalize();
                orientations.Add(v);
            }

            this.ribbonStart = ribbonStart;
            this.ribbonEnd = ribbonEnd;
            prevRibbonEnd = ribbonEnd;
            prevRibbonStart = ribbonStart;

            body = BodyFactory.CreateBody(world);
            body.BodyType = BodyType.Static;
            body.Position = new Vector2(0, 0);
            body.UserData = this;

            Shape chain = GenerateShape();
            fixtures = new Queue<Fixture>();
            fixtures.Enqueue(body.CreateFixture(chain));

            elements = new List<RibbonElement>();
            features = new List<RibbonFeature>();
            shrinking = 0.0f;

            ribbonContacts = 0;

            //Ribbon drawing.

            //ribbonTiles = new RibbonTiles(Tileset.Build(canvas, "ribbon"), path);
            //path.Add(path[0]);
                        CreateRobustPath();
                        foreach (Vector2 v in robustPath)
                            Console.WriteLine(v);
            ribbon3D = new Ribbon3D(CreateDrawPath(), loop);
            ribbonEffect = canvas.Assets.GetEffect("RibbonEffect");
            //ribbonEffect.Parameters["ColorDivisions"].SetValue(4);
            Texture2D ribbonEdge = canvas.Assets.GetTexture("ribbon_texture_2");
            ribbonEffect.Parameters["Texture"].SetValue(ribbonEdge);
            ribbonEffect.Parameters["TextureHeight"].SetValue(ribbonEdge.Height / GraphicsConstants.PIXELS_PER_UNIT);
            ribbonEffect.Parameters["ActiveTime"].SetValue(0);
            seamstressRecentContact = false;
        }

        public void SetRibbonSounds(AudioPlayer audioPlayer)
        {
            foreach (RibbonElement e in elements)
                e.SetRibbonSounds(new RibbonSounds(audioPlayer));
        }

        public void RemoveBodies()
        {
            body.Enabled = false;
            body.Dispose();
            foreach (RibbonElement element in elements)
            {
                element.element.body.Enabled = false;
                element.element.body.Dispose();
            }
        }

        public RibbonGemObject RibbonGem
        {
            get { return ribbonGem; }
        }

        public int RibbonColor
        {
            get { return ribbonColor; }
        }

        public void SetGem(RibbonGemObject g)
        {
            ribbonGem = g;
        }

        public void AddElement(RibbonElement re)
        {
            elements.Add(re);
        }

        public void AddElement(PhysicalObject bo, float pos, float rotation = 0.0f, bool flipped = false)
        {
            RibbonElement re = new RibbonElement(world, this, bo, pos, rotation, flipped);
            AddElement(re);
        }

        public void AddFeature(RibbonFeature rf)
        {
            features.Add(rf);
        }

        public void AddShrinking(float s)
        {
            shrinking = s;
        }

        private Shape GenerateShape()
        {
            points = new List<Vector2>();

            float startPos = ribbonStart;
            float endPos = ribbonEnd;

            if (loop)
            {
                while (startPos > ribbonLength)
                {
                    startPos = startPos - ribbonLength;
                    endPos = endPos - ribbonLength;
                }
                while (startPos < 0)
                {
                    startPos = startPos + ribbonLength;
                    endPos = endPos + ribbonLength;
                }

#if DEBUG
                /*Console.WriteLine("startPos: " + startPos);
                Console.WriteLine("endPos: " + endPos);*/
#endif

                if (startPos + ribbonLength > endPos - 0.5 && startPos + ribbonLength < endPos + 0.5)
                {
                    for (int j = 0; j < intervals.Count(); j++)
                    {
                        points.Add(path[j]);
            }

                    return new ChainShape(new Vertices(points), true);

                }
            }

            int i = 0;

            while (intervals[i] < startPos)
            {
                startPos -= intervals[i];
                endPos -= intervals[i];
                i++;
                while (i >= intervals.Count())
                {
                    i = i - intervals.Count();
                }
            }

            Vector2 v = new Vector2();
            v = path[i] + startPos * orientations[i];

            points.Add(v);

            while (intervals[i] < endPos)
            {
                endPos -= intervals[i];
                i++;
                while (i >= intervals.Count())
                {
                    i = i - intervals.Count();
                }
                points.Add(path[i]);
            }

            v = path[i] + endPos * orientations[i];
            points.Add(v);

            List<Vector2> clean_shape = new List<Vector2>();
            //now, need to "clean" shape (avoid vertices that are too close together)
            for (int k = 0; k + 1 < points.Count; k++)
            {
                if ((points[k] - points[k + 1]).Length() > 0.01f)
                {
                    clean_shape.Add(points[k]);
                }
            }
            clean_shape.Add(points[points.Count - 1]);
            points = clean_shape;

            if (clean_shape.Count == 2)
            {
                return new EdgeShape(clean_shape[0], clean_shape[1]);
            }
            else
            {
                return new ChainShape(new Vertices(points));
            }
        }

        public Vector2 PositionToPoint(float pos)
        {

            int i = 0;
            while (pos < 0)
            {
                pos += ribbonLength;
            }

            while (intervals[i] < pos)
            {
                pos -= intervals[i];
                i++;
                while (i >= intervals.Count())
                {
                    i = i - intervals.Count();
                }
            }

            return path[i] + pos * orientations[i];
        }

        public float PointToPosition(Vector2 v)
        {
            float totalPos = 0;
            float bestPos = 0;
            float minDist = float.MaxValue;

            for (int i = 0; i < intervals.Count(); i++)
            {
                Vector2 A = v - path[i];
                Vector2 B = path[(i + 1) % path.Count] - path[i];

                float pos = Vector2.Dot(A, B) / B.Length();
                if (i > 0)
                {
                    pos = Math.Max(pos, 0);
                }
                if (i + 1 < path.Count)
                {
                    pos = Math.Min(pos, intervals[i]);
                }

                Vector2 p = pos * orientations[i] + path[i];
                pos += totalPos;

                if (Vector2.DistanceSquared(p, v) < minDist)
                {
                    minDist = Vector2.DistanceSquared(p, v);
                    bestPos = pos;
                }

                totalPos += intervals[i];
            }

            return bestPos;
        }

        public Vector2 PositionToOrientation(float pos)
        {

            int i = 0;
            while (pos < 0)
            {
                pos += ribbonLength;
            }

            while (intervals[i] < pos)
            {
                pos -= intervals[i];
                i++;
                while (i >= intervals.Count())
                {
                    i = i - intervals.Count();
                }
            }

            return orientations[i];

        }

        public void Move(float dx, float dt)
        {
            //bool contact = false;
            if (!loop)
            {
                if (ribbonStart + dx < 0)
                {
                    dx = -ribbonStart;
                }
                if (ribbonEnd + dx > ribbonLength)
                {
                    dx = ribbonLength - ribbonEnd;
                }
            }

            ribbonStart += dx;
            ribbonEnd += dx;

            foreach (RibbonElement element in elements)
            {
                element.Move(dx, dt);
            }

            foreach (RibbonFeature feature in features)
            {
                feature.Move(dx, dt);
            }

            if (seamstressOnRibbon >= 1)
            {
                seamstress.RibbonSpeed = (dx / dt) * seamstressOrientation;
            }

            RegenerateFixture();

        }

        public bool Remove(RibbonElement re)
        {
            return elements.Remove(re);
        }

        public bool Contact()
        {
            int contacts = 0;
            foreach (RibbonElement element in elements)
            {
                if (element.GetContact())
                {
                    contacts += 1;
                }
            }

            bool ret = contacts > ribbonContacts;
            ribbonContacts = contacts;
            if (contacts > 0)
            {
                Console.WriteLine(contacts);
            }
            return contacts > 0;
        }


        public void RegenerateFixture()
        {
            Shape chain = GenerateShape();
            Fixture fixture = body.CreateFixture(chain);
            fixture.Friction = 0f;
            fixtures.Enqueue(fixture);

            if (fixtures.Count > 5)
            {
                body.DestroyFixture(fixtures.Dequeue());
            }
        }

        public void Flip()
        {
            foreach (RibbonElement element in elements)
            {
                element.Flip();
            }
        }

        public void SeamstressContactStarted(SeamstressObject seam)
        {
            seamstressOnRibbon++;
            if (seamstressOnRibbon >= 1)
                seamstress.isRibboned = true;
            if (seamstressOnRibbon == 1)
                seamstressRecentContact = true;
            seamstress = seam;
            if (seamstress.Ribbon != null && seamstress.Ribbon != this)
            {
                if (ribbonGem == null || ribbonGem.Enabled)
                {
                    seamstress.Ribbon = this;
                }
            }
            if (seamstress.Ribbon == null)
            {
                if (ribbonGem == null || ribbonGem.Enabled)
                {
                    seamstress.Ribbon = this;
                }
            }


            Vector2 v = seamstress.body.Position + seamstress.dimension.Y * (new Vector2(0, 0.5f));
            //v = seamstress.body.Position + (new Vector2(0, 0.5f));
            //debugPoint = v;
            seamstressRibbonPosition = PointToPosition(v);
#if DEBUG
            //Console.WriteLine(seamstressRibbonPosition);
            //Console.WriteLine(v);
#endif
            seamstressOrientation = PositionToOrientation(seamstressRibbonPosition);
        }

        public void SeamstressContactEnded()
        {
            seamstressOnRibbon--;
            if (seamstressOnRibbon <= 0)
            {
                seamstress.prevRibbonSpeed = new Vector2(0, 0);
                seamstress.isRibboned = false;
            }
        }

        public bool IsDiscrete()
        {
            return GraphicsHelper.Mod(ribbonStart, 1) < PhysicsConstants.RIBBON_DISCRETE_THRESHOLD
                || GraphicsHelper.Mod(ribbonStart, 1) > 1 - PhysicsConstants.RIBBON_DISCRETE_THRESHOLD;
        }

        private void CreateRobustPath()
        {
            robustPath = new List<Vector2>();
            List<Vector2> formattedPath = new List<Vector2>();
            foreach (Vector2 v in path)
                formattedPath.Add(v);
            if (loop)
            {
                //if (!(ribbonStart + ribbonLength > ribbonEnd - 0.5 && ribbonStart + ribbonLength < ribbonEnd + 0.5))
                    formattedPath.Add(path[0]);
            }
            for (int i = 0; i < formattedPath.Count - 1; i++)
            {
                Vector2 p1 = formattedPath[i];
                Vector2 p2 = formattedPath[(i + 1)];

                if (p1.X != p2.X)
                {
                    int difference = (int)Math.Abs(Math.Round(p2.X - p1.X));
                    for (int j = 0; j < difference; j++)
                    {
                        if (p1.X < p2.X)
                        {
                            robustPath.Add(new Vector2(p1.X + j, p1.Y));
                        }
                        else if (p2.X < p1.X)
                        {
                            robustPath.Add(new Vector2(p1.X - j, p1.Y));
                        }
                    }
                }
                else if (p1.Y != p2.Y);
                {
                    int difference = (int)Math.Abs(Math.Round(p2.Y - p1.Y));
                    for (int j = 0; j < difference; j++)
                    {
                        if (p1.Y < p2.Y)
                        {
                            robustPath.Add(new Vector2(p1.X, p1.Y + j));
                        }
                        else if (p2.Y < p1.Y)
                        {
                            robustPath.Add(new Vector2(p1.X, p1.Y - j));
                        }
                    }
                }
            }
        }

        private List<Vector2> CreateDrawPath()
        {
            List<Vector2> output = new List<Vector2>();
            Vector2 start = PositionToPoint(ribbonStart);
            Vector2 end = PositionToPoint(ribbonEnd);
            int? startIndex = null;
            int? endIndex = null;
            bool reversed = false;

            List<Vector2> formattedPath = new List<Vector2>();
            foreach (Vector2 v in robustPath)
                formattedPath.Add(v);

            for (int i = 0; i < formattedPath.Count; i++)
            {
                if (startIndex == null)
                {
                    if (i == formattedPath.Count - 1)
                        startIndex = i;
                    else if (FindPointPathIndex(start, i, formattedPath))
                        startIndex = i + 1;
                    
                    if (endIndex != null)
                        reversed = true;
                }
                if (endIndex == null)
                {
                    if (i == formattedPath.Count - 1)
                        endIndex = i;
                    else if (FindPointPathIndex(end, i, formattedPath))
                        endIndex = i + 1;
                }
            }

            if (startIndex != null && endIndex != null)
            {
                output.Add(start);
                if (reversed)
                {
                    if (startIndex == endIndex && loop)
                    {
                        drawSpools = false;
                    }
                    else
                    {
                        for (int i = startIndex.Value; i < formattedPath.Count - 1; i++)
                            output.Add(formattedPath.ElementAt(i));
                        for (int i = 0; i < endIndex.Value; i++)
                            output.Add(formattedPath.ElementAt(i));
                    }
                    //Console.WriteLine("reversed");
                }
                else
                {
                    if (startIndex == endIndex && loop)
                    {
                        drawSpools = false;
                        for (int i = startIndex.Value; i < formattedPath.Count - 1; i++)
                            output.Add(formattedPath.ElementAt(i));
                        for (int i = 0; i < startIndex.Value; i++)
                            output.Add(formattedPath.ElementAt(i));
                    }
                    else
                    {
                        for (int i = startIndex.Value; i < endIndex.Value; i++)
                            output.Add(formattedPath.ElementAt(i));
                    }
                    //Console.WriteLine("not");
                }
                output.Add(end);

            }
            return output;
        }

        private bool FindPointPathIndex(Vector2 point, int i, List<Vector2> formattedPath)
        {
            int mod1 = i;
            int mod2 = i + 1;
            bool isXAligned = formattedPath[mod1].X == point.X && formattedPath[mod2].X == point.X;
            bool isYBetweenP1P2 = formattedPath[mod1].Y <= formattedPath[mod2].Y && point.Y >= formattedPath[mod1].Y && point.Y <= formattedPath[mod2].Y;
            bool isYBetweenP2P1 = formattedPath[mod1].Y >= formattedPath[mod2].Y && point.Y <= formattedPath[mod1].Y && point.Y >= formattedPath[mod2].Y;

            bool isYAligned = formattedPath[mod1].Y == point.Y && formattedPath[mod2].Y == point.Y;
            bool isXBetweenP1P2 = formattedPath[mod1].X <= formattedPath[mod2].X && point.X >= formattedPath[mod1].X && point.X <= formattedPath[mod2].X;
            bool isXBetweenP2P1 = formattedPath[mod1].X >= formattedPath[mod2].X && point.X <= formattedPath[mod1].X && point.X >= formattedPath[mod2].X;

            return isXAligned && isYBetweenP1P2 || (isXAligned && isYBetweenP2P1) ||
                    isYAligned && isXBetweenP1P2 || isYAligned && isXBetweenP2P1;
        }

        public void ApplyLeftImpulse(float impulse)
        {
            leftImpulse = impulse;
        }

        public void ApplyRightImpulse(float impulse)
        {
            rightImpulse = impulse;
        }

        public void Update(float dt)
        {
            if(!seamstress.alive)
            {
                seamstress.Ribbon = null;
            }

            foreach (RibbonElement e in elements)
            {
                e.Update(dt);
            }

            UpdateSpeed();
            Move(ribbonSpeed, dt);

            ribbon3D = new Ribbon3D(CreateDrawPath(), loop);
            ribbon3D.Ticks += (int)time + 1;
            
            prevRibbonStart = ribbonStart;
            prevRibbonEnd = ribbonEnd;
        }

        public void UpdateSpeed()
        {
            if (Contact())
            {
                if (!resolvingConflict)
                {
                    resolvingConflict = true;
                    ribbonSpeed = -1.0f * ribbonSpeed;
                }

                holdMovement = 0;

                return;
            }

            resolvingConflict = false;

            ribbonSpeed -= RIBBON_SPEED * leftImpulse;
            ribbonSpeed += RIBBON_SPEED * rightImpulse;
            ribbonSpeed -= RIBBON_DYNAMIC_DRAG * ribbonSpeed;

            holdMovement += ribbonSpeed;

            if (rightImpulse == 0 && leftImpulse == 0)
            {
                //apply discrete movement
                if (PhysicsConstants.RIBBON_DISCRETE_ENABLE)
                {
                    if (IsDiscrete())
                    {
                        ribbonSpeed -= RIBBON_STATIC_DRAG * ribbonSpeed;
                    }
                }
                else if (PhysicsConstants.RIBBON_PULL_ENABLE)
                {
                    if (IsDiscrete())
                    {
                        ribbonSpeed -= RIBBON_STATIC_DRAG * ribbonSpeed;
                        holdMovement = 0;
                    }
                    else
                    {
                        float offset = GraphicsHelper.Mod(ribbonStart, 1);
                        
                        if (holdMovement > 0 && holdMovement < 1)
                        {
                            ribbonSpeed += (offset) * PhysicsConstants.RIBBON_PULL_POWER;
                        }
                        else if (holdMovement < 0 && holdMovement > -1)
                        {
                            ribbonSpeed -= (1 - offset) * PhysicsConstants.RIBBON_PULL_POWER;
                        }
                        else
                        {
                            ribbonSpeed += (offset) * PhysicsConstants.RIBBON_PULL_POWER;
                            ribbonSpeed -= (1 - offset) * PhysicsConstants.RIBBON_PULL_POWER;
                        }
                    }
                }
                else
                {
                    ribbonSpeed -= RIBBON_STATIC_DRAG * ribbonSpeed;
                }
            }

            rightImpulse = 0.0f;
            leftImpulse = 0.0f;

        }

        public void PlaySound(AudioPlayer audioPlayer)
        {
            foreach (RibbonElement e in elements)
            {
                e.PlaySound(audioPlayer);
            }
        }

        public void Draw(Canvas c)
        {
            /*c.DrawRibbon(ribbonTiles, new Color(0, 0.1f, 0, 0.4f));

            for (int i = 0; i + 1 < shape.Count; i++)
            {
                c.DrawLine(Color.Red, 2, shape[i], shape[i + 1], false);
            }*/

#if DEBUG
            Vector2 v = PositionToPoint(seamstressRibbonPosition);
            //c.DrawLine(Color.BlueViolet, 3, debugPoint, v, false);
#endif

            foreach (RibbonElement e in elements)
            {
                e.Draw(c);
            }
            foreach (RibbonFeature e in features)
            {
                e.Draw(c);
            }
        }

        public void Draw3D(Canvas c, Projections pass)
        {
            c.DrawRibbonPath(robustPath, RibbonColors.GetColor(ribbonColor), seamstress.Ribbon != this);
            time = c.Time;
            ribbonEffect.Parameters["Time"].SetValue(time);
            ribbonEffect = c.Assets.GetEffect("RibbonEffect");
            //ribbonEffect.Parameters["ColorDivisions"].SetValue(4);
            Texture2D ribbonEdge = c.Assets.GetTexture(ribbonTextureLabel);
            ribbonEffect.Parameters["Texture"].SetValue(ribbonEdge);
            ribbonEffect.Parameters["TextureHeight"].SetValue(ribbonEdge.Height / GraphicsConstants.PIXELS_PER_UNIT);
            ribbonEffect.Parameters["ActiveTime"].SetValue(0);
            // draw ribbon
            //ribbonEffect.Parameters["PulseCenter"].SetValue((c.Camera.Transform(seamstress.body.Position - seamstress.dimension.Y * Vector2.UnitY / 2) - c.Camera.Dimensions / 2) / c.Camera.Dimensions.X * 2);
            ribbonEffect.Parameters["RibbonColor"].SetValue(RibbonColors.GetColor(ribbonColor).ToVector4());
            //mod to find seamstress's true position
            float minDistance = ribbonStart - GraphicsHelper.Mod(ribbonStart, ribbonLength);
            float pos = seamstressRibbonPosition - ribbonStart;

            ribbonEffect.Parameters["PulseCenter"].SetValue(pos / (ribbonEnd - ribbonStart));
            ribbonEffect.Parameters["MinTexCoord"].SetValue(0);
            ribbonEffect.Parameters["MaxTexCoord"].SetValue(ribbonEnd - ribbonStart);
            ribbonEffect.Parameters["TotalLength"].SetValue((ribbonEnd - ribbonStart) * 2);
            //Console.WriteLine(ribbonStart + " >> " + ribbonEndModulo);
            if (this.Equals(seamstress.Ribbon))
            {
                ribbonEffect.Parameters["ActiveTime"].SetValue(seamstress.RibbonActiveTime);

                if (seamstressRecentContact)
                {
                    Console.WriteLine("entered");
                    if (time - contactTime >= 60)
                        contactTime = ribbon3D.Ticks;
                    ribbonEffect.Parameters["ContactTime"].SetValue(contactTime);
                    seamstressRecentContact = false;
                }
            }
            else
                ribbonEffect.Parameters["ActiveTime"].SetValue(0);

            c.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(ribbonEffect, ribbon3D, pass);
            
            c.BeginDraw(0,BlendState.NonPremultiplied);
            if (spoolSpin > 360)
                spoolSpin = 0 + spoolSpin % 360;

            Color spoolColor =RibbonColors.GetColor(ribbonColor);
            System.Drawing.Color storeColor = System.Drawing.Color.FromArgb(spoolColor.A, spoolColor.R, spoolColor.G, spoolColor.B);
            storeColor = ControlPaint.LightLight(storeColor);
            spoolColor.R = storeColor.R;
            spoolColor.G = storeColor.G;
            spoolColor.B = storeColor.B;
            spoolColor.A = storeColor.A;

            Color spoolLabel1Color;
            Color spoolLabel2Color;
            if (selectedSpool == "right")
            {
                spoolLabel1Color = Color.Gainsboro;
                spoolLabel1Color.A = (byte)(spoolColor.A * .3f);

                spoolLabel2Color = Color.Gold;
                spoolLabel2Color.A = (byte)(spoolColor.A * .95f);
            }

            else if (selectedSpool == "left")
            {
                spoolLabel1Color = Color.Gold;
                spoolLabel1Color.A = (byte)(spoolColor.A * .95f);

                spoolLabel2Color = Color.Gainsboro;
                spoolLabel2Color.A = (byte)(spoolColor.A * .3f);
            }

            else
            {
                spoolLabel1Color = Color.Gainsboro;
                spoolLabel1Color.A = (byte)(spoolColor.A * .95f);

                spoolLabel2Color = Color.Gainsboro;
                spoolLabel2Color.A = (byte)(spoolColor.A * .95f);
            }

            if (drawSpools)
            {
                c.DrawSprite(Sprite.Build(c, "spool"), spoolColor, PositionToPoint(ribbonStart), spoolSpin);
                c.DrawSprite(Sprite.Build(c, "spool_a"), spoolLabel1Color, PositionToPoint(ribbonStart));
                c.DrawSprite(Sprite.Build(c, "spool"), spoolColor, PositionToPoint(ribbonEnd), spoolSpin);
                c.DrawSprite(Sprite.Build(c, "spool_d"), spoolLabel2Color, PositionToPoint(ribbonEnd));
            }
            c.EndDraw();
        }

        public void DrawCollisionNotifications(Canvas c)
        {
            foreach (RibbonElement e in elements)
            {
                e.DrawCollisionNotification(c);
            }
        }

        public List<Vector2> PossibleRibbonBlockPoints()
        {
            List<Vector2> output = new List<Vector2>();

            int ribbonLength = (int)(ribbonEnd - ribbonStart);

            for (int i = 0; i <= ribbonLength; i++)
            {
                Vector2 orientation = PositionToOrientation(ribbonStart + i);
                Vector2 point = PositionToPoint(ribbonStart + i);

                //horizontal orientation
                if (orientation.X == 1 && orientation.Y == 0 || orientation.X == -1 && orientation.Y == 0)
                {
                    if (i + 1 <= ribbonLength)
                    {
                        if (PositionToOrientation(ribbonStart + i + 1) == orientation)
                        {
                            Vector2 point2 = PositionToPoint(ribbonStart + i + 1);
                            if (point2.X - point.X >= 0)
                            {
                                output.Add(point);
                                output.Add(new Vector2(point.X, point.Y + 1));
                            }
                            else
                            {
                                output.Add(new Vector2(point.X - 1, point.Y));
                                output.Add(new Vector2(point.X - 1, point.Y + 1));
                            }
                        }
                        else
                        {
                            Vector2 point2 = PositionToPoint(ribbonStart + i + 1);
                            if (point2.Y < point.Y)
                            {
                                if (i != 0)
                                {
                                    Vector2 point3 = PositionToPoint(ribbonStart + i - 1);
                                    if(point3.X < point.X)
                                        output.Add(new Vector2(point.X, point.Y));
                                    else
                                        output.Add(new Vector2(point.X - 1, point.Y));
                                }
                            }
                            else
                            {
                                if (i != 0)
                                {
                                    Vector2 point3 = PositionToPoint(ribbonStart + i - 1);
                                    if (point3.X > point.X)
                                        output.Add(new Vector2(point.X - 1, point.Y + 1));
                                    else
                                        output.Add(new Vector2(point.X, point.Y + 1));
                                }
                            }
                        }
                    }
                }
                //vertical orientation
                else if (orientation.X == 0 && orientation.Y == 1 || orientation.X == 0 && orientation.Y == -1)
                {
                    if (i + 1 <= ribbonLength)
                    {
                        if (PositionToOrientation(ribbonStart + i + 1) == orientation)
                        {
                            Vector2 point2 = PositionToPoint(ribbonStart + i + 1);
                            if (point2.Y - point.Y <= 0)
                            {
                                output.Add(point);
                                output.Add(new Vector2(point.X - 1, point.Y));
                            }
                            else
                            {
                                output.Add(new Vector2(point.X, point.Y + 1));
                                output.Add(new Vector2(point.X - 1, point.Y + 1));
                            }
                        }
                        else
                        {
                            Vector2 point2 = PositionToPoint(ribbonStart + i + 1);
                            if (point2.X < point.X)
                            {
                                if (i != 0)
                                {
                                    Vector2 point3 = PositionToPoint(ribbonStart + i - 1);
                                    if (point3.Y > point.Y)
                                        output.Add(new Vector2(point.X - 1, point.Y));
                                    else
                                        output.Add(new Vector2(point.X - 1, point.Y + 1));
                                }
                            }
                            else
                            {
                                if (i != 0)
                                {
                                    Vector2 point3 = PositionToPoint(ribbonStart + i - 1);
                                    if (point3.Y < point.Y)
                                        output.Add(new Vector2(point.X, point.Y + 1));
                                    else
                                        output.Add(new Vector2(point.X, point.Y));
                                }
                            }
                        }
                    }

    }
}
            return output;
        }
    }
}
