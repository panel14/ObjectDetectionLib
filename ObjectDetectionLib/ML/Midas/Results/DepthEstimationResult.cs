using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace ObjectDetectionLib.ML.Midas.Results
{
    public class DepthEstimationResult
    {
        public required float[] PredictedDepth { get; init; }

        public required Size OriginImageSize { get; init; }

        public required Size ResultImageSize { get; init; }

        public int? AimResizedIndex { get; init; }
    }
}
