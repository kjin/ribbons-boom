using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorUILibrary.Panels
{
    public class BackgroundPanel : Panel
    {
    
        public BackgroundPanel(Control mainWindowControl)
            :base()
        {
            this.BackColor = Color.DimGray;
            this.Width = (int)((float)Screen.GetWorkingArea(mainWindowControl).Width);
            this.Height = (int)((float)Screen.GetWorkingArea(mainWindowControl).Height);
            this.Location = new Point(0, 0);
            this.SendToBack();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}
