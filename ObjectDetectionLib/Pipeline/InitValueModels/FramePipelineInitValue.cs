
using Emgu.CV.Structure;
using ObjectDetectionLib.Cartographer.Models;
using ObjectDetectionLib.InputModels;

namespace ObjectDetectionLib.Pipeline.InitValueModels
{
    public class FramePipelineInitValue
    {
        public required FrameInfo FrameInfo { get; init; }

        public required string AIM { get; init; }

        public required double HorizontalFrontOfView { get; init; }

        public required double VerticalFrontOfView { get; init; }

        public required PointsCloudModel CloudModel { get; init; }
    }
}
