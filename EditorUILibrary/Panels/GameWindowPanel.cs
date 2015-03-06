using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EditorUILibrary.Panels
{
    public class GameWindowPanel : Panel
    {
        bool clicked;
        Point clickedPoint;
        int currentContext;
        bool doubleClicked;
        int prevTime;
        public new event EventHandler DoubleClick;
        int now;
        Control mainWindowControl;
        Microsoft.Xna.Framework.Rectangle clientSize;

        int scrollStatus;
        Point scrollPosition;
        int xPadding;
        int yPadding;

        public bool DoubleClicked { get { return doubleClicked; } set { doubleClicked = value; } }
        public Point ClickedPoint { get { return clickedPoint; } set { clickedPoint = value; } }
        public bool Clicked { get { return clicked; } set { clicked = value; } }
        public int CurrentContext { get { return currentContext; } set { currentContext = value; } }
        public int ScrollStatus { get { return scrollStatus; } set { scrollStatus = value; } }
        public Point ScrollPosition { get { return scrollPosition; } set { scrollPosition = value; } }

        public GameWindowPanel(Control mainWindowControl)
            :base()
        {
            this.mainWindowControl = mainWindowControl;
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);

            InitializeComponent();
            clicked = false;
            currentContext = 1;
            scrollStatus = 0;
            this.MouseWheel += GameWindowPanel_MouseWheel;
            doubleClicked = false;
            now = System.Environment.TickCount;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GameWindowPanel
            // 
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GameWindowPanel_MouseClick);
            this.Resize += new System.EventHandler(this.GameWindowPanel_Resize);
            this.ResumeLayout(false);

        }

        public void Initialize(Microsoft.Xna.Framework.Rectangle clientSize)
        {
            this.clientSize = clientSize;
            xPadding = (int)((float)clientSize.Width * 0.004f);
            yPadding = (int)((float)clientSize.Height * 0.01f);
            this.Width = (int)((float)clientSize.Width * .75f);
            this.Height = (this.Width / 16) * 9;
            this.Location = new System.Drawing.Point(xPadding,yPadding + (int)((float)clientSize.Height * 0.03f));
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        private void GameWindowPanel_MouseWheel(object sender, MouseEventArgs args)
        {
            if (args.Delta > 0)
            {
                scrollStatus = 1;
                scrollPosition = this.PointToClient(Cursor.Position);
                Console.WriteLine(scrollPosition);
            }
            if (args.Delta < 0)
            {
                scrollStatus = 2;
                scrollPosition = this.PointToClient(Cursor.Position);
                Console.WriteLine(scrollPosition);
            }
        }

        private void GameWindowPanel_MouseClick(object sender, MouseEventArgs e)
        {
               doubleClicked = false;
               clicked = true;
                clickedPoint = this.PointToClient(Cursor.Position);
                this.Select();
                this.Focus();
        }

        public void fullscreen()
        {
            this.Width = clientSize.Width;
            this.Height = clientSize.Height;
            this.Location = new System.Drawing.Point(0,0);
        }

        public void smallscreen()
        {
            this.Width = (int)((float)clientSize.Width * .75f);
            this.Height = (this.Width / 16) * 9;
            this.Location = new System.Drawing.Point(xPadding, yPadding + (int)((float)clientSize.Height * 0.03f));
        }

        private void GameWindowPanel_Resize(object sender, EventArgs e)
        {
                   
        }
    }
}
