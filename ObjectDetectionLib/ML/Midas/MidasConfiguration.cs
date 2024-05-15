namespace ObjectDetectionLib.ML.Midas
{
    public static class MidasConfiguration
    {
        public const string ModelName = "dpt_swin2_base_384.onnx";

        public const int ModelDimension = 384;

        public record MidasImageSettings
        {
            public const int ImageWidth = 384;
            public const int ImageHeight = 384;
        }

        public record MidasModelsDataSettings
        {
            public const string InputColumnName = "input_image";
            public const string OutputColumnName = "output_depth";
        }
    }
}
