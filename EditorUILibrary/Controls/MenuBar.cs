using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorUILibrary.Panels;

namespace EditorUILibrary.Controls
{
    public partial class MenuBar : UserControl
    {
        Control mainWindowControl;
        public MenuBar(Control mainWindowControl)
        {
            InitializeComponent();
            this.mainWindowControl = mainWindowControl;
        }

        private void MenuBar_Load(object sender, EventArgs e)
        {
            scaleFont(quit);
            scaleFont(menuStrip);
            scaleFont(fileToolStripMenuItem);
            scaleFont(minimize);
            this.Width = this.Parent.Width;
            this.Height = this.Parent.Height;

            fileToolStripMenuItem.Width = (int)(menuStrip.Width * 0.025f);
            fileToolStripMenuItem.Height = menuStrip.Height;

            menuStrip.Width = this.Width;
            menuStrip.Height = this.Height;

            quit.Height = this.Height + 2;
            quit.Width = (int)(this.Height * 1.25f) + 2;
            quit.Location = new Point(this.Width - quit.Width +1, -1);
            quit.BackColor = Color.Gainsboro;
            quit.BorderStyle = BorderStyle.FixedSingle;
            minimize.Height = this.Height + 2;
            minimize.Width = (int)(this.Height * 1.25f + 2);
            minimize.Location = new Point(quit.Left - minimize.Width + 1, -1);
            minimize.BackColor = Color.Gainsboro;
            minimize.BorderStyle = BorderStyle.FixedSingle;
        }

        private void scaleFont(Control c)
        {
            float oldSize = c.Font.Size;
            float newSize = oldSize / 1440f * Screen.GetWorkingArea(mainWindowControl).Height;
            c.Font = new Font("Arial", newSize);
        }

        private void scaleFont(ToolStripMenuItem c)
        {
            float oldSize = c.Font.Size;
            float newSize = oldSize / 1440f * Screen.GetWorkingArea(mainWindowControl).Height;
            c.Font = new Font("Arial", newSize);
        }

        private void quit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void quit_MouseHover(object sender, EventArgs e)
        {
            quit.BackColor = SystemColors.ControlDark;
        }

        private void quit_MouseLeave(object sender, EventArgs e)
        {
            quit.BackColor = Color.Gainsboro;
        }

        private void minimize_Click(object sender, EventArgs e)
        {          
           mainWindowControl.FindForm().WindowState = FormWindowState.Minimized;
        }

        private void minimize_MouseHover(object sender, EventArgs e)
        {
            minimize.BackColor = SystemColors.ControlDark;
        }

        private void minimize_MouseLeave(object sender, EventArgs e)
        {
            minimize.BackColor = Color.Gainsboro;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            if (save.ShowDialog() == DialogResult.OK)
            {
                MenuBarPanel temp = this.Parent as MenuBarPanel;
                temp.SavePath = save.FileName;
                temp.GenerateLevel = true;
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                MenuBarPanel temp = this.Parent as MenuBarPanel;
                temp.SavePath = open.FileName;
                temp.LoadLevel = true;
            }
        }
    }
}
