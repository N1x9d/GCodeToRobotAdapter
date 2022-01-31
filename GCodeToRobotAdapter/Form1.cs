using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace GCodeToRobotAdapter
{
    public partial class Form1 : Form
    {
        string InputFileInfo;
        string outFile;
        private GcodeReader GCode;
        public Form1()
        {
            InitializeComponent();
            GCode = new GcodeReader(this);
        }
        public string[] ArcStartText { get { return addMarker(textBox4.Lines,";ArcStart");  } set { textBox4.Lines = value; } }
        public string[] ArcEndText { get { return addMarker(textBox5.Lines,";ArcEnd"); } set { textBox5.Lines = value; } }
        public string[] SettingText { get { return addMarker(textBox3.Lines,";Sets"); } set { textBox3.Lines = value; } }
        public string InputFileInfo1 
        { 
            get => InputFileInfo; 
            
            set 
            { 
                InputFileInfo = value; 
                textBox1.Text = InputFileInfo;
                var a = InputFileInfo;
                a = a.Substring(0, a.LastIndexOf("."));
                OutFile = a + "1.gcode";
            } 
        }

        public string OutFile { get => outFile; set { outFile = value; textBox2.Text = outFile; } }

        private void button1_Click(object sender, EventArgs e)
        {
            GCode.on_btn_Open_clicked();
        }
        private String[] addMarker(string[] InpLine, string Marker)
        {
            List<string> str = new List<string>();
            str.Add(Marker+'0');
            str.AddRange(InpLine);
            str.Add(Marker+'1');
            return str.ToArray();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            GCode.on_btn_Process_clicked();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GCode.Import();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            outFile = textBox2.Text;
        }
    }
}
