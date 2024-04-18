using System.Drawing;

namespace ObjectDetectionLib.ML.YoloV4.Results
{
    public record ObjectDetectionResult(float Left, float Top, float Right, float Buttom, string Label, float Confidence)
    {
        public float Width => Right - Left;
        public float Height => Buttom - Top;

        public Rectangle Rect => new ((int)Left, (int)Top, (int)Width, (int)Height);
    }
}
