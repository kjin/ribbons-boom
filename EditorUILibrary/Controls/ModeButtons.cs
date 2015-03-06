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
    public partial class ModeButtons : UserControl
    {
        Control mainWindowControl;
        public ModeButtons(Control mainWindowControl)
        {
            this.mainWindowControl = mainWindowControl;
            InitializeComponent();
        }

        private void ModeButtons_Load(object sender, EventArgs e)
        {
            this.Width = this.Parent.Width;
            this.Height = this.Parent.Height;

            scaleFont(testLabel);
            scaleFont(testCaption);
            scaleFont(test);
            scaleFont(visualLabel);

            panel1.Location = new Point(0, 0);
            panel1.Height = panel1.Parent.Height;
            panel1.Width = (int)(panel1.Parent.Width  * 0.686f);

            visualLabel.Width = visualLabel.Parent.Width + 2;
            visualLabel.Height = (int)(visualLabel.Font.Size * 2.25f) + 2;
            visualLabel.Location = new Point(-1, -1);

            buttonFormatPanel.Width = (int)(buttonFormatPanel.Parent.Width * 0.3f);
            buttonFormatPanel.Height = buttonFormatPanel.Parent.Height;
            buttonFormatPanel.Location = new Point(panel1.Right + (int)(buttonFormatPanel.Parent.Width * 0.014f), 0);

            testLabel.Width = testLabel.Parent.Width + 2;
            testLabel.Height = (int)(testLabel.Font.Size * 2.25f) + 2;
            testLabel.Location = new Point(-1, -1);

            test.Width = test.Parent.Width - 9;
            test.Height = (int)(test.Parent.Height * 0.4f);
            test.Location = new Point(4, test.Parent.Bottom - test.Height - 5);

            testCaption.Location = new Point((int)(test.Parent.Width * 0.048f), testLabel.Bottom + (int)(testCaption.Parent.Height * 0.1f));
            testCaption.Width = testCaption.Parent.Width - (int)(test.Parent.Width * 0.1f);
            testCaption.Height = (int)(testCaption.Parent.Height * 0.2f);

            ModePanel panel = this.Parent as ModePanel;
            panel.ContextSwitch = 1;
        }

        private void Check(CheckBox button)
        {
            button.BackColor = Color.IndianRed;
            button.Checked = true;
        }

        private void Uncheck(CheckBox button)
        {
            button.Checked = false;
            button.BackColor = Color.Gainsboro;
        }

        private void test_Click(object sender, EventArgs e)
        {
           ModePanel panel = this.Parent as ModePanel;
           panel.ContextSwitch = 4;
        }

        private void scaleFont(Control c)
        {
            float oldSize = c.Font.Size;
            float newSize = oldSize / 1440f * Screen.GetWorkingArea(mainWindowControl).Height;
            c.Font = new Font("Arial", newSize);
        }
    }
}
