using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.Drawing;
using ObjectDetectionLib.EmguCV.Exceptions;

namespace ObjectDetectionLib.EmguCV.DepthEstimation.DepthMap
{
    public class CameraCalibrationResult
    {
        public Matrix<double> CameraMatrix { get; set; } = new(3, 3);

        public VectorOfFloat DistCoefs { get; set; } = new();

        public double Error { get; set; } = double.MaxValue;
    }


    public class CameraCalibrationExtendedResults : CameraCalibrationResult
    {
        public required Matrix<double> RMatrix { get; set; }
        public required Matrix<double> TVector { get; set; }
    }

    public static class CvCameraCalibrator
    {
        // Default criteria - (30, 0.01)
        // Default winsize - (11, 11)

        public static CameraCalibrationResult TestCameraCalibration(string imageDirectoryPath, int imageCount,
            Size boardSize, Size imageSize)
        {

            CameraCalibrationResult bestResult = new ();
            MCvTermCriteria bestCriteria = new ();
            Size bestWinsize = new ();

            Console.WriteLine("Start testing.");
            for (int iter = 30; iter <= 60; iter += 5)
            {
                foreach (var eps in new double[] {0.1, 0.01, 0.001, 0.0001})
                {
                    MCvTermCriteria termCriteria = new (iter, eps);

                    for (int winDimension = 11; winDimension < 13; winDimension++)
                    {
                        Size winSize = new (winDimension, winDimension);

                        var result = CalibrateCamera(imageDirectoryPath, imageCount, boardSize, imageSize, 
                            termCriteria, winSize);

                        Console.WriteLine($"Criteria: ({iter}; {eps}); Win Size: ({winDimension};{winDimension})");
                        Console.WriteLine($"Results: Error: {result.Error};");
                        if (result.Error < bestResult.Error)
                        {
                            bestResult = result;
                            bestCriteria = termCriteria;
                            bestWinsize = winSize;
                        }
                    }
                }
            }
            Console.WriteLine("End testing. Best result:");
            Console.WriteLine($"Error: {bestResult.Error}; Matrix:");
            foreach (var value in bestResult.CameraMatrix.Data)
            {
                Console.WriteLine(value.ToString());
            }
            Console.WriteLine($"Best criteria: ({bestCriteria.MaxIter}; {bestCriteria.Epsilon})");
            Console.WriteLine($"Best win size: ({bestWinsize.Width}, {bestWinsize.Height})");

            return bestResult;
        }

        public static CameraCalibrationExtendedResults CalibrateCamera(string imageDirectoryPath, int imageCount,
            Size boardSize, Size imageSize, MCvTermCriteria criteria, Size winSize, bool saveCorners = false)
        {
            var rawImages = Directory.GetFiles(imageDirectoryPath);

            List<MCvPoint3D32f> objectPoints = [];
            List<List<MCvPoint3D32f>> objectPointsList = [];
            List<VectorOfPointF> cornersList = new(imageCount);

            for (int i = 0; i < boardSize.Height; i++)
            {
                for (int j = 0; j < boardSize.Width; j++)
                {
                    objectPoints.Add(new MCvPoint3D32f(j, i, 0));
                }
            }

            Console.WriteLine($"Starting calibration with criteria ({criteria.MaxIter}, {criteria.Epsilon})");

            bool[] founded = new bool[imageCount];

            for (int i = 0; i < imageCount; i++)
            {
                Image<Gray, byte> image = new(rawImages[i]);

                VectorOfPointF corners = new();

                bool found = CvInvoke.FindChessboardCornersSB(image, boardSize, corners);

                founded[i] = found;

                Console.WriteLine($"Image {rawImages[i]}\nFind corners: {found}");

                if (found)
                {
                    objectPointsList.Add(objectPoints);

                    Size zeroSize = new(-1, -1);

                    CvInvoke.CornerSubPix(image, corners, winSize, zeroSize, criteria);

                    cornersList.Add(corners);

                    if (saveCorners)
                    {
                        CvInvoke.DrawChessboardCorners(image, boardSize, corners, true);
                        CvInvoke.Imwrite(Path.Combine(imageDirectoryPath, $"corners_{i}.jpg"), image);
                    }
                }
            }

            if (!founded.Any(x => x == true))
            {
                throw new CalibrationException("No corners found on any image");
            }

            Matrix<double> cameraMatrix = new(3, 3);
            VectorOfFloat distCoeffs = new();

            Mat[] rVecs, tVecs;

            MCvPoint3D32f[][] objectPointsArray = new MCvPoint3D32f[objectPointsList.Count][];
            for (int i = 0; i < objectPointsList.Count; i++)
            {
                objectPointsArray[i] = [.. objectPointsList[i]];
            }

            PointF[][] cornersListArray = new PointF[cornersList.Count][];
            for (int i = 0; i < cornersList.Count; i++)
            {
                cornersListArray[i] = cornersList[i].ToArray();
            }

            double error = CvInvoke.CalibrateCamera(objectPointsArray, cornersListArray,
                imageSize, cameraMatrix, distCoeffs, CalibType.RationalModel, criteria, out rVecs, out tVecs);
 
            Matrix<double> translationVector = new(tVecs[0].Size);
            Matrix<double> rotationMatrix = new(3, 3);
            
            CvInvoke.Rodrigues(rVecs[0], rotationMatrix);
            tVecs[0].CopyTo(translationVector);
            
            return new CameraCalibrationExtendedResults
            {
                Error = error,
                CameraMatrix = cameraMatrix,
                DistCoefs = distCoeffs,
                RMatrix = rotationMatrix,
                TVector = translationVector
            };         
        }
    }
}
