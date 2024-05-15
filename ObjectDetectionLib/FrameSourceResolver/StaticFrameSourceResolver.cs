using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.InputModels;
using ObjectDetectionLib.Pipeline.InitValueModels;
using ObjectDetectionLib.Pipeline.Abstractions;
using Emgu.CV.Structure;
using ObjectDetectionLib.ML.Midas.Results;
using ObjectDetectionLib.Cartographer.PointsCombine.Models;

namespace ObjectDetectionLib.FrameSourceResolver
{
    public class StaticFrameSourceResolver
    {
        public static void StartStaticProcess(FramesSource source, IFramePipeline pipeline, string pointsFilename = @"F:\\points*.txt")
        {
            PointsCloudModel cloudModel = PointsCloudModel.GetEmpty();

            for (int i = 0; i < source.Frames.Count; i++)
            {
                Console.WriteLine($"Begin frame {i + 1} processing...");
                var result = pipeline.ExecutePipeline(

                    new FramePipelineInitValue() { 
                        AIM = source.AIM,
                        FrameInfo = source.Frames.ElementAt(i),
                        HorizontalFrontOfView = source.HorizontalFrontOfView,
                        VerticalFrontOfView = source.VerticalFrontOfView,
                        CloudModel = cloudModel,
                    });

                Console.WriteLine("Done.");

                var dimensions = (DepthEstimationResult)result.Results["step2"];
                var points = (PointsCloudModel)result.Results["step3"];

                //cloudModel.SetCloudWithDirection(points);
                Console.WriteLine("Writing points to file...");
                for (int j = 0; j < dimensions.ResultImageSize.Height; j++)
                {
                    for (int k = 0; k < dimensions.ResultImageSize.Width; k++)
                    {
                        var curPoint = points.Cloud![j, k];
                        File.AppendAllText(pointsFilename.Replace("*", (i + 1).ToString()), $"{curPoint.X};{curPoint.Y};{curPoint.Z}\n");
                    }
                }
                Console.WriteLine("Done.");
            }
        }
    }
}
