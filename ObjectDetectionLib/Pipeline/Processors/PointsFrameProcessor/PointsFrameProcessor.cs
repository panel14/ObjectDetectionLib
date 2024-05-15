using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.ML.Midas;
using ObjectDetectionLib.ML.Midas.Results;
using ObjectDetectionLib.Pipeline.Abstractions;
using ObjectDetectionLib.Pipeline.InitValueModels;
using System.Drawing;
using System.Runtime.InteropServices;
using ObjectDetectionLib.ML.Midas.PostProcessor;
using ObjectDetectionLib.Cartographer.PointsCombine.Models;

namespace ObjectDetectionLib.Pipeline.Processors.PointsFrameProcessor
{
    public class PointsFrameProcessor : IFrameProcessor
    {
        private static double GetVectorLength(Matrix<double> vector)
        {
            double xC = Math.Pow(vector.Data[0, 0], 2);
            double yC = Math.Pow(vector.Data[1, 0], 2);
            double zC = Math.Pow(vector.Data[2, 0], 2);

            return Math.Sqrt(xC + yC + zC);
        }

        private static Matrix<double> CalculateCameraDirectionUnit(Matrix<double> origin, Matrix<double> target)
        {
            var lenght = Math.Sqrt(Math.Pow(target.Data[0, 0] - origin.Data[0, 0], 2) + Math.Pow(target.Data[2, 0] - origin.Data[2, 0], 2));

            return new Matrix<double>([
                (target.Data[0, 0] - origin.Data[0, 0]) / lenght,
                0,
                (target.Data[2, 0] - origin.Data[2, 0]) / lenght]);
        }

        private static Matrix<double> CalculateCameraUpUnit(Matrix<double> direction, double angle)
        {
            return new Matrix<double>([0, 1, 0]);
        }

        private static Matrix<double> CalculateCameraRightUnit(Matrix<double> direction, Matrix<double> up, double angle)
        {
            var dirX = direction.Data[0, 0];
            var dirY = direction.Data[1, 0];
            var dirZ = direction.Data[2, 0];

            var upX = up.Data[0, 0];
            var upY = up.Data[1, 0];
            var upZ = up.Data[2, 0];

            return new Matrix<double>([(dirY * upZ - dirZ * upY), (dirZ * upX - dirX * upZ), (dirX * upY - dirY * upX)]);
        }

        private double GetDirectionLenght(Matrix<double> origin, Matrix<double> target)
        {
            var xProection = target.Data[0, 0] - origin.Data[0, 0];
            var zProection = target.Data[2, 0] - origin.Data[2, 0];

            return Math.Sqrt(Math.Pow(xProection, 2) + Math.Pow(zProection, 2));
        }

        private static (int, int) GetDimensionsLimits(Size depthMapSize)
        {
            int max = int.Max(depthMapSize.Width, depthMapSize.Height);
            int min = int.Min(depthMapSize.Width, depthMapSize.Height);

            int boards = (max - min) / 2;

            int heightLimit = 0;
            int widthLimit = 0;

            if (depthMapSize.Width > depthMapSize.Height)
            {
                heightLimit = boards;
            }
            else
            {
                widthLimit = boards;
            }

            return (widthLimit, heightLimit);
        }

        private static float[] GetNormalizedDepth(float[] depths, double limit = 100)
        {
            using Mat depthMat = new (MidasConfiguration.ModelDimension, MidasConfiguration.ModelDimension, DepthType.Cv32F, 1);
            Marshal.Copy(depths, 0, depthMat.DataPointer, depths.Length);

            CvInvoke.Normalize(depthMat, depthMat, 0, limit, NormType.MinMax, DepthType.Cv32F);

            float[] normalizedDepths = new float[depths.Length];
            Marshal.Copy(depthMat.DataPointer, normalizedDepths, 0, normalizedDepths.Length);

            return normalizedDepths;
        }

        public ProcessorResult Process(IFramePipelineContext context)
        {
            var frameInfo = context.GetInitValue<FramePipelineInitValue>();

            var cameraOrigin = frameInfo.FrameInfo.CameraOrigin;

            var cameraDirectionUnit = CalculateCameraDirectionUnit(cameraOrigin, frameInfo.FrameInfo.Target);
            var cameraUpUnit = CalculateCameraUpUnit(cameraDirectionUnit, frameInfo.VerticalFrontOfView);
            var cameraRightUnit = CalculateCameraRightUnit(cameraDirectionUnit, cameraUpUnit, frameInfo.HorizontalFrontOfView);

            var lenght = GetDirectionLenght(cameraOrigin, frameInfo.FrameInfo.Target);

            var cameraDirection = cameraDirectionUnit * lenght;
            var cameraUp = cameraUpUnit * lenght;
            var cameraRight = cameraRightUnit * lenght;

            var depthsResult = context.GetProcessorResult<DepthEstimationResult>();

            MCvPoint3D64f[,] points = new MCvPoint3D64f[depthsResult.ResultImageSize.Height, depthsResult.ResultImageSize.Width];

            double halfWidth = depthsResult.ResultImageSize.Width / 2;
            double halfHeight = depthsResult.ResultImageSize.Height / 2;

            (int widthLimit, int heightLimit) = GetDimensionsLimits(depthsResult.ResultImageSize);

            var straightDepth = GetNormalizedDepth(MidasPostProcessor.InvertDepth(depthsResult.PredictedDepth));

            for (int y = 0; y < depthsResult.ResultImageSize.Height; y++)
            {
                for (int x = 0; x < depthsResult.ResultImageSize.Width; x++)
                {
                    Matrix<double> d = cameraDirection + 
                        cameraRight * (x - halfWidth) / halfWidth + 
                        cameraUp * (y - halfHeight) / halfHeight;

                    CvInvoke.Normalize(d, d);

                    var point = cameraOrigin +
                        //Индекс берется из рассчета, что ширина больше высоты (костыль - нужно поправить)
                        straightDepth[y * depthsResult.ResultImageSize.Width + x + (depthsResult.ResultImageSize.Width * heightLimit)] * d;

                    var xC = (float)point.Data[0, 0];
                    var yC = (float)point.Data[1, 0];
                    var zC = (float)point.Data[2, 0];

                    points[y, x] = new MCvPoint3D32f(xC, yC, zC);
                }
            }

            PointsCloudModel cloud = new (points, cameraOrigin, frameInfo.FrameInfo.Target);

            context.AddProcessorResult(cloud);

            return ProcessorResult.SUCCESS;
        }
    }
}
