using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Reflection;

namespace EditorUILibrary.Controls
{

    public partial class TestRightControl : UserControl
    {
       // private StringRedir RedirConsole;
        private TextWriter ConsoleWriter;

        public TestRightControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable, false);

            ConsoleWriter = Console.Out;
            //RedirConsole = new StringRedir(ref consoleBox);
            //Console.SetOut(RedirConsole);
        }

        private void TestRightControl_OnDestroy()
        {
            Console.SetOut(ConsoleWriter);	// Redirect Console back to original TextWriter. 
            //RedirConsole.Close();			// Close our StringRedir TextWriter. 
        }

        private void TestRightControl_Load(object sender, EventArgs e)
        {
            this.Width = this.Parent.Width;
            this.Height = this.Parent.Height;

            consoleBox.Width = this.Width - 20;
            consoleBox.Height = this.Height - 210;
            consoleBox.Location = new Point(this.Left + 10, this.Top + 200);

            consoleLabel.Location = new Point(5, consoleBox.Top - 30);
            debugLabel.Location = new Point(5, 5);
            tempLabel.Location = new Point(this.Width / 2 - tempLabel.Width / 2, (consoleBox.Top - 30)/2);
        }

   } 
   /* public class StringRedir : StringWriter
    { // Redirecting Console output to RichTextBox
        private RichTextBox outBox;
        delegate void Writeline(string x);
        private static BlockingCollection<string> m_Queue = new BlockingCollection<string>();
        private string storage;

        public StringRedir(ref RichTextBox textBox)
        {
            outBox = textBox;
            storage = "";
        }

        public void Print()
        {
            if (storage != "")
            {

            }   
        }

        public override void Write(string value)
        { // Centralize output in ONE function : this one 
            outBox.Invoke((MethodInvoker)delegate()
            {
                formatTextBox();
                outBox.AppendText(value);        
            });      
        }

        public override void Write(int value)
        {
            Write(value.ToString());
        }

        public override void Write(bool value)
        {
            Write(value.ToString());
        }
        public override void Write(char value)
        {
            Write(value.ToString());
        }
        public override void Write(float value)
        {
            Write(value.ToString());
        }
        public override void Write(double value)
        {
            Write(value.ToString());
        }

        public override void Write(object value)
        {
            Write(value.ToString());
        }
        
        public override void WriteLine(string value)
        {
            Write(value + "\n");
        }

        public override void WriteLine(int value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(bool value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(char value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(float value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(double value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(object value)
        {
            WriteLine(value.ToString());
        }

        public override void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        public override void WriteLine(string format, params object[] args)
        {
            Write(string.Format(format, args) + "\n");
        }

        public  void formatTextBox()
        {
            if (outBox.Lines.Count() > 2500){
                outBox.Text = outBox.Text.Substring(outBox.Lines[0].Length + Environment.NewLine.Length);
            }
        }
    }*/

}
