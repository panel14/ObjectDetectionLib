using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using Microsoft.ML.Transforms.Onnx;
using ObjectDetectionLib.ML.YoloV4.ModelData;

namespace ObjectDetectionLib.ML.YoloV4.OnnxScorer
{
    public class YoloV4OnnxScorer(MLContext mLContext, string modelPath)
    {
        private EstimatorChain<OnnxTransformer>? _pipeline;

        EstimatorChain<OnnxTransformer> Pipeline { 
            get 
            {
                _pipeline ??= CreatePipeLine();
                return _pipeline;
            }
        }

        record InputYoloV4ImageSettings
        {
            public const int ImageWidth = 416;
            public const int ImageHeight = 416;
        }

        record OutputYoloV4Columns
        {
            public const string Output1 = "Identity:0";
            public const string Output2 = "Identity_1:0";
            public const string Output3 = "Identity_2:0";
        }

        private EstimatorChain<OnnxTransformer> CreatePipeLine(int? gpuDeviceId = null)
        {
            var pipeline = mLContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: "input_1:0",
                imageWidth: InputYoloV4ImageSettings.ImageWidth, imageHeight: InputYoloV4ImageSettings.ImageHeight, resizing: ImageResizingEstimator.ResizingKind.IsoPad)

                .Append(mLContext.Transforms.ExtractPixels("input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                .Append(mLContext.Transforms.ApplyOnnxModel(

                    // For yolov4 model
                    shapeDictionary: new Dictionary<string, int[]>()
                    {
                        { "input_1:0", [1, 416, 416, 3] },
                        { "Identity:0", [1, 52, 52, 3, 85] },
                        { "Identity_1:0", [1, 26, 26, 3, 85] },
                        { "Identity_2:0", [1, 13, 13, 3, 85] },
                    },

                    inputColumnNames: ["input_1:0"],

                    outputColumnNames: ["Identity:0", "Identity_1:0", "Identity_2:0"],

                    modelFile: modelPath,
                    fallbackToCpu: (gpuDeviceId == null),

                    gpuDeviceId: gpuDeviceId

                    ));

            return pipeline ?? throw new ApplicationException("Error during creating pipeline");
        }

        private IEnumerable<YoloOutputData> PredictDataUsingModel(IDataView testData, ITransformer model)
        {
            IDataView predictions = model.Transform(testData);

            var predictedData = mLContext.Data.CreateEnumerable<YoloOutputData>(predictions, reuseRowObject: false);

            return predictedData;
        }

        public IEnumerable<YoloOutputData> Score(IDataView data, int? gpuDeviceId = null)
        {
            var model = Pipeline?.Fit(mLContext.Data.LoadFromEnumerable(new List<YoloInputData>()));

            return PredictDataUsingModel(data, model);
        }

        public PredictionEngine<YoloInputData, YoloOutputData> CreateEngine(int? gpuDeviceId = null)
        {
            var pipeline = CreatePipeLine(gpuDeviceId);
            var model = pipeline.Fit(mLContext.Data.LoadFromEnumerable(new List<YoloInputData>()));
            var predictionEngine = mLContext.Model.CreatePredictionEngine<YoloInputData, YoloOutputData>(model);

            return predictionEngine;
        }
    }
}
