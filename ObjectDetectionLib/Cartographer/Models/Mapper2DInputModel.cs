using Emgu.CV.Structure;

namespace ObjectDetectionLib.Cartographer.Models
{
    public class Mapper2DInputModel
    {
        public required List<MCvPoint3D64f> Points { get; init; }

        public int Scale { get; init; } = 10;

        public required MCvPoint2D64f Start { get; init; }

        public required MCvPoint2D64f? Aim { get; set; }
        public bool NeedRoute => RoutePoints != null;

        public MCvPoint2D64f[]? RoutePoints { get; init; }
    }
}
