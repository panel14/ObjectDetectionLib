
using System.Drawing;

namespace ObjectDetectionLib.Cartographer.Models
{
    public class PointsCombinerInputModel
    {
        public required PointsCloudModel FirstCloud { get; init; }

        public required PointsCloudModel SecondCloud { get; init; }

        public required Size MegringSize { get; init; }
    }
}
