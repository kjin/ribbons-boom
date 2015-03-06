namespace EditorUILibrary.Controls
{
    partial class ModeButtons
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
            this.buttonFormatPanel = new System.Windows.Forms.Panel();
            this.testCaption = new System.Windows.Forms.Label();
            this.testLabel = new System.Windows.Forms.Label();
            this.test = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.visualLabel = new System.Windows.Forms.Label();
            this.buttonFormatPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonFormatPanel
            // 
            this.buttonFormatPanel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFormatPanel.Controls.Add(this.testCaption);
            this.buttonFormatPanel.Controls.Add(this.testLabel);
            this.buttonFormatPanel.Controls.Add(this.test);
            this.buttonFormatPanel.Location = new System.Drawing.Point(353, 17);
            this.buttonFormatPanel.Name = "buttonFormatPanel";
            this.buttonFormatPanel.Size = new System.Drawing.Size(214, 281);
            this.buttonFormatPanel.TabIndex = 4;
            // 
            // testCaption
            // 
            this.testCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F);
            this.testCaption.Location = new System.Drawing.Point(21, 57);
            this.testCaption.Name = "testCaption";
            this.testCaption.Size = new System.Drawing.Size(168, 82);
            this.testCaption.TabIndex = 6;
            this.testCaption.Text = "Load and test the level you have created";
            this.testCaption.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // testLabel
            // 
            this.testLabel.BackColor = System.Drawing.Color.Gainsboro;
            this.testLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.testLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.testLabel.ForeColor = System.Drawing.Color.Black;
            this.testLabel.Location = new System.Drawing.Point(9, 5);
            this.testLabel.Name = "testLabel";
            this.testLabel.Size = new System.Drawing.Size(196, 31);
            this.testLabel.TabIndex = 5;
            this.testLabel.Text = "Test Mode";
            this.testLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // test
            // 
            this.test.BackColor = System.Drawing.Color.IndianRed;
            this.test.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.test.Location = new System.Drawing.Point(15, 187);
            this.test.Name = "test";
            this.test.Size = new System.Drawing.Size(186, 73);
            this.test.TabIndex = 2;
            this.test.Text = "Run";
            this.test.UseVisualStyleBackColor = false;
            this.test.Click += new System.EventHandler(this.test_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.visualLabel);
            this.panel1.Location = new System.Drawing.Point(13, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(297, 281);
            this.panel1.TabIndex = 3;
            // 
            // visualLabel
            // 
            this.visualLabel.BackColor = System.Drawing.Color.Gainsboro;
            this.visualLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.visualLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.visualLabel.Location = new System.Drawing.Point(35, 5);
            this.visualLabel.Name = "visualLabel";
            this.visualLabel.Size = new System.Drawing.Size(218, 41);
            this.visualLabel.TabIndex = 0;
            this.visualLabel.Text = "Visual Settings";
            this.visualLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ModeButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.buttonFormatPanel);
            this.Controls.Add(this.panel1);
            this.Name = "ModeButtons";
            this.Size = new System.Drawing.Size(589, 323);
            this.Load += new System.EventHandler(this.ModeButtons_Load);
            this.buttonFormatPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel buttonFormatPanel;
        private System.Windows.Forms.Label testLabel;
        private System.Windows.Forms.Button test;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label testCaption;
        private System.Windows.Forms.Label visualLabel;
    }
}
