
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System.Drawing;

namespace ObjectDetectionLib.ML.YoloV4.ModelData
{
    public class YoloInputData(MLImage image)
    {
        [ColumnName("bitmap")]
        [ImageType(416, 416)]
        public MLImage Image { get; } = image;

        [ColumnName("width")]
        public float ImageWidth => Image.Width;

        [ColumnName("height")]
        public float ImageHeight => Image.Height;
    }
}
