using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;
using CodeLibrary.Input;
using CodeLibrary.Audio;
using CodeLibrary.Engine;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using XMLContent;

namespace CodeLibrary.Context
{
    public class EditorBuildContext : GameContext
    {
        SeamstressObject seamstress;
        RibbonPointStoreObject buildingRibbon;
        List<RibbonObject> ribbons;
        List<RibbonPointStoreObject> ribbonStores;
        RibbonObject ribbon;

        ActAssets actAssets;
        Point seamstressPos;
        Engine.GameObject selection;
        RibbonLabel ribbonSelection;
        Engine.GameObject[,] gridModel;
        RibbonElementPoint[,] elePointModel;
        GroundObject ground;
        MiasmaObject miasma;

        World world;
        CollisionController collisionController;
        int scrollCounter;
        float scrollStore;
        List<Engine.GameObject> objects;
        bool drawGridChange;
        bool moveToRibbon;
        bool ribbonLoop;
        bool ribbonOnly;
        bool removeRibbonBody;
        bool removeStoreRibbon;
        int removeRibbonID;
        bool resetGrid;

        public bool ResetGrid { get { return resetGrid; } set { resetGrid = value; } }
        public SeamstressObject Seamstress { get { return seamstress; } }
        public List<RibbonObject> Ribbons { get { return ribbons; } }
        public List<RibbonPointStoreObject> RibbonStores { get { return ribbonStores; } }

        public GroundObject Ground { get { return ground; } }
        public MiasmaObject Miasma { get { return miasma; } }
        public Engine.GameObject[,] GridModel { get { return gridModel; }}
        public bool RibbonOnly { get { return ribbonOnly; } set { ribbonOnly = value; } }
        public RibbonLabel RibbonSelection{get{return ribbonSelection; }set{ribbonSelection = value;}}
        public Engine.GameObject Selection { get { return selection; } set { selection = value; } }
        public bool RibbonLoop { get { return ribbonLoop; } set { ribbonLoop = value; } }
        public RibbonObject Ribbon { get { return ribbon; } set { ribbon = value; } }
        public RibbonPointStoreObject BuildingRibbon { get { return buildingRibbon; } set { buildingRibbon = value; } }
        public bool MoveToRibbon { get { return moveToRibbon; } set { moveToRibbon = value; } }
        public int ScrollCounter { get { return scrollCounter; } set { scrollCounter = value; } }

        public EditorBuildContext(Level level, GameContext other)
            : this(other, level.gameGrid.X, -level.gameGrid.Y)
        {
            addSeamstress(level.seamstress.position.X - 0.5f, level.seamstress.position.Y + 0.5f);
            foreach (Ribbon r in level.ribbons)
            {
                ribbonLoop = r.loop;
                Vector2 pathStart = r.path[0];
                Vector2 pathEnd = r.path[r.path.Count - 1];
                List<Vector2> pathPoints = new List<Vector2>();
                for(int i = 1; i < r.path.Count - 1; i++){
                    pathPoints.Add(r.path[i]);
                }

                addRibbonPoint("pathStart",pathStart.X, pathStart.Y);
                foreach (Vector2 pathPoint in pathPoints)
                {
                    addRibbonPoint("pathPoint", pathPoint.X, pathPoint.Y);
                }
                addRibbonPoint("pathEnd", pathEnd.X, pathEnd.Y);
                addRibbonToPath();
                moveRibbonStart(r.start);
                moveRibbonEnd(r.end);
                createRibbon();
            }
            foreach (RibbonBox r in level.ribbonBoxes)
            {
                int rID = r.ribbonID;
                float position = r.position;
                Vector2 orientation = ribbons.ElementAt(rID).PositionToOrientation(position);
                bool flipped = r.flipped;
                LoadRibbonObject("box", rID, orientation, position, flipped);   
            }
            foreach (Hook h in level.hooks)
            {
                int rID = h.ribbonID;
                float position = h.ribbonPosition;
                Vector2 orientation = ribbons.ElementAt(rID).PositionToOrientation(position);
                bool flipped = h.flipped;
                LoadRibbonObject("hook", rID, orientation, position, flipped); 
            }
            foreach (Shooter s in level.shooters)
            {
                int rID = s.ribbonID;
                float position = s.ribbonPosition;
                Vector2 orientation = ribbons.ElementAt(rID).PositionToOrientation(position);
                bool flipped = s.flipped;
                LoadRibbonObject("shooter", rID, orientation, position, flipped); 
            }
            foreach (TelescopingBlock t in level.telescopingBlocks)
            {
                int rID = t.ribbonID;
                float position = t.ribbonPosition;
                Vector2 orientation = ribbons.ElementAt(rID).PositionToOrientation(position);
                bool flipped = t.flipped;
                LoadRibbonObject("telescoping", rID, orientation, position, flipped); 
            }
            foreach (Ground g in level.ground)
            {
                addGround(g.position.X, g.position.Y + 1, 1,1);
            }
            GenerateGround();
            foreach (Miasma m in level.miasma)
            {
                addMiasma(m.position.X, m.position.Y + 1, 1,1);
                GenerateMiasma();
            }
            foreach (SaveRock sr in level.saveRocks)
            {
                addSave(sr.position.X - 0.5f, sr.position.Y + 0.5f, sr.endFlag, 0, 0);
            }

        }

        public EditorBuildContext(GameContext other,float gridWidth = 20, float gridHeight = 11)
            : base(other)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            resetGrid = true;
            gridModel = new Engine.GameObject[(int)GridWidth, (int)GridHeight];
            elePointModel = new RibbonElementPoint[(int)GridWidth, (int)GridHeight];
            GridChanged = true;
            world = new World(new Vector2(0, 0));
            scrollCounter = 0;
            scrollStore = 1;

            buildingRibbon = new RibbonPointStoreObject();

            seamstress = new SeamstressObject(new SeamstressSprites(Canvas), new SeamstressSounds(AudioPlayer), world);
            addSeamstress((float)Math.Round(GridWidth / 2), (float)Math.Round(-GridHeight / 2));
            objects = new List<Engine.GameObject>();
            BackgroundColor = Color.LightSteelBlue;
            drawGridChange = true;
            moveToRibbon = false;
            ribbons = new List<RibbonObject>();
            ribbonStores = new List<RibbonPointStoreObject>();
            removeRibbonBody = false;
            removeRibbonID = 0;
            ribbonOnly = false;
            removeStoreRibbon = false;
            actAssets = new ActAssets(Canvas, 1);
        }

        public override void Initialize()
        {
        }

        private void LoadRibbonObject(string type, int rID, Vector2 orientation, float position, bool flipped)
        {
            if (orientation == new Vector2(-1, 0))
            {
                Vector2 pos = ribbons.ElementAt(rID).PositionToPoint(position + 1);
                if (flipped)
                    AddCorrectObject(type,pos.X, pos.Y);
                else
                    AddCorrectObject(type,pos.X, pos.Y + 1);
            }
            else if (orientation == new Vector2(0, 1))
            {
                Vector2 pos = ribbons.ElementAt(rID).PositionToPoint(position + 1);
                if (flipped)
                    AddCorrectObject(type,pos.X - 1, pos.Y);
                else
                    AddCorrectObject(type,pos.X, pos.Y);
            }
            else if (orientation == new Vector2(1, 0))
            {
                Vector2 pos = ribbons.ElementAt(rID).PositionToPoint(position);
                if (flipped)
                    AddCorrectObject(type,pos.X, pos.Y + 1);
                else
                    AddCorrectObject(type,pos.X, pos.Y);
            }
            else
            {
                Vector2 pos = ribbons.ElementAt(rID).PositionToPoint(position);
                if (flipped)
                    AddCorrectObject(type,pos.X, pos.Y);
                else
                    AddCorrectObject(type,pos.X - 1, pos.Y);
            }
        }

        private void AddCorrectObject(string type, float x, float y)
        {
            if (type == "box")
                addBox(x, y);
            else if (type == "hook")
                addHook(x, y);
            else if (type == "shooter")
                addRibbonShooter(x, y, 5);
            else if (type == "telescoping")
                addTelescoping(x, y, 3);
        }
        public override void Dispose()
        {
            world.Clear();
        }

        public override void Update(GameTime gameTime)
        {

            if (removeRibbonBody)
            {
                if (ribbons != null && ribbons.Count > 0)
                {
                    Console.WriteLine("remove");
                    if (ribbons.ElementAt(0) != null)
                    ribbons.ElementAt(0).RemoveBodies();
                    objects.Remove(ribbons.ElementAt(0));
                    ribbons.RemoveAt(removeRibbonID);
                    removeStoreRibbon = true;
                }
                removeRibbonBody = false;
                removeRibbonID = 0;
            }
            if (removeStoreRibbon)
            {
                if (ribbon != null)
                {
                    ribbon.body.Enabled = false;
                    ribbon.RemoveBodies();
                    objects.Remove(ribbon);
                    ribbon = null;
                }
                removeStoreRibbon = false;
            }
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            world.Step(dt);
            if (GridChanged)
            {
                drawGridChange = true;
                Console.WriteLine("changed");
                resizeGridModel();
                GridChanged = false;
            }
            foreach (Engine.GameObject o in gridModel)
            {
                if (o != null)
                {
                    if (o is SeamstressObject){}
                    else{
                        o.Update(dt);
                    }
                }
            }
            foreach (Engine.GameObject o in objects)
            {
                if (o != null)
                {
                    o.Update(dt);
        }
            }
            foreach (RibbonObject r in ribbons)
            {
                r.Update(dt);
            }

            if (buildingRibbon != null && buildingRibbon.MoveToRibbon)
            {
                moveToRibbon = true;
            }
            if (ground != null)
            {
                ground.Update(dt);
            }
            if (miasma != null)
            {
                miasma.Update(dt);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {         
            Canvas.Camera.Update(gameTime);
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            if (!ribbonOnly)
            {
                Canvas.DrawRectangle(Color.Gainsboro, Color.Gainsboro, 1, new Rectangle(0, -(int)GridHeight, (int)GridWidth, (int)GridHeight), false);
            }
            else
            {
                Canvas.DrawRectangle(Color.DarkGray, Color.DarkGray, 1, new Rectangle(0, -(int)GridHeight, (int)GridWidth, (int)GridHeight), false);
                foreach (RibbonElementPoint point in elePointModel)
                {
                    if (point != null)
                    {
                        point.Draw(Canvas);
                    }
                }
            }
            DrawGrid(GridWidth, GridHeight, 2);
            if (drawGridChange)
            {
                Canvas.Camera.Scale = calculateScale(GridWidth, GridHeight);
                Canvas.Camera.Position = new Vector2(GridWidth / 2f, GridHeight / 2f - GridHeight);
                drawGridChange = false;
            }

            Draw3D(Projections.Behind);
            foreach (GameObject o in objects)
            {
                if (o != null)
                {
                    if (o is RibbonElementStore) { }
                    else
                    o.Draw(Canvas);
                }   
            }
            foreach (GameObject o in gridModel)
            {
                if (o != null && o is RibbonLabel)
                    o.Draw(Canvas);
                else if (o != null && o is RibbonElementStore)
                    o.Draw(Canvas);
            }
            if (seamstress != null)
            {
                seamstress.Draw(Canvas);
            }
            if (ground != null)
            {
                ground.Draw(Canvas);
            }
            if (miasma != null)
            {
                miasma.Draw(Canvas);
            }
            foreach (RibbonObject r in ribbons)
            {
                r.Draw(Canvas);
            }
            Draw3D(Projections.Front);
            if (selection != null)
            {
                if (selection is SeamstressObject)
                {
                    SeamstressObject temp = selection as SeamstressObject;
                    temp.DrawSelection(Canvas);
                }
                else
                {
                    PhysicalObject temp = selection as PhysicalObject;
                    temp.DrawSelection(Canvas);
                }
            }
            
            if (ribbonSelection != null)
            {
                ribbonSelection.DrawSelection(Canvas);
            }

            if (ribbon == null && buildingRibbon != null)
            {
                buildingRibbon.DrawRibbonTemp(Canvas, ribbonLoop);
            }
            if (buildingRibbon != null)
                buildingRibbon.Draw(Canvas);
        }

        private void Draw3D(Projections pass)
        {
            Canvas.EndDraw();
            Canvas.Begin3D();
            foreach (RibbonObject r in ribbons)
                r.Draw3D(Canvas, pass);
            if (ribbon != null)
            {
                ribbon.Draw3D(Canvas, pass);
            }
            Canvas.End3D();
            Canvas.BeginDraw();
        }

        public override void PlayAudio(GameTime gameTime)
        {
            
        }

        public bool isSquareOccupied(int x, int y)
        {
            return gridModel[x, -y] != null;
        }

        public void setSelection(int x, int y, bool selectSeamstress, bool selectObjects, bool selectRibbon)
        {
            if (selectObjects && !selectSeamstress && ! selectRibbon)
            {
                if (gridModel[x, -y] is RibbonLabel) { Console.WriteLine("label"); }
                else if (gridModel[x, -y] is SeamstressObject) { Console.WriteLine("Seamstress"); }
                else{
                    selection = gridModel[x, -y];
                }
            }
            else if (selectRibbon && !selectSeamstress && !selectObjects)
            {
                if (gridModel[x, -y] is RibbonLabel) {
                    selection = gridModel[x, -y];
                }
            }
            else if (!selectRibbon && !selectSeamstress && !selectObjects) { }
            else
        {
            selection = gridModel[x, -y];
            }
        }

        public void setRibbonSelection(int x, int y)
        {
            if (gridModel[x, -y] is RibbonLabel)
            {
                RibbonLabel label = gridModel[x, -y] as RibbonLabel;
                ribbonSelection = label;
            }
        }

        public void removeSelection()
        {
            selection = null;
        }

        public bool selectionSet()
        {
            return selection != null;
        }

        public void deleteSelection()
        {
            if (selection != null)
            {
                if (selection is SeamstressObject){}
                else if (selection is GridStoreObject)
                {
                    GridStoreObject temp = selection as GridStoreObject;
                    if (temp.Type == "ground")
                        removeGround(temp.body.Position.X, temp.body.Position.Y);
                    else if(temp.Type == "miasma")
                        removeMiasma(temp.body.Position.X, temp.body.Position.Y);
                    selection = null;
                    temp.body.Dispose();
                }
                else if (selection is RibbonLabel)
                {
                    RibbonLabel temp = selection as RibbonLabel;
                    if (ribbons != null)
                    {
                        removeRibbonBody = true;
                        removeRibbonID = temp.RibbonID;
                    }
                    if (ribbonStores != null)
                    {
                        RibbonPointStoreObject rps = ribbonStores.ElementAt(temp.RibbonID);
                        rps.PathStart.body.Dispose();
                        objects.Remove(rps.PathStart);
                        int counter2 = 0;
                        foreach (RibbonPointObject point in rps.PathPoints)
                {
                            counter2++;
                            point.body.Dispose();
                            objects.Remove(point);
                        }
                        //Console.WriteLine("rps: " + counter2);
                        rps.PathEnd.body.Dispose();
                        objects.Remove(rps.PathEnd);
                        rps.RibbonStart.body.Dispose();
                        objects.Remove(rps.RibbonStart);
                        rps.RibbonEnd.body.Dispose();
                        objects.Remove(rps.RibbonEnd);
                        ribbonStores.RemoveAt(temp.RibbonID);
                    }
                    gridModel[(int)(temp.body.Position.X - 0.5f), -(int)(temp.body.Position.Y + 0.5f)] = null;
                    selection = null;
                    removeRibbonPoints(temp.RibbonID);
                    for (int i = 0; i < gridModel.GetLength(0); i++)
                    {
                        for (int j = 0; j < gridModel.GetLength(1); j++)
                        {
                            if (gridModel[i, j] is RibbonElementStore)
                            {
                                RibbonElementStore res = gridModel[i, j] as RibbonElementStore;
                                if (res.RibbonID == temp.RibbonID)
                                {
                                    gridModel[i, j] = null;
                                    res.body.Dispose();
                                    objects.Remove(res);
                                }
                            }
                        }
                        objects.Remove(temp);
                        collisionController = new CollisionController(world, seamstress, objects);
                        temp.body.Dispose();
                    }
                }
                else if (selection is RibbonElementStore)
                {
                    RibbonElementStore temp = selection as RibbonElementStore;
                    if (ribbons != null)
                    {
                        if (ribbons.ElementAt(temp.RibbonID) != null){
                            if(ribbons.ElementAt(temp.RibbonID).elements != null)
                        {
                            foreach (RibbonElement ele in ribbons.ElementAt(temp.RibbonID).elements)
                            {
                                if (ele != null)
                                {
                                    if (ele.pos1 == temp.RibbonPos && ele.Flipped == temp.RibbonFlipped)
                {
                                        ribbons.ElementAt(temp.RibbonID).elements.Remove(ele);
                                        gridModel[(int)(temp.body.Position.X), -(int)(temp.body.Position.Y)] = null;
                                        selection = null;
                                        ele.Body.Dispose();
                                        break;
                                    }
                                }
                            }
                        }
                        }
                        objects.Remove(temp);
                        collisionController = new CollisionController(world, seamstress, objects);
                        temp.body.Dispose();
                    }
                }
                else {
                    PhysicalObject temp = selection as PhysicalObject;
                    gridModel[(int)(temp.body.Position.X), -(int)(temp.body.Position.Y)] = null;
                    selection = null;
                    objects.Remove(temp);
                    collisionController = new CollisionController(world, seamstress, objects);
                    temp.body.Dispose();
                }

                }
            int counter = 0;
            foreach (Body body in world.BodyList)
            {
                counter++;
            }
            //Console.WriteLine("num bodies: " + counter);
        }

        public void moveSelection(int x, int y)
        {
            if (!isSquareOccupied(x, y))
            {
                if (selection is SeamstressObject)
                {
                    SeamstressObject temp = selection as SeamstressObject;
                    gridModel[(int)temp.body.Position.X, -(int)temp.body.Position.Y] = null;
                    temp.body.Position = new Vector2(x + 0.5f, y - 0.5f);
                    selection = temp;
                    gridModel[x,-y] = temp;
                    //Console.WriteLine(temp.body.Position);      
                }
                else if (selection is GridStoreObject)
                {
                    GridStoreObject temp = selection as GridStoreObject;
                    if (temp.Type == "ground")
                    {
                        gridModel[(int)temp.body.Position.X, -(int)temp.body.Position.Y] = null;
                        addGround(x, y,1,1);
                        GenerateGround();
                        selection = gridModel[(int)(x + 0.5f), -(int)(y - 0.5f)];
                    }
                    else if (temp.Type == "miasma")
                    {
                        gridModel[(int)temp.body.Position.X, -(int)temp.body.Position.Y] = null;
                        addMiasma(x, y,1,1);
                        GenerateMiasma();
                        selection = gridModel[(int)(x + 0.5f), -(int)(y - 0.5f)];
                    }
                }
                else
                {
                    PhysicalObject temp = selection as PhysicalObject;
                    gridModel[(int)temp.body.Position.X, -(int)temp.body.Position.Y] = null;
                    temp.body.Position = new Vector2(x + 0.5f, y - 0.5f);
                    selection = temp;
                    gridModel[x, -y] = temp;
                    //Console.WriteLine(temp.body.Position);
                }
            }
            else
            {
                setSelection(x, y, true, true, true);
            }
        }

        public void resizeGridModel()
        {
            Console.WriteLine("resize called");
            Engine.GameObject[,] temp = new Engine.GameObject[(int)GridWidth, (int)GridHeight];
            RibbonElementPoint[,] tempEle = new RibbonElementPoint[(int)GridWidth, (int)GridHeight];
            foreach (RibbonElementPoint ele in elePointModel)
            {
                if (ele != null)
                {
                    Console.WriteLine(ele.body.Position);
                    tempEle[(int)ele.body.Position.X, -(int)ele.body.Position.Y] = ele;
                }
            }
            if (ribbons != null)
            {
                for(int i = 0; i < ribbons.Count; i++)
                {
                    updateRibbonPointModel(i);
                }
            }

            /*List<RibbonPointStoreObject> rpsStore = new List<RibbonPointStoreObject>();
            if (ribbonStores != null)
            {
                foreach (RibbonPointStoreObject rps in ribbonStores)
                {
                    rpsStore.Add(rps);
                }
                ribbonStores.Clear();
            }

            if (rpsStore != null)
            {
                foreach (RibbonPointStoreObject rps in rpsStore)
                {
                    Vector2 pathStart = rps.PathStart.Point;
                    Vector2 pathEnd = rps.PathEnd.Point;
                    List<RibbonPointObject> pathPoints = rps.PathPoints;
                    Vector2 ribbonStart = rps.RibbonStart.Point;
                    Vector2 ribbonEnd = rps.RibbonEnd.Point;
                    addRibbonPoint("pathStart", pathStart.X, pathStart.Y);
                    foreach (RibbonPointObject pathPoint in pathPoints)
                    {
                        Console.WriteLine(pathPoint.Point);
                        addRibbonPoint("pathPoint", pathPoint.Point.X, pathPoint.Point.Y);
                    }
                    addRibbonPoint("pathEnd", pathEnd.X, pathEnd.Y);
                    addRibbonToPath();
                    createRibbon();
                }
            }*/
            

            foreach (Engine.GameObject o in gridModel)
            {
                if (o is SeamstressObject)
                {
                    SeamstressObject temp2 = o as SeamstressObject;
                    if (temp2 != null && temp2.body.Position.X <= GridWidth && -temp2.body.Position.Y <= GridHeight)
                    {
                        temp[(int)temp2.body.Position.X, -(int)temp2.body.Position.Y] = temp2;
                    }
                }
                else
                {
                    PhysicalObject temp2 = o as PhysicalObject;
                    if (temp2 != null && temp2.body.Position.X <= GridWidth && -temp2.body.Position.Y <= GridHeight)
                    {
                        temp[(int)temp2.body.Position.X, -(int)temp2.body.Position.Y] = temp2;
                    }
                }
                if (selection is SeamstressObject)
                {
                    SeamstressObject temp2 = selection as SeamstressObject;
                    if (selection == null || temp2.body.Position.X > GridWidth || -temp2.body.Position.Y > GridHeight)
                    {
                        selection = null;
                    }
                }
                else
                {
                    PhysicalObject temp2 = selection as PhysicalObject;
                    if (selection == null || temp2.body.Position.X > GridWidth || -temp2.body.Position.Y > GridHeight)
                    {
                        selection = null;
                    }
                }
            }
            gridModel = temp;
            elePointModel = tempEle;
        }

        private void DrawGrid(float width, float height, int numSquareParts)
        {
            int cubeSize = 1;
            float step = 1 / (float)numSquareParts;

            for (float i = step; i <= width; i = i + step)
            {
                float position = cubeSize * i;
                if (i % 1 == 0)
                {
                    Canvas.DrawLine(Color.DimGray, 2f, new Vector2(position, 0), new Vector2(position, -cubeSize * height), false);

                }
                else
                {
                    //Canvas.DrawLine(Color.DarkGray, 2f, new Vector2(position, 0), new Vector2(position, -cubeSize * height), false);
                }
                    if (i == step)
                {
                    Canvas.DrawLine(Color.DimGray, 2f, new Vector2(0, 0), new Vector2(0, -cubeSize * height), false);
                }
            }

            for (float i = step; i <= height; i = i + step)
            {
                float position = cubeSize * i;
                if (i % 1 == 0)
                {
                    Canvas.DrawLine(Color.DimGray, 2f, new Vector2(0, -position), new Vector2(cubeSize * width, -position), false);

                }
                else
                {
                    //Canvas.DrawLine(Color.DarkGray, 2f, new Vector2(0, -position), new Vector2(cubeSize * width, -position), false);
                
                }
                if (i == step)
                {
                    Canvas.DrawLine(Color.DimGray, 2f, new Vector2(0, 0), new Vector2(cubeSize * width,0), false);
                }
            }
        }

        public void screenZoom(int scrollStatus, Vector2 scrollPoint)
        {
            float zoomScale = 1.5f;
            if (scrollStatus == 1 && Canvas.Camera.Scale * zoomScale <= calculateScale(1, 1))
            {
                Canvas.Camera.Scale *= zoomScale;
                if (GridWidth % 2 == 1)
                {
                    Canvas.Camera.Position = new Vector2(scrollPoint.X + 0.5f, scrollPoint.Y);
                }
                else
                {
                    Canvas.Camera.Position = scrollPoint;
                }
                
                scrollCounter++;
                scrollStore *= zoomScale;

            }
            if (scrollStatus == 2 && Canvas.Camera.Scale * 1 / zoomScale >= calculateScale(GridWidth, GridHeight))
            {
                Canvas.Camera.Scale *= 1 / zoomScale;
                scrollCounter--;

                                if (GridWidth % 2 == 1)
                {
                    Canvas.Camera.Position = new Vector2(scrollPoint.X + 0.5f, scrollPoint.Y);
                }
                else
                {
                    Canvas.Camera.Position = scrollPoint;
                }
                scrollStore *= 1 / zoomScale;
                
            }
            if (scrollStatus == 2 && Canvas.Camera.Scale * 1 / zoomScale <= calculateScale(GridWidth,GridHeight))
            {
                Canvas.Camera.Position = new Vector2(GridWidth / 2f, GridHeight / 2f - GridHeight);
                scrollCounter = 0;
            }
            //Console.WriteLine(scrollStore);
        }

        public float calculateZoomScale()
        {
            float output = 1f;
            float zoomScale = 1.5f;
            if (scrollCounter > 0)
            {
                for (int i = 0; i < scrollCounter; i++)
                {
                    output = output * zoomScale;
                }
            }
            else if (scrollCounter < 0)
            {
                for (int i = 0; i > scrollCounter; i--)
                {
                    output = output / zoomScale;
                }
            }
            else
            {
                output = 1f;
            }
            return output;
        }

        private float calculateScale(float width, float height)
        {
            float scaledGridX;
            float scaledGridY;

            float cubeSize = 64;
            float aspectRatio = 16f / 9f;
            Vector2 defaultScreenSize = new Vector2(1280, 720);
            
            float gridX = width * cubeSize;
            float gridY = height * cubeSize;

            if (gridX / gridY > aspectRatio)
            {
                scaledGridX = gridX + .5f * cubeSize;
                scaledGridY = gridX / defaultScreenSize.X * defaultScreenSize.Y + .5f * cubeSize;
                return defaultScreenSize.X / scaledGridX;
            }
            else
            {
                scaledGridY = gridY + .5f * cubeSize;
                //Console.WriteLine(scaledGridY);
                scaledGridX = gridY / defaultScreenSize.Y * defaultScreenSize.X + .5f * cubeSize;
                return defaultScreenSize.Y / scaledGridY;
            }
            //Console.WriteLine(defaultScreenSize.X / scaledGridX + " " + defaultScreenSize.Y / scaledGridY);
        }
        public void checkSeamPos(int x, int y)
        {
            if (seamstressPos == new Point(x, y))
            {
                seamstressPos = new Point(-1, -1);
            }
        }
        public void addSeamstress(float x, float y)
        {
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            seamstress = new SeamstressObject(new SeamstressSprites(Canvas), new SeamstressSounds(AudioPlayer), world);
            seamstress.body.Position = new Vector2((x + .5f), (y - .5f));
            seamstress.IsGrounded = true;
            seamstress.FramesGrounded = 1000;
            seamstress.body.Enabled = false;
            if (seamstressPos != null)
            {
                for (int i = 0; i < gridModel.GetLength(0); i++)
                {
                    for (int j = 0; j < gridModel.GetLength(1); j++)
                    {
                        if (gridModel[i, j] is SeamstressObject)
            {
                            gridModel[i, j] = null;
                        }
                    }
                }
            }
            gridModel[(int)x, -(int)y] = seamstress;
            seamstressPos = new Point((int)x, -(int)y);
            collisionController = new CollisionController(world, seamstress, objects);
        }

        public void addRibbonObject(string type, PhysicalObject box, float x, float y, bool flipped)
        {            
            
            int ribbonID = elePointModel[(int)x, -(int)y].RibbonID;
            Vector2 position = elePointModel[(int)x, -(int)y].Position;
            RibbonObject holder = ribbons.ElementAt(ribbonID);
            Vector2 orientation = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y)));
            float rPos;
            if (orientation.X == 1 && orientation.Y == 0)
            {
                //Console.WriteLine("bottom");
                //Console.WriteLine(holder.PointToPosition(position));
                if(flipped){
                    Vector2 orientation2 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y + 1)));
                    if (orientation2 != orientation)
                    {
                        ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)) + 2, 0.0f, flipped);
                        rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X + 1, position.Y)) + 1;
                        gridModel[(int)x, -(int)y] = new RibbonElementStore(type, ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                    }
                    else
                    {
                        ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)), 0.0f, flipped);
                        rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X + 1, position.Y));
                        gridModel[(int)x, -(int)y] = new RibbonElementStore(type, ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                    }
                }
                    
                else{
                    Vector2 orientation2 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X - 1, position.Y)));
                    Vector2 orientation3 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y + 1)));
                    if (orientation2 != orientation && orientation3 != orientation)
                    {
                        //Console.WriteLine("entered");
                        ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)), 0.0f, !flipped);
                        rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X + 1, position.Y - 1));
                        gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, !flipped);
                    }
                    else
                    {
                        ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)), 0.0f, flipped);
                        rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y));
                        gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                    }
                }
                    
            }
            else if (orientation.X == 0 && orientation.Y == -1)
            {
                //Console.WriteLine("right");
                //Console.WriteLine(holder.PointToPosition(position));
                Vector2 orientation2 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y + 1)));
                if (orientation2 != orientation)
                {
                    if (flipped)
                    {
                        Vector2 orientation3 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y)) - 1);
                        if (orientation3 != orientation)
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)), 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                        else
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) + 2, 0f, !flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)) - 1;
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, !flipped);
                        }
                    }
                    else
                    {
                        Vector2 orientation3 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y)) - 1);
                        Vector2 orientation4 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X - 2, position.Y)));
                        if (orientation3 != orientation && orientation4 == orientation)
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)), 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X - 1, position.Y + 1));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                        else
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)), 0f, !flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, !flipped);
                        }
                    }
                }
                else
                {
                    if (flipped){

                        Vector2 orientation3 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X + 1, position.Y + 1)));
                        Vector2 orientation4 = holder.PositionToOrientation(holder.PointToPosition(position) + 1);
                        if (orientation3 != orientation && orientation4 != orientation)
        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)), 0f, !flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, !flipped);
                        }
                        else
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)), 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                    }
                    else{
                        ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)), 0f, flipped);
                        rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)); 
                        gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                    }       
                }
            }
            else if (orientation.X == 0 && orientation.Y == 1)
            {
                //Console.WriteLine("left");
                //Console.WriteLine(holder.PointToPosition(position));
                if (flipped){
                    Vector2 orientation2 = holder.PositionToOrientation(holder.PointToPosition(position) + 1);
                    if (orientation2 != orientation)
                    {
                        ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)), 0f, flipped);
                        rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) + 1;
                        gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                    }
                    else
                    {
                        Vector2 orientation3 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X - 1, position.Y)));
                        if (orientation3 != orientation)
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)), 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)) + 1;
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                        else
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)), 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                    }
                }
                else
                {
                     Vector2 orientation2 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X + 1, position.Y)));
                     Vector2 orientation3 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y)) + 2);
                     if (orientation2 != orientation && orientation3 != orientation)
                     {
                         ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)) + 2, 0f, flipped);
                         rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)) + 2;
                         gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                     }
                     else
                     {
                         ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y)), 0f, flipped);
                         rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y));
                         gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                     }
                }
                    
            }
            else
            {
                //Console.WriteLine("top");
                //Console.WriteLine(holder.PointToPosition(position));
                Vector2 orientation2 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X + 1, position.Y)));
                if (orientation2 != orientation)
                {
                    
                    Vector2 orientation3 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y + 2)));
                    if (orientation3 != orientation)
                    {
                        if (flipped){
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) - 1, 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                        else
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)), 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                    }
                    else
                    {
                        ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) - 1, 0f, flipped);
                        if (flipped){
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X - 1, position.Y + 1)) - 1;
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                        else
                        {
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) - 1;
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                    }
                }
                else
                {
                    
                    Vector2 orientation3 = holder.PositionToOrientation(holder.PointToPosition(new Vector2(position.X, position.Y)) + 1);
                    if (orientation3 != orientation)
                    {
                        if (flipped)
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) - 1, 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X - 1, position.Y + 1));
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                        else
                        {
                            ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) - 1, 0f, flipped);
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) - 1;
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                    }
                    else
                    {
                        ribbons.ElementAt(ribbonID).AddElement(box, ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) - 1, 0f, flipped);
                        if (flipped)
                        {
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X - 1, position.Y + 1)) - 1;
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                        else
                        {
                            rPos = ribbons.ElementAt(ribbonID).PointToPosition(new Vector2(position.X, position.Y + 1)) - 1;
                            gridModel[(int)x, -(int)y] = new RibbonElementStore(type,ribbonID, Sprite.Build(Canvas, "1x1plat"), world, new Vector2(position.X + 0.5f, position.Y + 0.5f), rPos, flipped);
                        }
                    }
                    
                }
            }
            collisionController = new CollisionController(world, seamstress, objects);
        }

        public void addBox(float x, float y)
        {
            BoxObject box = new BoxObject(Sprite.Build(Canvas, "1x1plat"), world, new Vector2(0,0),0,null,new Vector2(0,0));
            if (box != null && elePointModel[(int)x,-(int)y] != null)
            {
                addRibbonObject("box", box, x, y, elePointModel[(int)x, -(int)y].FlipStatus);
            }
        }

        public void addPlatform(float x, float y)
        {
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            PhysicalObject platform = new BoxObject(Sprite.Build(Canvas, "1x1plat2"), world,new Vector2(0,0),0.0f,null,new Vector2(0,0));
            platform.body.Position = new Vector2((x + .5f), (y - .5f));
            checkSeamPos((int)x, -(int)y);
            gridModel[(int)x, -(int)y] = platform;
            objects.Add(platform);
            collisionController = new CollisionController(world, seamstress, objects);
        }

        public void addHook(float x, float y)
        {
            SpriteCollection hookSprites = new SpriteCollection(Canvas);
            hookSprites.Add("hookbody");
            hookSprites.Add("hooktop");
            HookObject hook = new HookObject(hookSprites, world, new Vector2(x + .5f, y - .5f), 0);
            addRibbonObject("hook",hook, x, y, elePointModel[(int)x, -(int)y].FlipStatus);
        }

        public void addRibbonShooter(float x, float y, int cooldown)
        {
            ShooterBox shooter = new ShooterBox(Sprite.Build(Canvas, "miamonster2"), Sprite.Build(Canvas, "bullet"), world, new Vector2((x + .5f), (y - .5f)), 0f, cooldown);
            addRibbonObject("shooter", shooter, x, y, elePointModel[(int)x, -(int)y].FlipStatus);
        }

        public void addTelescoping(float x, float y, int height)
        {
            SpriteCollection teleSprites = new SpriteCollection(Canvas);
            teleSprites.Add("telescopeblock1x1");
            teleSprites.Add("telescopeinside");
            TelescopicBox telescope = new TelescopicBox(teleSprites, world, new Vector2((x + .5f), (y - .5f)), 0, height);
            addRibbonObject("telescoping", telescope, x, y, elePointModel[(int)x, -(int)y].FlipStatus);
        }

        public void addFlipBar(float x, float y)
        {
            Vector2 point = new Vector2(x + .5f, y - .5f);
            //FlipBarObject flip = new FlipBarObject(world,ribbons.ElementAt(elePointModel[(int),-(int)y].RibbonID),ribbons.ElementAt(elePointModel[(int)point.X,(int)point.Y].RibbonID).PointToPosition(new Vector2(point.X,point.Y)), Sprite.Build(Canvas, "flipbar"), 0);
            //addRibbonObject("flipbar", flip, x, y, elePointModel[(int)x, -(int)y].FlipStatus);
        }

        public void addGround(float x, float y, int width, int height) 
        {
            List<Vector2> groundGrid = new List<Vector2>();
            bool intersect = false;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if ((int)x + i < GridWidth && (int)x + i >= 0 && -(int)y - j < GridHeight && -(int)y - j >= 0)
                    {
                        if (gridModel[(int)x + i, -(int)y - j] == null)
                        {
                            groundGrid.Add(new Vector2((int)x + i, -(int)y - j));
                        }
                        else
                        {
                            intersect = true;
                            break;
                        }
                    }
                    else
                    {
                        intersect = true;
                        break;
                    }
                }
            }
            if (!intersect)
            {
                foreach(Vector2 pos in groundGrid)
                {
                    GridStoreObject gridStore = new GridStoreObject(Canvas, world, new Vector2(pos.X + 0.5f, -pos.Y - 0.5f), "ground", 1);
                    gridStore.Width = width;
                    gridStore.Height = height;
                    gridStore.body.Enabled = false;
                    checkSeamPos((int)pos.X, (int)pos.Y);
                    gridModel[(int)pos.X, (int)pos.Y] = gridStore;
                }
            }
        }

        public void GenerateGround()
        {
            List<Rectangle> groundRects = new List<Rectangle>();
            foreach (Object o in gridModel)
            {
                if (o is GridStoreObject)
                {
                    GridStoreObject curr = o as GridStoreObject;
                    if (curr != null && curr.Type == "ground")
                    {
                        Vector2 pos = curr.body.Position;
                        groundRects.Add(new Rectangle((int)(pos.X - 0.5f), (int)(pos.Y - 0.5f), 1, 1));
                    }
                }
            }
            if (ground != null)
            {
                ground.body.Dispose();
            }
            ground = new GroundObject(Canvas, actAssets, groundRects, world);
            collisionController = new CollisionController(world, seamstress, objects);
        }

        public void removeGround(float x, float y)
        {
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            if (gridModel[(int)x, -(int)y] is GridStoreObject)
            {
                GridStoreObject curr = gridModel[(int)x, -(int)y] as GridStoreObject;
                if (curr.Type == "ground")
                {
                    curr.body.Dispose();
                    gridModel[(int)x, -(int)y] = null;

                    List<Rectangle> groundRects = new List<Rectangle>();
                    foreach (Object o in gridModel)
                    {
                        if (o is GridStoreObject)
                        {
                            GridStoreObject curr2 = o as GridStoreObject;
                            if (curr2 != null && curr2.Type == "ground")
                            {
                                Vector2 pos = curr2.body.Position;
                                groundRects.Add(new Rectangle((int)(pos.X - 0.5f), (int)(pos.Y - 0.5f), 1, 1));
                            }
                        }
                    }
                    if (ground != null)
                    {
                        ground.body.Dispose();
                    }
                    ground = new GroundObject(Canvas, actAssets, groundRects, world);
                    collisionController = new CollisionController(world, seamstress, objects);
                }
            }
        }

        public void addMiasma(float x, float y, int width, int height)
        {
            List<Vector2> miasmaGrid = new List<Vector2>();
            bool intersect = false;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if ((int)x + i < GridWidth && (int)x + i >= 0 && -(int)y - j < GridHeight && -(int)y - j >= 0)
                    {
                        if (gridModel[(int)x + i, -(int)y - j] == null)
                        {
                            miasmaGrid.Add(new Vector2((int)x + i, -(int)y - j));
                        }
                        else
                        {
                            intersect = true;
                            break;
                        }
                    }
                    else
                    {
                        intersect = true;
                        break;
                    }
                }
            }
            if (!intersect)
            {
                foreach (Vector2 pos in miasmaGrid)
        {
                    GridStoreObject gridStore = new GridStoreObject(Canvas, world, new Vector2(pos.X + 0.5f, -pos.Y - 0.5f), "miasma", 1);
                    gridStore.Width = width;
                    gridStore.Height = height;
            gridStore.body.Enabled = false;
                    checkSeamPos((int)pos.X, (int)pos.Y);
                    gridModel[(int)pos.X, (int)pos.Y] = gridStore;
                }

            }
        }

        public void GenerateMiasma()
        {
            List<Rectangle> miasmaRects = new List<Rectangle>();
            foreach (Object o in gridModel)
            {
                if (o is GridStoreObject)
                {
                    GridStoreObject curr = o as GridStoreObject;
                    if (curr != null && curr.Type == "miasma")
                    {
                        Vector2 pos = curr.body.Position;
                        miasmaRects.Add(new Rectangle((int)(pos.X - 0.5f), (int)(pos.Y - 0.5f), 1, 1));
                    }
                }
            }
            if (miasma != null)
            {
                miasma.body.Dispose();
            }
            miasma = new MiasmaObject(Canvas, actAssets, miasmaRects, world);
            collisionController = new CollisionController(world, seamstress, objects);
        }

        public void removeMiasma(float x, float y)
        {
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            if (gridModel[(int)x, -(int)y] is GridStoreObject)
            {
                GridStoreObject curr = gridModel[(int)x, -(int)y] as GridStoreObject;
                if (curr.Type == "miasma")
                {
                    curr.body.Dispose();
                    gridModel[(int)x, -(int)y] = null;

                    List<Rectangle> miasmaRects = new List<Rectangle>();
                    foreach (Object o in gridModel)
                    {
                        if (o is GridStoreObject)
                        {
                            GridStoreObject curr2 = o as GridStoreObject;
                            if (curr2 != null && curr2.Type == "miasma")
                            {
                                Vector2 pos = curr2.body.Position;
                                miasmaRects.Add(new Rectangle((int)(pos.X - 0.5f), (int)(pos.Y - 0.5f), 1, 1));
                            }
                        }
                    }
                    if (miasma != null)
                    {
                        miasma.body.Dispose();
                    }
                    miasma = new MiasmaObject(Canvas, actAssets, miasmaRects, world);
            collisionController = new CollisionController(world, seamstress, objects);
                }
            }
        }

        public void addShooter(float x, float y, float rotation, int cooldown)
        {
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            ShooterBox shooter = new ShooterBox(Sprite.Build(Canvas, "miamonster1"), Sprite.Build(Canvas, "bullet"), world, new Vector2((x + .5f), (y - .5f)), rotation, cooldown);
            checkSeamPos((int)x, -(int)y);
            gridModel[(int)x, -(int)y] = shooter;
            objects.Add(shooter);
            collisionController = new CollisionController(world, seamstress, objects);
        }
        public void addRotating(float x, float y, float rotation, Vector2 dimensions)
        {
            /*Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            SpriteCollection sc = new SpriteCollection(Canvas);
            sc.Add("rotationblock");
            sc.Add("rotationcircle");
            RotatingObject rotate = new RotatingObject(sc, world, new Vector2((x + .5f), (y - .5f)), rotation, LevelGenerator.generateLayout(dimensions), new Vector2(0, 0));
            checkSeamPos((int)x, -(int)y);
            gridModel[(int)x, -(int)y] = rotate;
            objects.Add(rotate);
            collisionController = new CollisionController(world, seamstress, objects);*/
        }

        public void addSpikes(float x, float y, float rotation, Vector2 dimensions)
        {
            /*Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            SpikeBox spike = new SpikeBox(Sprite.Build(Canvas, "spikes"), world, new Vector2((x + .5f), (y - .5f)), rotation,LevelGenerator.generateLayout(dimensions), new Vector2(0,0.5f), 1.0f);
            checkSeamPos((int)x, -(int)y);
            gridModel[(int)x, -(int)y] = spike;
            objects.Add(spike);
            collisionController = new CollisionController(world, seamstress, objects);*/
        }

        public void addSave(float x, float y, bool endFlag, int worldNum, int levelNum)
        {
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            SaveRockObject save = new SaveRockObject(Sprite.Build(Canvas, "saverock_rock"), Sprite.Build(Canvas, "saverock_bow"), world, new Vector2((x +.5f),(y-.5f)), 0, endFlag, new Storage.LevelInfo(worldNum, levelNum));
            save.body.Position = new Vector2((x + .5f), (y - .5f));
            checkSeamPos((int)x, -(int)y);
            gridModel[(int)x, -(int)y] = save;
            objects.Add(save);
            collisionController = new CollisionController(world, seamstress, objects);
        }

        public void addRibbonPoint(string pointType, float x, float y)
        {
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            Sprite sprite;
            if (pointType == "pathStart")
            {
                sprite= Sprite.Build(Canvas,"circle_green");
            }
            else if (pointType == "pathPoint")
            {
                sprite = Sprite.Build(Canvas,"circle_yellow");
            }
            else if (pointType == "pathEnd")
            {
                sprite = Sprite.Build(Canvas,"circle_red");
            }
            else if (pointType == "ribbonStart")
            {
                sprite = Sprite.Build(Canvas,"circle_blue");
            }
            else
            {
                sprite = Sprite.Build(Canvas, "circle_orange");
            }
            RibbonPointObject point = new RibbonPointObject(pointType, sprite, world, new Vector2(x,y));
            point.body.Enabled = false;
            if (buildingRibbon == null)
            {
                buildingRibbon = new RibbonPointStoreObject();
            }
            buildingRibbon.addPoint(point);
            if (buildingRibbon.RecreateRibbon && seamstress != null)
            {
                if (ribbon == null)
                {
                    ribbon = new RibbonObject(Canvas, world, seamstress, 0, buildingRibbon.createPath(), 1f, 1f, ribbonLoop);     
                }
                ribbon.RemoveBodies();
                ribbon = new RibbonObject(Canvas, world, seamstress, 0, buildingRibbon.createPath(), ribbon.PointToPosition(buildingRibbon.RibbonStart.body.Position), ribbon.PointToPosition(buildingRibbon.RibbonEnd.body.Position), ribbonLoop);
                buildingRibbon.RecreateRibbon = false;
            }
        }

        public void addRibbonToPath()
        {
            addRibbonPoint("ribbonStart", buildingRibbon.PathStart.body.Position.X, buildingRibbon.PathStart.body.Position.Y);
            addRibbonPoint("ribbonEnd", buildingRibbon.PathEnd.body.Position.X, buildingRibbon.PathEnd.body.Position.Y);

            buildingRibbon.RibbonStart.body.Position = movePointAlongRibbon(buildingRibbon.RibbonStart.body.Position, 1f);
            buildingRibbon.RibbonEnd.body.Position = movePointAlongRibbon(buildingRibbon.RibbonEnd.body.Position, -1f);

            buildingRibbon.RibbonStart.Point = buildingRibbon.RibbonStart.body.Position;
            buildingRibbon.RibbonEnd.Point = buildingRibbon.RibbonEnd.body.Position;
            if(ribbon != null)
                ribbon.RemoveBodies();
            ribbon = new RibbonObject(Canvas, world, seamstress, 0, buildingRibbon.createPath(), 1f, 1f, ribbonLoop);
            ribbon.RemoveBodies();
            ribbon = new RibbonObject(Canvas,world, seamstress, 0, buildingRibbon.createPath(), ribbon.PointToPosition(buildingRibbon.RibbonStart.body.Position), ribbon.PointToPosition(buildingRibbon.RibbonEnd.body.Position), ribbonLoop);  
        }

        public void moveRibbonStart(float pos){
            if (ribbon != null)
            {
                Vector2 startPosVector = ribbon.PositionToPoint(pos);
                addRibbonPoint("ribbonStart", buildingRibbon.PathStart.body.Position.X, buildingRibbon.PathStart.body.Position.Y);
                buildingRibbon.RibbonStart.body.Position = movePointAlongRibbon(buildingRibbon.RibbonStart.body.Position, pos);
                buildingRibbon.RibbonStart.Point = buildingRibbon.RibbonStart.body.Position;
                ribbon.RemoveBodies();
                ribbon = new RibbonObject(Canvas, world, seamstress, 0, buildingRibbon.createPath(), ribbon.PointToPosition(buildingRibbon.RibbonStart.body.Position), ribbon.PointToPosition(buildingRibbon.RibbonEnd.body.Position), ribbonLoop);
            }
        }

        public void moveRibbonEnd(float pos)
        {
            if (ribbon != null)
            {
                Vector2 endPosVector = ribbon.PositionToPoint(pos);
                addRibbonPoint("ribbonEnd", buildingRibbon.PathStart.body.Position.X, buildingRibbon.PathStart.body.Position.Y);
                buildingRibbon.RibbonEnd.body.Position = movePointAlongRibbon(buildingRibbon.RibbonEnd.body.Position, pos);
                buildingRibbon.RibbonEnd.Point = buildingRibbon.RibbonEnd.body.Position;
                ribbon.RemoveBodies();
                ribbon = new RibbonObject(Canvas,world, seamstress, 0, buildingRibbon.createPath(), ribbon.PointToPosition(buildingRibbon.RibbonStart.body.Position), ribbon.PointToPosition(buildingRibbon.RibbonEnd.body.Position), ribbonLoop);
            }
        }

        public void createRibbon()
        {
            Vector2 position = ribbon.PositionToPoint(lengthOfPath(buildingRibbon.RibbonStart.Point, buildingRibbon.RibbonEnd.Point) / 2);
            position = new Vector2((float)Math.Round((double)position.X), (float)Math.Round((double)position.Y));
            ribbons.Add(ribbon);
            ribbon = null;
            ribbonStores.Add(buildingRibbon);
            RibbonLabel ribbonLabel = new RibbonLabel(ribbons.Count, Sprite.Build(Canvas, "circle_grey"), world, new Vector2(position.X + 0.5f, position.Y - 0.5f));
            ribbonLabel.body.Enabled = false;
            gridModel[(int)position.X, -(int)position.Y] = ribbonLabel;
            buildingRibbon = null;
            updateRibbonPointModel(ribbons.Count - 1);
        }

        public bool labelSelected()
        {
            return selection is RibbonLabel;
        }

        public bool ribbonObjectSelected()
        {
            return selection is BoxObject || selection is MiasmaObject || selection is HookObject || selection is TelescopicBox || selection is FlipBarObject || selection is ShooterBox;
        }

        public bool ribbonSelected()
        {
            return ribbonSelection != null;
        }
        
        public void selectRibbon()
        {
            RibbonLabel label = selection as RibbonLabel;
            //Console.WriteLine(label.RibbonID);
            buildingRibbon = ribbonStores.ElementAt(label.RibbonID);
            ribbonStores.RemoveAt(label.RibbonID);
            ribbons.RemoveAt(label.RibbonID);
            gridModel[(int)(label.body.Position.X - 0.5f), -(int)(label.body.Position.Y + 0.5f)] = null;
            buildingRibbon.RibbonEnd = null;
            buildingRibbon.RibbonStart = null;
            selection = null;
            
        }

        public void updateRibbonPointModel(int ribbonID)
        {
            List<Vector2> ribbonPoints = ribbons.ElementAt(ribbonID).PossibleRibbonBlockPoints();
            RibbonObject ribbon = ribbons.ElementAt(ribbonID);
            foreach (Vector2 point in ribbonPoints)
            {
                Vector2 orientation = ribbon.PositionToOrientation(ribbon.PointToPosition(point));
                RibbonElementPoint ele;
                if (orientation.X == 1 && orientation.Y == 0)
                {
                    Vector2 orientation2 = ribbon.PositionToOrientation(ribbon.PointToPosition(point));
                    if (orientation2 == orientation)
                    {
                        if (point == ribbon.PositionToPoint(ribbon.PointToPosition(point)))
                            ele = new RibbonElementPoint(Sprite.Build(Canvas, "1x1plat"), world, new Vector2(point.X + 0.5f, point.Y - 0.5f), false, ribbon.PointToPosition(point), ribbonID);
                        else
                            ele = new RibbonElementPoint(Sprite.Build(Canvas, "1x1plat"), world, new Vector2(point.X + 0.5f, point.Y - 0.5f), true, ribbon.PointToPosition(point), ribbonID);
                    }
                    else
                        ele = new RibbonElementPoint(Sprite.Build(Canvas, "1x1plat"), world, new Vector2(point.X + 0.5f, point.Y - 0.5f), false, ribbon.PointToPosition(point), ribbonID);
                }
                else if (orientation.X == 0 && orientation.Y == 1)
                {
                    if (point == ribbon.PositionToPoint(ribbon.PointToPosition(point)))
                        ele = new RibbonElementPoint(Sprite.Build(Canvas, "1x1plat"), world, new Vector2(point.X + 0.5f, point.Y - 0.5f), false, ribbon.PointToPosition(point), ribbonID);
                    else
                        ele = new RibbonElementPoint(Sprite.Build(Canvas, "1x1plat"), world, new Vector2(point.X + 0.5f, point.Y - 0.5f), true, ribbon.PointToPosition(point), ribbonID);
                }
                else
                {
                    if (point == ribbon.PositionToPoint(ribbon.PointToPosition(point)))
                        ele = new RibbonElementPoint(Sprite.Build(Canvas, "1x1plat"), world, new Vector2(point.X + 0.5f, point.Y - 0.5f), true, ribbon.PointToPosition(point), ribbonID);
                    else
                        ele = new RibbonElementPoint(Sprite.Build(Canvas, "1x1plat"), world, new Vector2(point.X + 0.5f, point.Y - 0.5f), false, ribbon.PointToPosition(point), ribbonID);
                }
                //Console.WriteLine(-(int)point.Y);
                if (point.X >= 0 && -point.Y >= 0 && point.X < GridWidth && -point.Y < GridHeight)
                    elePointModel[(int)point.X, -(int)point.Y] = ele;
            }
        }

        public bool checkIfRibbonPoint(int x, int y)
        {
            return elePointModel[x, -y] != null;
        }

        public void removeRibbonPoints(int ribbonID)
        {
            for (int i = 0; i < elePointModel.GetLength(0); i++)
            {
                for (int k = 0; k < elePointModel.GetLength(1); k++)
                {
                    if (elePointModel[i,k] != null && elePointModel[i, k].RibbonID == ribbonID)
                    {
                        elePointModel[i, k] = null;
                    }
                }
            }
        }

        public Vector2 movePointAlongRibbon(Vector2 point, float step)
        {
            float position = ribbon.PointToPosition(point);
            position = position + step;
            return ribbon.PositionToPoint(position);
        }

        public float lengthOfPath(Vector2 p1, Vector2 p2)
        {
            if (ribbon != null)
            {
                float r1 = ribbon.PointToPosition(p1);
                float r2 = ribbon.PointToPosition(p2);
                if (ribbonLoop)
                {
                    float x = (p2.X - p1.X) * (p2.X - p1.X);
                    float y = (p2.Y - p1.Y) * (p2.Y - p1.Y);
                    float result = (float)Math.Sqrt(x + y);
                    return r2 - r1 + result;
                }
                else
                {
                    return r2 - r1;
                }

            }
            else
            {
                return -1f;
            }
        }

        public Vector2 PanelToScreenCoords(int x, int y, int width, int height)
        {
            //Console.WriteLine("panelRun");
            int cubeSize = 64;
            float scaledCubeSize = cubeSize * Canvas.Camera.Scale;

            Vector2 gridCenter = new Vector2(GridWidth / 2f, GridHeight / 2f - GridHeight);
            float xTransform = gridCenter.X - Canvas.Camera.Position.X;
            float yTransform = gridCenter.Y - Canvas.Camera.Position.Y;

            Vector2 panelSize = new Vector2(width, height);
            Vector2 gameSize = Canvas.Camera.Dimensions;
            Vector2 gridSize = new Vector2(GridWidth * cubeSize, GridHeight * cubeSize) * Canvas.Camera.Scale;

            float leftGridPaddingX = ((gameSize.X - gridSize.X) / 2) + xTransform * scaledCubeSize;
            float leftGridPaddingY = (gameSize.Y - gridSize.Y) / 2 - yTransform * scaledCubeSize;


            //Console.WriteLine("transforms: " + xTransform );
            //Console.WriteLine("padding" + (gameSize.X - gridSize.X) / 2);
            //Console.WriteLine("transformedPadding: " + leftGridPaddingX);

            float gameScaledX = x / panelSize.X * gameSize.X + xTransform * scaledCubeSize;
            float gameScaledY = y / panelSize.Y * gameSize.Y - yTransform * scaledCubeSize;

            float gridScaledX = x / panelSize.X * gridSize.X;
            float gridScaledY = x / panelSize.Y * gridSize.Y;

            float cubeStart = 0 + xTransform * scaledCubeSize;
            float cubeEnd = scaledCubeSize + xTransform * scaledCubeSize;

                double? cubeX = null;
                int cubeXCounter = 0;
                for (int i = 0; i < GridWidth; i++)
                {
                    if (gameScaledX - leftGridPaddingX > cubeStart && gameScaledX - leftGridPaddingX < cubeEnd)
                    {
                        cubeX = cubeXCounter;
                        break;
                    }
                    cubeXCounter++;
                    cubeStart = cubeEnd;
                    cubeEnd = cubeEnd + scaledCubeSize;
                }

                double? cubeY = null;
                cubeStart = 0 + yTransform * scaledCubeSize;
                cubeEnd = scaledCubeSize + yTransform * scaledCubeSize;
                float cubeYCounter = 0;
                for (int i = 0; i < GridHeight; i++)
                {
                    if (gameScaledY - leftGridPaddingY > cubeStart && gameScaledY - leftGridPaddingY < cubeEnd)
                    {
                        cubeY = -(GridHeight - 1 - cubeYCounter);
                        break;
                    }
                    cubeYCounter = cubeYCounter + 1f;
                    cubeStart = cubeEnd;
                    cubeEnd = cubeEnd + scaledCubeSize;
                }
                //Console.WriteLine(cubeX + " " + cubeY);



                if(cubeX == null || cubeY == null){
                    //Console.WriteLine("null\n");
                    return new Vector2(-10000,10000);
                }else{
                    Vector2 output = new Vector2((float)cubeX.Value, (float)cubeY.Value);
                    //Console.WriteLine("output: " + output + "\n");

                        return output;
                    
                }
        }

        private void calculatePadding(){
            Vector2 position = Canvas.Camera.Position;
        }
    }
}
