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
using ObjectDetectionLib.Cartographer.Models;

namespace ObjectDetectionLib.Pipeline.Processors.PointsFrameProcessor
{
    public class PointsFrameProcessor : IFrameProcessor
    {
        private static Matrix<double> CalculateCameraDirectionUnit(Matrix<double> origin, Matrix<double> target)
        {
            var lenght = Math.Sqrt(Math.Pow(target.Data[0, 0] - origin.Data[0, 0], 2) + Math.Pow(target.Data[2, 0] - origin.Data[2, 0], 2));

            return new Matrix<double>([
                (target.Data[0, 0] - origin.Data[0, 0]) / lenght,
                0,
                (target.Data[2, 0] - origin.Data[2, 0]) / lenght]);
        }

        private static Matrix<double> CalculateCameraUpUnit()
        {
            return new Matrix<double>([0, 1, 0]);
        }

        private static Matrix<double> CalculateCameraRightUnit(Matrix<double> direction, Matrix<double> up)
        {
            var dirX = direction.Data[0, 0];
            var dirY = direction.Data[1, 0];
            var dirZ = direction.Data[2, 0];

            var upX = up.Data[0, 0];
            var upY = up.Data[1, 0];
            var upZ = up.Data[2, 0];

            return new Matrix<double>([(upY * dirZ - upZ * dirY), (upZ * dirX - upX * dirZ), (upX * dirY - upY * dirX)]);
            //return new Matrix<double>([(dirY * upZ - dirZ * upY), (dirZ * upX - dirX * upZ), (dirX * upY - dirY * upX)]);
        }

        private static double GetDirectionLenght(Matrix<double> origin, Matrix<double> target)
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

        private static float[] GetNormalizedDepth(float[] depths, double limit = 40)
        {
            using Mat depthMat = new (MidasConfiguration.ModelDimension, MidasConfiguration.ModelDimension, DepthType.Cv32F, 1);
            Marshal.Copy(depths, 0, depthMat.DataPointer, depths.Length);

            var min = depths.Min() / limit;
            var max = depths.Max() / limit;

            Console.WriteLine($"Normalazing process: MIN: {min}; MAX: {max}");

            CvInvoke.Normalize(depthMat, depthMat, 0, limit, NormType.MinMax, DepthType.Cv32F);

            float[] normalizedDepths = new float[depths.Length];
            Marshal.Copy(depthMat.DataPointer, normalizedDepths, 0, normalizedDepths.Length);

            return normalizedDepths;
        }

        public ProcessorResult Process(IFramePipelineContext context)
        {
            var frameInfo = context.GetInitValue<FramePipelineInitValue>();

            var aimPointIndex = context.GetProcessorResult<DepthEstimationResult>().AimResizedIndex;
            MCvPoint2D64f? aimPoint = null;

            var cameraOrigin = frameInfo.FrameInfo.CameraOrigin;

            var cameraDirectionUnit = CalculateCameraDirectionUnit(cameraOrigin, frameInfo.FrameInfo.Target);
            var cameraUpUnit = CalculateCameraUpUnit();
            var cameraRightUnit = CalculateCameraRightUnit(cameraDirectionUnit, cameraUpUnit);

            var lenght = GetDirectionLenght(cameraOrigin, frameInfo.FrameInfo.Target);

            var cameraDirection = cameraDirectionUnit * lenght;

            var halfFoVV = frameInfo.VerticalFrontOfView / 2;
            var cameraUp = cameraUpUnit * lenght * Math.Tan(halfFoVV * Math.PI / 180);

            var halfFoVH = frameInfo.HorizontalFrontOfView / 2;
            var cameraRight = cameraRightUnit * lenght * Math.Tan(halfFoVH * Math.PI / 180);

            var depthsResult = context.GetProcessorResult<DepthEstimationResult>();

            List<MCvPoint3D64f> points = [];

            double halfWidth = depthsResult.ResultImageSize.Width / 2;
            double halfHeight = depthsResult.ResultImageSize.Height / 2;

            (int widthLimit, int heightLimit) = GetDimensionsLimits(depthsResult.ResultImageSize);

            var straightDepth = GetNormalizedDepth(MidasPostProcessor.InvertDepth(depthsResult.PredictedDepth));

            for (int y = 0; y < depthsResult.ResultImageSize.Height; y++)
            {
                for (int x = 0; x < depthsResult.ResultImageSize.Width; x++)
                {
                    Matrix<double> d = cameraDirection + 
                        cameraRight * (x - halfWidth + 0.5) / halfWidth + 
                        cameraUp * -(y - halfHeight + 0.5) / halfHeight;

                    CvInvoke.Normalize(d, d);

                    var depthIndex = y * depthsResult.ResultImageSize.Width + x + (depthsResult.ResultImageSize.Width * heightLimit);
                    var currentDepth = straightDepth[depthIndex] + 10;

                    var point = cameraOrigin + currentDepth * d;

                    if (depthIndex == aimPointIndex)
                    {
                        aimPoint = new MCvPoint2D64f(point[0, 0], point[2, 0]);
                    }

                    var xC = (float)point.Data[0, 0];
                    var yC = (float)point.Data[1, 0];
                    var zC = (float)point.Data[2, 0];

                    points.Add(new (xC, yC, zC));
                }
            }

            PointsCloudModel cloud = new()
            {
                Cloud = points,
                IsEmpty = false,
                Origin = cameraOrigin,
                Target = frameInfo.FrameInfo.Target,
                AimPoint = aimPoint,
            };

            context.AddProcessorResult(cloud);

            return ProcessorResult.SUCCESS;
        }
    }
}
