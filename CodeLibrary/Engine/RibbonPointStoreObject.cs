using CodeLibrary.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Engine
{
    public class RibbonPointStoreObject
    {
        RibbonPointObject pathStartPoint;
        RibbonPointObject pathEndPoint;
        List<RibbonPointObject> pathPoints;
        RibbonPointObject ribbonStartPoint;
        RibbonPointObject ribbonEndPoint;

        bool recreateRibbon;
        bool moveToRibbon;

        public RibbonPointObject PathStart { get { return pathStartPoint; } set { pathStartPoint = value; } }
        public RibbonPointObject PathEnd { get { return pathEndPoint; } set { pathEndPoint = value; } }
        public List<RibbonPointObject> PathPoints { get { return pathPoints; } set { pathPoints = value; } }
        public RibbonPointObject RibbonStart { get { return ribbonStartPoint; } set { ribbonStartPoint = value; } }
        public RibbonPointObject RibbonEnd { get { return ribbonEndPoint; } set { ribbonEndPoint = value; } }

        public bool RecreateRibbon { get { return recreateRibbon; } set { recreateRibbon = value; } }
        public bool MoveToRibbon { get { return moveToRibbon; } set { moveToRibbon = value; } }
        public RibbonPointStoreObject()
        {
            recreateRibbon = false;
            pathPoints = new List<RibbonPointObject>();
        }

        public void RemoveBodies()
        {
            if (pathStartPoint != null)
            pathStartPoint.body.Dispose();
            if (pathEndPoint != null)
            pathEndPoint.body.Dispose();
            if (ribbonStartPoint != null)
            ribbonStartPoint.body.Dispose();
            if (ribbonEndPoint != null)
            ribbonEndPoint.body.Dispose();
            foreach (RibbonPointObject rp in pathPoints)
            {
                if (rp != null)
                rp.body.Dispose();
            }
        }

        public void addPoint(RibbonPointObject point)
        {
            if (point.PointType == "pathStart"){
                pathStartPoint = point;
            }
            else if (point.PointType == "pathEnd"){
                pathEndPoint = point;
            }
            else if (point.PointType == "ribbonStart"){
                ribbonStartPoint = point;
            }
            else if (point.PointType == "ribbonEnd"){
                ribbonEndPoint = point;
            }
            else{
                pathPoints.Add(point);
            }
            recreateRibbon = checkIfPopulated();
            moveToRibbon = checkIfPathDone();
        }

        public bool checkIfPopulated()
        {
            if (pathStartPoint != null && pathEndPoint != null && ribbonStartPoint != null && ribbonEndPoint != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkIfPathDone()
        {
            return pathStartPoint != null && pathEndPoint != null;
        }

        public List<Vector2> createPath()
        {
            List<Vector2> output = new List<Vector2>();
            if (pathStartPoint != null)
            {
                output.Add(pathStartPoint.Point);
            }   
            foreach(RibbonPointObject point in pathPoints){
                output.Add(point.Point);
            }
            if (pathEndPoint != null)
            {
                output.Add(pathEndPoint.Point);
            }    
            return output;
        }

        public void createRibbon()
        {

        }

        public void Draw(Canvas canvas)
        {
            if (pathStartPoint != null)
            {
                pathStartPoint.Draw(canvas);
            }
            if (pathEndPoint != null)
            {
                pathEndPoint.Draw(canvas);
            }
            foreach (RibbonPointObject point in pathPoints)
            {
                point.Draw(canvas);
            }
            if (ribbonStartPoint != null)
            {
                ribbonStartPoint.Draw(canvas);
            }
            if (ribbonEndPoint != null)
            {
                ribbonEndPoint.Draw(canvas);
            }
        }

        public void DrawRibbonTemp(Canvas canvas, bool ribbonLoop)
        {
            List<Vector2> pathPoints = createPath();
            if (pathPoints.Count > 1)
            {
                for (int i = 1; i <= pathPoints.Count - 1; i++)
                {
                    canvas.DrawLine(Color.CornflowerBlue, 6f, pathPoints[i - 1], pathPoints[i], false);
                    
                }
            }

            if (pathEndPoint != null && pathStartPoint != null && ribbonLoop)
            {
                canvas.DrawLine(Color.CornflowerBlue, 6f, pathEndPoint.Point, pathStartPoint.Point, false);
                    
            }

            /*bool startFound = false;
            bool startPoint = false;
            bool endFound = false;
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                if (!startFound)
                {
                    if (i == 0 && CheckOnPath(, pathPoints[i], ribbonStart))
                    {
                        startFound = true;
                        startPoint = true;
                    }
                    else if (CheckOnPath(pathPoints[i], pathPoints[i + 1], ribbonStart))
                    {
                        startFound = true;
                        startPoint = true;
                    }
                }
                else
                {
                    if (startPoint)
                    {
                        if (!endFound)
                        {
                            if (CheckOnPath(pathStart, pathPoints[i], ribbonEnd))
                            {
                                canvas.DrawLine(Color.Gray, 10, ribbonStart, ribbonEnd, false);
                                endFound = true;
                            }
                            else
                            {
                                canvas.DrawLine(Color.Gray, 10, ribbonStart, pathPoints[i], false);
                                canvas.DrawLine(Color.Gray, 10, pathPoints[i], pathPoints[i + 1], false);
                            }
                            startPoint = false;
                        }


                    }
                    else
                    {
                        if (!endFound)
                        {
                            if (CheckOnPath(pathPoints[i], pathPoints[i + 1], ribbonEnd))
                            {
                                canvas.DrawLine(Color.Gray, 10, pathPoints[i], ribbonEnd, false);
                                endFound = true;
                            }
                            else
                            {
                                canvas.DrawLine(Color.Gray, 10, pathPoints[i], pathPoints[i + 1], false);
                            }

                        }
                    }
                }

            }*/
        }

        private bool CheckOnPath(Vector2 point1, Vector2 point2, Vector2 checkPoint)
        {
            bool checkBool = false;
            if (point1.X == point2.X)
            {
                if (point1.Y > point2.Y)
                {
                    if (checkPoint.X == point1.X)
                    {
                        if (checkPoint.Y > point2.Y && checkPoint.Y < point2.Y)
                        {
                            checkBool = true;
                        }
                    }
                }
                else
                {
                    if (checkPoint.X == point1.X)
                    {
                        if (checkPoint.Y < point2.Y && checkPoint.Y > point1.Y)
                        {
                            checkBool = true;
                        }
                    }

                }
            }
            else
            {
                if (point1.X > point2.X)
                {
                    if (checkPoint.Y == point1.Y)
                    {
                        if (checkPoint.X < point1.X && checkPoint.X > point2.X)
                        {
                            checkBool = true;
                        }
                    }
                }
                else
                {
                    if (checkPoint.Y == point1.Y)
                    {
                        if (checkPoint.X > point1.X && checkPoint.X < point2.X)
                        {
                            checkBool = true;
                        }
                    }
                }

            }
            return checkBool;
        }
    }
}
