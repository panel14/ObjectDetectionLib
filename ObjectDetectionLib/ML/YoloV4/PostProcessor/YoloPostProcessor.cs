// ported from https://github.com/jerhon/hs-object-dectection-service/blob/main/src/Honlsoft.ML.ObjectDetection/Models/Yolov4/YoloPostProcessor.cs

using ObjectDetectionLib.ML.YoloV4;
using ObjectDetectionLib.ML.YoloV4.ModelData;
using ObjectDetectionLib.ML.YoloV4.Results;
using System.Diagnostics;

namespace ObjectDetectionLib.ML.YoloV4.PostProcessor
{
    public static class YoloPostProcessor
    {
        public static IReadOnlyList<ObjectDetectionResult> GetResults(YoloOutputData outputData, float scoreThres = 0.5f, float iouThres = 0.8f)
        {
            List<float[]> postProcesssedResults = [];
            int anchorsCount = YoloConfiguration.Anchors.Length;
            int classesCount = YoloConfiguration.ClassNames.Length;

            float[][]? results = [outputData.Output0, outputData.Output1, outputData.Output2];

            for (int i = 0; i < results.Length; i++)
            {
                var predict = results[i];
                var outputSize = YoloConfiguration.Shapes[i];

                for (int boxY = 0; boxY < outputSize; boxY++)
                {
                    for (int boxX = 0; boxX < outputSize; boxX++)
                    {
                        for (int anch = 0; anch < anchorsCount; anch++)
                        {
                            var offset = boxY * outputSize * (classesCount + 5) * anchorsCount + boxX * (classesCount + 5) * anchorsCount + anch * (classesCount + 5);
                            var predBbox = predict.Skip(offset).Take(classesCount + 5).ToArray();

                            // ported from https://github.com/onnx/models/tree/master/vision/object_detection_segmentation/yolov4#postprocessing-steps

                            // postprocess_bbbox()

                            var predXywh = predBbox.Take(4).ToArray();
                            var predConf = predBbox[4];
                            var predProb = predBbox.Skip(5).ToArray();

                            var rawDx = predXywh[0];
                            var rawDy = predXywh[1];
                            var rawDw = predXywh[2];
                            var rawDh = predXywh[3];

                            float predX = (Sigmoid(rawDx) * YoloConfiguration.XYScale[i] - 0.5f * (YoloConfiguration.XYScale[i] - 1) + boxX) * YoloConfiguration.Strides[i];
                            float predY = (Sigmoid(rawDy) * YoloConfiguration.XYScale[i] - 0.5f * (YoloConfiguration.XYScale[i] - 1) + boxY) * YoloConfiguration.Strides[i];
                            float predW = (float)Math.Exp(rawDw) * YoloConfiguration.Anchors[i][anch][0];
                            float predH = (float)Math.Exp(rawDh) * YoloConfiguration.Anchors[i][anch][1];

                            // postprocess_boxes
                            // (1) (x, y, w, h) --> (xmin, ymin, xmax, ymax)
                            float predX1 = predX - predW * 0.5f;
                            float predY1 = predY - predH * 0.5f;
                            float predX2 = predX + predW * 0.5f;
                            float predY2 = predY + predH * 0.5f;

                            // (2) (xmin, ymin, xmax, ymax) -> (xmin_org, ymin_org, xmax_org, ymax_org)
                            float org_h = outputData.ImageHeight;
                            float org_w = outputData.ImageWidth;

                            float inputSize = 416f;
                            float resizeRatio = Math.Min(inputSize / org_w, inputSize / org_h);
                            float dw = (inputSize - resizeRatio * org_w) / 2f;
                            float dh = (inputSize - resizeRatio * org_h) / 2f;

                            var orgX1 = 1f * (predX1 - dw) / resizeRatio; // left
                            var orgX2 = 1f * (predX2 - dw) / resizeRatio; // right
                            var orgY1 = 1f * (predY1 - dh) / resizeRatio; // top
                            var orgY2 = 1f * (predY2 - dh) / resizeRatio; // bottom

                            // (3) clip some boxes that are out of range
                            orgX1 = Math.Max(orgX1, 0);
                            orgY1 = Math.Max(orgY1, 0);
                            orgX2 = Math.Min(orgX2, org_w - 1);
                            orgY2 = Math.Min(orgY2, org_h - 1);
                            if (orgX1 > orgX2 || orgY1 > orgY2) continue; // invalid_mask

                            // (4) discard some invalid boxes
                            // TODO

                            // (5) discard some boxes with low scores
                            var scores = predProb.Select(p => p * predConf).ToList();

                            float scoreMaxCat = scores.Max();
                            if (scoreMaxCat > scoreThres)
                            {
                                postProcesssedResults.Add([orgX1, orgY1, orgX2, orgY2, scoreMaxCat, scores.IndexOf(scoreMaxCat)]);
                            }
                        }
                    }
                }
            }

            // Non-maximum Suppression
            postProcesssedResults = [.. postProcesssedResults.OrderByDescending(x => x[4])]; // sort by confidence
            List<ObjectDetectionResult> resultsNms = [];

            int f = 0;
            while (f < postProcesssedResults.Count)
            {
                var res = postProcesssedResults[f];
                if (res == null)
                {
                    f++;
                    continue;
                }

                var conf = res[4];
                string label = YoloConfiguration.ClassNames[(int)res[5]];

                resultsNms.Add(new ObjectDetectionResult(res[0], res[1], res[2], res[3], label, conf));
                postProcesssedResults[f] = null;

                var iou = postProcesssedResults.Select(bbox => bbox == null ? float.NaN : BoxIoU(res, bbox)).ToList();

                for (int i = 0; i < iou.Count; i++)
                {
                    if (float.IsNaN(iou[i])) continue;
                    if (iou[i] > iouThres)
                    {
                        postProcesssedResults[i] = null;
                    }
                }
                f++;
            }

            return resultsNms;
        }

        private static float Sigmoid(float x)
        {
            return 1f / (1f + (float)Math.Exp(x));
        }

        private static float BoxIoU(float[] boxes1, float[] boxes2)
        {
            static float BoxArea(float[] box)
            {
                return (box[2] - box[0]) * (box[3] - box[1]);
            }

            var area1 = BoxArea(boxes1);
            var area2 = BoxArea(boxes2);

            Debug.Assert(area1 >= 0);
            Debug.Assert(area2 >= 0);

            var dx = Math.Max(0, Math.Min(boxes1[2], boxes2[2]) - Math.Max(boxes1[0], boxes2[0]));
            var dy = Math.Max(0, Math.Min(boxes1[3], boxes2[3]) - Math.Max(boxes1[1], boxes2[1]));

            var inter = dx * dy;

            return inter / (area1 + area2 - inter);
        }
    }
}
