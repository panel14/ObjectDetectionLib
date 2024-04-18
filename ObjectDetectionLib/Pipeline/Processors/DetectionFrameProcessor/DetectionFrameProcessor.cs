using Microsoft.ML;
using Microsoft.ML.Data;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.ML.YoloV4.ModelData;
using ObjectDetectionLib.ML.YoloV4.PostProcessor;
using ObjectDetectionLib.Pipeline.Abstractions;

namespace ObjectDetectionLib.Pipeline.Processors.DetectionFrameProcessor
{
    public class DetectionFrameProcessor(PredictionEngine<YoloInputData, YoloOutputData> yoloEngine) 
        : IFrameProcessor
    {
        public ProcessorResult Process(IFramePipelineContext context)
        {
            MLImage frame = context.GetInitValue<MLImage>();

            var yoloPrediction = yoloEngine.Predict(new YoloInputData(frame));
            var yoloResults = YoloPostProcessor.GetResults(yoloPrediction);

            if (yoloResults == null || yoloResults.Count == 0) return ProcessorResult.NO_RESULT;

            context.AddProcessorResult(yoloResults);

            return ProcessorResult.SUCCESS;
        }
    }
}
