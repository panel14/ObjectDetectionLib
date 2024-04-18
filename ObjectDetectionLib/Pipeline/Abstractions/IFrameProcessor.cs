
using ObjectDetectionLib.Pipeline.Abstractions;

namespace ObjectDetectionLib.FramePipeline.Abstractions
{
    public interface IFrameProcessor
    {
        ProcessorResult Process(IFramePipelineContext context);
    }
}
