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

            foreach (var step in _pipelineSteps)
            {
                var result = step.Process(context);

                if (result == ProcessorResult.NO_RESULT)
                {
                    return new PipelineResult
                    {
                        ResultStatus = result,
                    };
                }
            }

            PipelineResult pipelineResult = new()
            {
                ResultStatus = ProcessorResult.SUCCESS,

                Results = new Dictionary<string, object>(context.GetAllProcessorsResult())
            };

            context.ClearContext();

            return pipelineResult;
        }
    }
}
