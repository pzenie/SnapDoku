using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace Puzzle_Image_Recognition.Sudoku_Normal
{
    public class DigitRecognizer
    {
        private readonly KNearest knn;

        public DigitRecognizer()
        {
            knn = KNearest.Create();
        }
        ~DigitRecognizer()
        {
            knn.Dispose();
        }

        /// <summary>
        /// Trains the knn using the specified training files
        /// </summary>
        /// <param name="trainPath">The path containing the training files
        ///                         training images should be seperated in folders signifying their associated digit</param>
        /// <returns>True if the training passed</returns>
        public bool Train(IReadOnlyCollection<ZipArchiveEntry> entries)
        {
            var trainingImages = ReadTrainingImages(entries);
            var samples = new Mat();
            foreach (var trainingImage in trainingImages)
            {
                samples.PushBack(trainingImage.Item1);
            }

            var labels = trainingImages.Select(x => x.Item2).ToArray();
            using (var responses = new Mat(labels.Length, 1, MatType.CV_32SC1, labels))
            {
                var tmp = responses.Reshape(1, 1);
                var responseFloat = new Mat();
                tmp.ConvertTo(responseFloat, MatType.CV_32FC1);

                knn.Train(samples, SampleTypes.RowSample, responseFloat);
                return true;
            }
        }

        /// <summary>
        /// Goes through the training folder and pulls out all the training images and their associated digit
        /// </summary>
        /// <param name="path">The training folder</param>
        /// <returns>List of images and their labels</returns>
        public List<Tuple<Mat, int>> ReadTrainingImages(IReadOnlyCollection<ZipArchiveEntry> entries)
        {
            var images = new List<Tuple<Mat, int>>();

            foreach(var entry in entries)
            {
                if(!entry.FullName.EndsWith("/"))
                {
                    string[] paths = entry.FullName.Split('/');
                    string labelString = paths[paths.Length - 2];
                    int label = int.Parse(labelString);
                    Stream stream = entry.Open();
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        Mat image = Cv2.ImDecode(imageBytes, ImreadModes.GrayScale);
                        Mat processedImage = ProcessTrainingImage(image);
                        if(processedImage != null)
                        {
                            images.Add(new Tuple<Mat, int>(processedImage, label));
                        }
                    }
                }
            }
            return images;
        }

        /// <summary>
        /// Processes the training image to be usable for training.
        /// </summary>
        /// <param name="img">The image to proccess</param>
        /// <returns>The proccessed image</returns>
        private static Mat ProcessTrainingImage(Mat img)
        {
            Cv2.Threshold(img, img, 200, 255, ThresholdTypes.Binary);
            img.ConvertTo(img, MatType.CV_32FC1, 1.0 / 255.0);
            Cv2.Resize(img, img, new Size(16, 16), 0, 0, InterpolationFlags.Nearest);
            img = img.Reshape(1, 1);
            return img;
        }

        /// <summary>
        /// Classifies the digit in the box
        /// </summary>
        /// <param name="box">The box with the digit to classify</param>
        /// <param name="resultAsArray">Unusued</param>
        /// <returns>The value of the classified digit</returns>
        public int Clasify(Mat box, OutputArray resultAsArray)
        {
            return (int)knn.FindNearest(box, 1, resultAsArray);
        }
    }
}
