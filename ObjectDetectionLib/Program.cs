using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.ML;
using ObjectDetectionLib.EmguCV;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.ML.Midas;
using ObjectDetectionLib.ML.Midas.OnnxScorer;
using ObjectDetectionLib.ML.YoloV4;
using ObjectDetectionLib.ML.YoloV4.OnnxScorer;
using ObjectDetectionLib.Pipeline.FramePipeline;
using ObjectDetectionLib.Pipeline.FramePipelineContext;
using ObjectDetectionLib.Pipeline.Processors.DepthFrameProcessor;
using ObjectDetectionLib.Pipeline.Processors.DetectionFrameProcessor;

string assetsRelativePath = @"../../../ML/assets";
string assetsPath = GetAbsolutePath(assetsRelativePath);

var midasModelPath = Path.Combine(assetsPath, "Models", MidasConfiguration.ModelName);
var yoloModalPath = Path.Combine(assetsPath, "Models", YoloConfiguration.ModelName);

var imagesFolder = Path.Combine(assetsPath, "testImages", "Depth");
var imagesDepthResults = Path.Combine(imagesFolder, "results");
var outputFolderEngine = Path.Combine(assetsPath, "outputImagesEngine");
var outputFolderPack = Path.Combine(assetsPath, "outputImagesPack");

//CvDepthMapper mapper = new ();
//
//SGBMOptions options = new()
//{
//    NumDisparities = 32,
//    BlockSize = 10,
//    Disp12MaxDiff = 10,
//    PreFilterCap = 0,
//    SpeckleWindowSize = 60,
//    SpeckleRange = 7
//};
//
//double focal = 1020.5;
//double baseline = 0.135;
//
//Mat left = CvInvoke.Imread(Path.Combine(imagesFolder, "lightL.jpg"));
//Mat right = CvInvoke.Imread(Path.Combine(imagesFolder, "lightR.jpg"));
//
//Point interest = new(679, 251);
//
//Rectangle rectangle = new(535, 401, 228, 274);
//
//mapper.CalibrateSGBM(left, right, rectangle, focal, baseline, 0.25);

MLContext mlContext = new();

var yoloScorer = new YoloV4OnnxScorer(mlContext, yoloModalPath);
var midasScorer = new MidasOnnxScorer(mlContext, midasModelPath);

var yoloEngine = yoloScorer.CreateEngine();
var midasEngine = midasScorer.CreateEngine();

IFramePipelineContext context = new FramePipelineContext();
FramePipeline pipeline = new (context);

IFrameProcessor detectionProcessor = new DetectionFrameProcessor(yoloEngine);
IFrameProcessor depthProcessor = new DepthFrameProcessor(midasEngine);

pipeline.AddStep(detectionProcessor);
pipeline.AddStep(depthProcessor);

CvCamera.StartCameraProcess(pipeline, "http://192.168.0.5:8080/video");

static string GetAbsolutePath(string relativePath)
{
    FileInfo _dataRoot = new (typeof(Program).Assembly.Location);
    string assemblyFolderPath = _dataRoot.Directory.FullName;

    string fullPath = Path.Combine(assemblyFolderPath, relativePath);

    return fullPath;
}

static void RenameFiles(string filePath)
{
    var files = Directory.GetFiles(filePath).Select((value, index) => new { value, index });

    foreach(var file in files)
    {
        string newName = Path.Combine(filePath, $"image{file.index + 1}.jpg");

        if (!File.Exists(newName))
            File.Move(file.value, newName);
    }
}

static Mat ConvertFloatsToMat(float[] points, int width, int height, int modelDimension)
{
    using Mat depthMat = new(modelDimension, modelDimension, DepthType.Cv32F, 1);

    System.Runtime.InteropServices.Marshal.Copy(points, 0, depthMat.DataPointer, points.Length);

    CvInvoke.Normalize(depthMat, depthMat, 0, 255, NormType.MinMax, DepthType.Cv32F);

    Mat displayMat = new();
    depthMat.ConvertTo(displayMat, DepthType.Cv8U);


    //Mat resizedMat = new ();
    //CvInvoke.Resize(displayMat, resizedMat, new Size(width, height), 0, 0, Inter.Cubic);

    return displayMat;
}
