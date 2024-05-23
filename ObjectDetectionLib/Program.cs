using Microsoft.ML;
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
using ObjectDetectionLib.FrameSourceResolver;
using System.Diagnostics;
using ObjectDetectionLib.Pipeline.Processors.MapFrameProcessor;
using Microsoft.ML.Data;
using System.IO;
using ObjectDetectionLib.ML.YoloV4.PostProcessor;
using Emgu.CV;
using ObjectDetectionLib.Extensions;
using ObjectDetectionLib.EmguCV;

string assetsRelativePath = @"../../../ML/assets";
string assetsPath = GetAbsolutePath(assetsRelativePath);

var midasModelPath = Path.Combine(assetsPath, "Models", MidasConfiguration.ModelName);
var yoloModalPath = Path.Combine(assetsPath, "Models", YoloConfiguration.ModelName);

var imagesFolder = Path.Combine(assetsPath, "testImages");


#region Working pipeline

Console.WriteLine("Enter absolute path to configuration file:");
string? path = Console.ReadLine();

if (!ValidateConfigPath(path))
{
    Console.WriteLine("Incorrect input. File not exist or not JSON.");
    return;
}

Stopwatch st = Stopwatch.StartNew();

FramesSource data = FramesSource.LoadFromFile(path!);
MLContext mlContext = new();

var yoloScorer = new YoloV4OnnxScorer(mlContext, yoloModalPath);
var midasScorer = new MidasOnnxScorer(mlContext, midasModelPath);

Console.WriteLine($"{DateTime.Now}: Creating YOLO engine...");
using var yoloEngine = yoloScorer.CreateEngine();
Console.WriteLine($"{DateTime.Now}: Done.");

Console.WriteLine($"{DateTime.Now}: Creating MiDaS engine...");
using var midasEngine = midasScorer.CreateEngine();
Console.WriteLine($"{DateTime.Now}: Done.");

Console.WriteLine($"{DateTime.Now}: Creating pipeline...");
IFramePipelineContext context = new FramePipelineContext();
FramePipeline pipeline = new (context);

IFrameProcessor detectionProcessor = new DetectionFrameProcessor(yoloEngine);
IFrameProcessor depthProcessor = new DepthFrameProcessor(midasEngine);
IFrameProcessor pointsProcessor = new PointsFrameProcessor();
IFrameProcessor mapProcessor = new MapFrameProcessor();

pipeline.AddStep(detectionProcessor);
pipeline.AddStep(depthProcessor);
pipeline.AddStep(pointsProcessor);
pipeline.AddStep(mapProcessor);

st.Stop();

Console.WriteLine($"Done. Execution Time (s): {st.Elapsed.TotalSeconds}");
Console.WriteLine($"{DateTime.Now}: -- Start Processing --");
st.Start();
StaticFrameSourceResolver.StartStaticProcess(data, pipeline);
st.Stop();
Console.WriteLine($"{DateTime.Now}: Done. Execution Time (s): {st.Elapsed.TotalSeconds}");

#endregion

#region Depth Estimation
//MLContext mlContext = new();
//var midasScorer = new MidasOnnxScorer(mlContext, midasModelPath);
//
//Console.WriteLine("Creating MiDaS engine...");
//using var midasEngine = midasScorer.CreateEngine();
//Console.WriteLine("Done.");
//
//var imagePaths = new string[] { "frame21.jpg", "frame22.jpg", "frame23.jpg" };
//
//foreach (var path in imagePaths)
//{
//    var image = MLImage.CreateFromFile(Path.Combine(imagesFolder, path));
//    var ext = Path.GetExtension(path);
//
//    Stopwatch stopwatch = Stopwatch.StartNew();
//    Console.WriteLine($"{DateTime.Now}: Start prediction...");
//
//    var result = midasEngine.Predict(new MidasInputData(image));
//
//    stopwatch.Stop();
//
//    Console.WriteLine($"{DateTime.Now}: Done. Total estimation time (s): {stopwatch.Elapsed.TotalSeconds}");
//
//    var postResult = MidasPostProcessor.GetResult(result, new Size(image.Width, image.Height));
//    Mat pureDepths = MidasPostProcessor.ConvertFloatsToMat(postResult.PredictedDepth);
//    CvInvoke.Imwrite(Path.Combine(imagesFolder, path.Replace(ext, "_depth" + ext)), pureDepths);
//}
#endregion

#region Object Detection

//MLContext mlContext = new();
//
//var yoloScorer = new YoloV4OnnxScorer(mlContext, yoloModalPath);
//
//Console.WriteLine($"{DateTime.Now}: Creating YOLO engine...");
//using var yoloEngine = yoloScorer.CreateEngine();
//Console.WriteLine($"{DateTime.Now}: Done.");
//
//var mat = new Mat(Path.Combine(imagesFolder, "cat.jpg"));
//var image = mat.ToMLImage();
//var result = yoloEngine.Predict(new ObjectDetectionLib.ML.YoloV4.ModelData.YoloInputData(image));
//
//var postResults = YoloPostProcessor.GetResults(result);
//
//var boxes = CvDrawer.DrawBoxesOnly(postResults.Select(r => r.Rect), mat);
//
//var cat = postResults.FirstOrDefault();
//CvInvoke.PutText(mat, $"Label: {cat.Label}; Confidence: {cat.Confidence}",
//    new System.Drawing.Point((int)cat!.Left, (int)cat.Top), Emgu.CV.CvEnum.FontFace.HersheyDuplex, 1.0, new Emgu.CV.Structure.MCvScalar(0, 0, 0), 2);
//
//CvInvoke.Imwrite(Path.Combine(imagesFolder, "cat_result.jpg"), boxes);

#endregion

static string GetAbsolutePath(string relativePath)
{
    FileInfo _dataRoot = new (typeof(Program).Assembly.Location);
    string assemblyFolderPath = _dataRoot.Directory.FullName;

    string fullPath = Path.Combine(assemblyFolderPath, relativePath);

    return fullPath;
}

static bool ValidateConfigPath(string? path)
{
    return !string.IsNullOrEmpty(path) && 
        File.Exists(path) && 
        (Path.GetExtension(path).Equals(".json"));
}
