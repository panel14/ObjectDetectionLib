using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;
using Microsoft.ML.Transforms.Image;
using ObjectDetectionLib.ML.Midas.ModelData;

namespace ObjectDetectionLib.ML.Midas.OnnxScorer
{
    public class MidasOnnxScorer(MLContext mLContext, string modelPath)
    {

        private EstimatorChain<OnnxTransformer> CreatePipeline()
        {
            var pipeline = mLContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: MidasConfiguration.MidasModelsDataSettings.InputColumnName,
                imageWidth: MidasConfiguration.MidasImageSettings.ImageWidth, imageHeight: MidasConfiguration.MidasImageSettings.ImageHeight, 
                resizing: ImageResizingEstimator.ResizingKind.IsoPad)
                .Append(mLContext.Transforms.ExtractPixels(outputColumnName: MidasConfiguration.MidasModelsDataSettings.InputColumnName))
                .Append(mLContext.Transforms.ApplyOnnxModel(

                    shapeDictionary: new Dictionary<string, int[]>
                    {
                        { MidasConfiguration.MidasModelsDataSettings.InputColumnName, [1, 3, MidasConfiguration.MidasImageSettings.ImageWidth, MidasConfiguration.MidasImageSettings.ImageHeight] },
                        { MidasConfiguration.MidasModelsDataSettings.OutputColumnName, [1, MidasConfiguration.MidasImageSettings.ImageWidth, MidasConfiguration.MidasImageSettings.ImageHeight] }
                    },

                    modelFile: modelPath,

                    inputColumnName: MidasConfiguration.MidasModelsDataSettings.InputColumnName,

                    outputColumnName: MidasConfiguration.MidasModelsDataSettings.OutputColumnName

                    ));

            return pipeline;
        }

        public PredictionEngine<MidasInputData, MidasOutputData> CreateEngine()
        {
            var pipeline = CreatePipeline();
            var model = pipeline.Fit(mLContext.Data.LoadFromEnumerable(new List<MidasInputData>()));
            var predictionEngine = mLContext.Model.CreatePredictionEngine<MidasInputData, MidasOutputData>(model);

            return predictionEngine;
        }
    }
}
