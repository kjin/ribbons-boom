using EditorUILibrary.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EditorUILibrary.Panels
{
    public class BottomControlPanel : SwitchablePanel
    {
        BuildBottomControl bottomControl;
        string objectSelected;
        int buildPanelContext;
        bool clearSelection;
        public Control mainWindowControl;
        GameWindowPanel gameWindowPanel;
        bool addRibbonPoints;
        float startScrollValue;
        float endScrollValue;
        bool ribbonCancel;
        bool backButton;
        bool createButton;
        bool ribbonLooped;
        bool terminateButton;
        bool loopButton;
        bool anchorPressed;
        bool onlyRibbon;
        Microsoft.Xna.Framework.Rectangle clientSize;

        public bool OnlyRibbon { get { return onlyRibbon; } set { onlyRibbon = value; } }
        public bool AnchorPressed { get { return anchorPressed; } set { anchorPressed = value; } }
        public bool TerminateButton { get { return terminateButton; } set { terminateButton = value; } }
        public bool LoopButton { get { return loopButton; } set { loopButton = value; } }
        public bool RibbonLooped { get { return ribbonLooped; } set { ribbonLooped = value; } }
        public bool CreateButton { get { return createButton; } set { createButton = value; } }
        public bool BackButton { get { return backButton; } set { backButton = value; } }
        public bool RibbonCancel { get { return ribbonCancel; } set { ribbonCancel = value; } }
        public bool ClearSelection { get { return clearSelection; } set { clearSelection = value; } }
        public bool AddRibbonPoints { get { return addRibbonPoints; } set { addRibbonPoints = value; } }
        public string ObjectSelected { get { return objectSelected; } set { objectSelected = value; } }
        public int BuildPanelContext { get { return buildPanelContext; } set { buildPanelContext = value; } }

        public float StartScrollValue { get { return startScrollValue; } set { startScrollValue = value; } }
        public float EndScrollValue { get { return endScrollValue; } set { endScrollValue = value; } }
        
 
        public int getObjectWidth(){
            return bottomControl.getObjectWidth();
        }

        public int getObjectHeight()
        {
            return bottomControl.getObjectHeight();
        }

        public bool getFlipped()
        {
            return bottomControl.getFlipped();
        }

        public bool getCheckpointStatus()
        {
            return bottomControl.getCheckpointStatus();
        }
        public BottomControlPanel(Control mainWindowControl, GameWindowPanel gameWindowPanel, RightControlPanel rightControlPanel)
            :base()
        {
            this.mainWindowControl = mainWindowControl;
            this.gameWindowPanel = gameWindowPanel;
            objectSelected = "";
            addRibbonPoints = false;
            startScrollValue = 1;
            endScrollValue = 10;
            ribbonCancel = false;
            backButton = false;
            createButton = false;
            ribbonLooped = true;
            onlyRibbon = false;
        }

        public void Initialize(Microsoft.Xna.Framework.Rectangle clientSize)
        {
            this.Width = (int)(this.Parent.Width * 0.004f + this.Parent.Width * .66f);
            this.Height = clientSize.Height - gameWindowPanel.Bottom - (int)(clientSize.Height * 0.02f);
            this.Location = new System.Drawing.Point((int)(clientSize.Width * 0.004f), gameWindowPanel.Bottom + (int)(clientSize.Height * 0.01f));
            this.BackColor = System.Drawing.Color.DimGray;
            this.BorderStyle = BorderStyle.None;
        }

        public void moveToRibbon(bool move)
        {
            BuildBottomControl curr = bottomControl as BuildBottomControl;
            curr.MoveToRibbon = move;
        }

        public override void SetContextControl(int context)
        {
            this.Controls.Remove(bottomControl);

            if (context > 0 && context < 5)
            {
                if (context == 1)
                {
                    bottomControl = new BuildBottomControl(mainWindowControl, clientSize);
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

            this.Controls.Add(bottomControl);
        }

        public void setMaxRibbonPath(int max)
        {
            if (bottomControl is BuildBottomControl)
            {
                BuildBottomControl curr = bottomControl as BuildBottomControl;
                curr.setMaxRibbonPath(max);
            }
        }

        public void setEndPos(int pos)
        {
            if (bottomControl is BuildBottomControl)
            {
                BuildBottomControl curr = bottomControl as BuildBottomControl;
                curr.setEndPos(pos);
            }
        }
        public void showRibbonEdit()
        {
            if (bottomControl is BuildBottomControl)
            {
                BuildBottomControl curr = bottomControl as BuildBottomControl;
                curr.showRibbonEdit();
            }
        }

        public void UpdateTitleText(string c, int gridWidth, int gridHeight)
        {
            bottomControl.UpdateTitleText(c, gridWidth, gridHeight);
        }

        public void PopulateDataGrid(string selection)
        {
            bottomControl.PopulateDataGrid(selection);
        }
    }
}
