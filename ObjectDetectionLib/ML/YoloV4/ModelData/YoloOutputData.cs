using Microsoft.ML.Data;

namespace ObjectDetectionLib.ML.YoloV4.ModelData
{
    public class YoloOutputData
    {
        [ColumnName("Identity:0")]
        [VectorType(1, 52, 52, 3, 85)]
        public float[] Output0 { get; set; } = [];

        [ColumnName("Identity_1:0")]
        [VectorType(1, 26, 26, 3, 85)]
        public float[] Output1 { get; set; } = [];

        [ColumnName("Identity_2:0")]
        [VectorType(1, 13, 13, 3, 85)]
        public float[] Output2 { get; set; } = [];

        [ColumnName("width")]
        public float ImageWidth { get; set; }

        [ColumnName("height")]
        public float ImageHeight { get; set; }
    }
}
