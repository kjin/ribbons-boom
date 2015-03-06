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
    public partial class BuildRightControl : UserControl
    {
        Control selected;
        Control mainWindowControl;
        Microsoft.Xna.Framework.Rectangle clientSize;

        public BuildRightControl(Control mainWindowControl, Microsoft.Xna.Framework.Rectangle clientSize)
        {
            InitializeComponent();
            this.mainWindowControl = mainWindowControl;
            this.clientSize = clientSize;
        }

        public void setGridWidth(int width)
        {
            widthBox.Value = width;
        }

        public void setGridHeight(int height)
        {
            heightBox.Value = height;
        }
        private void widthBox_ValueChanged(object sender, EventArgs e)
        {
            RightControlPanel panel = this.Parent as RightControlPanel;
            panel.GridWidth = (int)widthBox.Value;
            panel.GridChanged = true;
        }

        private void heightBox_ValueChanged(object sender, EventArgs e)
        {
            RightControlPanel panel = this.Parent as RightControlPanel;
            panel.GridHeight = (int)heightBox.Value;
            panel.GridChanged = true;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            RightControlPanel panel = this.Parent as RightControlPanel;
            panel.EditPressed = true;
        }

        private void anchorButton_Click(object sender, EventArgs e)
        {
            RightControlPanel panel = this.Parent as RightControlPanel;
            panel.AnchorPressed = true;
        }

        private void BuildRightControl_Load(object sender, EventArgs e)
        {
            properties.Font = new Font("Arial", 11);
            scaleFont(properties);
            scaleFont(levelSettingsLabel);
            scaleFont(levelNameTag);
            scaleFont(textBox1);
            scaleFont(levelNumTag);
            scaleFont(numericUpDown1);
            scaleFont(worldNumTag);
            scaleFont(numericUpDown2);
            scaleFont(widthLabel);
            scaleFont(widthBox);
            scaleFont(heightLabel);
            scaleFont(heightBox);
            scaleFont(propertiesLabel);
            scaleFont(selectedLabel);

            this.BackColor = Color.DimGray;
            this.BorderStyle = BorderStyle.None;
            this.Size = this.Parent.Size;

            levelDataPanel.Width = levelDataPanel.Parent.Width;
            levelDataPanel.Height = (int)(levelDataPanel.Parent.Height * 0.17f);
            levelDataPanel.Location = new Point(0, 0);
            
            levelSettingsLabel.Height = (int)(levelSettingsLabel.Font.Size * 2.25f + 2);
            levelSettingsLabel.Width = levelSettingsLabel.Parent.Width + 2;
            levelSettingsLabel.Location = new Point(-1,-1);

            levelNameTag.Width = (int)(levelNameTag.Parent.Width * 0.20f);
            levelNameTag.Height = (int)(levelNameTag.Font.Size * 2f);
            levelNameTag.Location = new Point((int)(levelNameTag.Parent.Width * 0.045f), (int)(levelSettingsLabel.Bottom + levelNameTag.Parent.Height * 0.1f));
            levelNameTag.TextAlign = ContentAlignment.MiddleLeft;

            textBox1.Width = (int)(textBox1.Parent.Width * 0.65f);
            textBox1.Height = levelNameTag.Height;
            textBox1.Location = new Point((int)(levelNameTag.Right + textBox1.Parent.Width * 0.05f), levelNameTag.Top);

            levelNumTag.Width = (int)(levelNumTag.Parent.Width * 0.23f);
            levelNumTag.Height = (int)(levelNumTag.Font.Size * 2f);
            levelNumTag.Location = new Point((int)(levelNumTag.Parent.Width * 0.11f), (int)(levelNameTag.Bottom + levelNumTag.Parent.Height * 0.1f));
            levelNumTag.TextAlign = ContentAlignment.MiddleLeft;

            numericUpDown1.Width = (int)(numericUpDown1.Parent.Width * 0.1f);
            numericUpDown1.Height = levelNumTag.Height;
            numericUpDown1.Location = new Point((int)(levelNumTag.Right + numericUpDown1.Parent.Width * 0.02f), levelNumTag.Top);

            worldNumTag.Width = (int)(worldNumTag.Parent.Width * 0.23f);
            worldNumTag.Height = (int)(worldNumTag.Font.Size * 2f);
            worldNumTag.Location = new Point((int)(worldNumTag.Parent.Width * 0.11f), (int)(levelNumTag.Bottom + worldNumTag.Parent.Height * 0.1f));
            worldNumTag.TextAlign = ContentAlignment.MiddleLeft;

            numericUpDown2.Width = (int)(numericUpDown2.Parent.Width * 0.1f);
            numericUpDown2.Height = worldNumTag.Height;
            numericUpDown2.Location = new Point((int)(worldNumTag.Right + numericUpDown2.Parent.Width * 0.02f), worldNumTag.Top);

            widthLabel.Width = (int)(widthLabel.Parent.Width * 0.2f);
            widthLabel.Height = (int)(widthLabel.Font.Size * 2f);
            widthLabel.Location = new Point((int)(numericUpDown2.Right + widthLabel.Parent.Width * 0.11f), (int)(levelNumTag.Top));
            widthLabel.TextAlign = ContentAlignment.MiddleLeft;

            widthBox.Width = (int)(widthBox.Parent.Width * 0.12f);
            widthBox.Height = widthLabel.Height;
            widthBox.Location = new Point((int)(widthLabel.Right + widthBox.Parent.Width * 0.02f), widthLabel.Top);

            heightLabel.Width = (int)(heightLabel.Parent.Width * 0.2f);
            heightLabel.Height = (int)(heightLabel.Font.Size * 2f);
            heightLabel.Location = new Point((int)(numericUpDown2.Right + heightLabel.Parent.Width * 0.11f), (int)(worldNumTag.Top));
            heightLabel.TextAlign = ContentAlignment.MiddleLeft;

            heightBox.Width = (int)(heightBox.Parent.Width * 0.12f);
            heightBox.Height = widthLabel.Height;
            heightBox.Location = new Point((int)(widthLabel.Right + heightBox.Parent.Width * 0.02f), heightLabel.Top);

            selectorPanel.Width = selectorPanel.Parent.Width;
            selectorPanel.Height = (int)(selectorPanel.Parent.Height * 0.5f);
            selectorPanel.Location = new Point(0, (int)(levelDataPanel.Bottom + levelDataPanel.Parent.Height * .013f));

            selectedLabel.Width = selectedLabel.Parent.Width + 2;
            selectedLabel.Height = (int)(selectedLabel.Font.Size * 2.25f);
            selectedLabel.Location = new Point(-1, -1);

            objectPanel.Width = objectPanel.Parent.Width - 8;
            objectPanel.Height = objectPanel.Parent.Height - 8 - selectedLabel.Height;
            objectPanel.Location = new Point(4, selectedLabel.Bottom + 4);

            propertiesPanel.Location = new Point(levelDataPanel.Left, (int)(selectorPanel.Bottom + levelDataPanel.Parent.Height * .013f));
            propertiesPanel.Width = levelDataPanel.Width;
            propertiesPanel.Height = (int)(levelDataPanel.Parent.Height * 0.303f + 1);

            propertiesLabel.BackColor = Color.Gainsboro;
            propertiesLabel.Height = (int)(propertiesLabel.Font.Size * 2.25f) + 2;
            propertiesLabel.Width = propertiesLabel.Parent.Width + 3;
            propertiesLabel.Location = new Point((int)(propertiesLabel.Parent.Width / 2f - propertiesLabel.Width / 2f) - 1, -1);
            properties.Width = (int)(properties.Parent.Width - 8);
            properties.Height = (int)(properties.Parent.Height -5 - propertiesLabel.Bottom - (int)(properties.Parent.Height * 0.01f));
            properties.Location = new Point(4,propertiesLabel.Bottom + 4);
            properties.Rows.Add(1);

            properties.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            ((TextBox)numericUpDown1.Controls[1]).HideSelection = false;

            DataGridViewRow r = properties.Rows[0];
            r.Cells["prop"].Value = "test";
            r.Cells["Value"].Value = "value";
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Select();
        }

        public void UpdateTitleText(string s){
            Control active = this.ActiveControl;
            if (active is TextBox){
                TextBox temp = active as TextBox;
                temp.AppendText(s);
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space && this.ActiveControl == textBox1)
                textBox1.AppendText(" ");
        }

        private void scaleFont(Control c)
        {
            float oldSize = c.Font.Size;
            float newSize = oldSize / 1440f * Screen.GetWorkingArea(mainWindowControl).Height;
            c.Font = new Font("Arial", newSize);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            RightControlPanel temp = this.Parent as RightControlPanel;
            temp.WorldNum = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            RightControlPanel temp = this.Parent as RightControlPanel;
            temp.LevelNum = (int)numericUpDown1.Value;
        }
    }
}
