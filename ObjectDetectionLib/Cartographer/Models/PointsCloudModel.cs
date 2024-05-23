using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;

namespace ObjectDetectionLib.Cartographer.Models
{
    public class PointsCloudModel()
    {
        public List<MCvPoint3D64f> Cloud { get; set; } = [];

        public Size CloudDimensions { get; set; } = new Size(0, 0);

        public bool IsEmpty { get; set; } = true;

        public Matrix<double> Origin { get; set; } = new Matrix<double>(3, 1);

        public Matrix<double> Target { get; set; } = new Matrix<double>(3, 1);

        public MCvPoint2D64f? AimPoint { get; set; }
    }
}
