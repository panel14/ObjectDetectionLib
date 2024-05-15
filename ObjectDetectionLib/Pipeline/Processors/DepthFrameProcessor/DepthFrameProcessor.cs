using Microsoft.ML;
using Microsoft.ML.Data;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.ML.Midas.ModelData;
using ObjectDetectionLib.Pipeline.Abstractions;
using ObjectDetectionLib.Pipeline.InitValueModels;
using ObjectDetectionLib.Extensions;
using ObjectDetectionLib.ML.Midas.PostProcessor;
using System.Drawing;

namespace ObjectDetectionLib.Pipeline.Processors.DepthFrameProcessor
{
    public class DepthFrameProcessor(PredictionEngine<MidasInputData, MidasOutputData> midasEngine) : IFrameProcessor
    {
        public ProcessorResult Process(IFramePipelineContext context)
        {
            using MLImage image = context.GetInitValue<FramePipelineInitValue>().FrameInfo.Frame.ToMLImage();
            var depthMap = midasEngine.Predict(new MidasInputData(image));

            context.AddProcessorResult(MidasPostProcessor.GetResult(depthMap, new Size(image.Width, image.Height)));

            return ProcessorResult.SUCCESS;
        }
    }
}
