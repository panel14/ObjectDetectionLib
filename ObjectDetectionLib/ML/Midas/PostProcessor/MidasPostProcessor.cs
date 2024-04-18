
using ObjectDetectionLib.ML.Midas.ModelData;
using System.Drawing;

namespace ObjectDetectionLib.ML.Midas.PostProcessor
{
    public static class MidasPostProcessor
    {
        private static Point GetMidlePoint(Rectangle rectangle)
        {
            int x = rectangle.Left + rectangle.Width / 2;
            int y = rectangle.Top + rectangle.Height / 2;

            return new Point(x, y);
        }

        public static float GetDepth(MidasOutputData data, int srcWidth, int srcHeight, Rectangle? area = null)
        {
            if (data.PredictedDepth.Length > 0)
            {
                if (area.HasValue)
                {
                    int newHeight;
                    int newWidth;

                    if (srcHeight > srcWidth)
                    {
                        float scaleCoef = (float)MidasConfiguration.ModelDimension / srcHeight;
                        newHeight = MidasConfiguration.ModelDimension;
                        newWidth = (int) (srcWidth * scaleCoef);
                    }
                    else
                    {
                        float scaleCoef = (float)MidasConfiguration.ModelDimension / srcWidth;
                        newHeight = (int)(srcHeight * scaleCoef);
                        newWidth = MidasConfiguration.ModelDimension;
                    }

                    float widthScale = newWidth / (float)srcWidth;
                    float heightScale = newHeight / (float)srcHeight;

                    var middlePoint = GetMidlePoint(area.Value);

                    var convertedMiddlePoint = new Point((int)(middlePoint.X * widthScale), (int)(middlePoint.Y * heightScale));

                    return data.PredictedDepth[newWidth * convertedMiddlePoint.Y + convertedMiddlePoint.X];
                }

                return data.PredictedDepth.Max();
            }

            return 0;
        }
    }
}
