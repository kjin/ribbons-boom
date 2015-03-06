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
    public partial class BuildBottomControl : UserControl
    {
        Color selected = Color.LightGreen;
        Color unselected = Color.LightSteelBlue;
        Control mainWindowControl;
        bool moveToRibbon;
        Microsoft.Xna.Framework.Rectangle clientSize;

        public bool MoveToRibbon { get { return moveToRibbon; } set { moveToRibbon = value; } }
        public BuildBottomControl(Control mainWindowControl, Microsoft.Xna.Framework.Rectangle clientSize)
        {
            this.clientSize = clientSize;
            InitializeComponent();
            unselectControlButtons();
            button1.BackColor = Color.IndianRed;
            button1.Checked = true;
            this.mainWindowControl = mainWindowControl;
            moveToRibbon = false;
            objectVariables.Rows.Add("width", "1");
            objectVariables.Rows.Add("height", "1");
            objectVariables.Rows.Add("flipped", "true");
            objectVariables.Rows.Add("isCheckPoint", "true");

            foreach (DataGridViewRow row in objectVariables.Rows)
            {
                row.Visible = false;
            }
        }

        private void BuildBottomControl_Load(object sender, EventArgs e)
        {
            objectVariables.Font = new Font("Arial", 11);
            scaleFont(objectVariables);
            scaleFont(chooseCaption);
            scaleFont(button1);
            scaleFont(button2);
            scaleFont(tabPage1);
            scaleFont(tabPage2);
            scaleFont(tabPage3);
            scaleFont(ribbonObjectsTab);
            scaleFont(objectVariablesLabel);
            scaleFont(label1);
            scaleFont(loopedRibbonButton);
            scaleFont(terminatingRibbonButton);
            scaleFont(ribbonBuildLabel);
            scaleFont(ribbonPathCaption);
            scaleFont(label3);
            scaleFont(label4);
            scaleFont(label5);
            scaleFont(next1);
            scaleFont(button4);
            scaleFont(label13);
            scaleFont(label8);
            scaleFont(label6);
            scaleFont(label7);
            scaleFont(submit1);
            scaleFont(back1);
            scaleFont(cancel2);
            scaleFont(startPosBox);
            scaleFont(endPosBox);
            scaleFont(colorLabel);
            scaleFont(colorBox);
            ribbonStartPanel.Visible = false;
            ribbonBuildPanel.Visible = false;
            ribbonPathPanel.Visible = false;
            this.Size = this.Parent.Size;
            bottomBuildButtons.Width = (int)(this.Width);
            bottomBuildButtons.Height = (int)(this.Height);
            bottomBuildButtons.Location = new Point(0, 0);
            bottomBuildButtons.SendToBack();
            button1.Width = (int)(this.Width * .10f) - 8;
            button1.Height = (int)((button1.Parent.Height) * 0.2f);
            button2.Width = button1.Width;
            button2.Height = button1.Height;
            backgroundPanel.SendToBack();
            backgroundPanel.Width = button2.Width + 8;
            backgroundPanel.Height = button1.Height + button2.Height + 12;
            seamstressPanel.Height = bottomBuildButtons.Height;
            seamstressPanel.Width = (int)(this.Parent.Parent.Width * .2f);
            ribbonObjectsTab.Width = (int)(this.Width * .6f) + 4;
            ribbonObjectsTab.Height = seamstressPanel.Height;
            tabPage2.Height = (int)(ribbonObjectsTab.Height * .8f);
            tabPage3.Height = tabPage2.Height;
            tabPage1.Height = tabPage2.Height;
            button1.Location = new Point(4, (int)(button1.Parent.Height / 2f - button1.Height - 2));
            button2.Location = new Point(4, button1.Bottom + 4);
            backgroundPanel.Location = new Point(0, button1.Top - 4);
            seamstressPanel.Location = new Point(button1.Right,0);
            ribbonObjectsTab.Location = new Point(seamstressPanel.Right,0);
            resizeButtons(platformButton, 1);
            resizeButtons(miasmaButton, 2);
            resizeButtons(shooterButton, 3);
            resizeButtons(rotatingButton, 4);
            resizeButtons(spikesButton, 5);
            resizeButtons(flipBarButton, 5);
            resizeButtons(saveRockButton, 1);
            resizeButtons(boxButton, 1);
            resizeButtons(hookButton, 2);
            resizeButtons(ribbonShooterButton, 4);
            resizeButtons(telescopingBlock, 3);

            ribbonStartPanel.Location = seamstressPanel.Location;
            ribbonStartPanel.Width = (int)(seamstressPanel.Width + 4 + ribbonObjectsTab.Width);
            ribbonStartPanel.Height = seamstressPanel.Height;
            ribbonBuildPanel.Location = ribbonStartPanel.Location;
            ribbonBuildPanel.Width = ribbonStartPanel.Width;
            ribbonBuildPanel.Height = ribbonStartPanel.Height;
            ribbonPathPanel.Width = ribbonBuildPanel.Width;
            ribbonPathPanel.Height = ribbonBuildPanel.Height;
            ribbonPathPanel.Location = ribbonBuildPanel.Location;

            objectVariables.Width = objectVariables.Parent.Width - 4;
            objectVariables.Height = tabPage2.Height;
            objectVariables.Location = new Point(4,tabPage1.Top);
            objectVariablesLabel.Width = objectVariablesLabel.Parent.Width + 1;
            objectVariablesLabel.Height = (int)(objectVariablesLabel.Font.Size * 2.5f) + 2;
            objectVariablesLabel.Location = new Point(-1,-1);

            label1.Width = label1.Parent.Width + 2;
            label1.Height = (int)(label1.Font.Size * 2 + label1.Parent.Height * 0.02f) + 2;
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.BackColor = Color.Gainsboro;
            label1.Location = new Point(-1, -1);

            chooseCaption.Width = (int)(chooseCaption.Parent.Width * 0.7f);
            chooseCaption.Height = (int)(chooseCaption.Font.Size * 4 + chooseCaption.Parent.Height * 0.02f);
            chooseCaption.Location = new Point((int)(chooseCaption.Parent.Width * 0.15f), label1.Bottom + (int)(chooseCaption.Parent.Height * 0.02f));
            chooseCaption.TextAlign = ContentAlignment.MiddleCenter;

            loopedRibbonButton.Width = (int)(loopedRibbonButton.Parent.Width * 0.2f);
            loopedRibbonButton.BackColor = Color.LightSteelBlue;
            loopedRibbonButton.Height = (int)(loopedRibbonButton.Parent.Height * 0.3f);
            loopedRibbonButton.Location = new Point((int)(loopedRibbonButton.Parent.Width / 2f - loopedRibbonButton.Width - loopedRibbonButton.Parent.Width * 0.025f), chooseCaption.Bottom + (int)(loopedRibbonButton.Parent.Height * 0.05f));

            terminatingRibbonButton.Width = (int)(terminatingRibbonButton.Parent.Width * 0.2f);
            terminatingRibbonButton.BackColor = Color.LightSteelBlue;
            terminatingRibbonButton.Height = loopedRibbonButton.Height;
            terminatingRibbonButton.Location = new Point((int)(terminatingRibbonButton.Parent.Width / 2f + terminatingRibbonButton.Parent.Width * 0.025f), chooseCaption.Bottom + (int)(terminatingRibbonButton.Parent.Height * 0.05f));


            label13.Width = label13.Parent.Width + 2;
            label13.Height = (int)(label13.Font.Size * 2 + label13.Parent.Height * 0.02f) + 2;
            label13.BorderStyle = BorderStyle.FixedSingle;
            label13.TextAlign = ContentAlignment.MiddleCenter;
            label13.BackColor = Color.Gainsboro;
            label13.Location = new Point(-1,-1);


            ribbonBuildLabel.Width = ribbonBuildLabel.Parent.Width + 2;
            ribbonBuildLabel.Height = (int)(ribbonBuildLabel.Font.Size * 2 + ribbonBuildLabel.Parent.Height * 0.02f) + 2;
            ribbonBuildLabel.BorderStyle = BorderStyle.FixedSingle;
            ribbonBuildLabel.TextAlign = ContentAlignment.MiddleCenter;
            ribbonBuildLabel.BackColor = Color.Gainsboro;
            ribbonBuildLabel.Location = new Point(-1, -1);

            ribbonPathCaption.Width = (int)(ribbonPathCaption.Parent.Width * 0.8f);
            ribbonPathCaption.Height = (int)(ribbonPathCaption.Font.Size * 4 + ribbonPathCaption.Parent.Height * 0.02f);
            ribbonPathCaption.Location = new Point((int)(ribbonPathCaption.Parent.Width * 0.1f), (int)(label1.Bottom + ribbonPathCaption.Parent.Height * 0.03f));
            ribbonPathCaption.TextAlign = ContentAlignment.MiddleCenter;

            label8.Width = (int)(label8.Parent.Width * 0.8f);
            label8.Height = (int)(label8.Font.Size * 2.75 + label8.Parent.Height * 0.02f);
            label8.Location = new Point((int)(label8.Parent.Width * 0.1f), label1.Bottom);
            label8.TextAlign = ContentAlignment.MiddleCenter;

            pathPointPanel.Width = (int)(pathPointPanel.Parent.Width * 0.3f);
            pathPointPanel.Height = (int)(pathPointPanel.Parent.Height * 0.4f);
            pathPointPanel.Location = new Point((int)(pathPointPanel.Parent.Width / 2f - pathPointPanel.Width * .8f), ribbonPathCaption.Bottom + (int)(ribbonPathCaption.Parent.Height * 0.05f));

            colorPanel.Width = (int)(colorPanel.Parent.Width * 0.15f);
            colorPanel.Height = pathPointPanel.Height;
            colorPanel.Location = new Point((int)(pathPointPanel.Right + colorPanel.Parent.Width * .01f), pathPointPanel.Top);

            colorLabel.Width = colorLabel.Parent.Width;
            colorLabel.Height = (int)(colorLabel.Font.Size * 2.8f);
            colorLabel.Location = new Point(0, 0);

            colorBox.Width = (int)(colorLabel.Parent.Width * 0.8f);
            colorBox.Height = (int)(colorBox.Font.Size * 2f);
            colorBox.Location = new Point((int)(colorBox.Parent.Width * 0.1f),colorLabel.Bottom + (int)(colorBox.Parent.Height * 0.1f));
   
            pathStartButton.Width = (int)(pathStartButton.Parent.Width * 0.12f);
            pathStartButton.Height = pathStartButton.Width;
            pathStartButton.Location = new Point((int)(pathStartButton.Parent.Width * 0.14f), (int)(pathStartButton.Parent.Height * 0.28f));

            label3.Font = new Font("Arial", 11);
            label3.Width = (int)(pathStartButton.Width * 2f);
            label3.Height = (int)(label3.Font.Size * 2f);
            label3.Location = new Point((int)((pathStartButton.Right - pathStartButton.Left) / 2 + pathStartButton.Left - label3.Width / 2), (int)(pathStartButton.Parent.Height * 0.03f));
            label3.TextAlign = ContentAlignment.TopCenter;

            pathPointButton.Width = (int)(pathPointButton.Parent.Width * 0.12f);
            pathPointButton.Height = pathPointButton.Width;
            pathPointButton.Location = new Point((int)(pathPointButton.Parent.Width * 0.18f + pathStartButton.Right), (int)(pathPointButton.Parent.Height * 0.28f));

            label4.Font = new Font("Arial", 11);
            label4.Width = (int)(pathStartButton.Width * 2f);
            label4.Height = (int)(label4.Font.Size * 2f);
            label4.Location = new Point((int)((int)((pathPointButton.Right - pathPointButton.Left) / 2 + pathPointButton.Left - label4.Width / 2)), (int)(pathPointButton.Parent.Height * 0.03f));
            label4.TextAlign = ContentAlignment.TopCenter;
            
            pathEndButton.Width = (int)(pathEndButton.Parent.Width * 0.12f);
            pathEndButton.Height = pathEndButton.Width;
            pathEndButton.Location = new Point((int)(pathEndButton.Parent.Width * 0.18f + pathPointButton.Right), (int)(pathEndButton.Parent.Height * 0.28f));

            label5.Font = new Font("Arial", 11);
            label5.Width = (int)(pathStartButton.Width * 2f);
            label5.Height = (int)(label5.Font.Size * 2f);
            label5.Location = new Point((int)((pathEndButton.Right - pathEndButton.Left) / 2 + pathEndButton.Left - label5.Width / 2), (int)(pathEndButton.Parent.Height * 0.03f));
            label5.TextAlign = ContentAlignment.TopCenter;

            button4.Width = (int)(button4.Parent.Width * 0.1f);
            button4.Height = (int)(button4.Parent.Height * 0.2f);
            button4.Location = new Point((int)(button4.Parent.Width - button4.Width - button4.Parent.Width * 0.01f),(int)(button4.Parent.Height - button4.Height -button4.Parent.Width * 0.01f));

            next1.Width = (int)(next1.Parent.Width * 0.1f);
            next1.Height = (int)(next1.Parent.Height * 0.2f);
            next1.Location = new Point((int)(next1.Parent.Width - next1.Width - next1.Parent.Width * 0.01f), (int)(button4.Top - next1.Height - next1.Parent.Width * 0.0025f));

            cancel2.Width = (int)(cancel2.Parent.Width * 0.1f);
            cancel2.Height = (int)(cancel2.Parent.Height * 0.2f);
            cancel2.Location = new Point((int)(cancel2.Parent.Width - cancel2.Width - cancel2.Parent.Width * 0.01f), (int)(cancel2.Parent.Height - cancel2.Height - cancel2.Parent.Width * 0.01f));

            back1.Width = (int)(back1.Parent.Width * 0.1f);
            back1.Height = (int)(back1.Parent.Height * 0.2f);
            back1.Location = new Point((int)(back1.Parent.Width - back1.Width - back1.Parent.Width * 0.01f), (int)(button4.Top - back1.Height - back1.Parent.Width * 0.0025f));

            submit1.Width = (int)(submit1.Parent.Width * 0.1f);
            submit1.Height = (int)(submit1.Parent.Height * 0.2f);
            submit1.Location = new Point((int)(submit1.Parent.Width - submit1.Width - submit1.Parent.Width * 0.01f), (int)(back1.Top - submit1.Height - submit1.Parent.Width * 0.0025f));

            ribbonPointPanel.Width = (int)(ribbonPointPanel.Parent.Width * 0.5f);
            ribbonPointPanel.Height = (int)(ribbonPointPanel.Parent.Height * 0.57f);
            ribbonPointPanel.Location = new Point((int)(ribbonPointPanel.Parent.Width / 2f - ribbonPointPanel.Width / 2f), (int)(label8.Bottom));

            label6.Width = label6.Parent.Width;
            label6.Height = (int)(label6.Font.Size * 2);
            label6.Location = new Point((int)(label6.Parent.Width * 0.025f), (int)(label6.Parent.Height * 0.075f));
            
            pictureBox1.Width = (int)(pictureBox1.Parent.Width * .04f);
            pictureBox1.Height = pictureBox1.Width;
            pictureBox1.Location = new Point((int)(pictureBox1.Parent.Width * 0.03f), (int)(label6.Bottom + pictureBox1.Parent.Height * 0.01f));

            startTrack.Height = (int)(pictureBox1.Height * 0.8f);
            startTrack.Width = (int)(startTrack.Parent.Width * .8f);
            startTrack.Location = new Point((int)(pictureBox1.Right + startTrack.Parent.Width * .01f), pictureBox1.Top);

            startPosBox.Height = startTrack.Height;
            startPosBox.Width = (int)(startTrack.Parent.Width * .08f);
            startPosBox.Location = new Point((int)(startTrack.Right + startPosBox.Parent.Width * .01f), startTrack.Top);

            label7.Width = label7.Parent.Width;
            label7.Height = (int)(label7.Font.Size * 2);
            label7.Location = new Point((int)(label7.Parent.Width * 0.025f), (int)(pictureBox1.Bottom + label7.Parent.Height * 0.06f));

            pictureBox2.Width = (int)(pictureBox2.Parent.Width * .04f);
            pictureBox2.Height = pictureBox2.Width;
            pictureBox2.Location = new Point((int)(pictureBox2.Parent.Width * 0.03f), (int)(label7.Bottom + pictureBox2.Parent.Height * 0.01f));

            endTrack.Height = (int)(pictureBox2.Height * 0.8f);
            endTrack.Width = (int)(endTrack.Parent.Width * .8f);
            endTrack.Location = new Point((int)(pictureBox2.Right + endTrack.Parent.Width * .01f), pictureBox2.Top);

            endPosBox.Height = endTrack.Height;
            endPosBox.Width = (int)(endTrack.Parent.Width * .08f);
            endPosBox.Location = new Point((int)(endTrack.Right + endPosBox.Parent.Width * .01f), endTrack.Top);

            formatObjectLabel(groundLabel, 1);
            formatObjectLabel(miasmaLabel, 2);
            formatObjectLabel(shooterLabel, 3);
            formatObjectLabel(rotatingLabel, 4);
            formatObjectLabel(needleLabel, 5);
            formatObjectLabel(boxLabel, 1);
            formatObjectLabel(hookLabel, 2);
            formatObjectLabel(ribbonShooterLabel, 3);
            formatObjectLabel(telescopingLabel, 4);
            formatObjectLabel(flipbarLabel, 5);
            formatObjectLabel(saveRockLabel, 1);

            scaleFont(label3);
            scaleFont(label4);
            scaleFont(label5);
            this.PerformAutoScale();
        }

        public void UpdateTitleText(string s, int gridWidth, int gridHeight)
        {
            Control active = this.ActiveControl;
            if (objectVariables.IsCurrentCellInEditMode)
            {
                objectVariables.EndEdit();

                string title = objectVariables.Rows[objectVariables.CurrentCell.RowIndex].Cells[0].Value.ToString();
                Console.WriteLine(title);
                if (title == "width" || title == "height")
                {
                    int checkNum;
                    string checkString;
                    if (objectVariables.CurrentCell.Value == null)
                        checkString = s;
                    else
                        checkString = objectVariables.CurrentCell.Value.ToString() + s;
                    bool validate = int.TryParse(checkString, out checkNum);

                    if (title == "width" && validate && checkNum <= gridWidth || title == "height" && validate && checkNum <= gridHeight)
                    objectVariables.CurrentCell.Value += s;
                }
                if (title == "isCheckPoint" || title == "flipped")
                {
                    string prevValue = objectVariables.CurrentCell.Value.ToString();
                    if (s == "t" || s == "T")
                    {
                        objectVariables.CurrentCell.Value = "true";
                    }
                    else if (s == "f" || s == "F")
                    {
                        objectVariables.CurrentCell.Value = "false";
                    }
                    else
                    {
                        objectVariables.CurrentCell.Value = prevValue;
                    }
                }
                objectVariables.BeginEdit(false);
            }
        }

        private void objectVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            Console.WriteLine("test");
            objectVariables.EndEdit();
            DataGridViewCell cell = objectVariables.Rows[e.RowIndex].Cells[1];
            string title = objectVariables.Rows[e.RowIndex].Cells[0].Value.ToString();
            if (title == "isCheckPoint" || title == "flipped")
            {
                if (cell.Value == null)
                {
                    cell.Value = "true";
                }
                else if (cell.Value.ToString().ElementAt(0) == 'f')
                {
                    cell.Value = "false";
                }
                else
                {
                    cell.Value = "true";
                }
            }
            if (title == "width" || title == "height")
            {
                Console.WriteLine(cell.Value);
                if (cell.Value == null)
                {
                    Console.WriteLine("null");
                    cell.Value = "1";
                }
            }
        }

        public int getObjectWidth()
        {
            return int.Parse(objectVariables.Rows[0].Cells[1].Value.ToString());
        }

        public int getObjectHeight()
        {
            return int.Parse(objectVariables.Rows[1].Cells[1].Value.ToString());
        }

        public bool getFlipped()
        {
            if (objectVariables.Rows[2].Cells[1].Value.ToString() == "false")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool getCheckpointStatus()
        {
            if (objectVariables.Rows[3].Cells[1].Value.ToString() == "false")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void formatObjectLabel(Label c, int x)
        {
            c.TextAlign = ContentAlignment.TopCenter;
            c.AutoSize = false;
            c.BackColor = SystemColors.Control;
            c.BorderStyle = BorderStyle.FixedSingle;
            c.Width = platformButton.Width;
            c.Height = (int)(platformButton.Height * 0.2f);
            c.Location = new Point((int)((c.Width + c.Width * .08f) * (x - 1) + c.Width * .08f), platformButton.Top);
            c.Font = new Font("Arial", 11);
            scaleFont(c);
        }

        private void scaleFont(Control c)
        {
            float oldSize = c.Font.Size;
            float newSize = oldSize / 1440f * Screen.GetWorkingArea(mainWindowControl).Height;
            c.Font = new Font("Arial", newSize);
        }

        private void resizeButtons(PictureBox c, int x)
        {
            c.Width = (int)(tabPage2.Width / 5 - tabPage2.Width / 5 * .3f - tabPage2.Width / 5 * .3f / 5 / 2);
            c.Height = c.Width;
            c.Location = new Point((int)((c.Width + c.Width * .08f) * (x - 1) + c.Width * .08f), (int)(c.Parent.Height / 2f - c.Height / 2f));

        }

        private float scale(int x, bool isWidth)
        {
            int heightScale = Screen.FromControl(mainWindowControl).Bounds.Height;
            int widthScale = Screen.FromControl(mainWindowControl).Bounds.Height;
            if (isWidth)
            {
                return ((float)x / 2560f * (float)widthScale);
            }
            else
            {
                return ((float)x / 1440f * (float)heightScale);
            }
        }
        private void unselectControlButtons()
        {
            button1.BackColor = Color.Gainsboro;
            button2.BackColor = Color.Gainsboro;
            button1.Checked = false;
            button2.Checked = false;
        }

        private void hideControlPanels()
        {
            ribbonObjectsTab.Visible = false;
            seamstressPanel.Visible = false;
            ribbonStartPanel.Visible = false;
            ribbonBuildPanel.Visible = false;
            ribbonPathPanel.Visible = false;
        }

        private void disableControls()
        {
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void enableControls()
        {
            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.BuildPanelContext = 1;
            unselectControlButtons();
            button1.BackColor = Color.IndianRed;
            button1.Checked = true;
            hideControlPanels();
            ribbonObjectsTab.Visible = true;
            seamstressPanel.Visible = true;
            panel.ObjectSelected = "seamstress";
            unselectButtons();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.BuildPanelContext = 2;
            unselectControlButtons();
            button2.BackColor = Color.IndianRed;
            button2.Checked = true;
            hideControlPanels();
            ribbonStartPanel.Visible = true;
            panel.ObjectSelected = "none";
        }

        private void seamstressButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "seamstress";
            unselectButtons();
        }

        private void unselectButtons()
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ClearSelection = true;
            boxButton.BackColor = unselected;
            hookButton.BackColor = unselected;
            platformButton.BackColor = unselected;
            miasmaButton.BackColor = unselected;
            shooterButton.BackColor = unselected;
            rotatingButton.BackColor = unselected;
            spikesButton.BackColor = unselected;
            saveRockButton.BackColor = unselected;
            telescopingBlock.BackColor = unselected;
            ribbonShooterButton.BackColor = unselected;
            flipBarButton.BackColor = unselected;
            pathStartButton.BackColor = unselected;
            pathPointButton.BackColor = unselected;
            pathEndButton.BackColor = unselected;
        }

        private void boxButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "box";
            unselectButtons();
            boxButton.BackColor = selected;
            panel.OnlyRibbon = true;
        }



        private void hookButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "hook";
            unselectButtons();
            hookButton.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void platformButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "platform";
            unselectButtons();
            platformButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void miasmaButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "miasma";
            unselectButtons();
            miasmaButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void shooterButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "shooter";
            unselectButtons();
            shooterButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void rotatingButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "rotating";
            unselectButtons();
            rotatingButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void spikesButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "spikes";
            unselectButtons();
            spikesButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void saveRockButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "save";
            unselectButtons();
            saveRockButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void telescopingBlock_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "telescoping";
            unselectButtons();
            telescopingBlock.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void ribbonShooterButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "ribbonShooter";
            unselectButtons();
            ribbonShooterButton.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void flipBarButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "flipBar";
            unselectButtons();
            flipBarButton.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void loopedRibbonButton_Click(object sender, EventArgs e)
        {
            ribbonBuildPanel.Visible = true;
            ribbonStartPanel.Visible = false;
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "pathStart";
            unselectButtons();
            pathStartButton.BackColor = selected;
            disableControls();
            panel.RibbonLooped = true;
            panel.LoopButton = true;
            panel.OnlyRibbon = false;
        }

        private void terminatingRibbonButton_Click(object sender, EventArgs e)
        {
            ribbonBuildPanel.Visible = true;
            ribbonStartPanel.Visible = false;
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "pathStart";
            unselectButtons();
            pathStartButton.BackColor = selected;
            disableControls();
            panel.RibbonLooped = false;
            panel.TerminateButton = true;
            panel.OnlyRibbon = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ribbonStartPanel.Visible = true;
            ribbonBuildPanel.Visible = false;
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "none";
            unselectButtons();
            enableControls();
            panel.RibbonCancel = true;
            panel.LoopButton = true;
        }

        private void pathStartButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "pathStart";
            unselectButtons();
            pathStartButton.BackColor = selected;
        }

        private void pathPointButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "pathPoint";
            unselectButtons();
            pathPointButton.BackColor = selected;
        }

        private void pathEndButton_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "pathEnd";
            unselectButtons();
            pathEndButton.BackColor = selected;
        }

        private void next1_Click_1(object sender, EventArgs e)
        {
            if (moveToRibbon)
            {
                hideControlPanels();
                ribbonPathPanel.Visible = true;
                BottomControlPanel curr = this.Parent as BottomControlPanel;
                curr.AddRibbonPoints = true;
                unselectButtons();
                curr.ObjectSelected = "none";
            }   
            else
            {
                MessageBox.Show("Please add the required points to the path before continuing.");
            }
        }

        private void cancel2_Click(object sender, EventArgs e)
        {
            ribbonStartPanel.Visible = true;
            ribbonPathPanel.Visible = false;
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "none";
            unselectButtons();
            enableControls();
            panel.RibbonCancel = true;
        }

        private void back1_Click(object sender, EventArgs e)
        {
            ribbonBuildPanel.Visible = true;
            ribbonPathPanel.Visible = false;
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.BackButton = true;
        }

        private void startTrack_ValueChanged(object sender, EventArgs e)
        {
            startPosBox.Text = startTrack.Value.ToString();
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.StartScrollValue = startTrack.Value;
        }

        private void endTrack_ValueChanged(object sender, EventArgs e)
        {
            endPosBox.Text = endTrack.Value.ToString();
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.EndScrollValue = endTrack.Value;
        }

        public void setMaxRibbonPath(int max)
        {
            endTrack.Maximum = max;
            endPosBox.Text = endTrack.Value.ToString();
            startTrack.Maximum = endTrack.Value - 1;
            endTrack.Minimum = startTrack.Value + 1;
        }

        public void setEndPos(int pos)
        {
            if (pos < endTrack.Maximum && pos >= endTrack.Minimum)
                endTrack.Value = pos;
            else
            {
                MessageBox.Show("This ribbon shape is invalid.  Please try again.");
            }
        }

        private void submit1_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.CreateButton = true;
            hideControlPanels();
            enableControls();
            ribbonObjectsTab.Visible = true;
            seamstressPanel.Visible = true;
        }

        public void showRibbonEdit(){
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.BuildPanelContext = 2;
            unselectControlButtons();
            button2.BackColor = Color.IndianRed;
            button2.Checked = true;
            hideControlPanels();
            ribbonBuildPanel.Visible = true;
            panel.ObjectSelected = "none";
        }

        private void groundLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "platform";
            unselectButtons();
            platformButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void miasmaLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "miasma";
            unselectButtons();
            miasmaButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void shooterLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "shooter";
            unselectButtons();
            shooterButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void rotatingLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "rotating";
            unselectButtons();
            rotatingButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void needleLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "spikes";
            unselectButtons();
            spikesButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void saveRockLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "save";
            unselectButtons();
            saveRockButton.BackColor = selected;
            panel.OnlyRibbon = false;
        }

        private void boxLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "box";
            unselectButtons();
            boxButton.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void hookLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "hook";
            unselectButtons();
            hookButton.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void telescopingLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "ribbonShooter";
            unselectButtons();
            ribbonShooterButton.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void ribbonShooterLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "telescoping";
            unselectButtons();
            telescopingBlock.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void flipbarLabel_Click(object sender, EventArgs e)
        {
            BottomControlPanel panel = this.Parent as BottomControlPanel;
            panel.ObjectSelected = "flipBar";
            unselectButtons();
            flipBarButton.BackColor = selected;
            panel.OnlyRibbon = true;
        }

        private void objectVariables_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (objectVariables.CurrentCell != null)
            objectVariables.BeginEdit(false);
        }

        private void resetDataGrid()
        {
            if (objectVariables != null)
            {
                objectVariables.Rows[0].Cells[1].Value = "1";
                objectVariables.Rows[1].Cells[1].Value = "1";
                objectVariables.Rows[2].Cells[1].Value = "true";
                objectVariables.Rows[3].Cells[1].Value = "true";
            }

            foreach (DataGridViewRow row in objectVariables.Rows)
            {
                row.Visible = false;
            }
        }

        private void showRow(int i)
        {
            objectVariables.Rows[i].Visible = true;
        }

        public void PopulateDataGrid(string selection)
        {
            resetDataGrid();
            if (selection == "platform")
            {
                showRow(0);
                showRow(1);
            }
            if (selection == "save")
            {
                showRow(3);
            }
            if (selection == "miasma")
            {
                showRow(0);
                showRow(1);
            }
        }
    }
}
