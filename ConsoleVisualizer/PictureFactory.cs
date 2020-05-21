using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleVisualizer
{
    static class PictureFactory
    {
        public static Image DrawImage(string albumArt, string song, string artist, string album, List<int> values)
        {
            Bitmap image = new Bitmap(512, 256);
            Graphics graphics = Graphics.FromImage(image);
            for(int i = 0; i < values.Count; i++)
            {
                double xCoord = i + 1 + 10 * i + 5;
                var p1 = new PointF((float)xCoord, image.Height);
                var p2 = new PointF((float)xCoord, image.Height - (float)values[i] - 1);
                graphics.DrawLine(new Pen(new SolidBrush(Color.White)), p1, p2);
            }
            return new Bitmap(512, 256, graphics);
        }
    }
}
