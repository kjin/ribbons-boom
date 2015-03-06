using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EditorUILibrary.Controls;
using EditorUILibrary.Panels;
using EditorUILibrary;
using System.Drawing;

namespace EditorUILibrary.UI
{
    public class UIModel
    {
        Control mainWindowControl;
        BackgroundPanel backgroundPanel;
        GameWindowPanel gameWindowPanel;
        RightControlPanel rightControlPanel;
        BottomControlPanel bottomControlPanel;
        MenuBarPanel menuBarPanel;
        ModePanel modePanel;
        int scrollStatus;
        Point scrollPosition;
        Microsoft.Xna.Framework.GameWindow window;

        public Control MainWindowControl { get { return mainWindowControl; } }

        public GameWindowPanel GameWindowPanel { get { return gameWindowPanel; } }

        public RightControlPanel RightControlPanel { get { return rightControlPanel; } }
       
        public BottomControlPanel BottomControlPanel { get { return bottomControlPanel; } }
        
        public MenuBarPanel MenuBarPanel { get { return menuBarPanel; } }
        
        public ModePanel ModePanel { get { return modePanel; } }

        public int ContextSwitch
        {
            get { return modePanel.ContextSwitch; }
            set { modePanel.ContextSwitch = value; }
        }

        public UIModel(Control mainWindowControl, Microsoft.Xna.Framework.GameWindow window)
        {
            this.window = window;
            Form MyGameForm = (Form)Form.FromHandle(window.Handle);
            MyGameForm.FormBorderStyle = FormBorderStyle.None;
            this.mainWindowControl = mainWindowControl;
            gameWindowPanel = new GameWindowPanel(mainWindowControl);
            scrollStatus = 0;
            
        }

        public void Initialize()
        {
            mainWindowControl.BackColor = System.Drawing.Color.DarkGray;

            backgroundPanel = new BackgroundPanel(mainWindowControl);
            rightControlPanel = new RightControlPanel(mainWindowControl, gameWindowPanel);
            bottomControlPanel = new BottomControlPanel(mainWindowControl, gameWindowPanel, rightControlPanel);
            modePanel = new ModePanel(mainWindowControl, gameWindowPanel, rightControlPanel, bottomControlPanel);
            menuBarPanel = new MenuBarPanel(mainWindowControl, gameWindowPanel, rightControlPanel, bottomControlPanel, modePanel);

            mainWindowControl.Controls.Add(backgroundPanel);
            mainWindowControl.Controls.Add(gameWindowPanel);
            mainWindowControl.Controls.Add(rightControlPanel);
            mainWindowControl.Controls.Add(bottomControlPanel);
            mainWindowControl.Controls.Add(modePanel);
            mainWindowControl.Controls.Add(menuBarPanel);

            menuBarPanel.AddControl();
            modePanel.AddControl();
        }

        public void LoadContent()
        {
            mainWindowControl.Width = Screen.GetWorkingArea(mainWindowControl).Width;
            mainWindowControl.Height = Screen.GetWorkingArea(mainWindowControl).Height;
            mainWindowControl.Location = new System.Drawing.Point(0, 0);
            menuBarPanel.Initialize(window.ClientBounds);
            gameWindowPanel.Initialize(window.ClientBounds);
            rightControlPanel.Initialize(window.ClientBounds);
            bottomControlPanel.Initialize(window.ClientBounds);
            modePanel.Initialize(window.ClientBounds);
            backgroundPanel.SendToBack();
        }

        public void fullscreen()
        {
            gameWindowPanel.fullscreen();
        }

        public void smallscreen()
        {
            gameWindowPanel.smallscreen();
        }
    }
}
