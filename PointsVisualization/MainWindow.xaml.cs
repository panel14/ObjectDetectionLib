using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using System.IO;
using System.Windows;
using SharpDX;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PointsVisualization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static double GetDistance(Vector3 point)
        {
            return Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2));
        }

        private static Color4 GetColorFromDistance(Vector3 point, double min, double max)
        {
            var distance = GetDistance(point);

            var normalized = (distance - min) / (max - min);

            return new Color4((float)normalized);
        }

        private static LineGeometry3D CreateGridGeometry(int limit = 50)
        {
            var geometry = new LineGeometry3D();
            var positions = new List<Point3D>();
            var indices = new List<int>();
            int index = 0;

            // Создаем линии по горизонтали
            for (int i = -limit; i <= limit; i++)
            {
                positions.Add(new Point3D(-limit, 0, i));
                positions.Add(new Point3D(limit, 0, i));
                indices.Add(index++);
                indices.Add(index++);
            }

            // Создаем линии по вертикали
            for (int i = -limit; i <= limit; i++)
            {
                positions.Add(new Point3D(i, 0, -limit));
                positions.Add(new Point3D(i, 0, limit));
                indices.Add(index++);
                indices.Add(index++);
            }

            geometry.Positions = new Vector3Collection(positions.Select(p => new Vector3((float)p.X, (float)p.Y, (float)p.Z)));
            geometry.Indices = new IntCollection(indices);

            return geometry;
        }

        public MainWindow()
        {
            InitializeComponent();

            int scale = 1;

            for (int j = 0; j < 1; j += 1)
            {
                var pointsStr = File.ReadAllText($@"F:\\points.txt").Trim().Split("\n");

                Vector3Collection points = [];
                Color4Collection colors = [];
                int[] indisies = new int[pointsStr.Length];

                //double max = 50;
                //double min = 0;

                for (int i = 0; i < pointsStr.Length; i += 1)
                {
                    var pConverted = pointsStr[i].Split(';').Select(double.Parse).ToArray();

                    var currentPoint = new Vector3((float)pConverted[0] / scale, (float)pConverted[1] / scale, -(float)pConverted[2] / scale);

                    points.Add(currentPoint);
                    //colors.Add(GetColorFromDistance(currentPoint, min, max));
                    //indisies[i] = i;
                }

                PointGeometry3D geometry3D = new()
                {
                    Positions = points,
                    //Colors = colors,
                    //Indices = new IntCollection(indisies)
                };

                PointGeometryModel3D pgm = new()
                {
                    Geometry = geometry3D,
                    Size = new(2, 2),
                    Color = Colors.Red
                };

                viewPort.Items.Add(pgm);

                var lines = new LineGeometryModel3D
                {
                    Geometry = CreateGridGeometry(),
                    Color = Colors.Black,
                };
                
                viewPort.Items.Add(lines);

            }
        }
    }
}