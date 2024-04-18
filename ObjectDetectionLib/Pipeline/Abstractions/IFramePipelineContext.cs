
namespace ObjectDetectionLib.FramePipeline.Abstractions
{
    public interface IFramePipelineContext
    {
        void AddValue<T>(string key, T value) where T : notnull;

        void AddProcessorResult<T>(T result) where T : notnull;

        void AddInitValue<T>(T value) where T : notnull;

        T GetValue<T> (string key);

        T GetProcessorResult<T> () where T : notnull;

        T GetInitValue<T>();

        Dictionary<string, object> GetAllProcessorsResult ();

        void ClearContext();
    }
}
