using Emgu.CV;
using Emgu.CV.CvEnum;
using ObjectDetectionLib.EmguCV;

namespace SGMSetupUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            DepthMapper = new();
        }

        TextBox[] textBoxes;

        readonly CvDepthMapper DepthMapper;

        Mat LeftImage;
        Mat RightImage;

        Point interest;

        private int MinDisparity = 0;
        private int NumDisparities = 0;
        private int BlockSize = 0;
        private int P1 = 0;
        private int P2 = 0;
        private int Disp12MaxDiff = 0;
        private int PreFilterCap = 0;
        private int UniquenessRatio = 0;
        private int SpeckleWindowSize = 0;
        private int SpeckleRange = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if (LeftImage == null)
            {
                textBox1.ForeColor = Color.Red;
                textBox1.Text = "Not loaded";
                textBox1.ForeColor = Color.Black;
                return;
            }

            if (!checkBox1.Checked && RightImage == null)
            {
                textBox2.ForeColor = Color.Red;
                textBox2.Text = "Not loaded";
                textBox2.ForeColor = Color.Black;
                return;
            }

            SGBMOptions options = new()
            {
                MinDisparity = MinDisparity,
                NumDisparities = NumDisparities,
                BlockSize = BlockSize,
                P1 = P1,
                P2 = P2,
                Disp12MaxDiff = Disp12MaxDiff,
                PreFilterCap = PreFilterCap,
                UniquenessRatio = UniquenessRatio,
                SpeckleWindowSize = SpeckleWindowSize,
                SpeckleRange = SpeckleRange
            };

            double focal = string.IsNullOrEmpty(textBox13.Text) ? 98.63 : double.Parse(textBox13.Text);
            double baseline = string.IsNullOrEmpty(textBox14.Text) ? 0.07 : double.Parse(textBox14.Text);

            var result = DepthMapper.GetDepth(LeftImage, RightImage, interest, focal: focal, baseline: baseline, options: options);

            var mat = DepthMapper.GetDisparityMapAsMat(LeftImage, RightImage, options);

            //CvInvoke.Imwrite("result.jpg", mat);
            pictureBox1.Image = mat.ToBitmap();

            Status.Text = $"{DateTime.Now}\nDistance for interest point {result}\n";
        }

        private void SetErrorStatus(string text)
        {
            var oldColor = Status.ForeColor;

            Status.ForeColor = Color.Red;
            Status.Text = text;
            Status.ForeColor = oldColor;
        }

        private void LoadMat(string path, ref Mat frame)
        {
            if (string.IsNullOrEmpty(path))
            {
                SetErrorStatus("File path is null or empty");
                return;
            }

            if (!File.Exists(path))
            {
                SetErrorStatus("File not exist");
                return;
            }

            frame = CvInvoke.Imread(path, ImreadModes.Grayscale);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                var path = textBoxes[0].Text;

                using Mat img = CvInvoke.Imread(path, ImreadModes.Grayscale);

                Rectangle rectL = new(0, 0, img.Width / 2, img.Height);
                Rectangle rectR = new(img.Width / 2, 0, img.Width / 2, img.Height);

                LeftImage = new Mat(img, rectL);
                RightImage = new Mat(img, rectR);
            }
            else
            {
                var paths = textBoxes.Select(t => t.Text).ToArray();
                LoadMat(paths[0], ref LeftImage);
                LoadMat(paths[1], ref RightImage);
            }

            pictureBox1.Image = LeftImage.ToBitmap();

            Status.Text = "Images loaded";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxes = [textBox1, textBox2];
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox3.Text, out int value))
            {
                MinDisparity = value;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox4.Text, out int value))
            {
                NumDisparities = value;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox5.Text, out int value))
            {
                BlockSize = value;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox6.Text, out int value))
            {
                P1 = value;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox7.Text, out int value))
            {
                P2 = value;
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox8.Text, out int value))
            {
                Disp12MaxDiff = value;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox9.Text, out int value))
            {
                PreFilterCap = value;
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox10.Text, out int value))
            {
                UniquenessRatio = value;
            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox11.Text, out int value))
            {
                SpeckleWindowSize = value;
            }
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox12.Text, out int value))
            {
                SpeckleRange = value;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            interest = me.Location;
            
            string message = $"({interest.X}, {interest.Y})";

            if (checkBox2.Checked)
            {
                label9.Text += message + "\n";
            }
            else
            {
                label9.Text = message;
            }
        }
    }
}
