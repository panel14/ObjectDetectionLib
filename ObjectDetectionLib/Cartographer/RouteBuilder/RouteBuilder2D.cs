using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace ObjectDetectionLib.Cartographer.RouteBuilder
{
    public static class RouteBuilder2D
    {
        public static void DrawRoute(ref Image<Bgr, byte> src, Point[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                CvInvoke.Line(src, points[i], points[i + 1], new MCvScalar(0, 255, 0));
            }
        }
    }
}
