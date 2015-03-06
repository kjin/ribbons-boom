namespace EditorUILibrary.Controls
{
    partial class TestRightControl
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
            this.consoleBox = new System.Windows.Forms.RichTextBox();
            this.blockConsole = new System.Windows.Forms.Label();
            this.debugLabel = new System.Windows.Forms.Label();
            this.tempLabel = new System.Windows.Forms.Label();
            this.consoleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // consoleBox
            // 
            this.consoleBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.consoleBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consoleBox.CausesValidation = false;
            this.consoleBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.consoleBox.ForeColor = System.Drawing.Color.White;
            this.consoleBox.HideSelection = false;
            this.consoleBox.Location = new System.Drawing.Point(-36, 141);
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.ReadOnly = true;
            this.consoleBox.Size = new System.Drawing.Size(347, 760);
            this.consoleBox.TabIndex = 0;
            this.consoleBox.TabStop = false;
            this.consoleBox.Text = "";

            // 
            // blockConsole
            // 
            this.blockConsole.AutoSize = true;
            this.blockConsole.Location = new System.Drawing.Point(93, 349);
            this.blockConsole.Name = "blockConsole";
            this.blockConsole.Size = new System.Drawing.Size(0, 17);
            this.blockConsole.TabIndex = 1;
            // 
            // debugLabel
            // 
            this.debugLabel.AutoSize = true;
            this.debugLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.debugLabel.Location = new System.Drawing.Point(13, 13);
            this.debugLabel.Name = "debugLabel";
            this.debugLabel.Size = new System.Drawing.Size(152, 25);
            this.debugLabel.TabIndex = 2;
            this.debugLabel.Text = "Debug Settings:";
            // 
            // tempLabel
            // 
            this.tempLabel.AutoSize = true;
            this.tempLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.tempLabel.Location = new System.Drawing.Point(68, 55);
            this.tempLabel.Name = "tempLabel";
            this.tempLabel.Size = new System.Drawing.Size(196, 36);
            this.tempLabel.TabIndex = 3;
            this.tempLabel.Text = "Coming Soon";
            // 
            // consoleLabel
            // 
            this.consoleLabel.AutoSize = true;
            this.consoleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.consoleLabel.Location = new System.Drawing.Point(13, 104);
            this.consoleLabel.Name = "consoleLabel";
            this.consoleLabel.Size = new System.Drawing.Size(91, 25);
            this.consoleLabel.TabIndex = 4;
            this.consoleLabel.Text = "Console:";
            // 
            // TestRightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.consoleLabel);
            this.Controls.Add(this.tempLabel);
            this.Controls.Add(this.debugLabel);
            this.Controls.Add(this.blockConsole);
            this.Controls.Add(this.consoleBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TestRightControl";
            this.Size = new System.Drawing.Size(353, 766);
            this.Load += new System.EventHandler(this.TestRightControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox consoleBox;
        private System.Windows.Forms.Label blockConsole;
        private System.Windows.Forms.Label debugLabel;
        private System.Windows.Forms.Label tempLabel;
        private System.Windows.Forms.Label consoleLabel;




    }
}
