using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XMLContent;
using CodeLibrary.Audio;
using CodeLibrary.Graphics;
using FarseerPhysics.Dynamics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CodeLibrary.Storage;

namespace CodeLibrary.Engine
{

    public class LevelGenerator
    {
        #region fields
        private Level level;
        private List<RibbonObject> ribbons;
        private SeamstressObject seamstress;
        private GroundObject ground;
        private MiasmaObject miasma;
        private List<CameraHotspot> cameraHotspots;
        private List<GameObject> objects;
        private List<RibbonFeature> dynamics;
        private List<DecorationObject> decorations;
        private List<CollectableObject> collectables;
        private List<RibbonGemObject> gems;
        private Canvas canvas;
        private AudioPlayer audioPlayer;
        private World world;
        private List<Rectangle> groundRects;
        private ActAssets actAssets;
        private int actNumber;
        private LevelInfo levelInfo;
        private int gridWidth;
        private int gridHeight;
        private ParallaxBackgroundSet background;
        #endregion

        #region properties
        public Level Level
        {
            get
            {
                return level;
            }
        }

        public int ActNumber
        {
            get
            {
                return actNumber;
            }
        }

        public int GridWidth { get { return gridWidth; } }

        public int GridHeight { get { return gridHeight; } }

        public ActAssets ActAssets { get { return actAssets; } }

        public GroundObject Ground { get { return ground; } }
        public MiasmaObject Miasma { get { return miasma; } }
        public List<CameraHotspot> CameraHotspots { get { return cameraHotspots; } }
        public List<RibbonObject> Ribbons { get { return ribbons; } }
        public SeamstressObject Seamstress { get { return seamstress; } }
        public List<GameObject> Objects { get { return objects; } }
        public List<DecorationObject> Decorations { get { return decorations; } }
        public List<RibbonFeature> Dynamics { get { return dynamics; } }

        #endregion

        #region initialize
        public LevelGenerator(LevelInfo levelInfo, Canvas canvas, AudioPlayer audioPlayer, World world, bool gc)
        {
            this.levelInfo = levelInfo;
            objects = new List<GameObject>();
            string levelName = this.levelInfo.LevelName;
            level = canvas.Assets.GetLevel(levelName);
            this.canvas = canvas;
            this.audioPlayer = audioPlayer;
            this.world = world;
            groundRects = new List<Rectangle>();
            this.actNumber = level.levelNum;
            LoadContent(gc);
            //XMLGenerator.ToXML(level, "C:\\Users\\Will\\test.xml");
        }

        public LevelGenerator(Level level, LevelInfo levelInfo, Canvas canvas, AudioPlayer audioPlayer, World world, bool gc)
        {
            this.levelInfo = levelInfo;
            objects = new List<GameObject>();
            string levelName = this.levelInfo.LevelName;
            this.level = level;
            this.canvas = canvas;
            this.audioPlayer = audioPlayer;
            this.world = world;
            groundRects = new List<Rectangle>();
            this.actNumber = level.levelNum;
            LoadContent(gc);
            //XMLGenerator.ToXML(level, "C:\\Users\\Will\\test.xml");
        }

        #endregion

        #region methods

        public static Level CreateLevel(string path)
        {
            string xml = File.ReadAllText(path);
            Console.WriteLine(xml);
            Level level = (Level)new XmlSerializer(typeof(Level)).Deserialize(new StringReader(xml));
            return level;
        }

        public void LoadContent(bool gc)
        {

            actAssets = new ActAssets(canvas, level.levelNum);
            this.gridWidth = (int)level.gameGrid.X;
            this.gridHeight = (int)level.gameGrid.Y;
            SeamstressObject tempSeamstress = new SeamstressObject(new SeamstressSprites(canvas), new SeamstressSounds(audioPlayer), world);
            seamstress = tempSeamstress;
            seamstress.spawnID = levelInfo.SpawnID;
            seamstress.body.Position = level.seamstress.position;

            RectangleF gameGrid = new RectangleF(level.seamstress.position.X, level.seamstress.position.X, level.seamstress.position.Y, level.seamstress.position.Y);

            dynamics = new List<RibbonFeature>();

            List<Rectangle> groundRectangles = new List<Rectangle>();
            foreach (Ground g in level.ground)
            {
                Rectangle groundRect = new Rectangle((int)g.position.X, (int)g.position.Y, (int)g.dimensions.X, (int)g.dimensions.Y);
                groundRectangles.Add(groundRect);
                gameGrid = gameGrid.Envelope(new RectangleF(groundRect));
            }
            ground = new GroundObject(canvas, actAssets, groundRectangles, world);
            objects.Add(ground);

            //MiasmaObject miasma = new MiasmaObject(Sprite.Build(canvas, "miasma"), world, environmentObject.position + 0.5f * Vector2.One, environmentObject.rotation, generateLayout(environmentObject.dimensions));

            List<Rectangle> miasmaRectangles = new List<Rectangle>();
            foreach (Miasma m in level.miasma)
            {
                Rectangle miasmaRect = new Rectangle((int)m.position.X, (int)m.position.Y, (int)m.dimensions.X, (int)m.dimensions.Y);
                Rectangle[] splitRectangles = GraphicsHelper.SplitRectangle(miasmaRect);
                for (int n = 0; n < splitRectangles.Length; n++)
                    miasmaRectangles.Add(splitRectangles[n]);
                gameGrid = gameGrid.Envelope(new RectangleF(miasmaRect));
            }
            miasma = new MiasmaObject(canvas, actAssets, miasmaRectangles, world);
            objects.Add(miasma);

            ribbons = new List<RibbonObject>();
            foreach (Ribbon r in level.ribbons)
            {
                RibbonObject tempRibbon = new RibbonObject(canvas, world, seamstress, r.color, r.path, r.start, r.end, r.loop);
                ribbons.Add(tempRibbon);
            }

            //gems are behind almost everything, so load them before loading ribbon elements.
            gems = new List<RibbonGemObject>();
            foreach (Gem g in level.gems)
            {
                RibbonGemObject ribbonGem = new RibbonGemObject(canvas, world, g.position, g.color, (levelInfo.RibbonGems & (1 << g.color)) != 0);
                gems.Add(ribbonGem);
                objects.Add(ribbonGem);
                foreach (RibbonObject r in ribbons)
                {
                    if (r.RibbonColor == ribbonGem.ColorID)
                    {
                        r.SetGem(ribbonGem);
                    }
                }
            }
            foreach (RibbonObject r in ribbons)
                objects.Add(r);

            cameraHotspots = new List<CameraHotspot>();
            foreach (Hotspot h in level.cameras)
            {
                CameraHotspot ch = new CameraHotspot(h.area, h.followsPlayer, h.scale);
                cameraHotspots.Add(ch);
            }

            foreach (RibbonBox rb in level.ribbonBoxes)
            {
                PhysicalObject bo = new BoxObject(actAssets.PlatformTexture, world, new Vector2(0, 0), rb.rotation, generateRectangleList(rb.dimensions), new Vector2(0, 0));
                if (rb.ribbonID > -1)
                {
                    ribbons[rb.ribbonID].AddElement(bo, rb.position, rb.rotation, rb.flipped);
                }
                else
                {
                    objects.Add(bo);
                }
            }

            foreach (Shooter s in level.shooters)
            {
                ShooterBox sb = new ShooterBox(actAssets.Cannon, actAssets.Bullet, world, s.position, s.rotation, (int)s.frequency);
                if (s.ribbonID > -1)
                {
                    ribbons[s.ribbonID].AddElement(sb, s.ribbonPosition, s.rotation, s.flipped);
                }
                else
                {
                    objects.Add(sb);
                }
            }

            foreach (Needle n in level.needles)
            {
                SpikeBox sb = new SpikeBox(Sprite.Build(canvas, "spikes"), world, n.position, n.rotation, generateRectangleList(n.dimensions), n.amplitude, n.frequency);
                if (n.ribbonID > -1)
                {
                    ribbons[n.ribbonID].AddFeature(sb);
                }
                else
                {
                    dynamics.Add(sb);
                }
            }

            foreach (Hook h in level.hooks)
            {
                SpriteCollection hookSprites = new SpriteCollection(canvas);
                hookSprites.Add(actAssets.HookBody);
                hookSprites.Add(actAssets.HookTop);

                HookObject ho = new HookObject(hookSprites, world, new Vector2(0, 0), 0.0f, generateRectangleList(h.dimensions));
                if (h.ribbonID > -1)
                {
                    ribbons[h.ribbonID].AddElement(ho, h.ribbonPosition, 0.0f, h.flipped);
                }
                else
                {
                    objects.Add(ho);
                }
            }

            foreach (TelescopingBlock tb in level.telescopingBlocks)
            {
                SpriteCollection teleSprites = new SpriteCollection(canvas);
                teleSprites.Add(actAssets.TelescopingBase);
                teleSprites.Add(actAssets.TelescopingExtension);

                PhysicalObject bo = new TelescopicBox(teleSprites, world, new Vector2(0, 0), tb.rotation, tb.height);
                if (tb.ribbonID > -1)
                {
                    ribbons[tb.ribbonID].AddElement(bo, tb.ribbonPosition, tb.rotation, tb.flipped);
                }
                else
                {
                    objects.Add(bo);
                }
            }

            foreach (Flipbar fb in level.flipbars)
            {
                if (fb.ribbonID > -1)
                {
                    FlipBarObject fbo = new FlipBarObject(world, ribbons[fb.ribbonID], fb.ribbonPosition, Sprite.Build(canvas, "flipbar"), 0.0f);
                    ribbons[fb.ribbonID].AddElement(fbo);
                }
            }

            foreach (RotatingPlatform rp in level.rotatingPlatforms)
            {
                SpriteCollection sc = new SpriteCollection(canvas);
                sc.Add("rotationblock");
                sc.Add("rotationcircle");

                RotatingObject cog = new RotatingObject(sc, world, rp.position, rp.initialRotation, generateRectangleList(rp.dimensions), rp.pivot, rp.rotationSpeed);

                if (rp.ribbonID > -1)
                {
                    ribbons[rp.ribbonID].AddFeature(cog);
                }
                else
                {
                    dynamics.Add(cog);
                }
            }

            decorations = new List<DecorationObject>();
            foreach (Decoration d in level.decorations)
            {
                string s = d.image;
                if (gc && s.Contains("painting"))
                    s += "gc";
                DecorationObject dec = new DecorationObject(canvas, s, d.position, d.dimensions, d.rotation);
                decorations.Add(dec);
            }

            //above is Danny's new part ^
            int collectableIndex = 0;
            collectables = new List<CollectableObject>();
            foreach (Collectable c in level.collectables)
            {
                CollectableObject col = new CollectableObject(canvas, world, c.position, c.id, (levelInfo.CollectablesThisRound & (1 << c.id)) != 0, (levelInfo.Collectables & (1 << c.id)) != 0);
                collectables.Add(col);
                objects.Add(col);
                collectableIndex++;
            }

            //this MUST be after collectables and gems... violators BEWEAR
            int i = 1;
            foreach (SaveRock sr in level.saveRocks)
            {
                SaveRockObject save;
                if (sr.endFlag)
                {
                    save = new SaveRockObject(Sprite.Build(canvas, "saverock_rock"), Sprite.Build(canvas, "largeexitdoor"),
                        world, sr.position, i, sr.endFlag, levelInfo);
                }
                else
                {
                    save = new SaveRockObject(Sprite.Build(canvas, "saverock_rock"), Sprite.Build(canvas, "saverock_bow"),
                    world, sr.position, i, sr.endFlag, levelInfo);
                }
                //update save rocks so they have lists of all variables whose collection status need to be saved
                save.SetCollectablesAndGems(collectables, gems);
                if (i == levelInfo.SpawnID)
                {
                    seamstress.body.Position = sr.position;
                }
                objects.Add(save);
                i++;
            }

            background = ParallaxBackgroundSet.Build(canvas, level.seamstress.position, ActAssets.Theme);

            canvas.Camera.EnableBounds(gameGrid);
        }

        public LevelGridStore[,] GenerateGridModel()
        {
            LevelGridStore[,] output = new LevelGridStore[gridWidth, -gridHeight];
            //Console.WriteLine(gridWidth + " " + gridHeight);
            foreach (Ground g in level.ground)
            {
                //Console.WriteLine((int)g.position.X + " " + -(int)g.position.Y + 1);
                if ((int)g.position.X >= 0 && (int)g.position.X < output.GetLength(0) && -(int)g.position.Y >= 0 && -(int)g.position.Y < output.GetLength(1))
                    output[(int)g.position.X, -(int)g.position.Y] = (new LevelGridStore(g.position, "ground"));
            }
            foreach (Miasma m in level.miasma)
            {
                Rectangle miasmaRect = new Rectangle((int)m.position.X, (int)m.position.Y, (int)m.dimensions.X, (int)m.dimensions.Y);
                Rectangle[] splitRectangles = GraphicsHelper.SplitRectangle(miasmaRect);
                for (int n = 0; n < splitRectangles.Length; n++)
                {
                    try
                    {
                        output[splitRectangles[n].X, -splitRectangles[n].Y] = (new LevelGridStore(new Vector2(splitRectangles[n].X, splitRectangles[n].Y), "miasma"));
                    }
                    catch { }
                }
            }
            return output;
        }

        #endregion

        /// <summary>
        /// Generates a layout 2D array for block layout from a serializable dimension vector.
        /// </summary>
        /// <param name="dimension">width by height of desired layout</param>
        /// <returns>2D array describing layout of box.</returns>
        public static int[,] generateLayout(Vector2 dimension)
        {
            int[,] layout = new int[(int)dimension.Y + 2, (int)dimension.X + 2];
            for (int ii = 1; ii < layout.GetLength(0) - 1; ii++)
            {
                for (int jj = 1; jj < layout.GetLength(1) - 1; jj++)
                {
                    layout[ii, jj] = 1;
                }
            }
            return layout;
        }

        /// <summary>
        /// Generates a list of rectangles that corresponds to the given box dimensions.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns>List of rectangle objects.</returns>
        public static List<Rectangle> generateRectangleList(Vector2 dimension)
        {
            List<Rectangle> rects = new List<Rectangle>();

            for (int ii = 0; ii < dimension.X; ii++)
            {
                for (int jj = 0; jj < dimension.Y; jj++)
                {
                    rects.Add(new Rectangle(ii, jj, 1, 1));
                }
            }

            return rects;
        }

        public ParallaxBackgroundSet Background { get { return background; } }
    }

}
