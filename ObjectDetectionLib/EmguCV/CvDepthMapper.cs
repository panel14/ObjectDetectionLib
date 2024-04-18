using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace ObjectDetectionLib.EmguCV
{
    public record SGBMOptions()
    {
        public int MinDisparity { get; set; } = 0;
        public int NumDisparities { get; set; } = 0;
        public int BlockSize { get; set; } = 2;
        public int P1 { get; set; } = 0;
        public int P2 { get; set; } = 0;
        public int Disp12MaxDiff { get; set; } = 0;
        public int PreFilterCap { get; set; } = 0;
        public int UniquenessRatio { get; set; } = 0;
        public int SpeckleWindowSize { get; set; } = 0;
        public int SpeckleRange { get; set; } = 0;
    }

    public class CvDepthMapper
    {
        public SGBMOptions CalibratedOptions { get; set; }

        public bool IsCalibrated { get; private set; }

        public CvDepthMapper()
        {
            CalibratedOptions = new SGBMOptions();
            IsCalibrated = false;
        }

        private StereoSGBM InitStereoSGBM(SGBMOptions? options = null)
        {
            return new StereoSGBM(options?.MinDisparity ?? 0,
                options?.NumDisparities ?? CalibratedOptions.NumDisparities,
                options?.BlockSize ?? CalibratedOptions.BlockSize,
                options?.P1 ?? CalibratedOptions.P1,
                options?.P2 ?? CalibratedOptions.P2,
                options?.Disp12MaxDiff ?? CalibratedOptions.Disp12MaxDiff,
                options?.PreFilterCap ?? CalibratedOptions.PreFilterCap,
                options?.UniquenessRatio ?? CalibratedOptions.UniquenessRatio,
                options?.SpeckleWindowSize ?? CalibratedOptions.SpeckleWindowSize,
                options?.SpeckleRange ?? CalibratedOptions.SpeckleRange,
                StereoSGBM.Mode.HH);
        }

        private void PrintOptions(SGBMOptions options)
        {
            Console.WriteLine($"SGBM Options:\n" +
                $"MinDisparity: {options.MinDisparity}\n" +
                $"NumDisparities: {options.NumDisparities}" +
                $"BlockSize: {options.BlockSize}" +
                $"P1: {options.P1}" +
                $"P2: {options.P2}" +
                $"Disp12MaxDiff: {options.Disp12MaxDiff}" +
                $"PreFilterCap: {options.PreFilterCap}" +
                $"UniquenessRatio: {options.UniquenessRatio}" +
                $"SpeckleWindowSize: {options.SpeckleWindowSize}" +
                $"SpeckleRange: {options.SpeckleRange}");
        }

        private double GetRealThreshold(double a, double b)
        {
            var min = double.Min(a, b);
            var max = double.Max(a, b);

            return min / max;
        }

        public void CalibrateSGBM(Mat left, Mat right, Rectangle interest,
            double focal, double baseline, double distanse, double threshold = 0.9)
        {
            var options = new SGBMOptions();

            int bestMinDisparity = 0;
            int bestNumDisparities = 1;
            int bestBlockSize = 2;

            double bestThreshold = 0;

            for (int curMinDisparity = -1; curMinDisparity < 10; curMinDisparity++)
            {
                options.MinDisparity = curMinDisparity;
                
                for (int curNumDisparities = 16; curNumDisparities < 256; curNumDisparities++)
                {
                    options.NumDisparities = curNumDisparities;

                    for (int curBlockSize = 2; curBlockSize < 10; curBlockSize++)
                    {
                        options.BlockSize = curBlockSize;

                        var curDistanse = GetDepth(left, right, interest, focal, baseline, options);

                        if (curDistanse.HasValue)
                        {
                            var curThreshold = GetRealThreshold(curDistanse.Value, distanse);

                            if (curThreshold >= threshold)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"{DateTime.Now}: GOOD THRESHOLD! on params:" +
                                    $"{curMinDisparity}; " +
                                    $"{curNumDisparities}; " +
                                    $"{curBlockSize}; COMPUTED DISTANSE: {curDistanse} (real threashold: {curThreshold})");
                            }
                            else if (curThreshold - threshold > 0.3)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"{DateTime.Now}: MIDDLE THRESHOLD on params:" +
                                    $"{curMinDisparity}; " +
                                    $"{curNumDisparities}; " +
                                    $"{curBlockSize}; COMPUTED DISTANSE: {curDistanse} (real threashold: {curThreshold})");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"{DateTime.Now}: BAD THRESHOLD on params:" +
                                    $"{curMinDisparity}; " +
                                    $"{curNumDisparities}; " +
                                    $"{curBlockSize}; COMPUTED DISTANSE: {curDistanse} (real threashold: {curThreshold} )");
                            }

                            if (curThreshold > bestThreshold)
                            {
                                bestThreshold = curThreshold;
                                bestMinDisparity = curMinDisparity;
                                bestNumDisparities = curNumDisparities;
                                bestBlockSize = curBlockSize;
                            }
                            
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.Now}: Can't count distanse with params: " +
                                $"{curMinDisparity}; " +
                                $"{curNumDisparities}; " +
                                $"{curBlockSize};");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            Console.WriteLine($"{DateTime.Now}: END OF CALIBRATION; " +
                $"Besties: Threshold{bestThreshold}" +
                $"MinDisparity: {bestMinDisparity}" +
                $"NumDisparities: {bestNumDisparities}" +
                $"BlockSize: {bestBlockSize}");
        }

        public Mat GetDisparityMapAsMat(Mat left, Mat right, SGBMOptions? options = null)
        {
            using var stereoSGBM = InitStereoSGBM(options);

            using Mat disparity = new ();

            stereoSGBM.Compute(left, right, disparity);

            Mat result = new();

            CvInvoke.Normalize(disparity, result, 0, 255, NormType.MinMax, DepthType.Cv8U);

            return result;
        }

        private Mat GetDisparityMap(Mat left, Mat right, SGBMOptions? options = null)
        {
            using var stereoSGBM = InitStereoSGBM(options);

            Mat disparity = new();

            stereoSGBM.Compute (left, right, disparity);

            return disparity;
        }

        public double? GetDepth(Mat left, Mat right, Point interest,
            double focal = 800, double baseline = 0.1, SGBMOptions? options = null)
        {
           using Mat disparity = GetDisparityMap(left, right, options);

           short? d = (short?) disparity.GetData().GetValue(interest.Y, interest.X);

           if (d.HasValue && d.Value > 0)
           {
               return (focal * baseline) / d.Value;
           }

           return null;
        }

        public double? GetDepth(Mat left, Mat right, Rectangle interest,
            double focal = 800, double baseline = 0.1, SGBMOptions? options=null)
        {
            using Mat disparity = GetDisparityMap(left, right, options);

            var shorts = (short[,])disparity.GetData();

            short? d = 0;

            for (int i = interest.Top; i < interest.Bottom; i++)
            {
                for (int j = interest.Left; j < interest.Right; j++)
                {
                    short? current = shorts[j, i];

                    if (current.HasValue) 
                    {
                        d += current.Value;
                    }
                }
            }

            short average = (short) (d / (interest.Width * interest.Height));

            return (focal * baseline) / average;
        }
    }
}
