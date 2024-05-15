using Emgu.CV;
using Microsoft.ML;
using Microsoft.ML.Data;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.InputModels;
using ObjectDetectionLib.ML.Midas;
using ObjectDetectionLib.ML.Midas.OnnxScorer;
using ObjectDetectionLib.ML.YoloV4;
using ObjectDetectionLib.ML.YoloV4.OnnxScorer;
using ObjectDetectionLib.Pipeline.FramePipeline;
using ObjectDetectionLib.Pipeline.FramePipelineContext;
using ObjectDetectionLib.Pipeline.Processors.DepthFrameProcessor;
using ObjectDetectionLib.Pipeline.Processors.DetectionFrameProcessor;
using ObjectDetectionLib.Pipeline.Processors.PointsFrameProcessor;
using ObjectDetectionLib.ML.Midas.ModelData;
using System.Drawing;
using ObjectDetectionLib.ML.Midas.PostProcessor;
using ObjectDetectionLib.FrameSourceResolver;

string assetsRelativePath = @"../../../ML/assets";
string assetsPath = GetAbsolutePath(assetsRelativePath);

var midasModelPath = Path.Combine(assetsPath, "Models", MidasConfiguration.ModelName);
var yoloModalPath = Path.Combine(assetsPath, "Models", YoloConfiguration.ModelName);

var imagesFolder = Path.Combine(assetsPath, "testImages", "Depth");
var calibrationImagesFolder = Path.Combine(assetsPath, "testImages", "Calibration", "chessNewDataSet");
var imagesDepthResults = Path.Combine(imagesFolder, "results");
var outputFolderEngine = Path.Combine(assetsPath, "outputImagesEngine");
var outputFolderPack = Path.Combine(assetsPath, "outputImagesPack");


#region Working pipeline

FramesSource data = FramesSource.LoadFromFile(@"F:\\file.json");
MLContext mlContext = new();

var yoloScorer = new YoloV4OnnxScorer(mlContext, yoloModalPath);
var midasScorer = new MidasOnnxScorer(mlContext, midasModelPath);

Console.WriteLine("Creating YOLO engine...");
using var yoloEngine = yoloScorer.CreateEngine();
Console.WriteLine("Done.");

Console.WriteLine("Creating MiDaS engine...");
using var midasEngine = midasScorer.CreateEngine();
Console.WriteLine("Done.");

Console.WriteLine("Creating pipeline...");
IFramePipelineContext context = new FramePipelineContext();
FramePipeline pipeline = new (context);

IFrameProcessor detectionProcessor = new DetectionFrameProcessor(yoloEngine);
IFrameProcessor depthProcessor = new DepthFrameProcessor(midasEngine);
IFrameProcessor pointsProcessor = new PointsFrameProcessor();

pipeline.AddStep(detectionProcessor);
pipeline.AddStep(depthProcessor);
pipeline.AddStep(pointsProcessor);

Console.WriteLine("Done.");

var image = MLImage.CreateFromFile(Path.Combine(imagesFolder, "frame1.png"));
var result = midasEngine.Predict(new MidasInputData(image));
var postResult = MidasPostProcessor.GetResult(result, new Size(image.Width, image.Height));

var inverted = MidasPostProcessor.InvertDepth(postResult.PredictedDepth);

Mat pureDepths = MidasPostProcessor.ConvertFloatsToMat(postResult.PredictedDepth);
Mat invertedDepths = MidasPostProcessor.ConvertFloatsToMat(inverted);

CvInvoke.Imwrite(Path.Combine(imagesFolder, "pureDepth.jpg"), pureDepths);
CvInvoke.Imwrite(Path.Combine(imagesFolder, "invertedDepth.jpg"), invertedDepths);
StaticFrameSourceResolver.StartStaticProcess(data, pipeline);

#endregion

static string GetAbsolutePath(string relativePath)
{
    FileInfo _dataRoot = new (typeof(Program).Assembly.Location);
    string assemblyFolderPath = _dataRoot.Directory.FullName;

    string fullPath = Path.Combine(assemblyFolderPath, relativePath);

    return fullPath;
}
