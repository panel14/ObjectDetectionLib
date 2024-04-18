
using ObjectDetectionLib.Pipeline.Abstractions;

namespace ObjectDetectionLib.FramePipeline.Abstractions
{
    public interface IFramePipeline
    {
        void AddStep(IFrameProcessor frameProcessor);

        public PipelineResult ExecutePipeline<T>(T initValue) where T : notnull;
    }
}
