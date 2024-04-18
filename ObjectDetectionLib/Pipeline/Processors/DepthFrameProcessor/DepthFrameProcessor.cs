
using Microsoft.ML;
using Microsoft.ML.Data;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.ML.Midas.ModelData;
using ObjectDetectionLib.ML.YoloV4.Results;
using ObjectDetectionLib.Pipeline.Abstractions;
using ObjectDetectionLib.ML.Midas.PostProcessor;

namespace ObjectDetectionLib.Pipeline.Processors.DepthFrameProcessor
{
    public class DepthFrameProcessor(PredictionEngine<MidasInputData, MidasOutputData> midasEngine) : IFrameProcessor
    {
        public ProcessorResult Process(IFramePipelineContext context)
        {
            var detectionResults = context.GetProcessorResult<IReadOnlyCollection<ObjectDetectionResult>>();

            float[] depths = new float[detectionResults.Count];

            using MLImage image = context.GetInitValue<MLImage>();
            var depthMap = midasEngine.Predict(new MidasInputData(image));

            for (int i = 0; i < detectionResults.Count; i++)
            {
                var rect = detectionResults.ElementAt(i).Rect;
                var depth = MidasPostProcessor.GetDepth(depthMap!, image.Width, image.Height, rect);
                depths[i] = depth;
            }

            context.AddProcessorResult(depths);

            return ProcessorResult.SUCCESS;
        }
    }
}
