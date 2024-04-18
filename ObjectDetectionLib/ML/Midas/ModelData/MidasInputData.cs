
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;

namespace ObjectDetectionLib.ML.Midas.ModelData
{
    public class MidasInputData(MLImage image)
    {
        [ColumnName("bitmap")]
        [ImageType(MidasConfiguration.MidasImageSettings.ImageWidth, MidasConfiguration.MidasImageSettings.ImageHeight)]
        public MLImage Image { get; } = image;

        [ColumnName("width")]
        public float Width => Image.Width;

        [ColumnName("height")]
        public float Height => Image.Height;
    }
}
