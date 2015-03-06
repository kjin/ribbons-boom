using EditorUILibrary.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XMLContent;

namespace EditorUILibrary.Panels
{
    public class MenuBarPanel : Panel
    {
        MenuBar menuControl;
        Microsoft.Xna.Framework.Rectangle clientSize;
        Control mainWindowControl;
        String savePath;
        bool generateLevel;
        bool loadLevel;

        public bool LoadLevel { get { return loadLevel; } set { loadLevel = value; } }
        public bool GenerateLevel { get { return generateLevel; } set { generateLevel = value;  } }

        public String SavePath { get { return savePath; } set { savePath = value; } }

        public MenuBarPanel(Control mainWindowControl, GameWindowPanel gameWindowPanel, RightControlPanel rightControlPanel, BottomControlPanel bottomControlPanel, ModePanel modePanel)
            :base()
        {
            this.mainWindowControl = mainWindowControl;
        }

        public void Initialize(Microsoft.Xna.Framework.Rectangle clientSize)
        {
            this.clientSize = clientSize;
            this.Width = clientSize.Width;
            this.Height = (int)(clientSize.Height * 0.03f);
            this.Location = new System.Drawing.Point(0, 0);
            this.BackColor = SystemColors.Control;
        }


        public void AddControl(){
            menuControl = new MenuBar(mainWindowControl);
            this.Controls.Add(menuControl);
        }
    }
}
