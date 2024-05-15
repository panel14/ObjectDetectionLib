using Microsoft.ML;
using Microsoft.ML.Data;
using ObjectDetectionLib.Extensions;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.ML.YoloV4.ModelData;
using ObjectDetectionLib.ML.YoloV4.PostProcessor;
using ObjectDetectionLib.Pipeline.Abstractions;
using ObjectDetectionLib.Pipeline.InitValueModels;

namespace ObjectDetectionLib.Pipeline.Processors.DetectionFrameProcessor
{
    public class DetectionFrameProcessor(PredictionEngine<YoloInputData, YoloOutputData> yoloEngine) 
        : IFrameProcessor
    {
        public ProcessorResult Process(IFramePipelineContext context)
        {
            FramePipelineInitValue init = context.GetInitValue<FramePipelineInitValue>();
            using MLImage frame = init.FrameInfo.Frame.ToMLImage();

            var yoloPrediction = yoloEngine.Predict(new YoloInputData(frame));
            var yoloResults = YoloPostProcessor.GetResults(yoloPrediction);

            var filteredResults = yoloResults.Where(r => r.Label == init.AIM).ToList();
            
            context.AddProcessorResult(filteredResults);

            if (filteredResults.Count > 0) {
                return ProcessorResult.SUCCESS;
            }

            return ProcessorResult.NO_RESULT;
        }
    }
}
