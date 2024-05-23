using Emgu.CV;
using Emgu.CV.Structure;
using ObjectDetectionLib.Cartographer.Models;

namespace ObjectDetectionLib.Cartographer.PointsCombine
{
    public static class PointsCombiner
    {
        const double DeltaMin = 1;
        const double DeltaMax = 4;

        private static double GetDistance(MCvPoint3D64f pointA, MCvPoint3D64f pointB)
        {
            return Math.Sqrt(
                Math.Pow(pointA.X - pointB.X, 2) +
                Math.Pow(pointA.Y - pointB.Y, 2) +
                Math.Pow(pointA.Z - pointB.Z, 2));
        }

        private static MCvPoint3D64f GetPointBetween(MCvPoint3D64f pointA, MCvPoint3D64f pointB)
        {
            return new MCvPoint3D64f((pointA.X + pointB.X) / 2, (pointA.Y + pointB.Y) / 2, (pointA.Z + pointB.Z) / 2);
        }

        private static bool IsVectorsCodirected(Matrix<double> originA, Matrix<double> targetA, Matrix<double> originB, Matrix<double> targetB)
        {
            var directionA = targetA - originA;
            var directionB = targetB - originB;

            var isCollinear = directionA[0, 0] / directionB[0, 0] == directionA[2, 0] / directionB[2, 0];

            var scallarMultiply = directionA[0, 0] * directionB[0, 0] + directionA[2, 0] * directionB[2, 0];

            return isCollinear && scallarMultiply > 0;
        }

        public static List<MCvPoint3D64f> CombineClouds(PointsCombinerInputModel model)
        {
            var mainCloud = model.FirstCloud.Cloud!;
            var newCloud = model.SecondCloud.Cloud!;

            //var isCodir = IsVectorsCodirected(model.FirstCloud.Origin, model.FirstCloud.Target, model.SecondCloud.Origin, model.SecondCloud.Target);

            for (int i = 0; i < newCloud.Count; i++)
            {
                //var accDistance = -1.0;
                //MCvPoint3D64f nearest = new();
                //for (int j = 0; j < newCloud.Count; j++)
                //{
                //    var distance = GetDistance(mainCloud[i], newCloud[j]);
                //
                //    if (distance < accDistance) 
                //    { 
                //        accDistance = distance;
                //        nearest = newCloud[j];
                //    }
                //
                //    //if (isCodir)
                //    //{
                //    //    var distance = GetDistance(mainCloud[i], newCloud[i]);
                //    //
                //    //    if (distance <= DeltaMin)
                //    //    {
                //    //        mainCloud.Add(GetPointBetween(mainCloud[i], newCloud[i]));
                //    //    }
                //    //    else if (distance >= DeltaMax)
                //    //    {
                //    //        mainCloud[i] = newCloud[i];
                //    //    }
                //    //    else
                //    //    {
                //    //        mainCloud.Add(newCloud[i]);
                //    //    }
                //    //}
                //    //mainCloud.Add(newCloud[i]);
                //}
                //if (accDistance != -1)
                //{
                //    mainCloud[i] = GetPointBetween(mainCloud[i], nearest);
                //}

                mainCloud.Add(newCloud[i]);
            }

            return mainCloud;//resultCloud;
        }
    }
}
