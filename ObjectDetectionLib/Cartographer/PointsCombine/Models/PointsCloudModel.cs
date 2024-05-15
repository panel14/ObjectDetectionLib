using Emgu.CV.Structure;
using Emgu.CV;

namespace ObjectDetectionLib.Cartographer.PointsCombine.Models
{
    public class PointsCloudModel(MCvPoint3D64f[,] cloud, Matrix<double> origin, Matrix<double> target)
    {
        public MCvPoint3D64f[,]? Cloud { get; private set; } = cloud;

        public Matrix<double>? Origin { get; private set; } = origin;

        public Matrix<double>? Target { get; private set; } = target;

        public bool IsEmpty { get; private set; } = true;

        public PointsCloudModel() : this(null, null, null)
        {
        }

        public void SetCloudWithDirection(PointsCloudModel model)
        {
            Cloud = model.Cloud;
            Origin = model.Origin;
            Target = model.Target;

            IsEmpty = false;
        }

        public static PointsCloudModel GetEmpty()
        {
            return new PointsCloudModel() { IsEmpty = true };
        }
    }
}
