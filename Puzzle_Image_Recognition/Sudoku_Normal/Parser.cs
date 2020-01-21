using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using OpenCvSharp;

namespace Puzzle_Image_Recognition.Sudoku_Normal
{
    public static class Parser
    {
        public static List<int> Solve(byte[] file)
        {
            Mat sudoku = Cv2.ImDecode(file, ImreadModes.GrayScale);
            Mat border = sudoku.Clone();
            border = PrepImage(border);
            Point[] corners = FindCorners(border);

            var result = CropAndWarp(sudoku, corners);
            Mat undistorted = result.Item1;
            int maxLength = Convert.ToInt32(result.Item2);

            Mat undistortedPrepped = PrepImage(undistorted);

            MemoryStream s = new MemoryStream(Properties.Resources.digits);
            ZipArchive z = new ZipArchive(s);
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                z.ExtractToDirectory(path);
            }
            catch(Exception e)
            {
                string sasdasd = "asd";/*Prlly just means the file already exists */ }
            
            DigitRecognizer dr = new DigitRecognizer();
            dr.Train(path + "/digits");

            int boxSize = maxLength / 9;
            List<int> test = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int x = boxSize * i;
                    int y = boxSize * j;
                    Mat numberBox = new Mat(undistortedPrepped, new Rect(x, y, boxSize, boxSize));
                    numberBox = new Mat(numberBox, new Rect(boxSize/4, boxSize/5, boxSize/2, 2*(boxSize/3)));

                    Cv2.Threshold(numberBox, numberBox, 200, 255, ThresholdTypes.Otsu);
                    numberBox.ConvertTo(numberBox, MatType.CV_32FC1, 1.0 / 255.0);
                    Cv2.Resize(numberBox, numberBox, new Size(16, 16), 0, 0, InterpolationFlags.Linear);

                    numberBox = numberBox.Reshape(1, 1);

                    int resultClassify = dr.Clasify(numberBox, new Mat());
                    test.Add(resultClassify);
                }
            }

            return test;
        }

        private static Mat PrepImage(Mat img)
        {
            Cv2.GaussianBlur(img, img, new Size(11, 11), 0);
            Cv2.AdaptiveThreshold(img, img, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 5, 2);
            Cv2.BitwiseNot(img, img);
            Mat kernal = new Mat(3, 3, MatType.CV_8UC1, new byte[] { 0, 1, 0, 1, 1, 1, 0, 1, 0 });
            Cv2.Dilate(img, img, kernal);
            return img;
        }

        private static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + (Math.Pow(p2.Y - p1.Y, 2)));
        }

        private static Tuple<Mat, double> CropAndWarp(Mat img, Point[] corners)
        {
            Point topLeft = corners[0];
            Point bottomLeft = corners[1];
            Point topRight = corners[2];
            Point bottomRight = corners[3];
            double maxSide = Math.Max(
                Math.Max(Distance(bottomRight, topRight), Distance(topLeft, bottomLeft)),
                Math.Max(Distance(bottomRight, bottomLeft), Distance(topLeft, topRight)));
            Point2f[] src = new Point2f[] { topLeft, topRight, bottomRight, bottomLeft };
            Point2f[] dst = new Point2f[] { new Point(0, 0), new Point(maxSide - 1, 0), new Point(maxSide - 1, maxSide - 1), new Point(0, maxSide - 1) };

            Mat m = Cv2.GetPerspectiveTransform(src, dst);
            Mat result = new Mat(new Size(maxSide, maxSide), MatType.CV_8UC1);
            Cv2.WarpPerspective(img, result, m, new Size(maxSide, maxSide));
            return new Tuple<Mat, double>(result, maxSide);
        }

        private static Point[] FindCorners(Mat img)
        {
            Cv2.FindContours(img, out Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            Point[] corners = FindLargest(contours);
            if (corners == null) throw new Exception("Couldn't find corners!");
            return GetRectPoints(corners);
        }

        private static Point[] FindLargest(Point[][] contours)
        {
            Point[] result = null;
            double resultArea = 0;
            foreach(Point[] c in contours)
            {
                if (c.Length >= 4)
                {
                    double area = Cv2.ContourArea(c);
                    if (area > resultArea)
                    {
                        resultArea = area;
                        result = c;
                    }
                }
            }
            return result;
        }
        private static Point[] GetRectPoints(Point[] contour)
        {
            int bottomRight = 0;
            int bottomLeft = int.MaxValue;
            int topRight = 0;
            int topLeft = int.MaxValue;
            Point bottomRightPoint = new Point();
            Point bottomLeftPoint = new Point();
            Point topRightPoint = new Point();
            Point topLeftPoint = new Point();
            foreach (Point p in contour)
            {
                int rightToLeft = p.X + p.Y;
                int leftToRight = p.X - p.Y;
                if (bottomRight < rightToLeft)
                {
                    bottomRight = rightToLeft;
                    bottomRightPoint.X = p.X;
                    bottomRightPoint.Y = p.Y;
                }
                if (topLeft > rightToLeft)
                {
                    topLeft = rightToLeft;
                    topLeftPoint.X = p.X;
                    topLeftPoint.Y = p.Y;
                }
                if (bottomLeft > leftToRight)
                {
                    bottomLeft = leftToRight;
                    bottomLeftPoint.X = p.X;
                    bottomLeftPoint.Y = p.Y;
                }
                if (topRight < leftToRight)
                {
                    topRight = leftToRight;
                    topRightPoint.X = p.X;
                    topRightPoint.Y = p.Y;
                }
            }
            return new Point[] { topLeftPoint, bottomLeftPoint, topRightPoint, bottomRightPoint };
        }
    }
}