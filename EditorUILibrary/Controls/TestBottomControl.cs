using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EditorUILibrary.Controls
{
    public partial class TestBottomControl : UserControl
    {
        public TestBottomControl()
        {
            InitializeComponent();
        }

        private void TestBottomControl_Load(object sender, EventArgs e)
        {
            this.Width = this.Parent.Width;
            this.Height = this.Parent.Height;
        }
    }
}
