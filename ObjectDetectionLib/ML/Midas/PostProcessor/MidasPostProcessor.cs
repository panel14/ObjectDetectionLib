
using Emgu.CV.CvEnum;
using Emgu.CV;
using ObjectDetectionLib.ML.Midas.ModelData;
using ObjectDetectionLib.ML.Midas.Results;
using System.Drawing;

namespace ObjectDetectionLib.ML.Midas.PostProcessor
{
    public static class MidasPostProcessor
    {
        public static float[] InvertDepth(float[] depths)
        {
            var middle = (depths.Max() - depths.Min()) / 2;
            //var average = depths.Average();

            float[] result = new float[depths.Length];

            for (int i = 0; i < depths.Length; i++) 
            { 
                var diff = middle - depths[i];
                result[i] = depths[i] + 2 * diff;
            }
            return result;
        }

        public static Mat ConvertFloatsToMat(float[] points)
        {
            using Mat depthMat = new(MidasConfiguration.ModelDimension, MidasConfiguration.ModelDimension, DepthType.Cv32F, 1);

            System.Runtime.InteropServices.Marshal.Copy(points, 0, depthMat.DataPointer, points.Length);

            CvInvoke.Normalize(depthMat, depthMat, 0, 255, NormType.MinMax, DepthType.Cv32F);

            Mat displayMat = new();
            depthMat.ConvertTo(displayMat, DepthType.Cv8U);

            return displayMat;
        }

        private static Size ComputeNewDimensions(Size dimensions)
        {
            int newHeight;
            int newWidth;

            if (dimensions.Height > dimensions.Width)
            {
                float scaleCoef = (float)MidasConfiguration.ModelDimension / dimensions.Height;
                newHeight = MidasConfiguration.ModelDimension;
                newWidth = (int)(dimensions.Width * scaleCoef);
            }
            else
            {
                float scaleCoef = (float)MidasConfiguration.ModelDimension / dimensions.Width;
                newHeight = (int)(dimensions.Height * scaleCoef);
                newWidth = MidasConfiguration.ModelDimension;
            }

            return new Size(newWidth, newHeight);
        }

        public static DepthEstimationResult GetResult(MidasOutputData data, Size originImageSize)
        {
            return new () { 
                PredictedDepth = data.PredictedDepth,
                OriginImageSize = originImageSize,
                ResultImageSize = ComputeNewDimensions(originImageSize)
            };
        }
    }
}
