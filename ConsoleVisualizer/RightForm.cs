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
            TopMost = true;
        }


        public void RedrawImage(Image image)
        {
            pictureBox1.Image = image;
            Debug.WriteLine("OK: " + image.ToString());
        }
    }
}
