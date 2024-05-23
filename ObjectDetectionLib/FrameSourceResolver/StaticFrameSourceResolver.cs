using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.InputModels;
using ObjectDetectionLib.Pipeline.InitValueModels;
using Emgu.CV.Structure;
using ObjectDetectionLib.Cartographer.Models;
using ObjectDetectionLib.Cartographer.Mapper;
using Emgu.CV;
using System.Drawing;

namespace ObjectDetectionLib.FrameSourceResolver
{
    public class StaticFrameSourceResolver
    {
        public static void StartStaticProcess(FramesSource source, IFramePipeline pipeline)
        {
            PointsCloudModel cloudModel = new ();
            int detectionIndex = -1;

            for (int i = 0; i < source.Frames.Count; i++)
            {
                Console.WriteLine($"{DateTime.Now}: Begin frame {i + 1} processing...");
                var result = pipeline.ExecutePipeline(

                    new FramePipelineInitValue() { 
                        AIM = source.AIM,
                        FrameInfo = source.Frames.ElementAt(i),
                        HorizontalFrontOfView = source.HorizontalFrontOfView,
                        VerticalFrontOfView = source.VerticalFrontOfView,
                        CloudModel = cloudModel,
                    });

                Console.WriteLine($"{DateTime.Now}: Done.");

                var points = (PointsCloudModel)result.Results["step3"];

                cloudModel = new PointsCloudModel() 
                { 
                    Cloud = (List<MCvPoint3D64f>)result.Results["step4"]!,
                    Origin = points.Origin,
                    Target = points.Target,
                    IsEmpty = false,
                    AimPoint = points.AimPoint,
                };

                if (result.ResultStatus == Pipeline.Abstractions.ProcessorResult.SUCCESS )
                {
                    detectionIndex = i;
                    break;
                }
            }

            if (source.Save3DMap)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($"{DateTime.Now}: Writing 3D map (points) to {source.Path3DMap}");
                Console.ForegroundColor = color;

                for (int k = 0; k < cloudModel.Cloud.Count; k++)
                {
                    var curPoint = cloudModel.Cloud[k];
                    File.AppendAllText(source.Path3DMap, $"{curPoint.X};{curPoint.Y};{curPoint.Z}\n");
                }
                Console.WriteLine($"{DateTime.Now}: Done.");
            }

            MCvPoint2D64f[]? routePoints = null;

            if (detectionIndex >= 0)
            {
                routePoints = new MCvPoint2D64f[detectionIndex + 2];
                for (int i = 0; i < detectionIndex + 1; i++)
                {
                    var current = source.Frames.ElementAt(i).CameraOrigin;
                    routePoints[i] = new MCvPoint2D64f(current[0, 0], current[2, 0]);
                }
                routePoints[^1] = cloudModel.AimPoint!.Value;
            }

            var origin = source.Frames.ElementAt(0).CameraOrigin;
            var map2D = Mapper2D.Create2DMap(new Mapper2DInputModel()
            {
                Points = cloudModel.Cloud,
                Scale = 10,
                Start = new MCvPoint2D64f(origin[0, 0], origin[2, 0]),
                Aim = cloudModel.AimPoint,
                RoutePoints = routePoints
            });

            if (source.Save2DMap)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($"{DateTime.Now}: Writing 2D map (points) to {source.Path2DMap}");
                Console.ForegroundColor = color;

                CvInvoke.Imwrite(source.Path2DMap, map2D);
                Console.WriteLine($"{DateTime.Now}: Done.");
            }

            if (detectionIndex < 0)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now}: Aim not detected.");
                Console.ForegroundColor = color;
            }
            else
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{DateTime.Now}: Aim ({source.AIM.ToUpper()}) detected of frame {detectionIndex + 1}.");
                Console.ForegroundColor = color;
            }
        }
    }
}
