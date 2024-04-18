namespace ObjectDetectionLib.ML.Midas
{
    public static class MidasConfiguration
    {
        public const string ModelName = "dpt_swin2_tiny_256.onnx";

        public const int ModelDimension = 256;

        public record MidasImageSettings
        {
            public const int ImageWidth = 256;
            public const int ImageHeight = 256;
        }

        public record MidasModelsDataSettings
        {
            public const string InputColumnName = "input_image";
            public const string OutputColumnName = "output_depth";
        }
    }
}
