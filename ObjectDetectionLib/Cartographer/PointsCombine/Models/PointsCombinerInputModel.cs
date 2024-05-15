
namespace ObjectDetectionLib.Cartographer.PointsCombine.Models
{
    public class PointsCombinerInputModel
    {
        public required PointsCloudModel FirstCloud { get; init; }

        public required PointsCloudModel SecondCloud { get; init; }
    }
}
