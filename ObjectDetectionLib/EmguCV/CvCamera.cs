using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.ML;
using ObjectDetectionLib.ML.YoloV4.OnnxScorer;
using ObjectDetectionLib.ML.YoloV4.PostProcessor;
using ObjectDetectionLib.ML.YoloV4.Results;
using ObjectDetectionLib.ML.Midas.ModelData;
using ObjectDetectionLib.Extensions;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.Pipeline.Abstractions;
using Microsoft.ML.Data;

namespace ObjectDetectionLib.EmguCV
{
    public class CvCamera
    {
        public static int CamIndex { get; set; } = 0;

        public static void StartCameraProcess(IFramePipeline pipeline, string? url)
        {
            VideoCapture capture = string.IsNullOrEmpty(url) ? new(CamIndex) : new (url);

            using Mat frame = new ();

            while (true)
            {
                capture.Read(frame);

                if (!frame.IsEmpty)
                {
                    MLImage image = frame.ConvertMatToMLImage();
                    var result = pipeline.ExecutePipeline(image);

                    if (result != null)
                    {
                        Console.WriteLine($"{DateTime.Now}: -- Results --;");

                        if (result.ResultStatus == ProcessorResult.SUCCESS)
                        {
                            var detections = (IReadOnlyCollection<ObjectDetectionResult>)result.Results["step1"];
                            var depths = (float[])result.Results["step2"];

                            for (int i = 0; i < detections.Count; i++)
                            {
                                Console.WriteLine($"Results: label {detections.ElementAt(i).Label}; depth: {depths[i]}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No results (nothing detected);");
                        }
                    }
                }

                if (CvInvoke.WaitKey(10) >= 0) break;
            }
        }

        //Mb move to Yolo PostProcessor
        private static Image<Bgr, byte> DrawBoxes(int index, IReadOnlyList<ObjectDetectionResult> results, Mat src)
        {
            Image<Bgr, byte> img = src.ToImage<Bgr, byte>();

            foreach (var res in results)
            {
                CvInvoke.Rectangle(img, res.Rect, new MCvScalar(255, 0, 0));
            }

            return img;
        }

        private static void ProcessFramePack(List<Mat> pack, YoloV4OnnxScorer scorer, MLContext mLContext)
        {
            
            var inputs = pack.Select(m => m.ConvertMatToMLImage()).Select(ml => new MidasInputData(ml));

            var data = mLContext.Data.LoadFromEnumerable(inputs);

            var results = scorer.Score(data);

            var processedResults = results.Select(r => YoloPostProcessor.GetResults(r)).ToArray();

            Parallel.For(0, pack.Count, (index) => DrawBoxes(index, processedResults[index], pack[index]));

            return;
        }
    }
}
