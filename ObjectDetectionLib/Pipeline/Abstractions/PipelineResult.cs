
namespace ObjectDetectionLib.Pipeline.Abstractions
{
    public class PipelineResult
    {
        public ProcessorResult ResultStatus { get; set; } = ProcessorResult.NO_RESULT;

        public Dictionary<string, object> Results { get; set; } = [];
    }
}
