using Emgu.CV.Structure;
using ObjectDetectionLib.Cartographer.PointsCombine.Combiner;
using ObjectDetectionLib.Cartographer.PointsCombine.Models;
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

            if (cloud.IsEmpty)
            {
                return ProcessorResult.NO_RESULT;
            }

            var points = context.GetProcessorResult<PointsCloudModel>();
            
            PointsCombinerInputModel model = new () { FirstCloud =  cloud, SecondCloud = points };

            var combined = PointsCombiner.CombineClouds(model);

            // Добавить в резолвер класс модели облака и записывать туда каждый последний ориджин и таргет с PointsFrameProcessor

            //var resultCloud = PointsCombiner.CombineClouds(cloud, points);

            return ProcessorResult.SUCCESS;
        }
    }
}
