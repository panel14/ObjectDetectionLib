using Emgu.CV;
using Emgu.CV.Structure;
using ObjectDetectionLib.Cartographer.PointsCombine.Models;

namespace ObjectDetectionLib.Cartographer.PointsCombine.Combiner
{
    public static class PointsCombiner
    {
        private static MCvPoint3D64f[,] Transform(MCvPoint3D64f[,] cloud, Matrix<double> transformation)
        {
            for (int i = 0; i < cloud.GetLength(0); i++)
            {
                for (int j = 0; j < cloud.GetLength(1); j++)
                {
                    cloud[i, j] = new MCvPoint3D64f(
                        cloud[i, j].X + transformation.Data[0, 0],
                        cloud[i, j].Y + transformation.Data[1, 0],
                        cloud[i, j].Z + transformation.Data[2, 0]);
                }
            }
            return cloud;
        }

        public static MCvPoint3D64f[,] CombineClouds(PointsCombinerInputModel model)
        {
            var cloudsOffset = model.SecondCloud.Origin - model.FirstCloud.Origin;

            var secondWithOffset = Transform(model.SecondCloud.Cloud!, cloudsOffset);



            return new MCvPoint3D64f[,] { };
        }
    }
}
