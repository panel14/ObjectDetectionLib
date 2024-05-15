using HelixToolkit.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PointsVisualization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PointsVisual3D[] clouds = new PointsVisual3D[2];
            Color[] colors = [Colors.Red, Colors.Green];

            for (int j = 0; j < 5; j++)
            {
                var pointsStr = File.ReadAllText($@"F:\\points{j + 1}.txt").Trim().Split("\n");
                Point3DCollection datalist = [];

                for (int i = 0; i < pointsStr.Length; i += 20)
                {
                    var pConverted = pointsStr[i].Split(';').Select(double.Parse).ToArray();
                    datalist.Add(new Point3D(pConverted[2], pConverted[0], pConverted[1]));
                }
                clouds[j] = new ()
                {
                    Points = datalist,
                    Size = 3,
                    Color = colors[j]
                };

                viewPort.Children.Add(clouds[j]);
            }

            
        }
    }
}