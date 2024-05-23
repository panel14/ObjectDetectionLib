using Emgu.CV;
using Emgu.CV.Structure;
using ObjectDetectionLib.Cartographer.Models;
using System.Drawing;

namespace ObjectDetectionLib.Cartographer.Mapper
{
    public static class Mapper2D
    {
        private static double TransformToImageCoordinateX(double x, double minX, int scale, int shift)
        {
            if (x > 0) return (minX + x) * scale + shift / 2;
            else return (minX - Math.Abs(x)) * scale + shift / 2;
        }

        private static double TransformToImageCoordinateZ(double y, double maxZ, int scale, int shift)
        {
            if (y > 0) return (maxZ - y) * scale + shift / 2;
            else return (maxZ + Math.Abs(y)) * scale + shift / 2;
        }

        private static double GetMinOf3(double a, double b, double? c)
        {
            var abMin = double.Min(a, b);
            return c.HasValue ? double.Min(abMin, c.Value) : abMin;
        }

        private static double GetMaxOf3(double a, double b, double? c)
        {
            var abMax = double.Max(a, b);
            return c.HasValue ? double.Max(abMax, c.Value) : abMax;
        }

        private static void BuildHistorramm(IEnumerable<MCvPoint3D64f> points)
        {
            var yMin = (int)points.MinBy(p => p.Y).Y;
            var yMax = (int)points.MaxBy(p => p.Y).Y;

            List<int> heights = [];

            for (int i = yMin; i <= yMax; i++)
            {
                heights.Add(points.Count(p => p.Y > i && p.Y < i + 1));
            }

            var maxH = heights.Max();

            int scale = 15;
            int indent = 10;
            int shift = 50;
            int maxInPixels = 500;

            Image<Bgr, byte> hist = new(heights.Count * scale + indent * (heights.Count - 1) + shift,
                500 + shift);

            for (int i = 0; i < heights.Count; i++)
            {
                var curHeight = (double)heights[i] / maxH * maxInPixels;
                var x = shift / 2 + i * (scale + indent);
                var y = hist.Height - (int)curHeight - shift / 2;
                Rectangle rect = new(x, y, scale, (int)curHeight);
                var color = new MCvScalar(0, 0, 255);

                CvInvoke.Rectangle(hist, rect, color);
                CvInvoke.PutText(hist, $"{heights[i]:0}", new Point(x - 5, y - 3), Emgu.CV.CvEnum.FontFace.HersheyDuplex, 0.3f, color);
                CvInvoke.PutText(hist, $"{yMin + i:0}", new Point(x - 10, y + (int)curHeight + 10), Emgu.CV.CvEnum.FontFace.HersheyDuplex, 0.3f, color);
            }
            //CvInvoke.Imshow("Hist", hist);
            //CvInvoke.WaitKey(0);

            CvInvoke.Imwrite(@"F:\\hist.jpg", hist);
        }

        public static Image<Bgr, byte> Create2DMap(Mapper2DInputModel model)
        {
            var points = model.Points;
            points.Sort((x, y) => x.Y.CompareTo(y.Y));

            BuildHistorramm(points);

            int removedUp = (int)(points.Count * 0.25);
            int removedDown = (int)(points.Count * 0.2);

            points.RemoveRange(0, removedDown);
            points.RemoveRange(points.Count - removedUp, removedUp);

            var minX = Math.Abs(GetMinOf3(Math.Abs(points.MinBy(p => p.X).X), model.Start.X, model.Aim?.X));
            var maxX = GetMaxOf3(points.MaxBy(p => p.X).X, model.Start.X, model.Aim?.X);
            var minZ = Math.Abs(GetMinOf3(points.MinBy(p => p.Z).Z, model.Start.Y, model.Aim?.Y));
            var maxZ = GetMaxOf3(points.MaxBy(p => p.Z).Z, model.Start.Y, model.Aim?.Y);

            var scale = model.Scale;
            var shift = 20;

            Image<Bgr, byte> map = new ((int)(maxX + minX) * scale + shift, (int)(maxZ + minZ) * scale + shift);

            for (int i = 0; i < points.Count; i++)
            {
                var x = points[i].X;
                var y = points[i].Z;

                y = TransformToImageCoordinateZ(y, maxZ, scale, shift);

                x = TransformToImageCoordinateX(x, minX, scale, shift);

                map[(int)y, (int)x] = new Bgr(Color.Red);
            }

            if (model.Aim.HasValue)
            {
                var yAim = TransformToImageCoordinateZ(model.Aim.Value.Y, maxZ, scale, shift);
                var xAim = TransformToImageCoordinateX(model.Aim.Value.X, minX, scale, shift);
                CvInvoke.Circle(map, new Point((int)xAim, (int)yAim), 2, new MCvScalar(0, 255, 0), 2);
            }

            var yStart = TransformToImageCoordinateZ(model.Start.Y, maxZ, scale, shift);
            var xStart = TransformToImageCoordinateX(model.Start.X, minX, scale, shift);

            CvInvoke.Circle(map, new Point((int)xStart, (int)yStart), 2, new MCvScalar(255, 0, 0), 2);

            if (model.NeedRoute)
            {
                for (int i = 0; i < model.RoutePoints!.Length - 1; i++)
                {
                    var p1 = model.RoutePoints![i];
                    var p1Y = TransformToImageCoordinateZ(p1.Y, maxZ, scale, shift);
                    var p1X = TransformToImageCoordinateX(p1.X, minX, scale, shift);

                    var p2 = model.RoutePoints![i + 1];
                    var p2Y = TransformToImageCoordinateZ(p2.Y, maxZ, scale, shift);
                    var p2X = TransformToImageCoordinateX(p2.X, minX, scale, shift);

                    CvInvoke.Line(map, new Point((int)p1X, (int)p1Y), new Point((int)p2X, (int)p2Y), new MCvScalar(0, 255, 0));
                }
            }

            return map;
        }
    }
}
