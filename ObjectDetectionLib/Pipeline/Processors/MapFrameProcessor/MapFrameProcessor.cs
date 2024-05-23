using ObjectDetectionLib.Cartographer.Models;
using ObjectDetectionLib.Cartographer.PointsCombine;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.Pipeline.Abstractions;
using ObjectDetectionLib.Pipeline.InitValueModels;

namespace ObjectDetectionLib.Pipeline.Processors.MapFrameProcessor
{
    public class MapFrameProcessor : IFrameProcessor
    {
        public ProcessorResult Process(IFramePipelineContext context)
        {
            var cloud = context.GetInitValue<FramePipelineInitValue>().CloudModel;
            var points = context.GetProcessorResult<PointsCloudModel>();

            if (cloud.IsEmpty)
            {
                context.AddProcessorResult(points.Cloud!);
                return ProcessorResult.SUCCESS;
            }
            
            PointsCombinerInputModel model = new () { FirstCloud =  cloud, SecondCloud = points, MegringSize = points.CloudDimensions };

            var combined = PointsCombiner.CombineClouds(model);

            context.AddProcessorResult(combined);

            return ProcessorResult.SUCCESS;
        }
    }
}
