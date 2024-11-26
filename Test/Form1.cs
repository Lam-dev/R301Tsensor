using FGPcontrol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            fgPcontroller1.ConnectSensor("COM7");
            fgPcontroller1.EventGetFGPresult += FGPresult;
            fgPcontroller1.StartGetFGP();
        }

        void FGPresult(bool result, string err, List<byte> characteristics)
        {
            
        }
    }
}
