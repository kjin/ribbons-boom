namespace EditorUILibrary.Controls
{
    partial class BuildRightControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.widthBox = new System.Windows.Forms.NumericUpDown();
            this.heightBox = new System.Windows.Forms.NumericUpDown();
            this.widthLabel = new System.Windows.Forms.Label();
            this.heightLabel = new System.Windows.Forms.Label();
            this.properties = new System.Windows.Forms.DataGridView();
            this.prop = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.propertiesPanel = new System.Windows.Forms.Panel();
            this.propertiesLabel = new System.Windows.Forms.Label();
            this.levelDataPanel = new System.Windows.Forms.Panel();
            this.worldNumTag = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.levelNumTag = new System.Windows.Forms.Label();
            this.levelSettingsLabel = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.levelNameTag = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.selectorPanel = new System.Windows.Forms.Panel();
            this.objectPanel = new System.Windows.Forms.Panel();
            this.selectedLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.widthBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.properties)).BeginInit();
            this.propertiesPanel.SuspendLayout();
            this.levelDataPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.selectorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // widthBox
            // 
            this.widthBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.widthBox.Location = new System.Drawing.Point(473, 101);
            this.widthBox.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.widthBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.widthBox.Name = "widthBox";
            this.widthBox.Size = new System.Drawing.Size(60, 30);
            this.widthBox.TabIndex = 0;
            this.widthBox.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.widthBox.ValueChanged += new System.EventHandler(this.widthBox_ValueChanged);
            // 
            // heightBox
            // 
            this.heightBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.heightBox.Location = new System.Drawing.Point(473, 139);
            this.heightBox.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.heightBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.heightBox.Name = "heightBox";
            this.heightBox.Size = new System.Drawing.Size(60, 30);
            this.heightBox.TabIndex = 1;
            this.heightBox.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            this.heightBox.ValueChanged += new System.EventHandler(this.heightBox_ValueChanged);
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.BackColor = System.Drawing.SystemColors.Control;
            this.widthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.widthLabel.ForeColor = System.Drawing.Color.Black;
            this.widthLabel.Location = new System.Drawing.Point(357, 100);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(110, 25);
            this.widthLabel.TabIndex = 2;
            this.widthLabel.Text = "Grid Width:";
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.heightLabel.ForeColor = System.Drawing.Color.Black;
            this.heightLabel.Location = new System.Drawing.Point(357, 140);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(115, 25);
            this.heightLabel.TabIndex = 3;
            this.heightLabel.Text = "Grid Height:";
            // 
            // properties
            // 
            this.properties.AllowUserToAddRows = false;
            this.properties.AllowUserToDeleteRows = false;
            this.properties.AllowUserToResizeColumns = false;
            this.properties.AllowUserToResizeRows = false;
            this.properties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.properties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.prop,
            this.Value});
            this.properties.Location = new System.Drawing.Point(105, 78);
            this.properties.Name = "properties";
            this.properties.RowTemplate.Height = 24;
            this.properties.Size = new System.Drawing.Size(243, 150);
            this.properties.TabIndex = 6;
            // 
            // prop
            // 
            this.prop.HeaderText = "Property";
            this.prop.Name = "prop";
            this.prop.ReadOnly = true;
            this.prop.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // propertiesPanel
            // 
            this.propertiesPanel.BackColor = System.Drawing.SystemColors.Control;
            this.propertiesPanel.Controls.Add(this.propertiesLabel);
            this.propertiesPanel.Controls.Add(this.properties);
            this.propertiesPanel.Location = new System.Drawing.Point(59, 504);
            this.propertiesPanel.Name = "propertiesPanel";
            this.propertiesPanel.Size = new System.Drawing.Size(452, 313);
            this.propertiesPanel.TabIndex = 7;
            // 
            // propertiesLabel
            // 
            this.propertiesLabel.BackColor = System.Drawing.Color.Gainsboro;
            this.propertiesLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.propertiesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.propertiesLabel.ForeColor = System.Drawing.Color.Black;
            this.propertiesLabel.Location = new System.Drawing.Point(60, 0);
            this.propertiesLabel.Name = "propertiesLabel";
            this.propertiesLabel.Size = new System.Drawing.Size(337, 31);
            this.propertiesLabel.TabIndex = 7;
            this.propertiesLabel.Text = "Selected Object Properties";
            this.propertiesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // levelDataPanel
            // 
            this.levelDataPanel.BackColor = System.Drawing.SystemColors.Control;
            this.levelDataPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.levelDataPanel.Controls.Add(this.heightBox);
            this.levelDataPanel.Controls.Add(this.heightLabel);
            this.levelDataPanel.Controls.Add(this.worldNumTag);
            this.levelDataPanel.Controls.Add(this.numericUpDown2);
            this.levelDataPanel.Controls.Add(this.widthLabel);
            this.levelDataPanel.Controls.Add(this.levelNumTag);
            this.levelDataPanel.Controls.Add(this.levelSettingsLabel);
            this.levelDataPanel.Controls.Add(this.textBox1);
            this.levelDataPanel.Controls.Add(this.widthBox);
            this.levelDataPanel.Controls.Add(this.levelNameTag);
            this.levelDataPanel.Controls.Add(this.numericUpDown1);
            this.levelDataPanel.Location = new System.Drawing.Point(59, 139);
            this.levelDataPanel.Name = "levelDataPanel";
            this.levelDataPanel.Size = new System.Drawing.Size(578, 294);
            this.levelDataPanel.TabIndex = 8;
            // 
            // worldNumTag
            // 
            this.worldNumTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.worldNumTag.ForeColor = System.Drawing.Color.Black;
            this.worldNumTag.Location = new System.Drawing.Point(64, 141);
            this.worldNumTag.Name = "worldNumTag";
            this.worldNumTag.Size = new System.Drawing.Size(144, 25);
            this.worldNumTag.TabIndex = 7;
            this.worldNumTag.Text = "World Number:";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.numericUpDown2.Location = new System.Drawing.Point(236, 141);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(56, 30);
            this.numericUpDown2.TabIndex = 6;
            this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // levelNumTag
            // 
            this.levelNumTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.levelNumTag.ForeColor = System.Drawing.Color.Black;
            this.levelNumTag.Location = new System.Drawing.Point(68, 103);
            this.levelNumTag.Name = "levelNumTag";
            this.levelNumTag.Size = new System.Drawing.Size(139, 25);
            this.levelNumTag.TabIndex = 9;
            this.levelNumTag.Text = "Level Number:";
            // 
            // levelSettingsLabel
            // 
            this.levelSettingsLabel.BackColor = System.Drawing.Color.Gainsboro;
            this.levelSettingsLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.levelSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.levelSettingsLabel.ForeColor = System.Drawing.Color.Black;
            this.levelSettingsLabel.Location = new System.Drawing.Point(201, 0);
            this.levelSettingsLabel.Name = "levelSettingsLabel";
            this.levelSettingsLabel.Size = new System.Drawing.Size(185, 31);
            this.levelSettingsLabel.TabIndex = 9;
            this.levelSettingsLabel.Text = "Level Settings";
            this.levelSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.textBox1.Location = new System.Drawing.Point(207, 54);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(342, 30);
            this.textBox1.TabIndex = 4;
            this.textBox1.Click += new System.EventHandler(this.textBox1_Click);
            this.textBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyUp);
            // 
            // levelNameTag
            // 
            this.levelNameTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.levelNameTag.ForeColor = System.Drawing.Color.Black;
            this.levelNameTag.Location = new System.Drawing.Point(68, 54);
            this.levelNameTag.Name = "levelNameTag";
            this.levelNameTag.Size = new System.Drawing.Size(122, 25);
            this.levelNameTag.TabIndex = 8;
            this.levelNameTag.Text = "Level Name:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.numericUpDown1.Location = new System.Drawing.Point(236, 101);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(56, 30);
            this.numericUpDown1.TabIndex = 5;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // selectorPanel
            // 
            this.selectorPanel.BackColor = System.Drawing.SystemColors.Control;
            this.selectorPanel.Controls.Add(this.objectPanel);
            this.selectorPanel.Controls.Add(this.selectedLabel);
            this.selectorPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.selectorPanel.Location = new System.Drawing.Point(698, 313);
            this.selectorPanel.Name = "selectorPanel";
            this.selectorPanel.Size = new System.Drawing.Size(466, 386);
            this.selectorPanel.TabIndex = 9;
            // 
            // objectPanel
            // 
            this.objectPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.objectPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectPanel.Location = new System.Drawing.Point(77, 122);
            this.objectPanel.Name = "objectPanel";
            this.objectPanel.Size = new System.Drawing.Size(200, 100);
            this.objectPanel.TabIndex = 1;
            // 
            // selectedLabel
            // 
            this.selectedLabel.BackColor = System.Drawing.Color.Gainsboro;
            this.selectedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selectedLabel.ForeColor = System.Drawing.Color.Black;
            this.selectedLabel.Location = new System.Drawing.Point(209, 0);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(153, 41);
            this.selectedLabel.TabIndex = 0;
            this.selectedLabel.Text = "Object Selector";
            this.selectedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BuildRightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.selectorPanel);
            this.Controls.Add(this.levelDataPanel);
            this.Controls.Add(this.propertiesPanel);
            this.ForeColor = System.Drawing.Color.Gray;
            this.Name = "BuildRightControl";
            this.Size = new System.Drawing.Size(1615, 817);
            this.Load += new System.EventHandler(this.BuildRightControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.widthBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.properties)).EndInit();
            this.propertiesPanel.ResumeLayout(false);
            this.levelDataPanel.ResumeLayout(false);
            this.levelDataPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.selectorPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown widthBox;
        private System.Windows.Forms.NumericUpDown heightBox;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.DataGridView properties;
        private System.Windows.Forms.DataGridViewTextBoxColumn prop;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Panel propertiesPanel;
        private System.Windows.Forms.Label propertiesLabel;
        private System.Windows.Forms.Panel levelDataPanel;
        private System.Windows.Forms.Label levelNumTag;
        private System.Windows.Forms.Label levelNameTag;
        private System.Windows.Forms.Label worldNumTag;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label levelSettingsLabel;
        private System.Windows.Forms.Panel selectorPanel;
        private System.Windows.Forms.Label selectedLabel;
        private System.Windows.Forms.Panel objectPanel;

    }
}
