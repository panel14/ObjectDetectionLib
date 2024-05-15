using Emgu.CV;
using ObjectDetectionLib.ML.YoloV4.Results;
using ObjectDetectionLib.Extensions;
using ObjectDetectionLib.FramePipeline.Abstractions;
using ObjectDetectionLib.Pipeline.Abstractions;
using Microsoft.ML.Data;

namespace ObjectDetectionLib.FrameSourceResolver
{
    public class CvCameraSourceResolver
    {
        public static int CamIndex { get; set; } = 0;

        public static Mat CaptureFrame()
        {
            using VideoCapture capture = new(CamIndex);

            Mat frame = new();

            capture.Read(frame);

            return frame;
        }

        public static void StartCameraProcess(IFramePipeline pipeline, string? url)
        {
            using VideoCapture capture = string.IsNullOrEmpty(url) ? new(CamIndex) : new(url);

            using Mat frame = new();

            while (true)
            {
                capture.Read(frame);

                if (!frame.IsEmpty)
                {
                    MLImage image = frame.ToMLImage();
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
    }
}
