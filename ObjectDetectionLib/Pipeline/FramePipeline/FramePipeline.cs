using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.Pipeline.Abstractions;

namespace ObjectDetectionLib.Pipeline.FramePipeline
{
    public class FramePipeline(IFramePipelineContext context) : IFramePipeline
    {
        private readonly List<IFrameProcessor> _pipelineSteps = [];

        public void AddStep(IFrameProcessor frameProcessor)
        {
            _pipelineSteps.Add(frameProcessor);
        }

        public PipelineResult ExecutePipeline<T>(T initValue) where T : notnull
        {
            context.AddInitValue(initValue);

            ProcessorResult processorResult = ProcessorResult.NO_RESULT;
            List<ProcessorResult> results = [];


            foreach (var step in _pipelineSteps)
            {
                var result = step.Process(context);
                results.Add(result);
            }

            PipelineResult pipelineResult = new()
            {
                ResultStatus = results.All(r => r == ProcessorResult.SUCCESS) ? ProcessorResult.SUCCESS : ProcessorResult.NO_RESULT,

                Results = new Dictionary<string, object>(context.GetAllProcessorsResult())
            };

            context.ClearContext();

            return pipelineResult;
        }
    }
}
