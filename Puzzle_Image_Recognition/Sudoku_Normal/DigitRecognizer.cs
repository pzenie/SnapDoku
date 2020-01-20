using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace Puzzle_Image_Recognition.Sudoku_Normal
{
    public class ImageInfo
    {
        public Mat Image { set; get; }
        public int ImageGroupId { set; get; }
    }
    public class DigitRecognizer
    {
        private KNearest knn;

        public DigitRecognizer()
        {
            knn = KNearest.Create();
        }
        ~DigitRecognizer()
        {
            knn.Dispose();
        }

        public bool Train(string trainPath)
        {
            var trainingImages = ReadTrainingImages(trainPath);
            var samples = new Mat();
            foreach (var trainingImage in trainingImages)
            {
                samples.PushBack(trainingImage.Image);
            }

            var labels = trainingImages.Select(x => x.ImageGroupId).ToArray();
            var responses = new Mat(labels.Length, 1, MatType.CV_32SC1, labels);
            var tmp = responses.Reshape(1, 1);
            var responseFloat = new Mat();
            tmp.ConvertTo(responseFloat, MatType.CV_32FC1);

            knn.Train(samples, SampleTypes.RowSample, responseFloat);
            return true;
        }

        public IList<ImageInfo> ReadTrainingImages(string path)
        {
            var images = new List<ImageInfo>();

            foreach(var dir in new DirectoryInfo(path).GetDirectories())
            {
                var groupId = int.Parse(dir.Name);
                foreach(var imageFile in dir.GetFiles())
                {
                    var image = ProcessTrainingImage2(new Mat(imageFile.FullName, ImreadModes.Grayscale), groupId);

                    if (image == null)
                    {
                        continue;
                    }
                    images.Add(new ImageInfo
                    {
                        Image = image,
                        ImageGroupId = groupId
                    });
                }
            }
            return images;
        }
        private static Mat ProcessTrainingImage2(Mat img, int groupId)
        {
            Cv2.Threshold(img, img, 200, 255, ThresholdTypes.Otsu);
            img.ConvertTo(img, MatType.CV_32FC1, 1.0 / 255.0);
            Cv2.Resize(img, img, new Size(16, 16), 0, 0, InterpolationFlags.LinearExact);
            /*if (groupId > 0)
            {
                Cv2.ImShow("test", img);
                Cv2.WaitKey();
            }*/
            img = img.Reshape(1, 1);
            return img;
        }
        /*public Mat ProcessTrainingImage(Mat gray, int groupId)
        {
            var threshImage = new Mat();
            Cv2.Threshold(gray, threshImage, 80, 255, ThresholdTypes.BinaryInv); // Threshold to find contour

            Cv2.FindContours(
                threshImage,
                out Point[][] contours,
                out HierarchyIndex[] hierarchyIndexes,
                mode: RetrievalModes.CComp,
                method: ContourApproximationModes.ApproxSimple);

            if (contours.Length == 0)
            {
                return null;
            }

            Mat result = null;

            var contourIndex = 0;
            while ((contourIndex >= 0))
            {
                var contour = contours[contourIndex];

                var boundingRect = Cv2.BoundingRect(contour);
                var roi = new Mat(threshImage, boundingRect);

                var resizedImage = new Mat();
                var resizedImageFloat = new Mat();
                Cv2.Resize(roi, resizedImage, new Size(16, 16));
                resizedImage.ConvertTo(resizedImageFloat, MatType.CV_32FC1);
                result = resizedImageFloat.Reshape(1, 1);

                contourIndex = hierarchyIndexes[contourIndex].Next;
            }

            return result;
        }*/

        public int Clasify(Mat box, OutputArray resultAsArray)
        {
            return (int)knn.FindNearest(box, 1, resultAsArray);
        }
    }
}
