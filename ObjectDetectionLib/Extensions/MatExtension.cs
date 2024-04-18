
using Microsoft.ML.Data;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ObjectDetectionLib.Extensions
{
    public static class MatExtension
    {
        public static MLImage ConvertMatToMLImage(this Mat mat)
        {
            Image<Bgr, byte> emguImage = mat.ToImage<Bgr, byte>();
            byte[] imageBytes = emguImage.ToJpegData();

            using MemoryStream ms = new (imageBytes);

            return MLImage.CreateFromStream(ms);
        }
    }
}
