using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using OpenCvSharp;

namespace Puzzle_Image_Recognition.Sudoku_Normal
{
    public class SudokuImageParser
    {
        readonly DigitRecognizer dr;

        /// <summary>
        /// Create parser object and train the knn for future use.
        /// </summary>
        public SudokuImageParser()
        {
            dr = new DigitRecognizer();
            using (MemoryStream s = new MemoryStream(Properties.Resources.digits))
            {
                using (ZipArchive z = new ZipArchive(s))
                {
                    string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    try
                    {
                        z.ExtractToDirectory(path);
                    }
                    catch (Exception) { /* Prlly just means the file already exists, TODO should find a better way to handle this */ }

                    path += "/digits";
                    if (Directory.Exists(path))
                    {
                        dr.Train(path);
                    }
                }
            }
        }

        /// <summary>
        /// Find the board in the image
        /// Crop the image to only include the board
        /// Get each cell from the board and classify, return result of classification
        /// </summary>
        /// <param name="file">Image to parse for a sudoku board</param>
        /// <returns>List of classified digits from a sudoku board in the image</returns>
        public int[,] Solve(byte[] file)
        {
            Mat sudoku = Cv2.ImDecode(file, ImreadModes.GrayScale);
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            bool passed = Cv2.ImWrite(path + @"/test1.jpg", sudoku);
            Mat border = sudoku.Clone();
            border = PrepImage(border, true);
            Point[] corners = FindCorners(border);

            Mat undistorted = CropAndWarp(sudoku, corners);

            Cv2.Resize(undistorted, undistorted, new Size(500, 500));

            Mat undistortedPrepped = PrepImage(undistorted, true);

            Cv2.FindContours(undistortedPrepped, out Point[][] contours, out _, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);

            Point[] contour = FindLargestContour(contours);
            Cv2.FloodFill(undistortedPrepped, contour[0], new Scalar(0, 0, 0));

            int boxHeight = undistortedPrepped.Size().Height / 9;
            int boxWidth = undistortedPrepped.Size().Width / 9;

            List<Rect> boxes = GetDigitBoxes(boxWidth, boxHeight);

            return ConvertListToBoard(ClassifyBoard(undistortedPrepped, boxes));
        }

        /// <summary>
        /// Converts the column ordered list to a 2d array representing the sudoku board
        /// </summary>
        /// <param name="result">Result list from classification</param>
        /// <returns>2d array representing the board</returns>
        private int[,] ConvertListToBoard(List<int> result)
        {
            int row = 0;
            int col = 0;
            int[,] board = new int[9, 9];
            foreach (int i in result)
            {
                if (row >= 9)
                {
                    row = 0;
                    col++;
                }
                board[row, col] = i;
                row++;
            }
            return board;
        }

        /// <summary>
        /// Goes through each cell in a 9x9 grid, extraxts the cell image and classifies it using our knn
        /// </summary>
        /// <param name="board">Image of the baord</param>
        /// <param name="boxSize">The size of each box in the image</param>
        /// <returns>List of ints representing the parsed digits from the image</returns>
        private List<int> ClassifyBoard(Mat board, List<Rect> boxes)
        {
            List<int> result = new List<int>();
            foreach(Rect box in boxes)
            {
                Mat digit = ExtractAndCenterBox(board, box);
                int resultClassify = dr.Clasify(digit, new Mat());
                result.Add(resultClassify);
            }
            return result;
        }

        /// <summary>
        /// Finds the bounding box of the digit within the rect if the box contains one and return cropped and formatted verrsion
        /// </summary>
        /// <param name="board">Image of board</param>
        /// <param name="rect">area to check for digit</param>
        /// <returns>Formmated box</returns>
        private Mat ExtractAndCenterBox(Mat board, Rect rect)
        {
            Mat digit = new Mat(board, rect);

            Rect boundingBox = FindLargestFeature(digit);

            Mat numberBox = new Mat(digit, boundingBox);

            Cv2.Threshold(numberBox, numberBox, 200, 255, ThresholdTypes.Binary);
            numberBox.ConvertTo(numberBox, MatType.CV_32FC1, 1.0 / 255.0);
            Cv2.Resize(numberBox, numberBox, new Size(16, 16), 0, 0, InterpolationFlags.Nearest);

            numberBox = numberBox.Reshape(1, 1);

            return numberBox;
        }

        /// <summary>
        /// Zooms an image of a box onto the digit if there's one present
        /// </summary>
        /// <param name="digit">The box with the possible digit to zoom in on</param>
        /// <returns>The zoomed in image</returns>
        private Rect FindLargestFeature(Mat digit)
        {
            if (digit.CountNonZero() > 200) // ignore boxes without digits and just noise
            {
                Cv2.FindContours(digit, out Point[][] contours, out _, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);

                if (contours.Length > 0)
                {
                    Point[] contour = FindLargestContour(contours);
                    if (contour != null)
                    {
                        Rect bounding = Cv2.BoundingRect(contour);
                        if (bounding.Width < bounding.Height) //Make image square with digit at center
                        {
                            int difference = bounding.Height - bounding.Width;
                            bounding.X -= difference / 2;
                            if (bounding.X < 0) bounding.X = 0;
                            bounding.Width = bounding.Height;
                        }
                        else if (bounding.Width > bounding.Height) //Make image square with digit at center
                        {
                            int difference = bounding.Width - bounding.Height;
                            bounding.Y -= difference / 2;
                            if (bounding.Y < 0) bounding.Y = 0;
                            bounding.Height = bounding.Width;
                        }
                        while (bounding.X + bounding.Width > digit.Cols) bounding.Width--;
                        while (bounding.Y + bounding.Height > digit.Rows) bounding.Height--;

                        return bounding;
                    }
                }
            }
            return new Rect(0, 0, digit.Size().Width, digit.Size().Height);
        }

        private List<Rect> GetDigitBoxes(int boxWidth, int boxHeight)
        {
            List<Rect> rects = new List<Rect>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int x = boxWidth * i;
                    int y = boxHeight * j;
                    rects.Add(new Rect(x, y, boxWidth, boxHeight));
                }
            }
            return rects;
        }

        /// <summary>
        /// Preps the image for extracting the board
        /// </summary>
        /// <param name="img">image to prep</param>
        /// <returns>prepped image</returns>
        private Mat PrepImage(Mat img, bool dilate)
        {
            Cv2.GaussianBlur(img, img, new Size(11, 11), 0);
            Cv2.AdaptiveThreshold(img, img, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 5, 2);
            Cv2.BitwiseNot(img, img);
            Mat kernal = new Mat(3, 3, MatType.CV_8UC1, new byte[] { 0, 1, 0, 1, 1, 1, 0, 1, 0 });
            if (dilate)
            {
                Cv2.Dilate(img, img, kernal);
            }
            return img;
        }

        /// <summary>
        /// Finds distance between two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + (Math.Pow(p2.Y - p1.Y, 2)));
        }

        /// <summary>
        /// Crops the image to the corners and warps the perspective to make it flat
        /// </summary>
        /// <param name="img">The image to crop</param>
        /// <param name="corners">The corners to crop too</param>
        /// <returns>The cropped and warped image</returns>
        private Mat CropAndWarp(Mat img, Point[] corners)
        {
            Point topLeft = corners[0];
            Point bottomLeft = corners[1];
            Point topRight = corners[2];
            Point bottomRight = corners[3];
            double maxHeight = Math.Max(Distance(bottomRight, topRight), Distance(topLeft, bottomLeft));
            double maxWidth = Math.Max(Distance(bottomRight, bottomLeft), Distance(topLeft, topRight));
            Point2f[] src = new Point2f[] { topLeft, topRight, bottomRight, bottomLeft };
            Point2f[] dst = new Point2f[] { new Point(0, 0), new Point(maxWidth - 1, 0), new Point(maxWidth - 1, maxHeight - 1), new Point(0, maxHeight - 1) };

            Mat m = Cv2.GetPerspectiveTransform(src, dst);
            Mat result = new Mat(new Size(maxWidth, maxHeight), MatType.CV_8UC1);
            Cv2.WarpPerspective(img, result, m, new Size(maxWidth, maxHeight));
            return result;
        }

        /// <summary>
        /// Finds the corners of the largest box in the image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private Point[] FindCorners(Mat img)
        {
            Cv2.FindContours(img, out Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            Point[] corners = FindLargestContour(contours);
            if (corners == null) throw new Exception("Couldn't find corners!");
            return GetRectPoints(corners);
        }

        /// <summary>
        /// Finds the contour set with the largest area between corners
        /// </summary>
        /// <param name="contours">Set of contours in the image</param>
        /// <returns>The corners with the largest area</returns>
        private Point[] FindLargestContour(Point[][] contours)
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

        /// <summary>
        /// Gets the four points that create the largest rectangle from a contour
        /// </summary>
        /// <param name="contour">Contour to get the points from</param>
        /// <returns>Points comprising the largest rectangle in the contour</returns>
        private Point[] GetRectPoints(Point[] contour)
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