using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleVisualizer
{
    public partial class RightForm : Form
    {
        public RightForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.OnJoinDebug(textBox1.Text);
        }
        public void RedrawImage(Image image)
        {
            pictureBox1.Image = image;
            image.Save("image.png");
            Debug.WriteLine("OK: " + image.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.OpenFileAndPlay();
        }
    }
}
