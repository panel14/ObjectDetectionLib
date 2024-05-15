
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;

namespace ObjectDetectionLib.EmguCV
{
    public static class CvDrawer
    {
        public static Image<Bgr, byte> DrawBoxesOnly(IEnumerable<Rectangle> rects, Mat src)
        {
            Image<Bgr, byte> img = src.ToImage<Bgr, byte>();

            foreach (var res in rects)
            {
                int curX = res.Left + res.Width / 2;
                int curY = res.Top + res.Height / 2;

                CvInvoke.Rectangle(img, res, new MCvScalar(0, 0, 255));
                CvInvoke.Circle(img, new Point(curX, curY), 2, new MCvScalar(0, 0, 255), 2);
            }

            return img;
        }
    }
}
