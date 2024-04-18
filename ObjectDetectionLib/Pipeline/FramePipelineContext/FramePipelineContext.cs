
using ObjectDetectionLib.FramePipeline.Abstractions;

namespace ObjectDetectionLib.Pipeline.FramePipelineContext
{
    public class FramePipelineContext : IFramePipelineContext
    {
        private readonly Dictionary<string, object> _items = [];
        private int StepsCount = 0;

        private const string STEP_NAME = "step";
        private const string PROCESSOR_RESULT = "last";
        private const string INIT_VALUE = "init";

        public void AddValue<T>(string key, T value) where T : notnull
        {
            _items[key] = value;
        }

        public T GetValue<T>(string key)
        {
            if (_items.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"No objects in context with key {key}");
        }

        public void AddProcessorResult<T>(T result) where T: notnull
        {
            if (StepsCount != 0)
            {
                var step = _items[PROCESSOR_RESULT];
                _items[STEP_NAME + StepsCount.ToString()] = step;
            }
            _items[PROCESSOR_RESULT] = result;

            StepsCount++;
        }

        public T GetProcessorResult<T>() where T : notnull
        {
            return (T)_items[PROCESSOR_RESULT];
        }

        public void AddInitValue<T>(T value) where T : notnull
        {
            _items[INIT_VALUE] = value;
        }

        public T GetInitValue<T>()
        {
            return (T)_items[INIT_VALUE];
        }

        public Dictionary<string, object> GetAllProcessorsResult()
        {
            Dictionary<string, object> results = [];

            for (int i = 1; i < StepsCount; i++)
            {
                string currentKey = STEP_NAME + i.ToString();
                results[currentKey] = _items[currentKey];
            }

            if (_items.TryGetValue(PROCESSOR_RESULT, out var value))
            {
                results[STEP_NAME + StepsCount.ToString()] = value;
            };

            return results;
        }

        public void ClearContext()
        {
            StepsCount = 0;
            _items.Clear();
        }
    }
}
