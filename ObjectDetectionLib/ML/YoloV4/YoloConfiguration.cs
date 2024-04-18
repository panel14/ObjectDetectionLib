namespace ObjectDetectionLib.ML.YoloV4
{
    public static class YoloConfiguration
    {
        public const string ModelName = "yolov4.onnx";

        public static readonly float[][][] Anchors = [
            [[12f, 16f], [19f, 36f], [40f, 28f]],
            [[36f, 75f], [76f, 55f], [72f, 146f]],
            [[142f, 110f], [192f, 243f], [459f, 401f]]
        ];

        public static readonly float[] Strides = [8, 16, 32];

        public static readonly float[] XYScale = [1.2f, 1.1f, 1.05f];

        public static readonly int[] Shapes = [52, 26, 13];

        public static readonly string[] ClassNames = ["person", "bicycle", "car", "motorbike", "aeroplane",
            "bus", "train", "truck", "boat", "traffic light",
            "fire hydrant", "stop sign", "parking meter", "bench", "bird",
            "cat", "dog", "horse", "sheep", "cow",
            "elephant", "bear", "zebra", "giraffe", "backpack",
            "umbrella", "handbag", "tie", "suitcase", "frisbee",
            "skis", "snowboard", "sports ball", "kite", "baseball bat",
            "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle",
            "wine glass", "cup", "fork", "knife", "spoon",
            "bowl", "banana", "apple", "sandwich", "orange",
            "broccoli", "carrot", "hot dog", "pizza", "donut",
            "cake", "chair", "sofa", "pottedplant", "bed",
            "diningtable", "toilet", "tvmonitor", "laptop", "mouse",
            "remote", "keyboard", "cell phone", "microwave", "oven",
            "toaster", "sink", "refrigerator", "book", "clock",
            "vase", "scissors", "teddy bear", "hair drier", "toothbrush"];
    }
}
