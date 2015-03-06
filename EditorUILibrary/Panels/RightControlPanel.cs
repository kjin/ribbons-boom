using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorUILibrary.Controls;
using System.Drawing;

namespace EditorUILibrary.Panels
{
    public class RightControlPanel : SwitchablePanel
    {
        int DEFAULT_GRID_WIDTH = 20;
        int DEFAULT_GRID_HEIGHT = 10;

        BuildRightControl rightControl;
        int worldNum;
        int levelNum;
        int gridWidth;
        int gridHeight;
        bool gridChanged;
        bool editPressed;
        bool anchorPressed;
        Microsoft.Xna.Framework.Rectangle clientSize;
        GameWindowPanel gameWindowPanel;
        Control mainWindowControl;

        public int WorldNum { get { return worldNum; } set { worldNum = value; } }
        public int LevelNum { get { return levelNum; } set { levelNum = value; } }
        public bool AnchorPressed { get { return anchorPressed; } set { anchorPressed = value; } }
        public bool EditPressed { get { return editPressed; } set { editPressed = value; } }
        public int GridWidth
        {
            get { return gridWidth; }
            set { gridWidth = value; }
        }

        public int GridHeight { 
            get { return gridHeight; }
            set { gridHeight = value; }
        }

        public bool GridChanged
        {
            get { return gridChanged; }
            set { gridChanged = value; }
        }

        public void setGridWidth(int width)
        {
            rightControl.setGridWidth(width);
        }

        public void setGridHeight(int height)
        {
            rightControl.setGridHeight(height);
        }

        public RightControlPanel(Control mainWindowControl, GameWindowPanel gameWindowPanel)
            :base()
        {
            this.mainWindowControl = mainWindowControl;
            this.gameWindowPanel = gameWindowPanel;
            gridWidth = DEFAULT_GRID_WIDTH;
            gridHeight = DEFAULT_GRID_HEIGHT;
            gridChanged = true;
            anchorPressed = false;
            worldNum = 0;
            levelNum = 0;

        }

        public void Initialize(Microsoft.Xna.Framework.Rectangle clientSize)
        {
            this.clientSize = clientSize;
            this.Width = (int)((float)clientSize.Width * .24f - 3);
            this.Height = gameWindowPanel.Height;
            this.Location = new System.Drawing.Point(gameWindowPanel.Right + (int)((float)clientSize.Width * .004f), gameWindowPanel.Top);
            this.BackColor = Color.DimGray;
            this.BorderStyle = BorderStyle.None;
        }

        public override void SetContextControl(int context)
        {
            this.Controls.Remove(rightControl);

            if (context > 0 && context < 5)
            {
                if (context == 1)
                {
                    rightControl = new BuildRightControl(mainWindowControl, clientSize);
                }
                else if (context == 2)
                {

                }
                else if (context == 3)
                {

                }
                else if (context == 4)
                {
                }
            }

            this.Controls.Add(rightControl);
        }

        public void UpdateTitleText(string c)
        {
            rightControl.UpdateTitleText(c);
        }
    }
}
