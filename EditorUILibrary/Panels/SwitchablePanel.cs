using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EditorUILibrary.Panels
{
    public abstract class SwitchablePanel : Panel
    {
        public SwitchablePanel()
            :base()
        {
            this.BackColor = SystemColors.Control;
            this.BorderStyle = BorderStyle.FixedSingle;
            SetContextControl(1);
        }

        public abstract void SetContextControl(int context);

    }
}
