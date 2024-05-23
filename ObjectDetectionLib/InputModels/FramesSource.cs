
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Text.Json;

namespace ObjectDetectionLib.InputModels
{
    public class FrameInfo
    {
        public required Mat Frame { get; init; }
        public required Matrix<double> CameraOrigin { get; init; }
        public required Matrix<double> Target { get; init; }
    }

    public class FramesSource
    {
        private class RawJsonFrame
        {
            public required string FramePath { get; init; }
            public required string CameraOrigin { get; init; }
            public required string Target { get; init; }
        }

        private class RawJsonSource
        {
            public required string AIM { get; init; }

            public required IReadOnlyCollection<RawJsonFrame> Frames { get; init; }

            public required double HorizontalFrontOfView { get; init; }
                          
            public required double VerticalFrontOfView { get; init; }

            public required bool Save3DMap { get; init; }
            public required string Path3DMap { get; init; }

            public required bool Save2DMap { get; init; }
            public required string Path2DMap { get; init; }
        }

        public required string AIM { get; init; }

        public required IReadOnlyCollection<FrameInfo> Frames { get; init; }

        public required double HorizontalFrontOfView { get; init; }

        public required double VerticalFrontOfView { get; init; }

        public required bool Save3DMap { get; init; }
        public required string Path3DMap { get; init; }

        public required bool Save2DMap { get; init; }
        public required string Path2DMap { get; init; }

        public static FramesSource LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException(fileName);

            string jsonString = File.ReadAllText(fileName);
            RawJsonSource? raw = JsonSerializer.Deserialize<RawJsonSource>(jsonString);

            if (raw == null) throw new NullReferenceException(nameof(raw));

            List<FrameInfo> frames = [];

            foreach (var frame in raw.Frames)
            {
                Mat mat = new (frame.FramePath);
                
                var originArray = frame.CameraOrigin.Split(',').Select(float.Parse).ToArray();
                var directionArray = frame.Target.Split(',').Select(float.Parse).ToArray();

                Matrix<double> origin = new ([originArray[0], originArray[1], originArray[2]]);
                Matrix<double> direction = new([directionArray[0], directionArray[1], directionArray[2]]);

                FrameInfo info = new() { Frame = mat, CameraOrigin = origin, Target = direction };
                frames.Add(info);
            }

            return new FramesSource { 
                AIM = raw.AIM,
                Frames = frames, 
                HorizontalFrontOfView = raw.HorizontalFrontOfView, 
                VerticalFrontOfView = raw.VerticalFrontOfView,
                Save3DMap = raw.Save3DMap,
                Path3DMap = raw.Path3DMap,
                Save2DMap = raw.Save2DMap,
                Path2DMap = raw.Path2DMap,
            };
        }
    }
}
