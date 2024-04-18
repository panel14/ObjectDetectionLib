
using Microsoft.ML.Data;

namespace ObjectDetectionLib.ML.Midas.ModelData
{
    public class MidasOutputData
    {
        [ColumnName("output_depth")]
        public float[] PredictedDepth { get; set; } = [];
    }
}
