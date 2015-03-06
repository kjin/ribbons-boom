using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using EditorUILibrary.Controls;

namespace EditorUILibrary.Panels
{
    public class ModePanel : Panel
    {
        int contextSwitch;
        Control modeControl;
        Microsoft.Xna.Framework.Rectangle clientSize;
        RightControlPanel rightControlPanel;
        BottomControlPanel bottomControlPanel;
        Control mainWindowControl;

        public ModePanel(Control mainWindowControl, GameWindowPanel gameWindowPanel, RightControlPanel rightControlPanel, BottomControlPanel bottomControlPanel)
            :base()
        {
            this.rightControlPanel = rightControlPanel;
            this.bottomControlPanel = bottomControlPanel;
            contextSwitch = 0;
            this.mainWindowControl = mainWindowControl;
        }

        public void Initialize(Microsoft.Xna.Framework.Rectangle clientSize)
        {
            this.clientSize = clientSize;
            this.Width = clientSize.Width - bottomControlPanel.Right - (int)(this.Parent.Width * 0.008f);
            this.Height = bottomControlPanel.Height;
            this.Location = new System.Drawing.Point(bottomControlPanel.Right + (int)(this.Parent.Width * 0.004f), bottomControlPanel.Top);
        }

        public int ContextSwitch
        {
            get
            {
                return contextSwitch;
            }
            set
            {
                contextSwitch = value;
            }
        }

        public void AddControl()
        {
            modeControl = new ModeButtons(mainWindowControl);
            this.Controls.Add(modeControl);
        }
    }
}
